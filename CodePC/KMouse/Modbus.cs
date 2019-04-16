
//#define MODBUS_ERROR_MESSAGE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;//使用串口
using System.Runtime.InteropServices;//隐藏光标的
using System.Management;
using System.Diagnostics;


namespace KMouse
{
	class Modbus
	{
        public enum REG : byte
        {
            IDENTIFY = 1,
            MOUSE_PRESS = 3,
            MOUSE_CLICK = 5,
            MOUSE_SPEED = 7,
            KEYBOARD = 9,
            USB_RECONNECT = 11,
            SYSTEM_REBOOT = 66,
        }

		//Modbus使用的变量
		public const int SEND_MAX_LEN = 12;
		public const int RECV_MAX_LEN = 8;

		byte[] modbus_send_data = new byte[SEND_MAX_LEN];
		byte[] modbus_recv_data = new byte[RECV_MAX_LEN];
		private uint modbus_recv_cnt = 0;
		private uint modbus_recv_num = 8;
		private uint modbus_recv_timeout = Func.dwAllFF;
        private bool is_busy = false;
        private uint modbus_respone_timeout = 0;
        private bool is_handling = false;

        public uint success_cnt;
        public uint fail_cnt;
		
        public bool echo_en = false;    //是否回显命令
        public bool send_cmd_is_busy = false;

        public Action Action_UpdateModbussState;
        public delegate void Delegate_ModbusCallBack(uint value);
        public Delegate_ModbusCallBack Delegate_ModbusCallBack_Identify;
        public Delegate_ModbusCallBack Delegate_ModbusCallBack_Click;
        public Delegate_ModbusCallBack Delegate_ModbusCallBack_Speed;

        System.Timers.Timer timer_rcv_timeout;
        System.Timers.Timer timer_auto_snd;

        keyQ kq;
        public SerialPort com;
        Queue<string> queue_message;

        public void Init(keyQ _kq, SerialPort _com, Queue<string> _queue_message,
            Action _Action_UpdateModbussState,
            Delegate_ModbusCallBack _Delegate_ModbusCallBack_Identify,
            Delegate_ModbusCallBack _Delegate_ModbusCallBack_Click,
            Delegate_ModbusCallBack _Delegate_ModbusCallBack_Speed)
        {
            kq = _kq;
            com = _com;
            queue_message = _queue_message;
            Action_UpdateModbussState = _Action_UpdateModbussState;
            Delegate_ModbusCallBack_Identify = _Delegate_ModbusCallBack_Identify;
            Delegate_ModbusCallBack_Click = _Delegate_ModbusCallBack_Click;
            Delegate_ModbusCallBack_Speed = _Delegate_ModbusCallBack_Speed;

            
            timer_rcv_timeout = new System.Timers.Timer(10);
            timer_rcv_timeout.Elapsed += new System.Timers.ElapsedEventHandler(timer_rcv_timeout_Tick);
            timer_rcv_timeout.AutoReset = true;
            timer_rcv_timeout.Enabled = true;

            
            timer_auto_snd = new System.Timers.Timer(10);
            timer_auto_snd.Elapsed += new System.Timers.ElapsedEventHandler(timer_auto_snd_Tick);
            timer_auto_snd.AutoReset = true;
            timer_auto_snd.Enabled = true;
        }

        public void Close()
        {
            timer_rcv_timeout.Close();
            timer_auto_snd.Close();
        }

		//reg: 要读的寄存器, num: 要读的WORD数, val: 传递的参数)
		public void Send_03(REG Register, byte Number, uint val)
		{
			const Int32 MODBUS_SEND_03_NUM = 10;
			UInt16 crc_val;

			modbus_send_data[0] = 0x01;
			modbus_send_data[1] = 0x03;
			modbus_send_data[2] = (byte)Register;
			modbus_send_data[3] = Number;
			modbus_send_data[4] = (byte)((val & 0xFF000000) >> 24);
			modbus_send_data[5] = (byte)((val & 0x00FF0000) >> 16);
			modbus_send_data[6] = (byte)((val & 0x0000FF00) >> 8);
			modbus_send_data[7] = (byte)((val & 0x000000FF) >> 0);
			crc_val = Func.Get_CRC(modbus_send_data, 8);
			modbus_send_data[8] = (byte)((crc_val & 0xFF00) >> 8);
			modbus_send_data[9] = (byte)((crc_val & 0x00FF) >> 0);

			try
			{
                is_busy = true;
                modbus_respone_timeout = 0;

                com.Write(modbus_send_data, 0, MODBUS_SEND_03_NUM);

                Console.Write("PC->MCU:");
                for (int v = 0; v < MODBUS_SEND_03_NUM; v++)
                {
                    Console.Write(" {0:X}", modbus_send_data[v]);
                }
                Console.Write("\r\n\r\n");
                
                if (echo_en == true)
                {
                    String SerialIn = "";
                    SerialIn += "Send: ";
                    for (uint i = 0; i < MODBUS_SEND_03_NUM; i++)
                    {
                        SerialIn += "0x";
                        SerialIn += Func.GetHexHighLow(modbus_send_data[i], 0);
                        SerialIn += Func.GetHexHighLow(modbus_send_data[i], 1) + " ";
                    }
                    queue_message.Enqueue(SerialIn);
                }
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "提示");
			}
		}

        private void timer_auto_snd_Tick(object sender, EventArgs e)
        {
            if ((send_cmd_is_busy == false) && (kq.FIFO_HasData() == true))
            {
                send_cmd_is_busy = true;
                Send_03(Modbus.REG.KEYBOARD, 1, (uint)kq.FIFO_Output());
            }
        }

        private void timer_rcv_timeout_Tick(object sender, EventArgs e)	//10ms
		{
			//Console.WriteLine(DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
			//Console.WriteLine("modbus_recv_timeout:{0}", modbus_recv_timeout);

            //发了命令，但是1s内都没有回复
            if (is_busy == true)
            {
                modbus_respone_timeout++;
                if (modbus_respone_timeout == 100)
                {
                    modbus_respone_timeout = 0;                    

                    bool res;
                    res = Recv_Handle();

                    //MessageBox.Show("Modbus Respone timeout!!!", "Warning");
                    queue_message.Enqueue("Modbus Respone timeout!!!");
                    //清空FIFO，避免已进FIFO的key一直发送
                    kq.FIFO_Reset();

                    if (res == false)
                    {
                        //Func_COM_Close();
                        Console.WriteLine("Dont close COM B");
                    }
                }
            }

            //已经开始了接收，但是1s内都没有接收完
			if(modbus_recv_timeout != Func.dwAllFF)
			{
				modbus_recv_timeout++;
				if(modbus_recv_timeout == 100)	//1s后modbus接收超时
				{
					modbus_recv_timeout = Func.dwAllFF;
					String SerialIn = "";
					for(int i = 0; i < modbus_recv_cnt; i++)
					{
						//SerialIn += modbus_recv_data[i].ToString();
						if(modbus_recv_data[i] < 0x0f)
						{
							SerialIn += "0x0";
							SerialIn += Convert.ToString(modbus_recv_data[i], 16);
						}
						else
						{
							SerialIn += "0x";
							SerialIn += Convert.ToString(modbus_recv_data[i], 16);
						}

						SerialIn += " ";
					}

					modbus_recv_cnt = 0;

					//MessageBox.Show(SerialIn, "Warning, Modbus Recv timeout!!!");
                    queue_message.Enqueue("Modbus Recv timeout: " + SerialIn + "!!!");
                    System.Media.SystemSounds.Hand.Play();
				}
			}

            if((com.IsOpen == true) && (is_busy == false))
			{
				for(uint i = 0; i < keyQ.MOUSE.PRESS.ALL; i++)
				{
					if(kq.mouse_press_en[i] == true)
					{
						Send_03(Modbus.REG.MOUSE_PRESS, 1, i);
					}
					else if(kq.mouse_press_en_last[i] == true)					//松开按键的那一下
					{
						Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.LEAVE);
					}
					kq.mouse_press_en_last[i] = kq.mouse_press_en[i];				
				}
			}
		}

        public void Rcv_Data(byte[] buffer, int len)
        {
            if(is_handling == true)
            {
                return;
            }

            for(int i = 0; i < len; i++)
			{
				byte a = buffer[i];
				modbus_recv_data[modbus_recv_cnt] = a;
				if((modbus_recv_cnt == 1) && (modbus_recv_data[modbus_recv_cnt] == 0x03))
				{
					modbus_recv_num = 8;							//正常情况下03需要接收8个状态码
				}
				else if((modbus_recv_cnt == 1) && (modbus_recv_data[modbus_recv_cnt] == 0xFF))
				{
					modbus_recv_num = 6;							//命令异常时返回5个字节的数据
				}
				else												//不同的命令需要接收的字节数不一样
				{
					//modbus_recv_num = 8;
				}
				modbus_recv_cnt++;
				Console.WriteLine("     modbus_recv_cnt:{0}({1})", modbus_recv_cnt, modbus_recv_num);
				if(modbus_recv_cnt == modbus_recv_num)
				{
					bool res;
					res = Recv_Handle();
					if(res == false)
					{
						//Func_COM_Close();
                        Console.WriteLine("Dont close COM A");
					}
				}
			}

			if(modbus_recv_cnt != 0)								//modbus指令还没收完，启动定时器
			{
				modbus_recv_timeout = 0;
			}
			else
			{
				modbus_recv_timeout = Func.dwAllFF;
			}
        }

		private bool Recv_Handle()
		{
			bool res = true;
			is_handling = true;
			String SerialIn = "";
			for(int i = 0; i < modbus_recv_cnt; i++)
			{
				//SerialIn += modbus_recv_data[i].ToString();
				if(modbus_recv_data[i] < 0x0f)
				{
					SerialIn += "0x0";
					SerialIn += Convert.ToString(modbus_recv_data[i], 16);
				}
				else
				{
					SerialIn += "0x";
					SerialIn += Convert.ToString(modbus_recv_data[i], 16);
				}

				SerialIn += " ";
			}

            if (echo_en == true)
            {
                queue_message.Enqueue("Recv: " + SerialIn);
            }

			/*
				Send:
				[0]		   0x01: SubAddress
				[1]		   0x03: Read(WORD)
				[2]		   0x01: Register
				[3]		   0x01: Num(要读的WORD数)
				[4-7]0x00000001: Val(传递的参数)
				[8]		   0x**: CRCH
				[9]		   0x**: CRCL

				Rec:
				[0]		   0x01: SubAddress
				[1]		   0x03: Read(WORD)
				[2]		   0x01: Register
				[3]		   0x01: Num
				[4]		   0x00: ValH[0]
				[5]		   0x01: ValL[0]
				[6]		   0x**: CRCH
				[7]		   0x**: CRCL
			*/

			//send:01 03 01 01 00 00 00 01 4F 16->received:01 03 01 01 55 AA AA D9//读故障报警状态 

			ushort CRC_Result_Cal = 0;
            ushort CRC_Result_Trans = 0;
			byte Register_Address = 0;

			is_busy = false;
            switch (modbus_recv_data[1])									//功能码
			{
				case 0x03:													//读寄存器的值(WORD)
				{
					CRC_Result_Cal = Func.Get_CRC(modbus_recv_data, 6);		//CRC校验正确则发送数据给主站，否则等待主站再次发送数据
					CRC_Result_Trans = (ushort)((((ushort)modbus_recv_data[6]) << 8) | ((ushort)modbus_recv_data[7]));
					if(CRC_Result_Cal == CRC_Result_Trans)
					{
						byte Need_Read_WORD_Num;
						uint Func_Val = 0;

						Register_Address = modbus_recv_data[2];				//寄存器值
						Need_Read_WORD_Num = modbus_recv_data[3];			//要读的WORD数
						Func_Val = ((((uint)modbus_recv_data[4]) << 8) |
									(((uint)modbus_recv_data[5]) << 0));

						Console.WriteLine("MBI Reg:{0} Num:{1} Val:{2}", Register_Address, Need_Read_WORD_Num, Func_Val);

						switch((REG)Register_Address)
						{
							case REG.IDENTIFY:								//测试modbus通信是否正常
							{
								//Console.WriteLine("IDENTIFY:{0:X}", Func_Val);
								Delegate_ModbusCallBack_Identify(Func_Val);
								
								break;
							}

							case REG.MOUSE_PRESS:
							{
								//Console.WriteLine("MOUSE_PRESS:{0:X}", Func_Val);
								break;
							}

							case REG.MOUSE_CLICK:
							{
								Delegate_ModbusCallBack_Click(0);
								break;
							}

							case REG.MOUSE_SPEED:
							{
                                //Console.WriteLine("MOUSE_SPEED:{0:X}", Func_Val);
								Delegate_ModbusCallBack_Speed(Func_Val);
								break;
							}

							case REG.KEYBOARD:
							case REG.SYSTEM_REBOOT:
							case REG.USB_RECONNECT:
							{
								break;
							}

							default:
							{
                                #if MODBUS_ERROR_MESSAGE
                                    MessageBox.Show("error reg!!!: " + Register_Address.ToString(), "提示");
                                #else
                                    queue_message.Enqueue("error reg: " + Register_Address.ToString() + "!!!");
                                #endif

                                System.Media.SystemSounds.Hand.Play();								
								res = false;
								break;
							}
						}
                        send_cmd_is_busy = false;

                        if(kq.FIFO_HasData() == true)
                        {
                            send_cmd_is_busy = true;
                            Send_03(Modbus.REG.KEYBOARD, 1, (uint)kq.FIFO_Output());
                        }
                        else
                        {
                            //FIFO为空时则计数归零，避免buffer越界
                            kq.FIFO_Reset();
                        }
					}
					else
					{
                        #if MODBUS_ERROR_MESSAGE
						    MessageBox.Show("error crc!!!: " + CRC_Result_Cal.ToString(), "提示");
                        #else
                            queue_message.Enqueue("error crc: " + CRC_Result_Cal.ToString() + "!!!");
                        #endif
                        System.Media.SystemSounds.Hand.Play();	
						res = false;
					}
					break;
				}

				case 0x06:
				{
					break;
				}

				default:
				{
                    #if MODBUS_ERROR_MESSAGE
					    MessageBox.Show("error func!!!: " + modbus_recv_data[1].ToString(), "提示");
                    #else
                        queue_message.Enqueue("error func: " + modbus_recv_data[1].ToString() + "!!!");
                    #endif
                    System.Media.SystemSounds.Hand.Play();
					res = false;
					break;
				}
			}
            modbus_recv_data[1] = 0xFF;                                     //取完后立马清掉，表示已经用过了

			modbus_recv_cnt = 0;
			is_handling = false;

            if (res == true)
            {
                success_cnt++;
            }
            else
            {
                fail_cnt++;
                send_cmd_is_busy = false;
            }
            Action_UpdateModbussState.Invoke();

            Console.Write("\r\n");

			return res;
		}
	}
}
