﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;//使用串口
using System.ComponentModel;

namespace KMouse
{
    partial class COM
    {
        private void Update_SerialPortName(ComboBox _comboBox_COMNumber)
        {
            if(_comboBox_COMNumber.SelectedIndex == -1)
            {
                Dbg.WriteLine("Serial port not select!");
                serialport.PortName = "null";
            }
            else
            {
                string str = _comboBox_COMNumber.SelectedItem.ToString();
                int end = str.IndexOf(":");
                serialport.PortName = str.Substring(0, end);               //获得串口数
            }
        }

        private void Update_SerialBaudrate(ComboBox _comboBox_COMBaudrate)
        {
            if(_comboBox_COMBaudrate.SelectedIndex == -1)
            {
                serialport.BaudRate = 1;
            }
            else
            {
                serialport.BaudRate = Convert.ToInt32(_comboBox_COMBaudrate.SelectedItem.ToString());
            }
        }

        private void Update_SerialParityBit(ComboBox _comboBox_COMCheckBit)
        {
            if(_comboBox_COMCheckBit.SelectedIndex == -1)
            {
                serialport.Parity = Parity.Space;
            }
            else
            {
                switch(_comboBox_COMCheckBit.SelectedItem.ToString())           //获得校验位
                {
                    case "None":
                    serialport.Parity = Parity.None;
                    break;
                    case "Odd":
                    serialport.Parity = Parity.Odd;
                    break;
                    case "Even":
                    serialport.Parity = Parity.Even;
                    break;
                    default:
                    serialport.Parity = Parity.None;
                    break;
                }
            }
        }

        private void Update_SerialDataBit(ComboBox _comboBox_COMDataBit)
        {
            if(_comboBox_COMDataBit.SelectedIndex == -1)
            {
                serialport.DataBits = 1;
            }
            else
            {
                serialport.DataBits = Convert.ToInt32(_comboBox_COMDataBit.SelectedItem.ToString());
            }
        }

        private void Update_SerialStopBit(ComboBox _comboBox_COMStopBit)
        {
            if(_comboBox_COMStopBit.SelectedIndex == -1)
            {
                serialport.StopBits = StopBits.None;
            }
            else
            {
                switch(_comboBox_COMStopBit.SelectedItem.ToString())        //获得停止位
                {
                    case "0":
                    serialport.StopBits = StopBits.None;
                    break;
                    case "1":
                    serialport.StopBits = StopBits.One;
                    break;
                    case "2":
                    serialport.StopBits = StopBits.Two;
                    break;
                    case "1.5":
                    serialport.StopBits = StopBits.OnePointFive;
                    break;
                    default:
                    serialport.StopBits = StopBits.One;
                    break;
                }
            }
        }

        public void ControlModule_Init(ComboBox _comboBox_COMNumber, ComboBox _comboBox_COMBaudrate,
            ComboBox _comboBox_COMCheckBit, ComboBox _comboBox_COMDataBit, ComboBox _comboBox_COMStopBit)
        {
            //更新串口下来列表的选项
            Ruild_ComNumberList(_comboBox_COMNumber);

            //波特率
            Rebulid_BaudrateList(_comboBox_COMBaudrate);

            //校验位
            _comboBox_COMCheckBit.Items.Add("None");
            _comboBox_COMCheckBit.Items.Add("Odd");
            _comboBox_COMCheckBit.Items.Add("Even");

            //数据位
            _comboBox_COMDataBit.Items.Add("8");
            _comboBox_COMDataBit.Items.Add("7");
            _comboBox_COMDataBit.Items.Add("6");
            _comboBox_COMDataBit.Items.Add("5");

            //停止位
            _comboBox_COMStopBit.Items.Add("1");
            _comboBox_COMStopBit.Items.Add("2");
            _comboBox_COMStopBit.Items.Add("1.5");
            
            if((_comboBox_COMNumber.Items.Count > 0)
             && (Param.ini.com_select < _comboBox_COMNumber.Items.Count))    //串口列表选用号
            {
                _comboBox_COMNumber.SelectedIndex = Param.ini.com_select;
            }
            else
            {
                _comboBox_COMNumber.SelectedIndex = -1;
            }
            Update_SerialPortName(_comboBox_COMNumber);
            
            _comboBox_COMBaudrate.SelectedIndex = Param.ini.com_baudrate;
            Update_SerialBaudrate(_comboBox_COMBaudrate);

            _comboBox_COMCheckBit.SelectedIndex = 0;
            Update_SerialParityBit(_comboBox_COMCheckBit);

            _comboBox_COMDataBit.SelectedIndex = 0;
            Update_SerialDataBit(_comboBox_COMDataBit);

            _comboBox_COMStopBit.SelectedIndex = 0;
            Update_SerialStopBit(_comboBox_COMStopBit);
        }

        int combobox_comnumber_width_bak = 0;
        public void comboBox_COMNumber_DropDown(object sender)
        {
            ComboBox _comboBox_COMNumber = sender as ComboBox;

            combobox_comnumber_width_bak = _comboBox_COMNumber.Width;
            _comboBox_COMNumber.Width = 320;

            Ruild_ComNumberList(_comboBox_COMNumber);

            _comboBox_COMNumber.SelectedIndex = -1;
            Update_SerialPortName(_comboBox_COMNumber);
        }

        public void comboBox_COMNumber_SelectedIndexChanged(object sender)
        {
            ComboBox _comboBox_COMNumber = sender as ComboBox;

            Update_SerialPortName(_comboBox_COMNumber);
        }

        public void comboBox_COMNumber_DropDownClosed(object sender)
        {
            ComboBox _comboBox_COMNumber = sender as ComboBox;

            _comboBox_COMNumber.Width = combobox_comnumber_width_bak;
        }

        public void comboBox_COMBaudrate_SelectedIndexChanged(object sender)
        {
            ComboBox _comboBox_COMBaudrate = sender as ComboBox;

            Update_SerialBaudrate(_comboBox_COMBaudrate);

            //在串口运行的时候更改波特率，串口关闭时候修改的时候直接在按钮函数里改就行了
            if(serialport.IsOpen == true)
            {
                try
                {
                    serialport.Close();
                    serialport.Open();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Can't open the COM port " + ex.Message + Dbg.GetStack(), "Attention!");
                }
            }
        }

        public void comboBox_COMCheckBit_SelectedIndexChanged(object sender)
        {
            ComboBox _comboBox_COMCheckBit = sender as ComboBox;
            Update_SerialParityBit(_comboBox_COMCheckBit);
        }

        public void comboBox_COMDataBit_SelectedIndexChanged(object sender)
        {
            ComboBox _comboBox_COMDataBit = sender as ComboBox;
            Update_SerialDataBit(_comboBox_COMDataBit);
        }

        public void comboBox_COMStopBit_SelectedIndexChanged(object sender)
        {
            ComboBox _comboBox_COMStopBit = sender as ComboBox;
            Update_SerialStopBit(_comboBox_COMStopBit);
        }


        public void button_ComOpen_Click(object sender, FormMain fm)
        {
            //timer_CloseSerialPort.SynchronizingObject = Callback_CloseCOM;//跨线程访问的控件

            Dbg.WriteLine("COM open or close:{0}", serialport.IsOpen);

            Button _button_ComOpen = sender as Button;

            if(serialport.IsOpen == true)       //通过定时器来关闭串口
            {
                _button_ComOpen.Enabled = false;

                timer_CloseSerialPort.Enabled = true;
                timer_CloseSerialPort.fm = fm;
                timer_CloseSerialPort.delegate_callback = fm.SetComStatus;
            }
            else
            {
                fm.SetComStatus(Open());
            }
        }
    }
}
