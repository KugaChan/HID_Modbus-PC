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

//为变量定义别名
using u64 = System.UInt64;
using u32 = System.UInt32;
using u16 = System.UInt16;
using u8 = System.Byte;
using s64 = System.Int64;
using s32 = System.Int32;
using s16 = System.Int16;
using s8 = System.SByte;


namespace KMouse
{
	public partial class KMouse
	{
		//Modbus使用的变量
		const int MODBUS_SEND_MAX_LEN = 12;
		const int MODBUS_RECV_MAX_LEN = 8;

		byte[] modbus_send_data = new byte[MODBUS_SEND_MAX_LEN];
		byte[] modbus_recv_data = new byte[MODBUS_RECV_MAX_LEN];
		private u32 modbus_recv_cnt = 0;
		private u32 modbus_recv_num = 8;
		private u32 modbus_recv_timeout = dwAllFF;
        private u32 modbus_respone_timeout = 0;
		private bool modbus_handling = false;

		bool modbus_is_busy = false;
		// CRC 高位字节值表 
		u8[] auchCRCHi = 
		{
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
		};
		// CRC低位字节值表
		u8[] auchCRCLo =
		{
			0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
			0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
			0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
			0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
			0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
			0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
			0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
			0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
			0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
			0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
			0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
			0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
			0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
			0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
			0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
			0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
			0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
			0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
			0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
			0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
			0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
			0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
			0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
			0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
			0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
			0x43, 0x83, 0x41, 0x81, 0x80, 0x40
		};

		private u16 Func_Get_CRC(u8[] Data, u32 DataLen)
		{
			u8 CRCHi = 0xFF;						//高CRC字节初始化 
			u8 CRCLo = 0xFF;						//低CRC 字节初始化  
			u16 Res;
			u8 uIndex;							//CRC循环中的索引
			u8 i = 0;
			while(true)
			{
				DataLen--;
				uIndex = (u8)(CRCHi ^ Data[i]);			//计算CRC
				CRCHi = (u8)(CRCLo ^ auchCRCHi[uIndex]);
				CRCLo = auchCRCLo[uIndex];
				i++;
				if(DataLen == 0)
				{
					break;
				}
			}

			Res = (u16)(((u16)CRCHi << 8) | (u16)CRCLo);
			return Res;
		}

		//reg: 要读的寄存器, num: 要读的WORD数, val: 传递的参数)
		private void Func_Modbus_Send_03(u8 Register, u8 Number, u32 val)
		{
			const s32 MODBUS_SEND_03_NUM = 10;
			u16 crc_val;

			modbus_send_data[0] = 0x01;
			modbus_send_data[1] = 0x03;
			modbus_send_data[2] = Register;
			modbus_send_data[3] = Number;
			modbus_send_data[4] = (u8)((val & 0xFF000000) >> 24);
			modbus_send_data[5] = (u8)((val & 0x00FF0000) >> 16);
			modbus_send_data[6] = (u8)((val & 0x0000FF00) >> 8);
			modbus_send_data[7] = (u8)((val & 0x000000FF) >> 0);
			crc_val = Func_Get_CRC(modbus_send_data, 8);
			modbus_send_data[8] = (u8)((crc_val & 0xFF00) >> 8);
			modbus_send_data[9] = (u8)((crc_val & 0x00FF) >> 0);

			try
			{
                modbus_is_busy = true;
                modbus_respone_timeout = 0;

                com.Write(modbus_send_data, 0, MODBUS_SEND_03_NUM);

                Console.Write("PC->MCU:");
                for (int v = 0; v < MODBUS_SEND_03_NUM; v++)
                {
                    Console.Write(" {0:X}", modbus_send_data[v]);
                }
                Console.Write("\r\n");

                if (checkBox_ShowTxt.Checked == true)
                {
                    textBox_ComRec.Text += "Send: ";
                    for (u32 i = 0; i < MODBUS_SEND_03_NUM; i++)
                    {
                        textBox_ComRec.Text += "0x";
                        textBox_ComRec.Text += Func_GetHexHigh(modbus_send_data[i], 0);
                        textBox_ComRec.Text += Func_GetHexHigh(modbus_send_data[i], 1) + " ";
                    }
                    textBox_ComRec.Text += "\r\n";              
                }
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "提示");
			}

			//StackTrace st = new StackTrace(new StackFrame(true));
			//Console.WriteLine(" Stack trace for current level: {0}", st.ToString());
			//StackFrame sf = st.GetFrame(0);
			//Console.WriteLine(" File: {0}", sf.GetFileName());
			//Console.WriteLine(" Method: {0}", sf.GetMethod().Name);
			//Console.WriteLine(" Line Number: {0}", sf.GetFileLineNumber());
			//Console.WriteLine(" Column Number: {0}", sf.GetFileColumnNumber());
		}

		private bool Func_Modbus_Recv_Handle()
		{
			bool res = true;
			modbus_handling = true;
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

            if (checkBox_ShowTxt.Checked == true)
            {
                this.Invoke((EventHandler)(delegate
                {
                    this.textBox_ComRec.AppendText("Recv: ");
                    this.textBox_ComRec.AppendText(SerialIn);				//在接收文本中添加串口接收数据
                    this.textBox_ComRec.AppendText("\r\n");

                    if (textBox_ComRec.TextLength > 32768)
                    {
                        textBox_ComRec.Text = "";
                    }
                }));        
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

			u16 CRC_Result_Cal = 0;
			u16 CRC_Result_Trans = 0;
			u8 Register_Address = 0;

			modbus_is_busy = false;
            switch (modbus_recv_data[1])									//功能码
			{
				case 0x03:													//读寄存器的值(WORD)
				{
					CRC_Result_Cal = Func_Get_CRC(modbus_recv_data, 6);		//CRC校验正确则发送数据给主站，否则等待主站再次发送数据
					CRC_Result_Trans = (u16)((((u16)modbus_recv_data[6]) << 8) | ((u16)modbus_recv_data[7]));
					if(CRC_Result_Cal == CRC_Result_Trans)
					{
						u8 Need_Read_WORD_Num;
						u32 Func_Val = 0;

						Register_Address = modbus_recv_data[2];				//寄存器值
						Need_Read_WORD_Num = modbus_recv_data[3];			//要读的WORD数
						Func_Val = ((((u32)modbus_recv_data[4]) << 8) |
									(((u32)modbus_recv_data[5]) << 0));

						Console.WriteLine("MBI Reg:{0} Num:{1} Val:{2}", Register_Address, Need_Read_WORD_Num, Func_Val);

						switch(Register_Address)
						{
							case REG_IDENTIFY:								//测试modbus通信是否正常
							{
								Console.WriteLine("REG_IDENTIFY:{0:X}", Func_Val);
								this.Invoke((EventHandler)(delegate
								{
									label_Status.Text = Func_Val.ToString();
								}));
								
								break;
							}

							case REG_MOUSE_PRESS:
							{
								Console.WriteLine("REG_MOUSE:{0:X}", Func_Val);
								break;
							}

							case REG_MOUSE_CLICK:
							{
								this.Invoke((EventHandler)(delegate
								{
									if(button_ClickLeft.Enabled == false)
									{
										button_ClickLeft.Enabled = true;
									}
									if(button_ClickRight.Enabled == false)
									{
										button_ClickRight.Enabled = true;
									}
								}));

								break;
							}

							case REG_MOUSE_SPEED:
							{
								this.Invoke((EventHandler)(delegate
								{
									if(button_SpeedUp.Enabled == false)
									{
										button_SpeedUp.Enabled = true;
									}
									if(button_SpeedDown.Enabled == false)
									{
										button_SpeedDown.Enabled = true;
									}
									if(mouse_speed_chk == false)
									{
										mouse_speed_chk = true;
									}
									label_MouseSpeed.Text = Func_Val.ToString();
								}));							

								break;
							}

							case REG_KEYBOARD:
							case REG_SYSTEM_REBOOT:
							case REG_USB_RECONNECT:
							{
								break;
							}

							default:
							{
								MessageBox.Show("error reg!!!: " + Register_Address.ToString(), "提示");
								res = false;
								break;
							}
						}
                        modbus_send_cmd_is_busy = false;

						if(Func_KB_FIFO_HasData() == true)
						{
							modbus_send_cmd_is_busy = true;
							Func_Modbus_Send_03(REG_KEYBOARD, 1, Func_KB_FIFO_Output());
						}
					}
					else
					{
						MessageBox.Show("error crc!!!: " + CRC_Result_Cal.ToString(), "提示");
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
					MessageBox.Show("error func!!!: " + modbus_recv_data[1].ToString(), "提示");
					res = false;
					break;
				}
			}
            modbus_recv_data[1] = 0xFF;                                     //取完后立马清掉，表示已经用过了

			modbus_recv_cnt = 0;
			modbus_handling = false;

            if (res == true)
            {
                modbus_success_cnt++;
            }
            else
            {
                modbus_fail_cnt++;
                modbus_send_cmd_is_busy = false;
            }
            this.Invoke((EventHandler)(delegate
            {
                label_CmdSuccessCnt.Text = "CmdCnt: ";
                label_CmdSuccessCnt.Text += modbus_success_cnt.ToString();
                label_CmdSuccessCnt.Text += " | ";
                label_CmdSuccessCnt.Text += modbus_fail_cnt.ToString();
            }));

			return res;
		}

		private void button_Modbus_Send_Click(object sender, EventArgs e)
		{
			s32 Reg;
			s32 Val;

			if(textBox_Modbus_Reg.Text.Length == 0)
			{
				Reg = 0;
			}
			else
			{
				Reg = Convert.ToInt32(textBox_Modbus_Reg.Text);
			}
			if(textBox_Modbus_Val.Text.Length == 0)
			{
				Val = 0;
			}
			else
			{
				Val = Convert.ToInt32(textBox_Modbus_Val.Text);
			}

			Func_Modbus_Send_03((u8)Reg, 1, (u32)Val);
			//MessageBox.Show(aa.ToString(), "Reg");
		}
	}
}
