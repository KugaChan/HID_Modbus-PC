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
using System.Collections.Concurrent;    //使用ConcurrentQueue

namespace KMouse
{
	public partial class FormMain : Form
	{
        public Queue<string> queue_message = new Queue<string>();

        keyQ kq = new keyQ();
        COM com = new COM();

		//常量
		private const byte _VersionGit = 18;

        Modbus mdbs = new Modbus();

		public FormMain()
		{
			InitializeComponent();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
            this.Text = "KMouse Git" + _VersionGit.ToString();

            textBox_eKey.Enabled = false;
            textBox_eKey.Text = Properties.Settings.Default.eKey_string;

            com.me.comboBox_COMNumber = comboBox_COMNumber;
            com.me.comboBox_COMBaudrate = comboBox_COMBaudrate;
            com.me.comboBox_COMCheckBit = comboBox_COMCheckBit;
            com.me.comboBox_COMDataBit = comboBox_COMDataBit;
            com.me.comboBox_COMStopBit = comboBox_COMStopBit;

            com.Init(mdbs);
            bool res = com.Open();         
            if(res == true)
            {
                button_COMOpen.Text = "COM is opened";
                button_COMOpen.ForeColor = System.Drawing.Color.Green;             
            }

            kq.Init(queue_message);

            mdbs.Init(kq, com.serialport, queue_message,
                Action_UpdateModbussState,
                Delegate_ModbusCallBack_Identify,
                Delegate_ModbusCallBack_Click,
                Delegate_ModbusCallBack_Speed);
            mdbs.echo_en = checkBox_ShowTxt.Checked;
		}

        private void timer_CloseForm_Tick(object sender, EventArgs e)
        {
            if(com.serialport.IsOpen == true)
            {
                bool res = com.Close();
                if(res == false)
                {
                    return;
                }     
            }

            kq.Close();
            mdbs.Close();

            Func_PropertiesSettingsSave();  //关闭的时候保存参数

			notifyIcon.Dispose();
            this.Close();
            System.Environment.Exit(0);     //把netcom线程也结束了
        }

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)   //窗体关闭函数
		{
            e.Cancel = true;//取消窗体的关闭
            timer_CloseForm.Enabled = true;
		}

        private void Func_PropertiesSettingsSave()
        {
            Properties.Settings.Default._baudrate_select_index = comboBox_COMBaudrate.SelectedIndex;
            Properties.Settings.Default.eKey_string = textBox_eKey.Text;
            Properties.Settings.Default._com_num_select_index = comboBox_COMNumber.SelectedIndex;

            Properties.Settings.Default.Save();       
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

        private void button_eKeyClear_Click(object sender, EventArgs e)
        {
            textBox_eKey.Text = "";
        }

        private void checkBox_EKeyEN_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox_eKey.Enabled == true)
            {
                textBox_eKey.Enabled = false;
            }
            else
            {
                textBox_eKey.Enabled = true;
            }
        }

        private void button_Modbus_Send_Click(object sender, EventArgs e)
		{
			byte Reg;
			uint Val;

			if(textBox_Modbus_Reg.Text.Length == 0)
			{
				Reg = 0;
			}
			else
			{
				Reg = Convert.ToByte(textBox_Modbus_Reg.Text);
			}
			if(textBox_Modbus_Val.Text.Length == 0)
			{
				Val = 0;
			}
			else
			{
				Val = Convert.ToUInt32(textBox_Modbus_Val.Text);
			}

			mdbs.Send_03((Modbus.REG)Reg, 1, Val);
		}

        private void timer_background_Tick(object sender, EventArgs e)
        {
            if(queue_message.Count > 0)
            {
                textBox_ComRec.AppendText("\r\n" + queue_message.Dequeue());
            }
        }

        void Action_UpdateModbussState()
        {
            this.Invoke((EventHandler)(delegate
            {
                label_SuccessCmdCnt.Text = "Success: ";
                label_SuccessCmdCnt.Text += mdbs.success_cnt.ToString();

                label_FailCmdCnt.Text = "Fail: ";
                label_FailCmdCnt.Text += mdbs.fail_cnt.ToString();
            }));
        }

        void Delegate_ModbusCallBack_Identify(uint value)
        {
            this.Invoke((EventHandler)(delegate
            {
                label_Status.Text = value.ToString();
            }));
        }

        void Delegate_ModbusCallBack_Click(uint value)
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
        }

        void Delegate_ModbusCallBack_Speed(uint value)
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
			    if(kq.mouse_speed_chk == false)
			    {
				    kq.mouse_speed_chk = true;
			    }
			    label_MouseSpeed.Text = value.ToString();
            }));
        }

        private void checkBox_ShowTxt_CheckedChanged(object sender, EventArgs e)
        {
            mdbs.echo_en = checkBox_ShowTxt.Checked;
        }


        /********************与串口控制相关的 Start***************************/
        private void label_ClearRec_DoubleClick(object sender, EventArgs e)
        {
            textBox_ComRec.Text = "";
            label_Rec_Bytes.Text = "0";
            com.recv_cnt = 0;
        }

        private void comboBox_COMBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(com.serialport.IsOpen == false)
            {
                return;
            }

            bool res;

            res = com.Close();
            if(res == true)
            {
                button_COMOpen.Text = "COM is closed";
                button_COMOpen.ForeColor = System.Drawing.Color.Red;            
            }

            res = com.Open();         
            if(res == true)
            {
                button_COMOpen.Text = "COM is opened";
                button_COMOpen.ForeColor = System.Drawing.Color.Green;             
            }
        }

        private void comboBox_COMNumber_DropDown(object sender, EventArgs e)
        {
            com.build_com_list(comboBox_COMNumber);
            comboBox_COMNumber.SelectedIndex = -1;
        }

        private void comboBox_COMNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button_ComOpen_Click(object sender, EventArgs e)
        {
            if(com.serialport.IsOpen == true)
            {   
                button_COMOpen.Enabled = false;
                timer_CloseCom.Enabled = true;
            }
            else
            {
                bool res = com.Open();
                if(res == true)
                {
                    button_COMOpen.Text = "COM is opened";
                    button_COMOpen.ForeColor = System.Drawing.Color.Green;             
                }
            }
        }

        private void timer_CloseCom_Tick(object sender, EventArgs e)
        {
            bool res = com.Close();
            if(res == true)
            {
                button_COMOpen.Enabled = true;
                timer_CloseCom.Enabled = false;

                button_COMOpen.Text = "COM is closed";
                button_COMOpen.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                Console.WriteLine("#Close COM fail, try again!\n");
            }
        }
        /********************与串口控制相关的 End***************************/
    }
}
