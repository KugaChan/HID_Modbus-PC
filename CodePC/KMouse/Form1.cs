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
	public partial class KMouse : Form
	{
		//常量
		private const u8 _VersionGit = 2;

		//宏
		const u32 dwAllFF = 0xFFFFFFFF;

		//串口使用的变量
		
		private const float Tag_VersionNum = 1.0F;

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
		//窗体使用的变量
		private bool form_is_closed = false;

		public KMouse()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			s32 i;

            this.Text = "KMouse Git" + _VersionGit.ToString();
			
			/*********************初始化鼠标按键 start**********************/
			for(i = 0; i < KEY_MousePress_ALL; i++)
			{
				mouse_press_en[i] = false;
				mouse_press_en_last[i] = false;
			}
			/*********************初始化鼠标按键 end************************/


            /********************更新串口下来列表的选项-start******************/
            string[] strArr = Func_GetHarewareInfo(HardwareEnum.Win32_PnPEntity, "Name");
            int SerialNum = 0;

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
                SerialNum++;
            }
            /********************更新串口下来列表的选项-end********************/

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

            if((SerialNum > 0) && (Properties.Settings.Default._com_num_select_index < SerialNum))    //串口列表选用号
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
			com.DataReceived += Func_COM_DataRec;//指定串口接收函数

			Func_COM_Open();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)   //窗体关闭函数
		{ //关闭的时候保存参数            

			if(com_is_receiving == true)
			{
                com_allow_receive = false;
                form_is_closed = true;
				e.Cancel = true;//取消窗体的关闭
			}
			else
			{
                try
                {
                    com.Close();
                }
                catch
                {
                    MessageBox.Show("无法关闭串口", "提示");
                }

                Func_PropertiesSettingsSave();
			}

			notifyIcon.Dispose();
            System.Environment.Exit(0);     //把netcom线程也结束了
			//MessageBox.Show("是否关闭KCOM", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
		}		

		private char Func_GetHexHigh(byte n, byte mode)
		{
			char result = ' ';
			int check;
			if(mode == 0)
			{
				check = n >> 4;//返回高位
			}
			else
			{
				check = n & 0x0F;
			}

			switch (check)
			{
				case 0: result = '0'; break;
				case 1: result = '1'; break;
				case 2: result = '2'; break;
				case 3: result = '3'; break;
				case 4: result = '4'; break;
				case 5: result = '5'; break;
				case 6: result = '6'; break;
				case 7: result = '7'; break;
				case 8: result = '8'; break;
				case 9: result = '9'; break;
				case 10: result = 'A'; break;
				case 11: result = 'B'; break;
				case 12: result = 'C'; break;
				case 13: result = 'D'; break;
				case 14: result = 'E'; break;
				case 15: result = 'F'; break;
			}

			return result;
		}

        private void Func_PropertiesSettingsSave()
        {
            Properties.Settings.Default.Save();       
        }		

		private void timer_recv_timeout_Tick(object sender, EventArgs e)	//10ms
		{
			//Console.WriteLine(DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
			//Console.WriteLine("modbus_recv_timeout:{0}", modbus_recv_timeout);
            if (modbus_is_busy == true)
            {
                modbus_respone_timeout++;
                if (modbus_respone_timeout == 100)
                {
                    modbus_respone_timeout = 0;

                    MessageBox.Show("Modbus响应超时!", "警告");

                    bool res;
                    res = Func_Modbus_Recv_Handle();

                    if (res == false)
                    {
                        Func_COM_Close();
                    }
                }
            }

			if(modbus_recv_timeout != dwAllFF)
			{
				modbus_recv_timeout++;
				if(modbus_recv_timeout == 100)	//1s后modbus接收超时
				{
					modbus_recv_timeout = dwAllFF;
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
					MessageBox.Show(SerialIn, "Warning, Modbus Recv timeout!!!");
				}
			}

			if((com_is_open == true) && (modbus_is_busy == false))
			{
				for(u32 i = 0; i < KEY_MousePress_ALL; i++)
				{
					if(mouse_press_en[i] == true)
					{
						Func_Modbus_Send_03(REG_MOUSE_PRESS, 1, i);
					}
					else if(mouse_press_en_last[i] == true)					//松开按键的那一下
					{
						Func_Modbus_Send_03(REG_MOUSE_PRESS, 1, KEY_MOUSE_Leave);
					}
					mouse_press_en_last[i] = mouse_press_en[i];				
				}
			}
		}

		private void KMouse_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		private void KMouse_SizeChanged(object sender, EventArgs e)
		{
			if(this.WindowState == FormWindowState.Minimized)  //判断是否最小化
			{
				this.ShowInTaskbar = false; //不显示在系统任务栏
				notifyIcon.Visible = true;  //托盘图标可见
			}
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			//判断是否已经最小化于托盘 
			//if(WindowState == FormWindowState.Minimized)
			{
				WindowState = FormWindowState.Normal;	//还原窗体显示 
				this.Activate();						//激活窗体并给予它焦点 
				this.ShowInTaskbar = true;				//任务栏区显示图标 
				notifyIcon.Visible = false;				//托盘区图标隐藏 
			}
		}


	
	}
}
