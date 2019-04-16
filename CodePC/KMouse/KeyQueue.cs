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
using System.Management;

namespace KMouse
{
	class keyQ
	{
        public enum eKEY : uint    //设定enum的数据类型
        {
            A = 0x04,
            B = 0x05,
            C = 0x06,
            D = 0x07,
            E = 0x08,
            F = 0x09,
            G = 0x0A,
            H = 0x0B,
            I = 0x0C,
            J = 0x0D,
            K = 0x0E,
            L = 0x0F,
            M = 0x10,
            N = 0x11,
            O = 0x12,
            P = 0x13,
            Q = 0x14,
            R = 0x15,
            S = 0x16,
            T = 0x17,
            U = 0x18,
            V = 0x19,
            W = 0x1A,
            X = 0x1B,
            Y = 0x1C,
            Z = 0x1D,

            NUM1 = 0x1E,
            NUM2 = 0x1F,
            NUM3 = 0x20,
            NUM4 = 0x21,
            NUM5 = 0x22,
            NUM6 = 0x23,
            NUM7 = 0x24,
            NUM8 = 0x25,
            NUM9 = 0x26,
            NUM0 = 0x27,

            ENTER = 0x28,
            ESC = 0x29,
            BACK = 0x2A,
            TAB = 0x2B,
            SPACE = 0x2C,

            MIN = 0x2D,
            ADD = 0x2E,
            KUO1 = 0x2F,
            KUO2 = 0x30,
            OR = 0x31,
            MAO = 0x33,
            FEN = 0x34,
            DOU = 0x35,
            XIAO = 0x36,
            DA = 0x37,
            WEN = 0x38,

            CAPS = 0x39,
            F1 = 0x3A,
            F2 = 0x3B,
            F3 = 0x3C,
            F4 = 0x3D,
            F5 = 0x3E,
            F6 = 0x3F,
            F7 = 0x40,
            F8 = 0x41,
            F9 = 0x42,
            F10 = 0x43,
            F11 = 0x44,
            F12 = 0x45,
            PRINTSCREEN = 0x46,

            HOME = 0x4A,
            PAGEUP = 0x4B,
            DEL = 0x4C,
            END = 0x4D,
            PAGEDOWN = 0x4E,

            RIGHT = 0x4F,
            LEFT = 0x50,
            DOWN = 0x51,
            UP = 0x52,

            SHIFT = (1u << 16),
            CTRL = (1u << 17),		
            ALT = (1u << 18),

            NULL = 0x0000FFFF,

            INVAILD = Func.dwAllFF,
        }

        public struct MOUSE
        {
            public struct PRESS
            {
     	        public const uint MOVEUP = 2;
		        public const uint MOVEDOWN = 3;
		        public const uint MOVELEFT = 4;
		        public const uint MOVERIGHT = 5;
		        public const uint ROLLUP = 6;
		        public const uint ROLLDOWN = 7;
		        public const uint ALL = 8;
            }        

	        public struct CLICK
            {
     	        public const uint LEFT = 8;
		        public const uint RIGHT = 9;
            }
		
            public struct SPEED
            {
		        public const uint UP = 0;
		        public const uint DOWN = 1;
		        public const uint CHECK = 2;
            }

		    public const uint LEAVE = 0x0000FFFF;
        }

        public bool[] mouse_press_en = new bool[MOUSE.PRESS.ALL];
		public bool[] mouse_press_en_last = new bool[MOUSE.PRESS.ALL];
        public bool mouse_speed_chk = false;


        const uint MODBUS_KB_WAITING_MAX = 8;

        public Queue<keyQ.eKEY> queue_key = new Queue<keyQ.eKEY>();

        public void TimerHandler_QueueFull(object source, System.Timers.ElapsedEventArgs e)
        {
            timer_queue_full.Enabled = false;
            queue_message.Enqueue("FIFO receive recover");
        }

        System.Timers.Timer timer_queue_full;
        Queue<string> queue_message;
        public void Init(Queue<string> _queue_message)
        {
            timer_queue_full = new System.Timers.Timer(200); //实例化Timer类，设置间隔时间为1000毫秒
            timer_queue_full.Elapsed += new System.Timers.ElapsedEventHandler(TimerHandler_QueueFull);//到达时间的时候执行事件
            timer_queue_full.AutoReset = true; //设置是执行一次（false）还是一直执行(true)
            timer_queue_full.Enabled = false;   //是否执行System.Timers.Timer.Elapsed事件

            for(int i = 0; i < keyQ.MOUSE.PRESS.ALL; i++)
			{
				mouse_press_en[i] = false;
				mouse_press_en_last[i] = false;
			}

            queue_message = _queue_message;
        }

        public void Close()
        {
            timer_queue_full.Close();
        }

        public void FIFO_Reset()
        {
            queue_key.Clear();
        }

        public bool FIFO_Input(keyQ.eKEY key)
        {
            Console.WriteLine("keyQ In:{0}", key);

            if(timer_queue_full.Enabled == true)
            {
                return false;
            }

            if (queue_key.Count < MODBUS_KB_WAITING_MAX)
            {
                queue_key.Enqueue(key);
                return true;
            }
            else
            {
                //MessageBox.Show("FIFO已满", "提示");
                System.Media.SystemSounds.Beep.Play();
                queue_message.Enqueue("FIFO is full, pause receive!");
                timer_queue_full.Enabled = true;
                return false;
            }
        }

        public bool FIFO_HasData()
        {
            if (queue_key.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public keyQ.eKEY FIFO_Output()
        {
            keyQ.eKEY key;
            if (FIFO_HasData() == true)
            {
                key = queue_key.Dequeue();
                Console.WriteLine("keyQ Out:{0}", key);

                return key;
            }
            else
            {
                MessageBox.Show("###FIFO已空", "提示");
                return keyQ.eKEY.INVAILD;
            }
        }
	}
}

