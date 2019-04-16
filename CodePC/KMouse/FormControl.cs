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
	public partial class FormMain
	{
        bool caps_is_enable = false;

        void KB_EnterKey(keyQ.eKEY key)
        {
            if(com.serialport.IsOpen == true)
            {
                keyQ.eKEY key_add = key;
                if (button_Ctrl.BackColor == System.Drawing.Color.Yellow)
                {
                    button_Ctrl.BackColor = System.Drawing.Color.Gainsboro;
                    key_add |= keyQ.eKEY.CTRL;
                }

                if (button_Shift.BackColor == System.Drawing.Color.Yellow)
                {
                    button_Shift.BackColor = System.Drawing.Color.Gainsboro;
                    key_add |= keyQ.eKEY.SHIFT;
                }

                if (button_Alt.BackColor == System.Drawing.Color.Yellow)
                {
                    button_Alt.BackColor = System.Drawing.Color.Gainsboro;
                    key_add |= keyQ.eKEY.ALT;
                }

                kq.FIFO_Input(key_add);
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();  //没开串口就想点击发送热键，则有报警声
            }

            if ((mdbs.send_cmd_is_busy == false) && (kq.FIFO_HasData() == true))
            {
                mdbs.send_cmd_is_busy = true;
                mdbs.Send_03(Modbus.REG.KEYBOARD, 1, (uint)kq.FIFO_Output());
            }
        }

        /**********************鼠标单击键盘按钮，用得少，主要用热键***********************/
		private void button_Ctrl_Click(object sender, EventArgs e)
		{
            if (button_Ctrl.BackColor == System.Drawing.Color.Yellow)
            {
                //button_Ctrl.BackColor = System.Drawing.Color.Gainsboro;
                //KB_EnterKey(keyQ.eKEY.NULL + keyQ.eKEY.CTRL);
                KB_EnterKey(keyQ.eKEY.NULL); //底层去做这一级转换
            }
            else
            {
                button_Ctrl.BackColor = System.Drawing.Color.Yellow;
            }
		}

		private void button_Shift_L_Click(object sender, EventArgs e)
		{
            if (button_Shift.BackColor == System.Drawing.Color.Yellow)  //从亮到灭，即相当于只按shift一下
            {
                //button_Shift.BackColor = System.Drawing.Color.Gainsboro;
                //KB_EnterKey(keyQ.eKEY.NULL + keyQ.eKEY.Shift);
                KB_EnterKey(keyQ.eKEY.NULL);
            }
            else
            {
                button_Shift.BackColor = System.Drawing.Color.Yellow;
            }
		}

		private void button_ALt_Click(object sender, EventArgs e)
		{
            if (button_Alt.BackColor == System.Drawing.Color.Yellow)
            {
                //button_Alt.BackColor = System.Drawing.Color.Gainsboro;
                //KB_EnterKey(keyQ.eKEY.NULL + keyQ.eKEY.Alt);
                KB_EnterKey(keyQ.eKEY.NULL);
            }
            else
            {
                button_Alt.BackColor = System.Drawing.Color.Yellow;
            }
		}
		
        private void button_Win_Click(object sender, EventArgs e)
        {
            KB_EnterKey(keyQ.eKEY.NULL);
        }

		private void button_A_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.A);
		}

		private void button_B_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.B);
		}

		private void button_C_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.C);
		}

		private void button_D_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.D);
		}

		private void button_E_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.E);
		}

		private void button_F_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F);
		}

		private void button_G_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.G);
		}

		private void button_H_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.H);
		}

		private void button_I_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.I);
		}

		private void button_J_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.J);
		}
		private void button_K_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.K);
		}

		private void button_L_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.L);
		}

		private void button_M_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.M);
		}

		private void button_N_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.N);
		}

		private void button_O_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.O);
		}

		private void button_P_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.P);
		}

		private void button_Q_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.Q);
		}

		private void button_R_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.R);
		}

		private void button_S_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.S);
		}

		private void button_T_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.T);
		}

		private void button_U_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.U);
		}

		private void button_V_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.V);
		}

		private void button_W_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.W);
		}

		private void button_X_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.X);
		}

		private void button_Y_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.Y);
		}

		private void button_Z_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.Z);
		}
		/******************************************************************/
		private void button_Num1_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM1);
		}

		private void button_Num2_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM2);
		}

		private void button_Num3_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM3);
		}

		private void button_Num4_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM4);
		}

		private void button_Num5_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM5);
		}

		private void button_Num6_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM6);
		}

		private void button_Num7_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM7);
		}

		private void button_Num8_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM8);
		}

		private void button_Num9_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM9);
		}
		private void button_Num0_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.NUM0);
		}
		/******************************************************************/
		private void button_Enter_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.ENTER);
		}
		private void button_ESC_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.ESC);
		}

		private void button_BackSpace_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.BACK);
		}

		private void button_Tab_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.TAB);
		}

		private void button_Space_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.SPACE);
		}
		/******************************************************************/
		private void button_VV_Min_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.MIN);
		}

		private void button_VV_Add_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.ADD);
		}

		private void button_VV_kuo_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.KUO1);
		}

		private void button_VV_Kuo2_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.KUO2);
		}

		private void button_VV_Or_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.OR);
		}

		private void button_VV_Mao_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.MAO);
		}

		private void button_VV_Fen_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.FEN);
		}

		private void button_VV_Dou_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.DOU);
		}

		private void button_VV_Xiaoyu_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.XIAO);
		}

		private void button_VV_Dayu_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.DA);
		}

		private void button_WenHao_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.WEN);
		}
		/******************************************************************/
		private void button_Caps_Click(object sender, EventArgs e)
		{			
			if(caps_is_enable == false)
			{
				caps_is_enable = true;
				button_Caps.BackColor = System.Drawing.Color.Yellow;
			}
			else
			{
				caps_is_enable = false;
				button_Caps.BackColor = System.Drawing.Color.Gainsboro;		
			}
			KB_EnterKey(keyQ.eKEY.CAPS);
		}

		private void button_F1_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F1);
		}

		private void button_F2_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F2);
		}

		private void button_F3_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F3);
		}

		private void button_F4_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F4);
		}

		private void button_F5_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F5);
		}

		private void button_F6_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F6);
		}

		private void button_F7_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F7);
		}

		private void button_F8_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F8);
		}

		private void button_F9_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F9);
		}

		private void button_F10_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F10);
		}

		private void button_F11_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F11);
		}

		private void button_F12_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.F12);
		}

		private void button_Screen_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.PRINTSCREEN);
		}
		/******************************************************************/
		private void button_Home_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.HOME);
		}
		private void button_PageUp_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.PAGEUP);
		}
		private void button_Del_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.DEL);
		}
		private void button_End_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.END);
		}
		private void button_PageDwon_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.PAGEDOWN);
		}
		/******************************************************************/
		private void button_Right_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.RIGHT);
		}
		private void button_Left_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.LEFT);
		}
		private void button_Down_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.DOWN);
		}
		private void button_Up_Click(object sender, EventArgs e)
		{
			KB_EnterKey(keyQ.eKEY.UP);
		}
		/******************************************************************/

        /******************************************************************/
        private void Func_Mouse_Press(uint key, bool is_down)
		{
			if(is_down == true)
			{
				kq.mouse_press_en[key] = true;
				mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, key);
			}
			else
			{
				kq.mouse_press_en[key] = false;
			}
		}

		private void button_MoveUp_MouseDown(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVEUP, true);
		}

		private void button_MoveUp_MouseUp(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVEUP, false);
		}

		private void button_MoveDown_MouseDown(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVEDOWN, true);
		}

		private void button_MoveDown_MouseUp(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVEDOWN, false);
		}

		private void button_MoveLeft_MouseDown(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVELEFT, true);
		}

		private void button_MoveLeft_MouseUp(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVELEFT, false);
		}

		private void button_MoveRight_MouseDown(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVERIGHT, true);
		}

		private void button_MoveRight_MouseUp(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.MOVERIGHT, false);
		}		

		private void button_RollUp_MouseDown(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.ROLLUP, true);
		}

		private void button_RollUp_MouseUp(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.ROLLUP, false);
		}

		private void button_RollDown_MouseDown(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.ROLLDOWN, true);
		}

		private void button_RollDown_MouseUp(object sender, MouseEventArgs e)
		{
			Func_Mouse_Press(keyQ.MOUSE.PRESS.ROLLDOWN, false);
		}
		/******************************************************************/

        /******************************************************************/
        private void label_MouseSpeed_Click(object sender, EventArgs e)
		{
			if(kq.mouse_speed_chk == false)
			{
				kq.mouse_speed_chk = true;
				mdbs.Send_03(Modbus.REG.MOUSE_SPEED, 1, keyQ.MOUSE.SPEED.CHECK);
			}
		}
        private void button_SpeedUp_Click(object sender, EventArgs e)
		{
			if(button_SpeedUp.Enabled == true)
			{
				button_SpeedUp.Enabled = false;
				mdbs.Send_03(Modbus.REG.MOUSE_SPEED, 1, keyQ.MOUSE.SPEED.UP);
			}
		}

		private void button_SpeedDown_Click(object sender, EventArgs e)
		{
			if(button_SpeedDown.Enabled == true)
			{
				button_SpeedDown.Enabled = false;
				mdbs.Send_03(Modbus.REG.MOUSE_SPEED, 1, keyQ.MOUSE.SPEED.DOWN);
			}
		}

		private void button_ClickLeft_Click(object sender, EventArgs e)
		{
			if(button_ClickLeft.Enabled == true)
			{
				button_ClickLeft.Enabled = false;
				mdbs.Send_03(Modbus.REG.MOUSE_CLICK, 1, keyQ.MOUSE.CLICK.LEFT);
			}
		}

		private void button_ClickRight_Click(object sender, EventArgs e)
		{
			if(button_ClickRight.Enabled == true)
			{
				button_ClickRight.Enabled = false;
				mdbs.Send_03(Modbus.REG.MOUSE_CLICK, 1, keyQ.MOUSE.CLICK.RIGHT);
			}
		}
		/******************************************************************/
		private void button_P_Identify_Click(object sender, EventArgs e)
		{
			mdbs.Send_03(Modbus.REG.IDENTIFY, 1, 0);
		}

		private void button_Reboot_Click(object sender, EventArgs e)
		{
			mdbs.Send_03(Modbus.REG.SYSTEM_REBOOT, 1, 0);
		}

		private void button_Reconect_Click(object sender, EventArgs e)
		{
			mdbs.Send_03(Modbus.REG.USB_RECONNECT, 1, 0);
		}
        /***********************鼠标单击键盘按钮 END**************************/

        /**************************键盘热键 START****************************/
        private void timer_ReleaseFuncKey_Tick(object sender, EventArgs e)
        {
            if( (last_key_code == Keys.ControlKey) ||
                (last_key_code == Keys.ShiftKey)||
                (last_key_code == Keys.Menu) )
            {
                Console.WriteLine("release the func key:{0}", last_key_code);
                last_key_code = Keys.None;
                KB_EnterKey(keyQ.eKEY.NULL);
                return;
            }

            timer_ReleaseFuncKey.Enabled = false;
        }

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int keyCode);

        Keys last_key_code;
		private void Func_KMouse_KeyDown(Keys KeyCode)
		{
			uint key_func = (uint)KeyCode >> 16;
			KeyCode = (Keys)((uint)KeyCode & (0x0000FFFFu));

			Console.WriteLine("KEYDOWN>> code:{0} func:{1} last:{2}", KeyCode, key_func, last_key_code);
                        
            if( ((last_key_code == Keys.ControlKey) && (KeyCode == Keys.ControlKey)) ||
                ((last_key_code == Keys.ShiftKey) && (KeyCode == Keys.ShiftKey)) ||
                ((last_key_code == Keys.Menu) && (KeyCode == Keys.Menu)) )
            {
                timer_ReleaseFuncKey.Enabled = false;//释放定时器重新计时
                timer_ReleaseFuncKey.Enabled = true;

                Console.WriteLine("pressing the same func key, return...");
                return;
            }
            last_key_code = KeyCode;

            if (KeyCode == Keys.NumPad1) { KB_EnterKey(keyQ.eKEY.NUM1); }
            if (KeyCode == Keys.NumPad2) { KB_EnterKey(keyQ.eKEY.NUM2); }
            if (KeyCode == Keys.NumPad3) { KB_EnterKey(keyQ.eKEY.NUM3); }
            if (KeyCode == Keys.NumPad4) { KB_EnterKey(keyQ.eKEY.NUM4); }
            if (KeyCode == Keys.NumPad5) { KB_EnterKey(keyQ.eKEY.NUM5); }
            if (KeyCode == Keys.NumPad6) { KB_EnterKey(keyQ.eKEY.NUM6); }
            if (KeyCode == Keys.NumPad7) { KB_EnterKey(keyQ.eKEY.NUM7); }
            if (KeyCode == Keys.NumPad8) { KB_EnterKey(keyQ.eKEY.NUM8); }
            if (KeyCode == Keys.NumPad9) { KB_EnterKey(keyQ.eKEY.NUM9); }
            if (KeyCode == Keys.NumPad0) { KB_EnterKey(keyQ.eKEY.NUM0); }

            if (KeyCode == Keys.Divide) { KB_EnterKey(keyQ.eKEY.WEN); }//  /
            if (KeyCode == Keys.Multiply) { KB_EnterKey(keyQ.eKEY.NUM8 | keyQ.eKEY.SHIFT); }//  *
            if (KeyCode == Keys.Decimal) { KB_EnterKey(keyQ.eKEY.DA); }//  .


			keyQ.eKEY key_add = 0;
            if ((key_func & 0x0001) != 0)   //从打印中看出来的
            {
                key_add |= keyQ.eKEY.SHIFT;
            }
            if ((key_func & 0x0002) != 0)
            {
                key_add |= keyQ.eKEY.CTRL;
            }
            if ((key_func & 0x0004) != 0)
            {
                key_add |= keyQ.eKEY.ALT;
            }

			if(KeyCode == Keys.A) { KB_EnterKey(keyQ.eKEY.A | key_add); }
			if(KeyCode == Keys.B) { KB_EnterKey(keyQ.eKEY.B | key_add); }
			if(KeyCode == Keys.C) { KB_EnterKey(keyQ.eKEY.C | key_add); }
			if(KeyCode == Keys.D) { KB_EnterKey(keyQ.eKEY.D | key_add); }
			if(KeyCode == Keys.E) { KB_EnterKey(keyQ.eKEY.E | key_add); }
			if(KeyCode == Keys.F) { KB_EnterKey(keyQ.eKEY.F | key_add); }
			if(KeyCode == Keys.G) { KB_EnterKey(keyQ.eKEY.G | key_add); }
			if(KeyCode == Keys.H) { KB_EnterKey(keyQ.eKEY.H | key_add); }
			if(KeyCode == Keys.I) { KB_EnterKey(keyQ.eKEY.I | key_add); }
			if(KeyCode == Keys.J) { KB_EnterKey(keyQ.eKEY.J | key_add); }
			if(KeyCode == Keys.K) { KB_EnterKey(keyQ.eKEY.K | key_add); }
			if(KeyCode == Keys.L) { KB_EnterKey(keyQ.eKEY.L | key_add); }
			if(KeyCode == Keys.M) { KB_EnterKey(keyQ.eKEY.M | key_add); }
			if(KeyCode == Keys.N) { KB_EnterKey(keyQ.eKEY.N | key_add); }
			if(KeyCode == Keys.O) { KB_EnterKey(keyQ.eKEY.O | key_add); }
			if(KeyCode == Keys.P) { KB_EnterKey(keyQ.eKEY.P | key_add); }
			if(KeyCode == Keys.Q) { KB_EnterKey(keyQ.eKEY.Q | key_add); }
			if(KeyCode == Keys.R) { KB_EnterKey(keyQ.eKEY.R | key_add); }
			if(KeyCode == Keys.S) { KB_EnterKey(keyQ.eKEY.S | key_add); }
			if(KeyCode == Keys.T) { KB_EnterKey(keyQ.eKEY.T | key_add); }
			if(KeyCode == Keys.U) { KB_EnterKey(keyQ.eKEY.U | key_add); }
			if(KeyCode == Keys.V) { KB_EnterKey(keyQ.eKEY.V | key_add); }
			if(KeyCode == Keys.W) { KB_EnterKey(keyQ.eKEY.W | key_add); }
			if(KeyCode == Keys.X) { KB_EnterKey(keyQ.eKEY.X | key_add); }
			if(KeyCode == Keys.Y) { KB_EnterKey(keyQ.eKEY.Y | key_add); }
			if(KeyCode == Keys.Z) { KB_EnterKey(keyQ.eKEY.Z | key_add); }

			if(KeyCode == Keys.D1) { KB_EnterKey(keyQ.eKEY.NUM1 | key_add); }
			if(KeyCode == Keys.D2) { KB_EnterKey(keyQ.eKEY.NUM2 | key_add); }
			if(KeyCode == Keys.D3) { KB_EnterKey(keyQ.eKEY.NUM3 | key_add); }
			if(KeyCode == Keys.D4) { KB_EnterKey(keyQ.eKEY.NUM4 | key_add); }
			if(KeyCode == Keys.D5) { KB_EnterKey(keyQ.eKEY.NUM5 | key_add); }
			if(KeyCode == Keys.D6) { KB_EnterKey(keyQ.eKEY.NUM6 | key_add); }
			if(KeyCode == Keys.D7) { KB_EnterKey(keyQ.eKEY.NUM7 | key_add); }
			if(KeyCode == Keys.D8) { KB_EnterKey(keyQ.eKEY.NUM8 | key_add); }
			if(KeyCode == Keys.D9) { KB_EnterKey(keyQ.eKEY.NUM9 | key_add); }
			if(KeyCode == Keys.D0) { KB_EnterKey(keyQ.eKEY.NUM0 | key_add); }

			if(KeyCode == Keys.Escape) { KB_EnterKey(keyQ.eKEY.ESC | key_add); }
			if(KeyCode == Keys.Back) { KB_EnterKey(keyQ.eKEY.BACK | key_add); }
			if(KeyCode == Keys.Tab) { KB_EnterKey(keyQ.eKEY.TAB | key_add); }
			if(KeyCode == Keys.Space) { KB_EnterKey(keyQ.eKEY.SPACE | key_add); }

			if(KeyCode == Keys.OemMinus) { KB_EnterKey(keyQ.eKEY.MIN | key_add); }
			if(KeyCode == Keys.Oemplus) { KB_EnterKey(keyQ.eKEY.ADD | key_add); }
			if(KeyCode == Keys.OemOpenBrackets) { KB_EnterKey(keyQ.eKEY.KUO1 | key_add); }
			if(KeyCode == Keys.OemCloseBrackets) { KB_EnterKey(keyQ.eKEY.KUO2 | key_add); }
			if(KeyCode == Keys.OemPipe) { KB_EnterKey(keyQ.eKEY.OR | key_add); }
			if(KeyCode == Keys.OemSemicolon) { KB_EnterKey(keyQ.eKEY.MAO | key_add); }
			if(KeyCode == Keys.OemQuotes) { KB_EnterKey(keyQ.eKEY.FEN | key_add); }
			if(KeyCode == Keys.Oemtilde) { KB_EnterKey(keyQ.eKEY.DOU | key_add); }
			if(KeyCode == Keys.Oemcomma) { KB_EnterKey(keyQ.eKEY.XIAO | key_add); }
			if(KeyCode == Keys.OemPeriod) { KB_EnterKey(keyQ.eKEY.DA | key_add); }
			if(KeyCode == Keys.OemQuestion) { KB_EnterKey(keyQ.eKEY.WEN | key_add); }

			if(KeyCode == Keys.CapsLock) { KB_EnterKey(keyQ.eKEY.CAPS | key_add); }
			if(KeyCode == Keys.F1) { KB_EnterKey(keyQ.eKEY.F1 | key_add); }
			if(KeyCode == Keys.F2) { KB_EnterKey(keyQ.eKEY.F2 | key_add); }
			if(KeyCode == Keys.F3) { KB_EnterKey(keyQ.eKEY.F3 | key_add); }
			if(KeyCode == Keys.F4) { KB_EnterKey(keyQ.eKEY.F4 | key_add); }
			if(KeyCode == Keys.F5) { KB_EnterKey(keyQ.eKEY.F5 | key_add); }
			if(KeyCode == Keys.F6) { KB_EnterKey(keyQ.eKEY.F6 | key_add); }
			if(KeyCode == Keys.F7) { KB_EnterKey(keyQ.eKEY.F7 | key_add); }
			if(KeyCode == Keys.F8) { KB_EnterKey(keyQ.eKEY.F8 | key_add); }
			if(KeyCode == Keys.F9) { KB_EnterKey(keyQ.eKEY.F9 | key_add); }
			if(KeyCode == Keys.F10) { KB_EnterKey(keyQ.eKEY.F10 | key_add); }
			if(KeyCode == Keys.F11) { KB_EnterKey(keyQ.eKEY.F11 | key_add); }
			if(KeyCode == Keys.F12) { KB_EnterKey(keyQ.eKEY.F12 | key_add); }
			if(KeyCode == Keys.PrintScreen) { KB_EnterKey(keyQ.eKEY.PRINTSCREEN | key_add); }
            if (KeyCode == Keys.Delete) { KB_EnterKey(keyQ.eKEY.DEL | key_add); }

            bool NumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
            if (NumLock == true)//数字键亮起时，当做数字键盘来用，灭时当做操作鼠标来用
            {
                if (KeyCode == Keys.Right) { KB_EnterKey(keyQ.eKEY.RIGHT | key_add); }
                if (KeyCode == Keys.Left) { KB_EnterKey(keyQ.eKEY.LEFT | key_add); }
                if (KeyCode == Keys.Down) { KB_EnterKey(keyQ.eKEY.DOWN | key_add); }
                if (KeyCode == Keys.Up) { KB_EnterKey(keyQ.eKEY.UP | key_add); }

                if (KeyCode == Keys.Home) { KB_EnterKey(keyQ.eKEY.HOME | key_add); }
                if (KeyCode == Keys.End) { KB_EnterKey(keyQ.eKEY.END | key_add); }
                if (KeyCode == Keys.PageDown) { KB_EnterKey(keyQ.eKEY.PAGEDOWN | key_add); }
                if (KeyCode == Keys.PageUp) { KB_EnterKey(keyQ.eKEY.PAGEUP | key_add); }
                                
                if (KeyCode == Keys.Add) { KB_EnterKey(keyQ.eKEY.ADD | keyQ.eKEY.SHIFT); }// +
                if (KeyCode == Keys.Subtract) { KB_EnterKey(keyQ.eKEY.MIN); }//  -
            }
            else
            {
                if (KeyCode == Keys.Right) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.PRESS.MOVERIGHT);
                }
                if (KeyCode == Keys.Left) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.PRESS.MOVELEFT);
                }
                if (KeyCode == Keys.Down) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.PRESS.MOVEDOWN);
                }
                if (KeyCode == Keys.Up) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.PRESS.MOVEUP);
                }

                if (KeyCode == Keys.Home) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_CLICK, 1, keyQ.MOUSE.CLICK.LEFT);
                }
                if (KeyCode == Keys.PageUp)
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_CLICK, 1, keyQ.MOUSE.CLICK.RIGHT);
                }

                if (KeyCode == Keys.End) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.PRESS.ROLLUP);
                }
                if (KeyCode == Keys.PageDown) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_PRESS, 1, keyQ.MOUSE.PRESS.ROLLDOWN);
                }

                if (KeyCode == Keys.Add) 
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_SPEED, 1, keyQ.MOUSE.SPEED.UP);
                }

                if (KeyCode == Keys.Subtract)
                {
                    mdbs.Send_03(Modbus.REG.MOUSE_SPEED, 1, keyQ.MOUSE.SPEED.DOWN);
                }
            }

			if(KeyCode == Keys.Enter) { KB_EnterKey(keyQ.eKEY.ENTER | key_add); }

            //if( (KeyCode == Keys.ControlKey) ||
            //    (KeyCode == Keys.ShiftKey) ||
            //    (KeyCode == Keys.Menu) ||
            //    (KeyCode == Keys.ControlKey) ||
            //    (KeyCode == Keys.ControlKey) ||
            //    (KeyCode == Keys.ControlKey) )
            //{
            //    { KB_EnterKey(keyQ.eKEY.NULL | key_add); }
            //}

            if(KeyCode == Keys.ControlKey) { KB_EnterKey(keyQ.eKEY.NULL | key_add); }
            if(KeyCode == Keys.ShiftKey) { KB_EnterKey(keyQ.eKEY.NULL | key_add); }
            if(KeyCode == Keys.Menu) { KB_EnterKey(keyQ.eKEY.NULL | key_add); }
		}

		private void KMouse_KeyDown(object sender, KeyEventArgs e)	        //真正的KeyDown被架空，使用ProcessDialogKey作为keydown
		{
			//Func_KMouse_KeyDown();
		}

		protected override bool ProcessDialogKey(Keys keyData)		        //所有默认热键的keydown入口在这里
		{
			Func_KMouse_KeyDown(keyData);
			return true;
		}

		private void KMouse_KeyUp(object sender, KeyEventArgs e)
		{
#if false   //鼠标的动作才需要up和down，因为up时要释放按钮使能
            Console.WriteLine("KEYUP>>keycode:{0}", e.KeyCode);

			if(e.KeyCode == Keys.NumPad8) { Func_MouseUp_MoveUp(); }
			if(e.KeyCode == Keys.NumPad2) { Func_MouseUp_MoveDown(); }
			if(e.KeyCode == Keys.NumPad4) { Func_MouseUp_MoveLeft(); }
			if(e.KeyCode == Keys.NumPad6) { Func_MouseUp_MoveRight(); }
			if(e.KeyCode == Keys.NumPad1) { Func_MouseUp_RollUp(); }
			if(e.KeyCode == Keys.NumPad3) { Func_MouseUp_RollDown(); }

			/************************/
			if(e.KeyCode == Keys.NumPad7)
			{
				Func_ClickLeft_Click();
			}
			if(e.KeyCode == Keys.NumPad9)
			{
				Func_ClickRight_Click();
			}
			if(e.KeyCode == Keys.Add)
			{
				Func_SpeedUp_Click();
			}
			if(e.KeyCode == Keys.Subtract)
			{
				Func_SpeedDown_Click();
			}
			/************************/
#endif
		}

        /**************************键盘热键 END******************************/


        /**************************EKey Start******************************/
        private void button_eKey_Click(object sender, EventArgs e)
        {
	        uint i;

	        const byte SPEC_KEY_NONE = 0;
            const byte SPEC_KEY_CHK = 1;
            const byte SPEC_KEY_ING = 2;

            char[] spec_key_buff = new char[32];

	        uint special_key_cnt = 0;
	        byte special_key_step = SPEC_KEY_NONE;
            
            char[] char_buffer = textBox_eKey.Text.ToCharArray();

	        for(i = 0; i < textBox_eKey.TextLength; i++)
	        {
                Console.WriteLine("c:{0}\n", char_buffer[i]);

		        if(special_key_step == SPEC_KEY_NONE)
		        {                    
			        /**********************************将基本按键解释 Start*********************************/
			        switch(char_buffer[i])
			        {
				        case '`': {KB_EnterKey(keyQ.eKEY.DOU); break; }
				        case '1': {KB_EnterKey(keyQ.eKEY.NUM1); break; }
				        case '2': {KB_EnterKey(keyQ.eKEY.NUM2); break; }
				        case '3': {KB_EnterKey(keyQ.eKEY.NUM3); break; }
				        case '4': {KB_EnterKey(keyQ.eKEY.NUM4); break; }
				        case '5': {KB_EnterKey(keyQ.eKEY.NUM5); break; }
				        case '6': {KB_EnterKey(keyQ.eKEY.NUM6); break; }
				        case '7': {KB_EnterKey(keyQ.eKEY.NUM7); break; }
				        case '8': {KB_EnterKey(keyQ.eKEY.NUM8); break; }
				        case '9': {KB_EnterKey(keyQ.eKEY.NUM9); break; }
				        case '0': {KB_EnterKey(keyQ.eKEY.NUM0); break; }
				        case '-': {KB_EnterKey(keyQ.eKEY.MIN); break; }
				        case '=': {KB_EnterKey(keyQ.eKEY.ADD); break; }

				        case 'q': {KB_EnterKey(keyQ.eKEY.Q); break; }
				        case 'w': {KB_EnterKey(keyQ.eKEY.W); break; }
				        case 'e': {KB_EnterKey(keyQ.eKEY.E); break; }
				        case 'r': {KB_EnterKey(keyQ.eKEY.R); break; }
				        case 't': {KB_EnterKey(keyQ.eKEY.T); break; }
				        case 'y': {KB_EnterKey(keyQ.eKEY.Y); break; }
				        case 'u': {KB_EnterKey(keyQ.eKEY.U); break; }
				        case 'i': {KB_EnterKey(keyQ.eKEY.I); break; }
				        case 'o': {KB_EnterKey(keyQ.eKEY.O); break; }
				        case 'p': {KB_EnterKey(keyQ.eKEY.P); break; }
                        //case '[': {KB_EnterKey(keyQ.eKEY.KUO1); break; }
				        case ']': {KB_EnterKey(keyQ.eKEY.KUO2); break; }
				        case '\\': {KB_EnterKey(keyQ.eKEY.OR); break; }

				        case 'a': {KB_EnterKey(keyQ.eKEY.A); break; }
				        case 's': {KB_EnterKey(keyQ.eKEY.S); break; }
				        case 'd': {KB_EnterKey(keyQ.eKEY.D); break; }
				        case 'f': {KB_EnterKey(keyQ.eKEY.F); break; }
				        case 'g': {KB_EnterKey(keyQ.eKEY.G); break; }
				        case 'h': {KB_EnterKey(keyQ.eKEY.H); break; }
				        case 'j': {KB_EnterKey(keyQ.eKEY.J); break; }
				        case 'k': {KB_EnterKey(keyQ.eKEY.K); break; }
				        case 'l': {KB_EnterKey(keyQ.eKEY.L); break; }
				        case ';': {KB_EnterKey(keyQ.eKEY.MAO); break; }
				        case '\'':{KB_EnterKey(keyQ.eKEY.FEN); break; }

				        case 'z': {KB_EnterKey(keyQ.eKEY.Z); break; }
				        case 'x': {KB_EnterKey(keyQ.eKEY.X); break; }
				        case 'c': {KB_EnterKey(keyQ.eKEY.C); break; }
				        case 'v': {KB_EnterKey(keyQ.eKEY.V); break; }
				        case 'b': {KB_EnterKey(keyQ.eKEY.B); break; }
				        case 'n': {KB_EnterKey(keyQ.eKEY.N); break; }
				        case 'm': {KB_EnterKey(keyQ.eKEY.M); break; }
				        case ',': {KB_EnterKey(keyQ.eKEY.XIAO); break; }
				        case '.': {KB_EnterKey(keyQ.eKEY.DA); break; }
				        case '/': {KB_EnterKey(keyQ.eKEY.WEN); break; }
				        case ' ': {KB_EnterKey(keyQ.eKEY.SPACE); break; }
				        /**********************************将基本按键解释 END*********************************/

				        /**********************************将上档按键解释 Start*********************************/
				        case '~': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.DOU);break;}
				        case '!': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM1);break;}
				        case '@': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM2);break;}
				        case '#': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM3);break;}
				        case '$': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM4);break;}
				        case '%': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM5);break;}
				        case '^': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM6);break;}
				        case '&': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM7);break;}
				        case '*': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM8);break;}
				        case '(': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM9);break;}
				        case ')': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.NUM0);break;}
				        case '_': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.MIN);break;}
				        case '+': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.ADD);break;}

				        case 'Q': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.Q);break;}
				        case 'W': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.W);break;}
				        case 'E': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.E);break;}
				        case 'R': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.R);break;}
				        case 'T': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.T);break;}
				        case 'Y': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.Y);break;}
				        case 'U': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.U);break;}
				        case 'I': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.I);break;}
				        case 'O': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.O);break;}
				        case 'P': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.P);break;}
				        case '{': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.KUO1);break;}
				        case '}': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.KUO2);break;}
				        case '|': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.OR);break;}

				        case 'A': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.A);break;}
				        case 'S': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.S);break;}
				        case 'D': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.D);break;}
				        case 'F': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.F);break;}
				        case 'G': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.G);break;}
				        case 'H': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.H);break;}
				        case 'J': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.J);break;}
				        case 'K': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.K);break;}
				        case 'L': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.L);break;}
				        case ':': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.MAO);break;}
				        case '"': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.FEN);break;}

				        case 'Z': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.Z);break;}
				        case 'X': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.X);break;}
				        case 'C': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.C);break;}
				        case 'V': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.V);break;}
				        case 'B': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.B);break;}
				        case 'N': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.N);break;}
				        case 'M': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.M);break;}
				        case '<': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.XIAO);break;}
				        case '>': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.DA);break;}
				        case '?': {button_Shift.BackColor = System.Drawing.Color.Yellow;KB_EnterKey(keyQ.eKEY.WEN);break;}
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
                        KB_EnterKey(keyQ.eKEY.KUO1);
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
                        if (Func.Char_String_compare(spec_key_buff, "shift", 5) == true)	//Shift
				        {
                            button_Shift.BackColor = System.Drawing.Color.Yellow;
				        }
                        else if (Func.Char_String_compare(spec_key_buff, "ctrl", 4) == true)//Ctrl	
				        {
                            button_Ctrl.BackColor = System.Drawing.Color.Yellow;
				        }
                        else if (Func.Char_String_compare(spec_key_buff, "alt", 3) == true)	//Alt
				        {
                            button_Alt.BackColor = System.Drawing.Color.Yellow;
				        }
				        else
				        {

                            if (Func.Char_String_compare(spec_key_buff, "esc", 3) == true) { KB_EnterKey(keyQ.eKEY.ESC); }//ESC
                            else if (Func.Char_String_compare(spec_key_buff, "f1", 2) == true) { KB_EnterKey(keyQ.eKEY.F1); }//F1
                            else if (Func.Char_String_compare(spec_key_buff, "f2", 2) == true) { KB_EnterKey(keyQ.eKEY.F2); }//F2
                            else if (Func.Char_String_compare(spec_key_buff, "f3", 2) == true) { KB_EnterKey(keyQ.eKEY.F3); }//F3
                            else if (Func.Char_String_compare(spec_key_buff, "f4", 2) == true) { KB_EnterKey(keyQ.eKEY.F4); }//F4
                            else if (Func.Char_String_compare(spec_key_buff, "f5", 2) == true) { KB_EnterKey(keyQ.eKEY.F5); }//F5
                            else if (Func.Char_String_compare(spec_key_buff, "f6", 2) == true) { KB_EnterKey(keyQ.eKEY.F6); }//F6
                            else if (Func.Char_String_compare(spec_key_buff, "f7", 2) == true) { KB_EnterKey(keyQ.eKEY.F7); }//F7
                            else if (Func.Char_String_compare(spec_key_buff, "f8", 2) == true) { KB_EnterKey(keyQ.eKEY.F8); }//F8
                            else if (Func.Char_String_compare(spec_key_buff, "f9", 2) == true) { KB_EnterKey(keyQ.eKEY.F9); }//F9
                            else if (Func.Char_String_compare(spec_key_buff, "f10", 3) == true) { KB_EnterKey(keyQ.eKEY.F10); }//F10
                            else if (Func.Char_String_compare(spec_key_buff, "f11", 3) == true) { KB_EnterKey(keyQ.eKEY.F11); }//F11
                            else if (Func.Char_String_compare(spec_key_buff, "f12", 3) == true) { KB_EnterKey(keyQ.eKEY.F12); }//F12
                            else if (Func.Char_String_compare(spec_key_buff, "del", 3) == true) { KB_EnterKey(keyQ.eKEY.DEL); }//Del
                            else if (Func.Char_String_compare(spec_key_buff, "bp", 2) == true) { KB_EnterKey(keyQ.eKEY.BACK); }//BP
                            else if (Func.Char_String_compare(spec_key_buff, "tab", 3) == true) { KB_EnterKey(keyQ.eKEY.TAB); ; }//Tab
                            else if (Func.Char_String_compare(spec_key_buff, "caps", 4) == true) { KB_EnterKey(keyQ.eKEY.CAPS); ; }//Caps
                            else if (Func.Char_String_compare(spec_key_buff, "enter", 5) == true) { KB_EnterKey(keyQ.eKEY.ENTER); ; }//Enter
                            else if (Func.Char_String_compare(spec_key_buff, "home", 4) == true) { KB_EnterKey(keyQ.eKEY.HOME); ; }//Home
                            else if (Func.Char_String_compare(spec_key_buff, "psc", 3) == true) { KB_EnterKey(keyQ.eKEY.PRINTSCREEN); ; }//PrintScreen
                            else if (Func.Char_String_compare(spec_key_buff, "up", 2) == true) { KB_EnterKey(keyQ.eKEY.UP); ; }//Up
                            else if (Func.Char_String_compare(spec_key_buff, "end", 2) == true) { KB_EnterKey(keyQ.eKEY.END); ; }//End				
                            else if (Func.Char_String_compare(spec_key_buff, "pageup", 6) == true) { KB_EnterKey(keyQ.eKEY.PAGEUP); ; }//Page Up
                            else if (Func.Char_String_compare(spec_key_buff, "pagedown", 8) == true) { KB_EnterKey(keyQ.eKEY.PAGEDOWN); ; }//Page Down
                            else if (Func.Char_String_compare(spec_key_buff, "left", 4) == true) { KB_EnterKey(keyQ.eKEY.LEFT); ; }//Left
                            else if (Func.Char_String_compare(spec_key_buff, "down", 4) == true) { KB_EnterKey(keyQ.eKEY.DOWN); ; }//Down
                            else if (Func.Char_String_compare(spec_key_buff, "right", 5) == true) { KB_EnterKey(keyQ.eKEY.RIGHT); ; }//Right
                            else if (Func.Char_String_compare(spec_key_buff, "null", 4) == true) { KB_EnterKey(keyQ.eKEY.NULL); ; }//空按键，触发组合键
                            //else if(Func.Char_String_compare(spec_key_buff, "fn", 2) == true){KB_EnterKey(keyQ.eKEY.NULL);}//fn
                            //else if(Func.Char_String_compare(spec_key_buff, "win", 3) == true){KB_EnterKey(keyQ.eKEY.NULL);}//win
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
        /***************************EKey End*******************************/
	}
}

