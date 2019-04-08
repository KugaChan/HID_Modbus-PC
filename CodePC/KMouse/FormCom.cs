using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;//使用串口
using System.Management;


namespace KMouse
{
    class COM
    {
		public int recv_cnt;
        public SerialPort serialport = new SerialPort();

        private bool com_is_receiving = false;

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

        ComboBox comboBox_COMNumber;
        ComboBox comboBox_COMBaudrate;
        ComboBox comboBox_COMCheckBit;
        ComboBox comboBox_COMDataBit;
        ComboBox comboBox_COMStopBit;

        Modbus mdbs;
        public void Init(
            Modbus _mdbs,
            ComboBox _comboBox_COMNumber,
            ComboBox _comboBox_COMBaudrate,
            ComboBox _comboBox_COMCheckBit,
            ComboBox _comboBox_COMDataBit,
            ComboBox _comboBox_COMStopBit)
        {
            mdbs = _mdbs;

            comboBox_COMNumber = _comboBox_COMNumber;
            comboBox_COMBaudrate = _comboBox_COMBaudrate;
            comboBox_COMCheckBit = _comboBox_COMCheckBit;
            comboBox_COMDataBit = _comboBox_COMDataBit;
            comboBox_COMStopBit = _comboBox_COMStopBit;

            int i;

            //更新串口下来列表的选项
            build_com_list(comboBox_COMNumber);

            //波特率
            for(i = 0; i < badurate_array.Length; i++)
            {
                comboBox_COMBaudrate.Items.Add(badurate_array[i].ToString());
            }

            //校验位
            comboBox_COMCheckBit.Items.Add("None");
            comboBox_COMCheckBit.Items.Add("Odd");
            comboBox_COMCheckBit.Items.Add("Even");

            //数据位
            comboBox_COMDataBit.Items.Add("8");
            comboBox_COMStopBit.Items.Add("0");

            //停止位
            comboBox_COMStopBit.Items.Add("1");
            comboBox_COMStopBit.Items.Add("2");
            comboBox_COMStopBit.Items.Add("1.5");

            if((comboBox_COMNumber.Items.Count > 0) 
            && (Properties.Settings.Default._com_num_select_index < comboBox_COMNumber.Items.Count))    //串口列表选用号
            {
                comboBox_COMNumber.SelectedIndex = Properties.Settings.Default._com_num_select_index;
            }
            else
            {
                comboBox_COMNumber.SelectedIndex = -1;
            }

            comboBox_COMBaudrate.SelectedIndex = Properties.Settings.Default._baudrate_select_index;
            comboBox_COMCheckBit.SelectedIndex = 0;
            comboBox_COMDataBit.SelectedIndex = 0;
            comboBox_COMStopBit.SelectedIndex = 1;

            serialport.DataReceived += Func_COM_DataRec;//指定串口接收函数
        }

        public void build_com_list(ComboBox _comboBox_COMNumber)
        {
            _comboBox_COMNumber.Items.Clear();
            string[] strArr = Func.GetHarewareInfo(Func.HardwareEnum.Win32_PnPEntity, "Name");
            foreach(string vPortName in SerialPort.GetPortNames())
            {
                String SerialIn = "";
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
            String port_name_try = comboBox_COMNumber.SelectedItem.ToString();
            comboBox_COMCheckBit.Enabled = true;
            comboBox_COMDataBit.Enabled = true;
            comboBox_COMNumber.Enabled = true;
            comboBox_COMStopBit.Enabled = true;

            /****************串口异常断开则直接关闭窗体 Start**************/
            String PortName = port_name_try;
            int end = PortName.IndexOf(":");
            PortName = PortName.Substring(0, end);                      //截取获得COM口序号

            bool current_com_exist = false;

            string[] strArr = Func.GetHarewareInfo(Func.HardwareEnum.Win32_PnPEntity, "Name");
            foreach(string vPortName in SerialPort.GetPortNames())
            {
                if(vPortName == PortName)
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
                Console.WriteLine("COM_IsReceving == true, Get out!");
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
            serialport.BaudRate = Convert.ToInt32(comboBox_COMBaudrate.SelectedItem.ToString());   //获得波特率
            //Console.Write("@@@@{0}", serialport.BaudRate);
            switch(comboBox_COMCheckBit.SelectedItem.ToString())                            //获得校验位
            {
                case "None": serialport.Parity = Parity.None; break;
                case "Odd": serialport.Parity = Parity.Odd; break;
                case "Even": serialport.Parity = Parity.Even; break;
                default: serialport.Parity = Parity.None; break;
            }
            serialport.DataBits = Convert.ToInt16(comboBox_COMDataBit.SelectedItem.ToString());    //获得数据位
            switch(comboBox_COMStopBit.SelectedItem.ToString())                             //获得停止位
            {
                case "0": serialport.StopBits = StopBits.None; break;
                case "1": serialport.StopBits = StopBits.One; break;
                case "2": serialport.StopBits = StopBits.Two; break;
                case "1.5": serialport.StopBits = StopBits.OnePointFive; break;
                default: serialport.StopBits = StopBits.One; break;
            }

            if(comboBox_COMNumber.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose the COM port", "Attention!");
                return false;
            }

            String PortName = comboBox_COMNumber.SelectedItem.ToString();
            Console.WriteLine("Port name:{0}", PortName);
            int end = PortName.IndexOf(":");
            serialport.PortName = PortName.Substring(0, end);                  //获得串口数
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

                //comboBox_COMBaudrate.Enabled = false;
                comboBox_COMCheckBit.Enabled = false;
                comboBox_COMDataBit.Enabled = false;
                comboBox_COMNumber.Enabled = false;
                comboBox_COMStopBit.Enabled = false;

                return true;
            }
            else
            {
                return false;
            }
        }
		
        private void Func_COM_DataRec(object sender, SerialDataReceivedEventArgs e)  //串口接受函数
		{
            Dbg.Assert(serialport.IsOpen == true, "###serial port is closed, can recv data!");

            com_is_receiving = true;
            int com_recv_buff_size;
            byte[] com_recv_buffer = new byte[serialport.ReadBufferSize + 1];
				
            com_recv_buff_size = serialport.Read(com_recv_buffer, 0, serialport.ReadBufferSize);

            Console.Write("MCU->PC:");
            for(int v = 0; v < com_recv_buff_size; v++)
            {
                Console.Write(" {0:X}", com_recv_buffer[v]);
            }
            Console.Write("\r\n");

            if((com_recv_buff_size == 0) || (com_recv_buff_size > Modbus.RECV_MAX_LEN))
            {
                return;
            }

            recv_cnt += com_recv_buff_size;

            mdbs.Rcv_Data(com_recv_buffer, com_recv_buff_size);

            com_is_receiving = false;
            return;
        }
	}
}
