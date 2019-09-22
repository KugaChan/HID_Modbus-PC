using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;    //使用ConcurrentQueue
using System.Diagnostics;

namespace KMouse
{
    class Dbg
    {
        //跨线程要使用ConcurrentQueue而不是Queue
        static public ConcurrentQueue<string> queue_message = new ConcurrentQueue<string>();

        static private System.Timers.Timer timer_ShwoMessage;

        static TextBox textBox_Message;

        static int message_cnt = 0;

        public static void Init(TextBox _textBox_Message)
        {
            textBox_Message = _textBox_Message;

            timer_ShwoMessage = new System.Timers.Timer();
            timer_ShwoMessage.Elapsed += new System.Timers.ElapsedEventHandler(timer_ShwoMessage_Tick);
            timer_ShwoMessage.AutoReset = true;            
            timer_ShwoMessage.Interval = 1000;
            timer_ShwoMessage.Enabled = true;
        }


        static void timer_ShwoMessage_Tick(object sender, EventArgs e)
        {
            if(queue_message.Count > 0)
            {
                string message;
                if(Dbg.queue_message.TryDequeue(out message))
                {
                    textBox_Message.AppendText(message_cnt.ToString() + "," + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + ":" + message + "\r\n");
                    message_cnt++;
                }
            }
        }

        public static string GetStack()
        {
            string str = "";

            str += "\r\n";

            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);

            #if false
                string file_name;
                file_name = st.GetFrame(0).GetFileName();
                str += "  File:" + file_name;
            #endif

            string func_name;
            func_name = st.GetFrame(0).GetMethod().Name;
            str += "  Func:" + func_name;

            int line;
            line = st.GetFrame(0).GetFileLineNumber();
            str += "  Line:" + line.ToString();            

            return str;
        }

        public static void Assert(bool must_be_true, string last_words)
        {
            if(must_be_true == false)
            {
#if true
                throw new ArgumentException(last_words + GetStack(), "Assert!");
#else
                MessageBox.Show(last_words + GetStack(), "Assert!");
                System.Environment.Exit(0);
                //while(true);  //进while 1的话程序会一直卡死!
#endif
            }
        }

        public static void WriteLogFile(string str)
        {
            Process dbg_cmd = new Process();

            dbg_cmd.StartInfo.FileName = "cmd.exe";
            dbg_cmd.StartInfo.UseShellExecute = false;
            dbg_cmd.StartInfo.RedirectStandardInput = true;
            dbg_cmd.StartInfo.RedirectStandardOutput = true;
            dbg_cmd.StartInfo.RedirectStandardError = true;
            dbg_cmd.StartInfo.CreateNoWindow = true;

            dbg_cmd.Start();

            string cmdline;
            if(str == "")
            {
                cmdline = "echo= >> ./kcom_debug_log.txt";  //输出空行
            }
            else
            {
                cmdline = "echo " + str + " >> ./kcom_debug_log.txt";
            }
            
            dbg_cmd.StandardInput.WriteLine(cmdline);
            dbg_cmd.StandardInput.WriteLine("exit");
            //dbg_cmd.StandardInput.AutoFlush = true;
            dbg_cmd.StandardInput.Flush();

            dbg_cmd.WaitForExit();
            dbg_cmd.Close();
        }

        public static string Pirnt2String(string format, params object[] arg)
        {
            int arg_cnt = 0;
            string output_string = "";

            byte[] byteArray = System.Text.Encoding.Default.GetBytes(format);

            for(int i = 0; i < byteArray.Length; i++)
            {
                if(byteArray[i] == (byte)'%')
                {
                    output_string += arg[arg_cnt].ToString();
                    arg_cnt++;
                }
                else
                {
                    output_string += ((char)byteArray[i]);
                }
            }

            return output_string;
        }


        static private string MakeOutputString(string format, bool newline, params object[] arg)
        {
            int arg_cnt = 0;
            string output_string = "";

            byte[] byteArray = System.Text.Encoding.Default.GetBytes(format);

            for(int i = 0; i < byteArray.Length; i++)
            {
                if(byteArray[i] == (byte)'%')
                {
                    output_string += arg[arg_cnt].ToString();
                    arg_cnt++;
                }
                else
                {
                    output_string += ((char)byteArray[i]);
                }
            }

            if(echo_to_log_file == true)
            {
                WriteLogFile(output_string);
            }

            if(newline == true)
            {
                output_string += "\r\n";
            }

            return output_string;
        }

        //注意，打印到cmd时，不能存在转义字符！！！
        public static bool echo_to_log_file = false;
        public static void WriteLine(string format, params object[] arg)
        {
            string output_string = MakeOutputString(format, true, arg);

            Console.Write(output_string);
        }

        public static void WriteLine(bool newline, string format, params object[] arg)
        {
            string output_string = MakeOutputString(format, newline, arg);

            Console.Write(output_string);
        }
    }
}
