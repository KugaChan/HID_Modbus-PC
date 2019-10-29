using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            public const string TEST = "Test";
            public const string BOOT_PC = "Boot";
            public const string SHUTDOWN_PC = "Shutdown";
            public const string AUOTPOWER_PC = "AutoPower";
            public const string IO_HIGH = "High";
            public const string IO_LOW = "Low";
            public const string EXIT = "Exit";
            public const string IDENTIFY = "Identify";
            public const string DELAY = "Delay";
            public const string XDELAY = "XDelay";
            public const string BAT = "Bat";

            public const string AGAIN = "Again";
            public const string CLOSE = "Close";

            public const string LOCK = "Lock";
            public const string UNLOCK = "Unlock";

            public const string MOUSE_SET = "MouseSet";
            public const string MOUSE_LEFT = "MouseL";
            public const string MOUSE_DLEFT = "MouseDL";
            public const string MOUSE_RIGHT = "MouseR";
            public const string MOUSE_MIDDLE = "MouseM";
            public const string MOUSE_ROLLUP = "MouseU";
            public const string MOUSE_ROLLDOWN = "MouseD";
        }

        const int MAX_CMD_LIST_LENGTH = 128;
        string[] cmd_list_name;
        int cmd_list_total = 0;

        const int MAX_COMMAND_LENGTH = 1024;    //最大支持多少个命令
        const int MAX_PARAM_PER_COMMAND = 8;    //每个命令最多多少个参数
        class rgn
        {
            public int[] value;
            public int cnt;
        };

        List<rgn> cmd_list_args;

        MoveCursor mc;

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
            cmd_list_args = new List<rgn>();
            for(int i = 0; i < MAX_COMMAND_LENGTH; i++)
            {
                rgn new_rgn = new rgn();
                new_rgn.value = new int[MAX_PARAM_PER_COMMAND];
                new_rgn.cnt = 0;

                cmd_list_args.Add(new_rgn);
            }

            cmd_list_name = new string[MAX_CMD_LIST_LENGTH];

            timer_execute = new System.Timers.Timer(200);
            timer_execute.Elapsed += new System.Timers.ElapsedEventHandler(timer_execute_Tick);
            timer_execute.AutoReset = true;
            timer_execute.Enabled = false;

            button_run = _button_run;
            textBox_Cmdlist = _textBox_Cmdlist;
            textBox_Point = _textBox_Point;
            fm = _fm;
            mdbs = _mdbs;

            mc = new MoveCursor();
        }
                
        public void BatCall(TextBox textbox, SerialPort _serialport)
        {
            for(int i = 0; i < Program.parameters.Length; i++)
            {
                Dbg.WriteLine("Raw args[%]:%", i, Program.parameters[i]);
                textbox.Text += i.ToString() + ":" + Program.parameters[i] + "\r\n";

                string cmd_name = Func_GetCmd(Program.parameters[i]);
                if(cmd_name == "")
                {
                    continue;
                }
                
                int[] args_bat_array = new int[8];
                int args_bat_num = Func_GetArgs(Program.parameters[i], args_bat_array);

                cmd_list_total = 0;
                cmd_list_cnt = 0;

                rgn new_rgn = cmd_list_args[cmd_list_total];                

                new_rgn.cnt = args_bat_num;
                for(int j = 0; j < args_bat_num; j++)
                {
                    new_rgn.value[i] = args_bat_array[i];
                    Dbg.WriteLine("\t Args:%", args_bat_array[j]);
                }
                cmd_list_name[cmd_list_total] = cmd_name;

                cmd_list_total++;

                CmdList_Execute(cmd_name, cmd_list_args[0].value, cmd_list_args[0].cnt);
            }

            Dbg.WriteLine("Bat run cmd end.");
        }

        private int Func_GetArgs(string str_cmd, int[] value)
        {
            int value_cnt = 0;
            string string_value_all = "";
            int start_index;
            int end_index;

            start_index = str_cmd.IndexOf("(") + 1;
            end_index = str_cmd.IndexOf(")");

            if((start_index == 0) || (end_index == -1) || (end_index == start_index))
            {
                return 0;
            }

            string_value_all = str_cmd.Substring(start_index, end_index - start_index);
            
            string string_value_part = "";
            while(true)
            {
                int part_index = string_value_all.IndexOf(",");

                if(part_index != -1)
                {
                    string_value_part = string_value_all.Substring(0, part_index);
                    string_value_all = string_value_all.Substring(part_index + 1, string_value_all.Length - part_index - 1);                    

                    //Dbg.WriteLine("@@@At:% PartA:% Left:%", part_index, string_value_part, string_value_all);

                    try
                    {
                        value[value_cnt] = int.Parse(string_value_part);
                        value_cnt++;
                    }
                    catch
                    {
                        return 0;
                    }
                }
                else
                {
                    //Dbg.WriteLine("@@@PartB:%", string_value_all);

                    try
                    {
                        value[value_cnt] = int.Parse(string_value_all);
                        value_cnt++;
                    }
                    catch
                    {
                        return 0;
                    }

                    break;
                }
            }

             return value_cnt;
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

        static public string path_lock = ".\\lock";
        private void CmdList_Execute(string cmd, int[] args_value, int args_num)
        {
            Dbg.WriteLine("[%]ExeCMD. Cnt:%|% Cmd:% ArgsNum:% ArgsVal:%", 
                DateTime.Now.ToString("yy/MM/dd HH:mm:ss"), cmd_list_cnt, cmd_list_total,
                cmd_list_name[cmd_list_cnt], args_num, cmd_list_args[cmd_list_cnt].value[0]);

            string lock_master = "???";
            if( (File.Exists(path_lock) == true) &&
                ((cmd == CMD.TEST) ||
                 (cmd == CMD.LOCK) ||
                 (cmd == CMD.UNLOCK) ||
                 (cmd == CMD.MOUSE_SET) ||
                 (cmd == CMD.MOUSE_LEFT) ||
                 (cmd == CMD.MOUSE_DLEFT) ||
                 (cmd == CMD.MOUSE_RIGHT) ||
                 (cmd == CMD.MOUSE_MIDDLE) ||
                 (cmd == CMD.MOUSE_ROLLUP) ||
                 (cmd == CMD.MOUSE_ROLLDOWN)) )
            {
                StreamReader sr = new StreamReader(path_lock);
                lock_master = sr.ReadLine();
                sr.Close();

                if(lock_master != Param.path_ini_file)                      //不属于自己的锁，则要等待别的kmouse解锁
                {
                    fm.queue_message.Enqueue("Wait [" + lock_master + "] unlock...");

                    int wait_cnt = 0;
                    timer_execute.Stop();
                    while(true)
                    {
                        if(button_run.Text == "Run")
                        {
                            break;
                        }

                        if(File.Exists(path_lock) == true)
                        {
                            Dbg.WriteLine("wait lock:% done...", wait_cnt);
                            wait_cnt++;
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            fm.queue_message.Enqueue("Lock [" + lock_master + "] is free at " + wait_cnt.ToString());
                            break;
                        }
                    }
                    timer_execute.Start();
                }
            }

            if(cmd == CMD.TEST)
            {
                if(lock_master != "???")
                {
                    fm.queue_message.Enqueue("Get lock: " + lock_master);
                }

                Dbg.WriteLine("I am CMD Test");

                for(int i = 0; i < args_num; i++)
                {
                    Dbg.WriteLine("\t Args:%", args_value[i]);
                }
            }
            else if(cmd == CMD.BOOT_PC)
            {
                //MessageBox.Show("Run Boot!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.BOOT);
            }
            else if(cmd == CMD.SHUTDOWN_PC)
            {
                //MessageBox.Show("Run Shutdown!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.SHUTDOWN);
            }
            else if(cmd == CMD.AUOTPOWER_PC)
            {
                //MessageBox.Show("Run AuotPower!");
                mdbs.Send_03(Modbus.REG.MOVEMENT, 1, keyQ.MOUSE.MOVEMENT.AUTOPOWER);
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
                int XR = args_value[0];

                timer_execute.Stop();
                int delay_cnt = 0;
                while(true)
                {
                    if(button_run.Text == "Run")
                    {
                        break;
                    }

                    delay_cnt++;
                    fm.queue_message.Enqueue("Delay: " + delay_cnt.ToString() + "/" + XR.ToString());
                    Thread.Sleep(1000);
                    if(delay_cnt == XR)
                    {
                        break;
                    }
                }
                fm.queue_message.Enqueue("Delay done");
                timer_execute.Start();
            }
            else if(cmd == CMD.BAT)
            {
                System.Diagnostics.ProcessStartInfo pinfo = new System.Diagnostics.ProcessStartInfo();
                pinfo.UseShellExecute = true;
                pinfo.FileName = Param.ini.bat_path_string;

                //启动进程
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pinfo);
            }
            else if(cmd == CMD.LOCK)
            {
                if(File.Exists(path_lock) == false)
                {
                    StreamWriter sw = File.CreateText(path_lock);
                    sw.WriteLine(Param.path_ini_file);
                    sw.Close();
                }
            }
            else if(cmd == CMD.UNLOCK)
            {
                if(File.Exists(path_lock) == true)
                {
                    File.Delete(path_lock);
                }
            }
            else if(cmd == CMD.XDELAY)
            {
                int XR = args_value[0];
                int YR = args_value[1];

                Random rd = new Random();
                int data = rd.Next(XR,YR);

                Dbg.WriteLine("XDelay from % ~ %, at %", XR, YR, data);

                timer_execute.Stop();

                int delay_cnt = 0;
                while(true)
                {
                    if(button_run.Text == "Run")
                    {
                        break;
                    }

                    delay_cnt++;                    
                    fm.queue_message.Enqueue("XDelay: " + delay_cnt.ToString() + "/" + data.ToString());
                    Thread.Sleep(1000);
                    if(delay_cnt == data)
                    {
                        break;
                    }
                }
                fm.queue_message.Enqueue("XDelay done");

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
            else if(cmd == CMD.CLOSE)
            {
                Thread.Sleep(100);
                fm.Close();
            }
            else if(cmd == CMD.MOUSE_SET)
            {
                int XR = args_value[0] * 1000 / Screen.PrimaryScreen.Bounds.Width;
                int YR = args_value[1] * 1000 / Screen.PrimaryScreen.Bounds.Height;

                Dbg.WriteLine("MouseSet XR:% YR:%", XR, YR);

                mc.Mouse_AbsoluteMove(XR, YR);
            }
            else if(cmd == CMD.MOUSE_LEFT)
            {
                mc.Mouse_Single_LeftClick();
            }
            else if(cmd == CMD.MOUSE_DLEFT)
            {
                mc.Mouse_Single_DoubleClick();
            }
            else if(cmd == CMD.MOUSE_RIGHT)
            {
                mc.Mouse_RightClick();
            }
            else if(cmd == CMD.MOUSE_MIDDLE)
            {
                mc.Mouse_MiddleClick();
            }
            else if(cmd == CMD.MOUSE_ROLLUP)
            {
                mc.Mouse_WheelMove(100);
            }
            else if(cmd == CMD.MOUSE_ROLLDOWN)
            {
                mc.Mouse_WheelMove(-100);
            }
            else
            {
                //MessageBox.Show("Unknown CMD!");
                Dbg.WriteLine("##Unknown CMD!");
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

            CmdList_Execute(cmd_list_name[cmd_list_cnt], cmd_list_args[cmd_list_cnt].value, cmd_list_args[cmd_list_cnt].cnt);

            cmd_list_cnt++;
            if((cmd_list_cnt == cmd_list_total) || (cmd_list_total == 0))
            {
                Reset();
            }
        }

        void Reset()
        {
            Dbg.WriteLine("Reset");

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
            Dbg.WriteLine("Start");

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
                Dbg.WriteLine("eCMD count:%", textbox.Lines.Length);

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

                    int[] args_array = new int[8];
                    int args_num = Func_GetArgs(str_cmd, args_array);
                    
                    Dbg.WriteLine("Cnt:% Cmd:% num:%", cmd_list_total, cmd_list_name[cmd_list_total], args_num);

                    rgn new_rgn = cmd_list_args[cmd_list_total];
                    new_rgn.cnt = args_num;
                    for(int i = 0 ;i < args_num; i++)
                    {
                        new_rgn.value[i] = args_array[i];
                        Dbg.WriteLine("\t Args:%", args_array[i]);
                    }

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
