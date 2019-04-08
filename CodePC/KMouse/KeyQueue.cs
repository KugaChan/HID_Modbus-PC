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
        public struct KEY
        {
            public const UInt32 A = 0x04;
		    public const UInt32 B = 0x05;
		    public const UInt32 C = 0x06;
		    public const UInt32 D = 0x07;
		    public const UInt32 E = 0x08;
		    public const UInt32 F = 0x09;
		    public const UInt32 G = 0x0A;
		    public const UInt32 H = 0x0B;
		    public const UInt32 I = 0x0C;
		    public const UInt32 J = 0x0D;
		    public const UInt32 K = 0x0E;
		    public const UInt32 L = 0x0F;
		    public const UInt32 M = 0x10;
		    public const UInt32 N = 0x11;
		    public const UInt32 O = 0x12;
		    public const UInt32 P = 0x13;
		    public const UInt32 Q = 0x14;
		    public const UInt32 R = 0x15;
		    public const UInt32 S = 0x16;
		    public const UInt32 T = 0x17;
		    public const UInt32 U = 0x18;
		    public const UInt32 V = 0x19;
		    public const UInt32 W = 0x1A;
		    public const UInt32 X = 0x1B;
		    public const UInt32 Y = 0x1C;
		    public const UInt32 Z = 0x1D;

		    public const UInt32 NUM1 = 0x1E;
		    public const UInt32 NUM2 = 0x1F;
		    public const UInt32 NUM3 = 0x20;
		    public const UInt32 NUM4 = 0x21;
		    public const UInt32 NUM5 = 0x22;
		    public const UInt32 NUM6 = 0x23;
		    public const UInt32 NUM7 = 0x24;
		    public const UInt32 NUM8 = 0x25;
		    public const UInt32 NUM9 = 0x26;
		    public const UInt32 NUM0 = 0x27;

            public const UInt32 ENTER = 0x28;
		    public const UInt32 ESC = 0x29;
		    public const UInt32 BACK = 0x2A;
		    public const UInt32 TAB = 0x2B;
		    public const UInt32 SPACE = 0x2C;

		    public const UInt32 MIN = 0x2D;
		    public const UInt32 ADD = 0x2E;
		    public const UInt32 KUO1 = 0x2F;
		    public const UInt32 KUO2 = 0x30;
		    public const UInt32 OR = 0x31;
		    public const UInt32 MAO = 0x33;
		    public const UInt32 FEN = 0x34;
		    public const UInt32 DOU = 0x35;
		    public const UInt32 XIAO = 0x36;
		    public const UInt32 DA = 0x37;
		    public const UInt32 WEN = 0x38;

		    public const UInt32 CAPS = 0x39;
		    public const UInt32 F1 = 0x3A;
		    public const UInt32 F2 = 0x3B;
		    public const UInt32 F3 = 0x3C;
		    public const UInt32 F4 = 0x3D;
		    public const UInt32 F5 = 0x3E;
		    public const UInt32 F6 = 0x3F;
		    public const UInt32 F7 = 0x40;
		    public const UInt32 F8 = 0x41;
		    public const UInt32 F9 = 0x42;
		    public const UInt32 F10 = 0x43;
		    public const UInt32 F11 = 0x44;
		    public const UInt32 F12 = 0x45;
		    public const UInt32 PRINTSCREEN = 0x46;

		    public const UInt32 HOME = 0x4A;
		    public const UInt32 PAGEUP = 0x4B;
		    public const UInt32 DEL = 0x4C;
		    public const UInt32 END = 0x4D;
		    public const UInt32 PAGEDOWN = 0x4E;

		    public const UInt32 RIGHT = 0x4F;
		    public const UInt32 LEFT = 0x50;
		    public const UInt32 DOWN = 0x51;
		    public const UInt32 UP = 0x52;

            public const UInt32 SHIFT = 1u << 16;
		    public const UInt32 CTRL = 1u << 17;		
		    public const UInt32 ALT = 1u << 18;

            public const UInt16 NULL = 0xFFFF;
        }

		public struct MOUSE
        {
            public struct PRESS
            {
     	        public const UInt32 MOVEUP = 2;
		        public const UInt32 MOVEDOWN = 3;
		        public const UInt32 MOVELEFT = 4;
		        public const UInt32 MOVERIGHT = 5;
		        public const UInt32 ROLLUP = 6;
		        public const UInt32 ROLLDOWN = 7;
		        public const UInt32 ALL = 8;
            }        

	        public struct CLICK
            {
     	        public const UInt32 LEFT = 8;
		        public const UInt32 RIGHT = 9;
            }
		
            public struct SPEED
            {
		        public const UInt32 UP = 0;
		        public const UInt32 DOWN = 1;
		        public const UInt32 CHECK = 2;
            }

		    public const UInt32 LEAVE = 0x0000FFFF;
        }

        public bool[] mouse_press_en = new bool[MOUSE.PRESS.ALL];
		public bool[] mouse_press_en_last = new bool[MOUSE.PRESS.ALL];
        public bool mouse_speed_chk = false;


        const UInt32 MODBUS_KB_WAITING_MAX = 8;

        public Queue<UInt32> queue_key = new Queue<UInt32>();

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

        public bool FIFO_Input(UInt32 key)
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

        public UInt32 FIFO_Output()
        {
            UInt32 key;
            if (FIFO_HasData() == true)
            {
                key = queue_key.Dequeue();
                Console.WriteLine("keyQ Out:{0}", key);

                return key;
            }
            else
            {
                MessageBox.Show("###FIFO已空", "提示");
                return Func.dwAllFF;
            }
        }
	}
}

