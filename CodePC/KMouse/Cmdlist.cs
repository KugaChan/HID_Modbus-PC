using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMouse
{
    class Cmdlist
    {
        public struct CMD
        {
            public const string BOOT_PC = "boot";
            public const string SHUTDOWN_PC = "shutdown";
            public const string IO_HIGH = "high";
            public const string IO_LOW = "low";
            public const string EXIT = "exit";
            public const string IDENTIFY = "identify";
            public const string DELAY = "delay";
            public const string AGAIN = "again";
        }

        const int MAX_CMD_LIST_LENGTH = 128;
        string[] cmd_list_name;
        int[] cmd_list_value;
        int cmd_list_total = 0;


        public int cmd_list_cnt = 0;
        public int cycle_cnt = 0;
        public int cycle_total = 0;

        Button button_run;
        TextBox textBox_Cmdlist;
        TextBox textBox_Point;
        FormMain fm;
        Modbus mdbs;

        System.Timers.Timer timer_execute;

        public void Init(Button _button_run, TextBox _textBox_Cmdlist, TextBox _textBox_Point, Modbus _mdbs, FormMain _fm)
        {
            cmd_list_name = new string[MAX_CMD_LIST_LENGTH];
            cmd_list_value = new int[MAX_CMD_LIST_LENGTH];

            timer_execute = new System.Timers.Timer(200);
            timer_execute.Elapsed += new System.Timers.ElapsedEventHandler(timer_execute_Tick);
            timer_execute.AutoReset = true;
            timer_execute.Enabled = false;

            button_run = _button_run;
            textBox_Cmdlist = _textBox_Cmdlist;
            textBox_Point = _textBox_Point;
            fm = _fm;
            mdbs = _mdbs;
        }

        public void BatCall(TextBox textbox, SerialPort _serialport)
        {
            for(int i = 0; i < Program.parameters.Length; i++)
            {
                Console.WriteLine("Arg[{0}] = [{1}]", i, Program.parameters[i]);
                textbox.Text += i.ToString() + ":" + Program.parameters[i] + "\r\n";
            }
            if((_serialport.IsOpen == true) && (Program.parameters.Length == 1))
            {
                CmdList_Execute(Program.parameters[0], 0);

                Thread.Sleep(100);
                fm.Close();
            }
        }

        private int Func_GetParam(string str_cmd)
        {
            string value = "";
            int start_index;
            int end_index;

            start_index = str_cmd.IndexOf("(") + 1;
            end_index = str_cmd.IndexOf(")");

            if((start_index == -1) || (end_index == -1) || (end_index == start_index))
            {
                return 0;
            }

            value = str_cmd.Substring(start_index, end_index - start_index);

            try
            {
                return int.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        private string Func_GetCmd(string str_cmd)
        {
            int end_index;

            end_index = str_cmd.IndexOf("(");
            if(end_index == -1)
            {
                return "";
            }
            return str_cmd.Substring(0, end_index);
        }

        private void CmdList_Execute(string cmd, int value)
        {
            Console.WriteLine("@@[{0}]ExeCMD. Cnt:{1}|{2} Cmd:{3} Args:{4}", 
                DateTime.Now.ToString("yy/MM/dd HH:mm:ss"), cmd_list_cnt, cmd_list_total,
                cmd_list_name[cmd_list_cnt], cmd_list_value[cmd_list_cnt]);

            if(cmd == CMD.BOOT_PC)
            {
                //MessageBox.Show("Run Boot!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.BOOT);
            }
            else if(cmd == CMD.SHUTDOWN_PC)
            {
                //MessageBox.Show("Run Shutdown!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.SHUTDOWN);
            }
            else if(cmd == CMD.IO_HIGH)
            {
                //MessageBox.Show("Run IO_HIGH!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.IO_High);
            }
            else if(cmd == CMD.IO_LOW)
            {
                //MessageBox.Show("Run IO_LOW!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.IO_Low);
            }
            else if(cmd == CMD.IDENTIFY)
            {
                mdbs.Send_03(Modbus.REG.IDENTIFY, 1, 0);
            }
            else if(cmd == CMD.DELAY)
            {
                timer_execute.Stop();
                Thread.Sleep(value*1000);
                timer_execute.Start();            
            }
            else if(cmd == CMD.AGAIN)
            {
                if(cycle_cnt + 1 < cycle_total)
                {
                    cmd_list_cnt = -1;
                    cycle_cnt++;
                }
            }
            else
            {
                //MessageBox.Show("Unknown CMD!");
                Console.WriteLine("##Unknown CMD!");
            }
        }

        private void timer_execute_Tick(object sender, EventArgs e)
        {
            fm.Invoke((EventHandler)(delegate
            {
                textBox_Point.Text = "";
                for(int i = 0; i < cmd_list_cnt; i++)
                {
                    textBox_Point.Text += "\r\n";
                }
                textBox_Point.Text += "<<";
            }));

            CmdList_Execute(cmd_list_name[cmd_list_cnt], cmd_list_value[cmd_list_cnt]);

            cmd_list_cnt++;
            if((cmd_list_cnt == cmd_list_total) || (cmd_list_total == 0))
            {
                Reset();
            }
        }

        void Reset()
        {
            Console.WriteLine("Reset");

            fm.Invoke((EventHandler)(delegate
            {
                textBox_Point.Text = "";
                button_run.Text = "Run";
                textBox_Cmdlist.Enabled = true;
                textBox_Point.Enabled = true;
            }));
            
            cmd_list_cnt = 0;
            cmd_list_total = 0;
            cycle_cnt = 0;
            timer_execute.Enabled = false;
        }

        void Start()
        {
            Console.WriteLine("Start");

            fm.Invoke((EventHandler)(delegate
            {
                button_run.Text = "Stop";
                textBox_Cmdlist.Enabled = false;
                textBox_Point.Enabled = false;
            }));

            cmd_list_cnt = 0;
            timer_execute.Enabled = true;
        }

        public void Run(TextBox textbox)
        {
            if(button_run.Text == "Run")
            {
                Console.WriteLine("eCMD count:{0}", textbox.Lines.Length);

                cmd_list_total = 0;
                foreach(string str_cmd in textbox.Lines)
                {
                    cmd_list_name[cmd_list_total] = Func_GetCmd(str_cmd);
                    if(cmd_list_name[cmd_list_total] == "")
                    {
                        MessageBox.Show("Illegal CMD!");

                        Reset();

                        return;
                    }

                    cmd_list_value[cmd_list_total] = Func_GetParam(str_cmd);
                    Console.WriteLine("Cnt:{0} Cmd:{1} Args:{2}", cmd_list_total, cmd_list_name[cmd_list_total], cmd_list_value[cmd_list_total]);
                    cmd_list_total++;
                }

                if(cmd_list_total > 0)
                {
                    Start();
                }
            }
            else
            {
                Reset();
            }
        }
    }
}
