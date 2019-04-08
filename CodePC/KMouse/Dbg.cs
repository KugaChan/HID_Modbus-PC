using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KMouse
{
    class Dbg
    {
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
                MessageBox.Show(last_words + GetStack(), "Assert!");
                //System.Environment.Exit(0);
                while(true);
            }            
        }
    }
}
