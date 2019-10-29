using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMouse
{
	static class Program
	{
        static public string[] parameters;
        static public bool call_from_cmd;

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            parameters = args;
            if(args.Length > 0)
            {
                call_from_cmd = true;
            }
            Dbg.WriteLine("parameter count:%", args.Length);

            for(int i = 0; i < args.Length; i++)
            {
                Dbg.WriteLine("Arg[%]:%", i, args[i]);
            }
            //Console.ReadLine();
            
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormMain());
		}
	}
}
