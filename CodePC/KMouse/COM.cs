using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;//使用串口
using System.Management;


namespace KMouse
{
    partial class COM
    {
        const int COM_BUFFER_SIZE_MAX = 4096;

		public int recv_cnt;
        public SerialPort serialport = new SerialPort();

        private bool com_is_receiving = false;
        private MyTimer timer_CloseSerialPort;

        int[] badurate_array = 
		{
			4800,
			9600,
			19200,
			38400,
			115200,
            128000,
            230400,
            256000,
            460800,
            921600,
            1222400,
			1382400
		};

        Modbus mdbs;
        public void Init(Modbus _mdbs)
        {
            mdbs = _mdbs;
            
            serialport.DataReceived += Func_COM_DataRec;//指定串口接收函数
            serialport.ReadBufferSize = COM_BUFFER_SIZE_MAX;
            serialport.WriteBufferSize = COM_BUFFER_SIZE_MAX;

            timer_CloseSerialPort = new MyTimer();
            timer_CloseSerialPort.Elapsed += new System.Timers.ElapsedEventHandler(timer_CloseSerialPort_ticks);
            timer_CloseSerialPort.AutoReset = true;
            timer_CloseSerialPort.Enabled = false;
            timer_CloseSerialPort.Interval = 500;
        }

        public void Rebulid_BaudrateList(ComboBox _comboBox_COMBaudrate)
        {
            _comboBox_COMBaudrate.Items.Clear();
            //波特率
            for(int i = 0; i < badurate_array.Length; i++)
            {
                _comboBox_COMBaudrate.Items.Add(badurate_array[i].ToString());
            }
        }

        public void Ruild_ComNumberList(ComboBox _comboBox_COMNumber)
        {
            _comboBox_COMNumber.Items.Clear();
            string[] strArr = Func.GetHarewareInfo(Func.HardwareEnum.Win32_PnPEntity, "Name");
            foreach(string vPortName in SerialPort.GetPortNames())
            {
                string SerialIn = "";
                SerialIn += vPortName;
                SerialIn += ':';
                foreach(string s in strArr)
                {
                    if(s.Contains(vPortName))
                    {
                        SerialIn += s;
                    }
                }
                Console.WriteLine(SerialIn);
                _comboBox_COMNumber.Items.Add(SerialIn);                    //将设备列表里的COM放进下拉菜单上
            }
        }

        public bool Close()
        {
            /****************串口异常断开则直接关闭窗体 Start**************/
            bool current_com_exist = false;

            string[] strArr = Func.GetHarewareInfo(Func.HardwareEnum.Win32_PnPEntity, "Name");
            foreach(string vPortName in SerialPort.GetPortNames())
            {
                if(vPortName == serialport.PortName)
                {
                    current_com_exist = true;                           //当前串口还在设备列表里
                }
            }

            //关闭串口时发现正在使用的COM不见了，由于无法调用serialport.close()，所以只能异常退出了
            if(current_com_exist == false)
            {
                MessageBox.Show("COM is lost, KCOM forces to close!!!", "Warning!!!");
                System.Environment.Exit(0);
            }
            /****************串口异常断开则直接关闭窗体 End****************/

            recv_cnt = 0;
            if(com_is_receiving == true)
            {
                Console.WriteLine("COM is receving data, Wait...");
                return false;
            }

            try
            {
                serialport.Close();
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Can't close the COM port");
                return false;
            }
        }

        public bool Open()
        {
            Console.WriteLine("PortName:{0}", serialport.PortName);
            Console.WriteLine("Baudrate:{0}", serialport.BaudRate);
            Console.WriteLine("Parity:{0}", serialport.Parity);
            Console.WriteLine("Data:{0}", serialport.DataBits);
            Console.WriteLine("Stop:{0}", serialport.StopBits);

            if((serialport.PortName == "null") ||
                (serialport.BaudRate == 1) ||
                (serialport.Parity == Parity.Space) ||
                (serialport.DataBits == 1))
            {
                MessageBox.Show("Please choose the COM port" + Dbg.GetStack(), "Attention!");
                return false;
            }

            try
            {
                serialport.Open();

                serialport.DiscardInBuffer();
                serialport.DiscardOutBuffer();
            }
            catch
            {
                MessageBox.Show("Can't open the COM port", "Attention!");
                return false;
            }

            if(serialport.IsOpen == true)
            {
                com_is_receiving = false;
                return true;
            }
            else
            {
                return false;
            }
        }
		
        private void Func_COM_DataRec(object sender, SerialDataReceivedEventArgs e)  //串口接受函数
		{
            Console.WriteLine("en:{0}", timer_CloseSerialPort.Enabled);
            if(timer_CloseSerialPort.Enabled == true)
            {
                return;
            }

            Dbg.Assert(serialport.IsOpen == true, "###serial port is closed, can recv data!");

            com_is_receiving = true;
            int com_recv_buff_size;
            byte[] com_recv_buffer = new byte[serialport.ReadBufferSize + 1];
				
            com_recv_buff_size = serialport.Read(com_recv_buffer, 0, serialport.ReadBufferSize);

            Console.Write("MCU->PC:");
            for(int v = 0; v < com_recv_buff_size; v++)
            {
                Console.Write(" {0:X}", com_recv_buffer[v]);
                if(com_recv_buff_size > 20)
                {
                    Console.Write("...");
                    break;
                }
            }
            Console.Write("\r\n");

            if((com_recv_buff_size == 0) || (com_recv_buff_size > Modbus.RECV_MAX_LEN))
            {
                com_is_receiving = false;
                return;
            }

            recv_cnt += com_recv_buff_size;

            mdbs.Rcv_Data(com_recv_buffer, com_recv_buff_size);

            com_is_receiving = false;
            return;
        }

        void timer_CloseSerialPort_ticks(object sender, EventArgs e)
        {
            bool res = Close();

            Console.WriteLine("Close COM:{0}\n", res);

            if(res == true)
            {
                timer_CloseSerialPort.Enabled = false;

                timer_CloseSerialPort.fm.Invoke((EventHandler)(delegate
                {
                    timer_CloseSerialPort.delegate_callback(false);
                }));
            }
            else
            {
                Console.WriteLine("#Close COM fail, try again:{0}\n", timer_CloseSerialPort.Enabled);
            }
        }
    }
}
