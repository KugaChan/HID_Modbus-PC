using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;//使用串口
using System.Management;

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
		private u32 com_recv_cnt;
        private bool com_is_receiving = false;
        private bool com_allow_receive = true;
        //功能
        private SerialPort com = new SerialPort();
        private bool com_is_open = false;
		private bool com_change_baudrate = false;


        private u32 modbus_success_cnt;
        private u32 modbus_fail_cnt;

        public enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }

        /// <summary>
        /// Get the system devices information with windows api.
        /// </summary>
        /// <param name="hardType">Device type.</param>
        /// <param name="propKey">the property of the device.</param>
        /// <returns></returns>
        private static string[] Func_GetHarewareInfo(HardwareEnum hardType, string propKey)
        {
            List<string> strs = new List<string>();
            try
            {
                using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach(var hardInfo in hardInfos)
                    {
                        if(hardInfo.Properties[propKey].Value != null)
                        {
                            String str = hardInfo.Properties[propKey].Value.ToString();
                            strs.Add(str);
                        }
                    }
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                strs = null;
            }
        }

        private void label_ClearRec_DoubleClick(object sender, EventArgs e)
        {
            textBox_ComRec.Text = "";
            label_Rec_Bytes.Text = "0";
            com_recv_cnt = 0;
        }

        private void comboBox_COMBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
			Properties.Settings.Default._baudrate_select_index = comboBox_COMBaudrate.SelectedIndex;

            if(com_is_open == true) //在串口运行的时候更改波特率，串口关闭时候修改的时候直接在按钮函数里改就行了
            {
                if(com_is_receiving == true)
                {
                    com_allow_receive = false;
					com_change_baudrate = true;
                }
                else
                {
                    com.BaudRate = Convert.ToInt32(comboBox_COMBaudrate.SelectedItem.ToString());//赋值给串口

                    try
                    {
                        com.Close();
                        com.Open();
                    }
                    catch
                    {
                        MessageBox.Show("无法打开串口", "提示");
                    }
                }
                com_recv_cnt = 0;
            }
        }

        private void comboBox_COMNumber_DropDown(object sender, EventArgs e)
        {
            comboBox_COMNumber.Items.Clear();

            string[] strArr = Func_GetHarewareInfo(HardwareEnum.Win32_PnPEntity, "Name");

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
                comboBox_COMNumber.Items.Add(SerialIn);
            }

            comboBox_COMNumber.SelectedIndex = -1;
        }

		private String port_name_try;   //记录当前使用的COM的名字，由于是多线程访问，这个变量必须放在外面
		public void Func_COM_Close()
		{
			if(com_is_open == true)
			{
				/****************串口异常断开则直接关闭窗体 Start**************/
				int get_port_name_cnt = 0;
				while(true)
				{
					bool get_port_name_sta = false;

					try
					{
						this.Invoke((EventHandler)(delegate
						{
							comboBox_COMCheckBit.Enabled = true;
							comboBox_COMDataBit.Enabled = true;
							comboBox_COMNumber.Enabled = true;
							comboBox_COMStopBit.Enabled = true;

							port_name_try = comboBox_COMNumber.SelectedItem.ToString();
						}));
						get_port_name_sta = true;
					}
					catch(Exception ex)
					{
						//Console.WriteLine(ex.Message);
						get_port_name_cnt++;
						if(get_port_name_cnt % 999 == 0)
						{
							MessageBox.Show(ex.Message, "无法关闭串口");
							while(true) ;//发生这种情况会怎么样...
						}
					}

					if(get_port_name_sta == true)
					{
						break;
					}
					//System.Threading.Thread.Sleep(10);
				}

				String PortName = port_name_try;
				int end = PortName.IndexOf(":");
				PortName = PortName.Substring(0, end);                      //截取获得COM口序号

				bool current_com_exist = false;
				string[] strArr = Func_GetHarewareInfo(HardwareEnum.Win32_PnPEntity, "Name");
				foreach(string vPortName in SerialPort.GetPortNames())
				{
					if(vPortName == PortName)
					{
						current_com_exist = true;                           //当前串口还在设备列表里
					}

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
					this.Invoke((EventHandler)(delegate
					{
						comboBox_COMNumber.Items.Add(SerialIn);             //将设备列表里的COM放进下拉菜单上
					}));
				}

				//关闭串口时发现正在使用的COM不见了，由于无法调用com.close()，所以只能异常退出了
				if(current_com_exist == false)
				{
					MessageBox.Show("串口已失去连接，程序关闭!!!", "警告");
					System.Environment.Exit(0);
				}
				/****************串口异常断开则直接关闭窗体 End****************/

				com_allow_receive = false;//禁止接收数据
				if(com_is_receiving == true)
				{
					Console.WriteLine("COM_IsReceving == true, Get out!");
					return;
				}

				try
				{
					com.Close();
					com_is_open = false;
					this.Invoke((EventHandler)(delegate
					{
						button_COMOpen.Text = "串口已关";
						button_COMOpen.ForeColor = System.Drawing.Color.Red;
					}));
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message, "无法关闭串口");
				}
			}

			if(form_is_closed == true)
			{
				Func_PropertiesSettingsSave();

				this.Invoke((EventHandler)(delegate
				{
					this.Close();
				}));
			}

			if(com_change_baudrate == true)
			{
				com_change_baudrate = false;
				this.Invoke((EventHandler)(delegate
				{
					com.BaudRate = Convert.ToInt32(comboBox_COMBaudrate.SelectedItem.ToString());//赋值给串口
				}));

				try
				{
					com.Open();
					this.Invoke((EventHandler)(delegate
					{
						button_COMOpen.Text = "串口已开";
						button_COMOpen.ForeColor = System.Drawing.Color.Green;
						com_is_open = true;
						com_allow_receive = true;
					}));
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message, "无法打开串口");
				}
			}
		}

		public void Func_COM_Open()
		{
			if(com_is_open == true)//关闭串口
			{
				Func_COM_Close();
				//comboBox_COMBaudrate.Enabled = true;
				comboBox_COMCheckBit.Enabled = true;
				comboBox_COMDataBit.Enabled = true;
				comboBox_COMNumber.Enabled = true;
				comboBox_COMStopBit.Enabled = true;
			}
			else//打开串口
			{
				com.BaudRate = Convert.ToInt32(comboBox_COMBaudrate.SelectedItem.ToString());   //获得波特率
				//Console.Write("@@@@{0}", com.BaudRate);
				switch(comboBox_COMCheckBit.SelectedItem.ToString())                           //获得校验位
				{
					case "None": com.Parity = Parity.None; break;
					case "Odd": com.Parity = Parity.Odd; break;
					case "Even": com.Parity = Parity.Even; break;
					default: com.Parity = Parity.None; break;
				}
				com.DataBits = Convert.ToInt16(comboBox_COMDataBit.SelectedItem.ToString());    //获得数据位
				switch(comboBox_COMStopBit.SelectedItem.ToString())                            //获得停止位
				{
					case "0": com.StopBits = StopBits.None; break;
					case "1": com.StopBits = StopBits.One; break;
					case "2": com.StopBits = StopBits.Two; break;
					case "1.5": com.StopBits = StopBits.OnePointFive; break;
					default: com.StopBits = StopBits.One; break;
				}

				if(comboBox_COMNumber.SelectedIndex == -1)
				{
					MessageBox.Show("请选择串口", "提示");
					return;
				}

				String PortName = comboBox_COMNumber.SelectedItem.ToString();
				Console.WriteLine("Port name:{0}", PortName);
				int end = PortName.IndexOf(":");
				com.PortName = PortName.Substring(0, end);                  //获得串口数
				try
				{
					com.Open();

					com.DiscardInBuffer();
					com.DiscardOutBuffer();

					com_is_open = true;
				}
				catch
				{
					com_is_open = false;
					MessageBox.Show("无法打开串口", "提示");
				}

				if(com_is_open == true)
				{
					button_COMOpen.Text = "串口已开";
					button_COMOpen.ForeColor = System.Drawing.Color.Green;

					com_allow_receive = true;
					com_is_receiving = false;

					//comboBox_COMBaudrate.Enabled = false;
					comboBox_COMCheckBit.Enabled = false;
					comboBox_COMDataBit.Enabled = false;
					comboBox_COMNumber.Enabled = false;
					comboBox_COMStopBit.Enabled = false;
				}
			}		
		}

        private void button_ComOpen_Click(object sender, EventArgs e)
        {
			Func_COM_Open();
        }

        private void comboBox_COMNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default._com_num_select_index = comboBox_COMNumber.SelectedIndex;
        }
		
        private void Func_COM_DataRec(object sender, SerialDataReceivedEventArgs e)  //串口接受函数
		{
            if((com_allow_receive == true) && (modbus_handling == false))
            {
                com_is_receiving = true;
                s32 com_recv_buff_size;
                byte[] com_recv_buffer = new byte[com.ReadBufferSize + 1];
                //String SerialIn = "";										//把接收到的数据转换为字符串放在这里
				
                com_recv_buff_size = com.Read(com_recv_buffer, 0, com.ReadBufferSize);

				Console.WriteLine("Com IN:{0}|{1}", com_recv_buff_size, com.ReadBufferSize);

                if((com_recv_buff_size == 0) || (com_recv_buff_size > MODBUS_RECV_MAX_LEN))
                {
                    return;
                }

                com_recv_cnt += (u32)com_recv_buff_size;

				if(com_is_open == true)
				{
					for(int i = 0; i < com_recv_buff_size; i++)
					{
						Console.WriteLine("modbus_recv_cnt:{0} i:{1}", modbus_recv_cnt, i);
						Byte a = com_recv_buffer[i];
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
						Console.WriteLine("modbus_recv_cnt:{0} modbus_recv_num:{1}", modbus_recv_cnt, modbus_recv_num);
						if(modbus_recv_cnt == modbus_recv_num)
						{
							bool res;
							res = Func_Modbus_Recv_Handle();
							if(res == false)
							{
								Func_COM_Close();
							}
						}
					}

					if(modbus_recv_cnt != 0)								//modbus指令还没收完，启动定时器
					{
						modbus_recv_timeout = 0;
					}
					else
					{
						modbus_recv_timeout = dwAllFF;
					}

					com_is_receiving = false;
					return;
				}

				//textBox_ComRec的内容在modbus.cs里更新
			}
			else
			{
				Func_COM_Close();
			}
		}
	}
}
