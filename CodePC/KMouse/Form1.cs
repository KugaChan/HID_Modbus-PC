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
		private const u8 _VersionGit = 14;

		//宏
		const u32 dwAllFF = 0xFFFFFFFF;

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

            textBox_eKey.Enabled = false;

			/*********************初始化鼠标按键 start**********************/
			for(i = 0; i < KEY_MousePress_ALL; i++)
			{
				mouse_press_en[i] = false;
				mouse_press_en_last[i] = false;
			}
			/*********************初始化鼠标按键 end************************/

            Func_Com_Component_Init();           

            textBox_eKey.Text = Properties.Settings.Default.eKey_string;

			Func_Com_Open();
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
            Properties.Settings.Default._baudrate_select_index = comboBox_COMBaudrate.SelectedIndex;
            Properties.Settings.Default.eKey_string = textBox_eKey.Text;
            Properties.Settings.Default._com_num_select_index = comboBox_COMNumber.SelectedIndex;

            Properties.Settings.Default.Save();       
        }		

		private void timer_recv_timeout_Tick(object sender, EventArgs e)	//10ms
		{
			//Console.WriteLine(DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
			//Console.WriteLine("modbus_recv_timeout:{0}", modbus_recv_timeout);

            //发了命令，但是1s内都没有回复
            if (modbus_is_busy == true)
            {
                modbus_respone_timeout++;
                if (modbus_respone_timeout == 100)
                {
                    modbus_respone_timeout = 0;                    

                    bool res;
                    res = Func_Modbus_Recv_Handle();

                    //MessageBox.Show("Modbus Respone timeout!!!", "Warning");
                    this.Invoke((EventHandler)(delegate
                    {
                        textBox_ComRec.AppendText("Modbus Respone timeout!!!\r\n");
                    }));

                    //清空FIFO，避免已进FIFO的key一直发送
                    modbus_kb_input = 0;
                    modbus_kb_output = 0;

                    if (res == false)
                    {
                        //Func_COM_Close();
                        Console.WriteLine("Dont close COM B");
                    }
                }
            }

            //已经开始了接收，但是1s内都没有接收完
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

					//MessageBox.Show(SerialIn, "Warning, Modbus Recv timeout!!!");
                    this.Invoke((EventHandler)(delegate
                    {
                        textBox_ComRec.AppendText("Modbus Recv timeout: " + SerialIn + "!!!\r\n");
                    }));
                    System.Media.SystemSounds.Hand.Play();
				}
			}

            if((com.IsOpen == true) && (modbus_is_busy == false))
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

        private bool func_char_string_compare(char[] spec_key_buff, string str, u32 length)
        {
            u32 i;

            char[] char_buffer = str.ToCharArray();

            bool same = true;
            for (i = 0; i < length; i++)
            {
                if (spec_key_buff[i] == char_buffer[i])
                {
                    same = true;
                }
                else if (((spec_key_buff[i] + 0x20) == char_buffer[i]) || ((char_buffer[i] + 0x20) == spec_key_buff[i]))
                {
                    same = true;
                }
                else
                {
                    same = false;
                    break;
                }
            }

            return same;           
        }

        private void button_eKey_Click(object sender, EventArgs e)
        {
	        u32 i;

	        const u8 SPEC_KEY_NONE = 0;
            const u8 SPEC_KEY_CHK = 1;
            const u8 SPEC_KEY_ING = 2;

            char[] spec_key_buff = new char[32];

	        u32 special_key_cnt = 0;
	        u8 special_key_step = SPEC_KEY_NONE;
            
            char[] char_buffer = textBox_eKey.Text.ToCharArray();

	        for(i = 0; i < textBox_eKey.TextLength; i++)
	        {
                Console.WriteLine("c:{0}\n", char_buffer[i]);

		        if(special_key_step == SPEC_KEY_NONE)
		        {                    
			        /**********************************将基本按键解释 Start*********************************/
			        switch(char_buffer[i])
			        {
				        case '`': {Func_KB_Click(KEY_KEYBOARD_Dou); break; }
				        case '1': {Func_KB_Click(KEY_KEYBOARD_Num1); break; }
				        case '2': {Func_KB_Click(KEY_KEYBOARD_Num2); break; }
				        case '3': {Func_KB_Click(KEY_KEYBOARD_Num3); break; }
				        case '4': {Func_KB_Click(KEY_KEYBOARD_Num4); break; }
				        case '5': {Func_KB_Click(KEY_KEYBOARD_Num5); break; }
				        case '6': {Func_KB_Click(KEY_KEYBOARD_Num6); break; }
				        case '7': {Func_KB_Click(KEY_KEYBOARD_Num7); break; }
				        case '8': {Func_KB_Click(KEY_KEYBOARD_Num8); break; }
				        case '9': {Func_KB_Click(KEY_KEYBOARD_Num9); break; }
				        case '0': {Func_KB_Click(KEY_KEYBOARD_Num0); break; }
				        case '-': {Func_KB_Click(KEY_KEYBOARD_Min); break; }
				        case '=': {Func_KB_Click(KEY_KEYBOARD_Add); break; }

				        case 'q': {Func_KB_Click(KEY_KEYBOARD_Q); break; }
				        case 'w': {Func_KB_Click(KEY_KEYBOARD_W); break; }
				        case 'e': {Func_KB_Click(KEY_KEYBOARD_E); break; }
				        case 'r': {Func_KB_Click(KEY_KEYBOARD_R); break; }
				        case 't': {Func_KB_Click(KEY_KEYBOARD_T); break; }
				        case 'y': {Func_KB_Click(KEY_KEYBOARD_Y); break; }
				        case 'u': {Func_KB_Click(KEY_KEYBOARD_U); break; }
				        case 'i': {Func_KB_Click(KEY_KEYBOARD_I); break; }
				        case 'o': {Func_KB_Click(KEY_KEYBOARD_O); break; }
				        case 'p': {Func_KB_Click(KEY_KEYBOARD_P); break; }
                        //case '[': {Func_KB_Click(KEY_KEYBOARD_Kuo1); break; }
				        case ']': {Func_KB_Click(KEY_KEYBOARD_Kuo2); break; }
				        case '\\': {Func_KB_Click(KEY_KEYBOARD_Or); break; }

				        case 'a': {Func_KB_Click(KEY_KEYBOARD_A); break; }
				        case 's': {Func_KB_Click(KEY_KEYBOARD_S); break; }
				        case 'd': {Func_KB_Click(KEY_KEYBOARD_D); break; }
				        case 'f': {Func_KB_Click(KEY_KEYBOARD_F); break; }
				        case 'g': {Func_KB_Click(KEY_KEYBOARD_G); break; }
				        case 'h': {Func_KB_Click(KEY_KEYBOARD_H); break; }
				        case 'j': {Func_KB_Click(KEY_KEYBOARD_J); break; }
				        case 'k': {Func_KB_Click(KEY_KEYBOARD_K); break; }
				        case 'l': {Func_KB_Click(KEY_KEYBOARD_L); break; }
				        case ';': {Func_KB_Click(KEY_KEYBOARD_Mao); break; }
				        case '\'':{Func_KB_Click(KEY_KEYBOARD_Fen); break; }

				        case 'z': {Func_KB_Click(KEY_KEYBOARD_Z); break; }
				        case 'x': {Func_KB_Click(KEY_KEYBOARD_X); break; }
				        case 'c': {Func_KB_Click(KEY_KEYBOARD_C); break; }
				        case 'v': {Func_KB_Click(KEY_KEYBOARD_V); break; }
				        case 'b': {Func_KB_Click(KEY_KEYBOARD_B); break; }
				        case 'n': {Func_KB_Click(KEY_KEYBOARD_N); break; }
				        case 'm': {Func_KB_Click(KEY_KEYBOARD_M); break; }
				        case ',': {Func_KB_Click(KEY_KEYBOARD_Xiao); break; }
				        case '.': {Func_KB_Click(KEY_KEYBOARD_Da); break; }
				        case '/': {Func_KB_Click(KEY_KEYBOARD_Wen); break; }
				        case ' ': {Func_KB_Click(KEY_KEYBOARD_Space); break; }
				        /**********************************将基本按键解释 END*********************************/

				        /**********************************将上档按键解释 Start*********************************/
				        case '~': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Dou);break;}
				        case '!': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num1);break;}
				        case '@': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num2);break;}
				        case '#': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num3);break;}
				        case '$': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num4);break;}
				        case '%': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num5);break;}
				        case '^': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num6);break;}
				        case '&': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num7);break;}
				        case '*': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num8);break;}
				        case '(': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num9);break;}
				        case ')': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Num0);break;}
				        case '_': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Min);break;}
				        case '+': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Add);break;}

				        case 'Q': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Q);break;}
				        case 'W': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_W);break;}
				        case 'E': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_E);break;}
				        case 'R': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_R);break;}
				        case 'T': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_T);break;}
				        case 'Y': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Y);break;}
				        case 'U': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_U);break;}
				        case 'I': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_I);break;}
				        case 'O': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_O);break;}
				        case 'P': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_P);break;}
				        case '{': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Kuo1);break;}
				        case '}': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Kuo2);break;}
				        case '|': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Or);break;}

				        case 'A': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_A);break;}
				        case 'S': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_S);break;}
				        case 'D': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_D);break;}
				        case 'F': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_F);break;}
				        case 'G': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_G);break;}
				        case 'H': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_H);break;}
				        case 'J': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_J);break;}
				        case 'K': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_K);break;}
				        case 'L': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_L);break;}
				        case ':': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Mao);break;}
				        case '"': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Fen);break;}

				        case 'Z': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Z);break;}
				        case 'X': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_X);break;}
				        case 'C': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_C);break;}
				        case 'V': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_V);break;}
				        case 'B': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_B);break;}
				        case 'N': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_N);break;}
				        case 'M': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_M);break;}
				        case '<': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Xiao);break;}
				        case '>': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Da);break;}
				        case '?': {button_Shift.BackColor = System.Drawing.Color.Yellow;Func_KB_Click(KEY_KEYBOARD_Wen);break;}
				        /**********************************将上档按键解释 END*********************************/

				        //触发特殊按键
				        case '[':
				        {
					        special_key_step = SPEC_KEY_CHK;
					        continue;
				        }

				        default:
                        {
                            MessageBox.Show("无效的输入字符" + char_buffer[i].ToString(), "错误");
                            break;
                        }                        
			        }
		        }
		        else if(special_key_step == SPEC_KEY_CHK)
		        {
			        if(char_buffer[i] == '[')
			        {
                        Func_KB_Click(KEY_KEYBOARD_Kuo1);
				        special_key_step = SPEC_KEY_NONE;
			        }
			        else
			        {
				        spec_key_buff[special_key_cnt++] = char_buffer[i];
				        special_key_step = SPEC_KEY_ING;
			        }
		        }
		        else if(special_key_step == SPEC_KEY_ING)
		        {
			        if(char_buffer[i] == ']')
			        {
                        if (func_char_string_compare(spec_key_buff, "shift", 5) == true)	//Shift
				        {
                            button_Shift.BackColor = System.Drawing.Color.Yellow;
				        }
                        else if (func_char_string_compare(spec_key_buff, "ctrl", 4) == true)//Ctrl	
				        {
                            button_Ctrl.BackColor = System.Drawing.Color.Yellow;
				        }
                        else if (func_char_string_compare(spec_key_buff, "alt", 3) == true)	//Alt
				        {
                            button_Alt.BackColor = System.Drawing.Color.Yellow;
				        }
				        else
				        {

                            if (func_char_string_compare(spec_key_buff, "esc", 3) == true) { Func_KB_Click(KEY_KEYBOARD_ESC); }//ESC
                            else if (func_char_string_compare(spec_key_buff, "f1", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F1); }//F1
                            else if (func_char_string_compare(spec_key_buff, "f2", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F2); }//F2
                            else if (func_char_string_compare(spec_key_buff, "f3", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F3); }//F3
                            else if (func_char_string_compare(spec_key_buff, "f4", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F4); }//F4
                            else if (func_char_string_compare(spec_key_buff, "f5", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F5); }//F5
                            else if (func_char_string_compare(spec_key_buff, "f6", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F6); }//F6
                            else if (func_char_string_compare(spec_key_buff, "f7", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F7); }//F7
                            else if (func_char_string_compare(spec_key_buff, "f8", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F8); }//F8
                            else if (func_char_string_compare(spec_key_buff, "f9", 2) == true) { Func_KB_Click(KEY_KEYBOARD_F9); }//F9
                            else if (func_char_string_compare(spec_key_buff, "f10", 3) == true) { Func_KB_Click(KEY_KEYBOARD_F10); }//F10
                            else if (func_char_string_compare(spec_key_buff, "f11", 3) == true) { Func_KB_Click(KEY_KEYBOARD_F11); }//F11
                            else if (func_char_string_compare(spec_key_buff, "f12", 3) == true) { Func_KB_Click(KEY_KEYBOARD_F12); }//F12
                            else if (func_char_string_compare(spec_key_buff, "del", 3) == true) { Func_KB_Click(KEY_KEYBOARD_Del); }//Del
                            else if (func_char_string_compare(spec_key_buff, "bp", 2) == true) { Func_KB_Click(KEY_KEYBOARD_Back); }//BP
                            else if (func_char_string_compare(spec_key_buff, "tab", 3) == true) { Func_KB_Click(KEY_KEYBOARD_Tab); ; }//Tab
                            else if (func_char_string_compare(spec_key_buff, "caps", 4) == true) { Func_KB_Click(KEY_KEYBOARD_Caps); ; }//Caps
                            else if (func_char_string_compare(spec_key_buff, "enter", 5) == true) { Func_KB_Click(KEY_KEYBOARD_Enter); ; }//Enter
                            else if (func_char_string_compare(spec_key_buff, "home", 4) == true) { Func_KB_Click(KEY_KEYBOARD_Home); ; }//Home
                            else if (func_char_string_compare(spec_key_buff, "psc", 3) == true) { Func_KB_Click(KEY_KEYBOARD_PrintScreen); ; }//PrintScreen
                            else if (func_char_string_compare(spec_key_buff, "up", 2) == true) { Func_KB_Click(KEY_KEYBOARD_Up); ; }//Up
                            else if (func_char_string_compare(spec_key_buff, "end", 2) == true) { Func_KB_Click(KEY_KEYBOARD_End); ; }//End				
                            else if (func_char_string_compare(spec_key_buff, "pageup", 6) == true) { Func_KB_Click(KEY_KEYBOARD_PageUp); ; }//Page Up
                            else if (func_char_string_compare(spec_key_buff, "pagedown", 8) == true) { Func_KB_Click(KEY_KEYBOARD_PageDown); ; }//Page Down
                            else if (func_char_string_compare(spec_key_buff, "left", 4) == true) { Func_KB_Click(KEY_KEYBOARD_Left); ; }//Left
                            else if (func_char_string_compare(spec_key_buff, "down", 4) == true) { Func_KB_Click(KEY_KEYBOARD_Down); ; }//Down
                            else if (func_char_string_compare(spec_key_buff, "right", 5) == true) { Func_KB_Click(KEY_KEYBOARD_Right); ; }//Right
                            else if (func_char_string_compare(spec_key_buff, "null", 4) == true) { Func_KB_Click(KEY_KEYBOARD_NULL); ; }//空按键，触发组合键
                            //else if(func_char_string_compare(spec_key_buff, "fn", 2) == true){Func_KB_Click(KEY_KEYBOARD_NULL);}//fn
                            //else if(func_char_string_compare(spec_key_buff, "win", 3) == true){Func_KB_Click(KEY_KEYBOARD_NULL);}//win
					        else
					        {
                                MessageBox.Show("无效的特殊字符" + char_buffer[i].ToString(), "错误");
					        }
				        }

				        //特殊件处理结束
				        special_key_cnt = 0;
				        special_key_step = SPEC_KEY_NONE;
			        }
			        else
			        {
                        spec_key_buff[special_key_cnt++] = char_buffer[i];
			        }
		        }
		        else
		        {
                    MessageBox.Show("无效的处理流程" + char_buffer[i].ToString(), "错误");
		        }
	        }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox_eKey.Text = "";
        }

        private void timer_FIFO_Snd_Tick(object sender, EventArgs e)
        {
            if ((modbus_send_cmd_is_busy == false) && (Func_KB_FIFO_HasData() == true))
            {
                modbus_send_cmd_is_busy = true;
                Func_Modbus_Send_03(REG_KEYBOARD, 1, Func_KB_FIFO_Output());
            }
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
	}
}
