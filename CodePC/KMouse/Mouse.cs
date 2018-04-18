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
	public partial class KMouse
	{
		const u8 REG_IDENTIFY = 1;
		const u32 KEY_MOUSE_Leave = 0x0000FFFF;

		const u8 REG_MOUSE_PRESS = 3;
		const u32 KEY_MousePress_MoveUP = 0;
		const u32 KEY_MousePress_MoveDown = 1;
		const u32 KEY_MousePress_MoveLeft = 2;
		const u32 KEY_MousePress_MoveRight = 3;
		const u32 KEY_MousePress_RollUp = 4;
		const u32 KEY_MousePress_RollDown = 5;
		const u32 KEY_MousePress_ALL = 6;
		bool[] mouse_press_en = new bool[KEY_MousePress_ALL];
		bool[] mouse_press_en_last = new bool[KEY_MousePress_ALL];		

		const u8 REG_MOUSE_CLICK = 5;
		const u32 KEY_MouseClick_ClickLeft = 0;
		const u32 KEY_MouseClick_ClickRight = 1;
		
		const u8 REG_MOUSE_SPEED = 7;
		const u32 KEY_MouseSpeed_SpeedUp = 0;
		const u32 KEY_MouseSpeed_SpeedDown = 1;
		const u32 KEY_MouseSpeed_SpeedChk = 2;
		bool mouse_speed_chk = false;

		/******************************************************************/
		private void Func_ClickLeft_Click()
		{
			if(button_ClickLeft.Enabled == true)
			{
				button_ClickLeft.Enabled = false;
				Func_Modbus_Send_03(REG_MOUSE_CLICK, 1, KEY_MouseClick_ClickLeft);			
			}
		}

		private void Func_ClickRight_Click()
		{
			if(button_ClickRight.Enabled == true)
			{
				button_ClickRight.Enabled = false;
				Func_Modbus_Send_03(REG_MOUSE_CLICK, 1, KEY_MouseClick_ClickRight);
			}
		}
		private void Func_SpeedUp_Click()
		{
			if(button_SpeedUp.Enabled == true)
			{
				button_SpeedUp.Enabled = false;
				Func_Modbus_Send_03(REG_MOUSE_SPEED, 1, KEY_MouseSpeed_SpeedUp);		
			}
		}

		private void Func_SpeedDown_Click()
		{
			if(button_SpeedDown.Enabled == true)
			{
				button_SpeedDown.Enabled = false;
				Func_Modbus_Send_03(REG_MOUSE_SPEED, 1, KEY_MouseSpeed_SpeedDown);
			}
		}
		/******************************************************************/
		private void button_ClickLeft_Click(object sender, EventArgs e)
		{
			Func_ClickLeft_Click();
		}

		private void button_ClickRight_Click(object sender, EventArgs e)
		{
			Func_ClickRight_Click();
		}

		private void button_SpeedUp_Click(object sender, EventArgs e)
		{
			Func_SpeedUp_Click();
		}

		private void button_SpeedDown_Click(object sender, EventArgs e)
		{
			Func_SpeedDown_Click();
		}
		/******************************************************************/


		private void button_P_Identify_Click(object sender, EventArgs e)
		{
			Func_Modbus_Send_03(REG_IDENTIFY, 1, 0);
		}

		/******************************************************************/

		private void Func_Mouse_Press(u32 key, bool is_down)
		{
			if(is_down)
			{
				mouse_press_en[key] = true;
				Func_Modbus_Send_03(REG_MOUSE_PRESS, 1, key);	
			}
			else
			{
				mouse_press_en[key] = false;
			}
		}

		private void Func_MoveUp_MouseDown()
		{
			Func_Mouse_Press(KEY_MousePress_MoveUP, true);
		}

		private void Func_MoveUp_MouseUp()
		{
			Func_Mouse_Press(KEY_MousePress_MoveUP, false);
		}

		private void Func_MoveDown_MouseDown()
		{
			Func_Mouse_Press(KEY_MousePress_MoveDown, true);
		}

		private void Func_MoveDown_MouseUp()
		{
			Func_Mouse_Press(KEY_MousePress_MoveDown, false);
		}

		private void Func_MoveLeft_MouseDown()
		{
			Func_Mouse_Press(KEY_MousePress_MoveLeft, true);
		}

		private void Func_MoveLeft_MouseUp()
		{
			Func_Mouse_Press(KEY_MousePress_MoveLeft, false);
		}

		private void Func_MoveRight_MouseDown()
		{
			Func_Mouse_Press(KEY_MousePress_MoveRight, true);
		}

		private void Func_MoveRight_MouseUp()
		{
			Func_Mouse_Press(KEY_MousePress_MoveRight, false);
		}

		private void Func_RollUp_MouseDown()
		{
			Func_Mouse_Press(KEY_MousePress_RollUp, true);
		}

		private void Func_RollUp_MouseUp()
		{
			Func_Mouse_Press(KEY_MousePress_RollUp, false);
		}

		private void Func_RollDown_MouseDown()
		{
			Func_Mouse_Press(KEY_MousePress_RollDown, true);
		}

		private void Func_RollDown_MouseUp()
		{
			Func_Mouse_Press(KEY_MousePress_RollDown, false);
		}
		/******************************************************************/

		/******************************************************************/
		private void button_MoveUp_MouseDown(object sender, MouseEventArgs e)
		{
			Func_MoveUp_MouseDown();
		}

		private void button_MoveUp_MouseUp(object sender, MouseEventArgs e)
		{
			Func_MoveUp_MouseUp();
		}

		private void button_MoveDown_MouseDown(object sender, MouseEventArgs e)
		{
			Func_MoveDown_MouseDown();
		}

		private void button_MoveDown_MouseUp(object sender, MouseEventArgs e)
		{
			Func_MoveDown_MouseUp();
		}

		private void button_MoveLeft_MouseDown(object sender, MouseEventArgs e)
		{
			Func_MoveLeft_MouseDown();
		}

		private void button_MoveLeft_MouseUp(object sender, MouseEventArgs e)
		{
			Func_MoveLeft_MouseUp();
		}

		private void button_MoveRight_MouseDown(object sender, MouseEventArgs e)
		{
			Func_MoveRight_MouseDown();
		}

		private void button_MoveRight_MouseUp(object sender, MouseEventArgs e)
		{
			Func_MoveRight_MouseUp();
		}		

		private void button_RollUp_MouseDown(object sender, MouseEventArgs e)
		{
			Func_RollUp_MouseDown();
		}

		private void button_RollUp_MouseUp(object sender, MouseEventArgs e)
		{
			Func_RollUp_MouseUp();
		}

		private void button_RollDown_MouseDown(object sender, MouseEventArgs e)
		{
			Func_RollDown_MouseDown();
		}

		private void button_RollDown_MouseUp(object sender, MouseEventArgs e)
		{
			Func_RollDown_MouseUp();
		}
		/******************************************************************/


		private void label_MouseSpeed_Click(object sender, EventArgs e)
		{
			if(mouse_speed_chk == false)
			{
				mouse_speed_chk = true;
				Func_Modbus_Send_03(REG_MOUSE_SPEED, 1, KEY_MouseSpeed_SpeedChk);
			}
		}

		private void Func_KMouse_KeyDown(Keys KeyCode)
		{
			u32 key_func = (u32)KeyCode >> 16;
			KeyCode = (Keys)((u32)KeyCode & (0x0000FFFFu));

			Console.WriteLine("KEYDOWN>>keycode:{0} {1}", KeyCode, key_func);

			if(KeyCode == Keys.NumPad8) { Func_MoveUp_MouseDown(); }
			if(KeyCode == Keys.NumPad2) { Func_MoveDown_MouseDown(); }
			if(KeyCode == Keys.NumPad4) { Func_MoveLeft_MouseDown(); }
			if(KeyCode == Keys.NumPad6) { Func_MoveRight_MouseDown(); }
			if(KeyCode == Keys.NumPad1) { Func_RollUp_MouseDown(); }
			if(KeyCode == Keys.NumPad3) { Func_RollDown_MouseDown(); }

			u32 key_add = 0;
            if ((key_func & 0x0001) != 0)   //从打印中看出来的
            {
                key_add |= KEY_KEYBOARD_Shift;
            }
            if ((key_func & 0x0002) != 0)
            {
                key_add |= KEY_KEYBOARD_Ctrl;
            }
            if ((key_func & 0x0004) != 0)
            {
                key_add |= KEY_KEYBOARD_Alt;
            }

			if(KeyCode == Keys.A) { Func_KB_Click(KEY_KEYBOARD_A + key_add); }
			if(KeyCode == Keys.B) { Func_KB_Click(KEY_KEYBOARD_B + key_add); }
			if(KeyCode == Keys.C) { Func_KB_Click(KEY_KEYBOARD_C + key_add); }
			if(KeyCode == Keys.D) { Func_KB_Click(KEY_KEYBOARD_D + key_add); }
			if(KeyCode == Keys.E) { Func_KB_Click(KEY_KEYBOARD_E + key_add); }
			if(KeyCode == Keys.F) { Func_KB_Click(KEY_KEYBOARD_F + key_add); }
			if(KeyCode == Keys.G) { Func_KB_Click(KEY_KEYBOARD_G + key_add); }
			if(KeyCode == Keys.H) { Func_KB_Click(KEY_KEYBOARD_H + key_add); }
			if(KeyCode == Keys.I) { Func_KB_Click(KEY_KEYBOARD_I + key_add); }
			if(KeyCode == Keys.J) { Func_KB_Click(KEY_KEYBOARD_J + key_add); }
			if(KeyCode == Keys.K) { Func_KB_Click(KEY_KEYBOARD_K + key_add); }
			if(KeyCode == Keys.L) { Func_KB_Click(KEY_KEYBOARD_L + key_add); }
			if(KeyCode == Keys.M) { Func_KB_Click(KEY_KEYBOARD_M + key_add); }
			if(KeyCode == Keys.N) { Func_KB_Click(KEY_KEYBOARD_N + key_add); }
			if(KeyCode == Keys.O) { Func_KB_Click(KEY_KEYBOARD_O + key_add); }
			if(KeyCode == Keys.P) { Func_KB_Click(KEY_KEYBOARD_P + key_add); }
			if(KeyCode == Keys.Q) { Func_KB_Click(KEY_KEYBOARD_Q + key_add); }
			if(KeyCode == Keys.R) { Func_KB_Click(KEY_KEYBOARD_R + key_add); }
			if(KeyCode == Keys.S) { Func_KB_Click(KEY_KEYBOARD_S + key_add); }
			if(KeyCode == Keys.T) { Func_KB_Click(KEY_KEYBOARD_T + key_add); }
			if(KeyCode == Keys.U) { Func_KB_Click(KEY_KEYBOARD_U + key_add); }
			if(KeyCode == Keys.V) { Func_KB_Click(KEY_KEYBOARD_V + key_add); }
			if(KeyCode == Keys.W) { Func_KB_Click(KEY_KEYBOARD_W + key_add); }
			if(KeyCode == Keys.X) { Func_KB_Click(KEY_KEYBOARD_X + key_add); }
			if(KeyCode == Keys.Y) { Func_KB_Click(KEY_KEYBOARD_Y + key_add); }
			if(KeyCode == Keys.Z) { Func_KB_Click(KEY_KEYBOARD_Z + key_add); }

			if(KeyCode == Keys.D1) { Func_KB_Click(KEY_KEYBOARD_Num1 + key_add); }
			if(KeyCode == Keys.D2) { Func_KB_Click(KEY_KEYBOARD_Num2 + key_add); }
			if(KeyCode == Keys.D3) { Func_KB_Click(KEY_KEYBOARD_Num3 + key_add); }
			if(KeyCode == Keys.D4) { Func_KB_Click(KEY_KEYBOARD_Num4 + key_add); }
			if(KeyCode == Keys.D5) { Func_KB_Click(KEY_KEYBOARD_Num5 + key_add); }
			if(KeyCode == Keys.D6) { Func_KB_Click(KEY_KEYBOARD_Num6 + key_add); }
			if(KeyCode == Keys.D7) { Func_KB_Click(KEY_KEYBOARD_Num7 + key_add); }
			if(KeyCode == Keys.D8) { Func_KB_Click(KEY_KEYBOARD_Num8 + key_add); }
			if(KeyCode == Keys.D9) { Func_KB_Click(KEY_KEYBOARD_Num9 + key_add); }
			if(KeyCode == Keys.D0) { Func_KB_Click(KEY_KEYBOARD_Num0 + key_add); }

			if(KeyCode == Keys.Escape) { Func_KB_Click(KEY_KEYBOARD_ESC + key_add); }
			if(KeyCode == Keys.Back) { Func_KB_Click(KEY_KEYBOARD_Back + key_add); }
			if(KeyCode == Keys.Tab) { Func_KB_Click(KEY_KEYBOARD_Tab + key_add); }
			if(KeyCode == Keys.Space) { Func_KB_Click(KEY_KEYBOARD_Space + key_add); }

			if(KeyCode == Keys.OemMinus) { Func_KB_Click(KEY_KEYBOARD_Min + key_add); }
			if(KeyCode == Keys.Oemplus) { Func_KB_Click(KEY_KEYBOARD_Add + key_add); }
			if(KeyCode == Keys.OemOpenBrackets) { Func_KB_Click(KEY_KEYBOARD_Kuo1 + key_add); }
			if(KeyCode == Keys.OemCloseBrackets) { Func_KB_Click(KEY_KEYBOARD_Kuo2 + key_add); }
			if(KeyCode == Keys.OemPipe) { Func_KB_Click(KEY_KEYBOARD_Or + key_add); }
			if(KeyCode == Keys.OemSemicolon) { Func_KB_Click(KEY_KEYBOARD_Mao + key_add); }
			if(KeyCode == Keys.OemQuotes) { Func_KB_Click(KEY_KEYBOARD_Fen + key_add); }
			if(KeyCode == Keys.Oemtilde) { Func_KB_Click(KEY_KEYBOARD_Dou + key_add); }
			if(KeyCode == Keys.Oemcomma) { Func_KB_Click(KEY_KEYBOARD_Xiao + key_add); }
			if(KeyCode == Keys.OemPeriod) { Func_KB_Click(KEY_KEYBOARD_Da + key_add); }
			if(KeyCode == Keys.OemQuestion) { Func_KB_Click(KEY_KEYBOARD_Wen + key_add); }

			if(KeyCode == Keys.CapsLock) { Func_KB_Click(KEY_KEYBOARD_Caps + key_add); }
			if(KeyCode == Keys.F1) { Func_KB_Click(KEY_KEYBOARD_F1 + key_add); }
			if(KeyCode == Keys.F2) { Func_KB_Click(KEY_KEYBOARD_F2 + key_add); }
			if(KeyCode == Keys.F3) { Func_KB_Click(KEY_KEYBOARD_F3 + key_add); }
			if(KeyCode == Keys.F4) { Func_KB_Click(KEY_KEYBOARD_F4 + key_add); }
			if(KeyCode == Keys.F5) { Func_KB_Click(KEY_KEYBOARD_F5 + key_add); }
			if(KeyCode == Keys.F6) { Func_KB_Click(KEY_KEYBOARD_F6 + key_add); }
			if(KeyCode == Keys.F7) { Func_KB_Click(KEY_KEYBOARD_F7 + key_add); }
			if(KeyCode == Keys.F8) { Func_KB_Click(KEY_KEYBOARD_F8 + key_add); }
			if(KeyCode == Keys.F9) { Func_KB_Click(KEY_KEYBOARD_F9 + key_add); }
			if(KeyCode == Keys.F10) { Func_KB_Click(KEY_KEYBOARD_F10 + key_add); }
			if(KeyCode == Keys.F11) { Func_KB_Click(KEY_KEYBOARD_F11 + key_add); }
			if(KeyCode == Keys.F12) { Func_KB_Click(KEY_KEYBOARD_F12 + key_add); }
			if(KeyCode == Keys.PrintScreen) { Func_KB_Click(KEY_KEYBOARD_PrintScreen + key_add); }

			if(KeyCode == Keys.Home) { Func_KB_Click(KEY_KEYBOARD_Home + key_add); }
			if(KeyCode == Keys.PageUp) { Func_KB_Click(KEY_KEYBOARD_PageUp + key_add); }
			if(KeyCode == Keys.Delete) { Func_KB_Click(KEY_KEYBOARD_Del + key_add); }
			if(KeyCode == Keys.End) { Func_KB_Click(KEY_KEYBOARD_End + key_add); }
			if(KeyCode == Keys.PageDown) { Func_KB_Click(KEY_KEYBOARD_PageDown + key_add); }

			if(KeyCode == Keys.Right) { Func_KB_Click(KEY_KEYBOARD_Right + key_add); }
			if(KeyCode == Keys.Left) { Func_KB_Click(KEY_KEYBOARD_Left + key_add); }
			if(KeyCode == Keys.Down) { Func_KB_Click(KEY_KEYBOARD_Down + key_add); }
			if(KeyCode == Keys.Up) { Func_KB_Click(KEY_KEYBOARD_Up + key_add); }

			if(KeyCode == Keys.Enter) { Func_KB_Click(KEY_KEYBOARD_Enter + key_add); }

			//if(KeyCode == Keys.ControlKey) { Func_KB_Set(REG_KEYBOARD_SET_CTRL); }
			//if(KeyCode == Keys.ShiftKey) { Func_KB_Set(REG_KEYBOARD_SET_SHIFT); }
			//if(KeyCode == Keys.Menu) { Func_KB_Set(REG_KEYBOARD_SET_ALT); }	
		}

		private void KMouse_KeyDown(object sender, KeyEventArgs e)	//真正的KeyDown被架空，使用ProcessDialogKey作为keydown
		{
			//Func_KMouse_KeyDown();
		}

		protected override bool ProcessDialogKey(Keys keyData)		//所有默认热键的keydown入口在这里
		{
			Func_KMouse_KeyDown(keyData);
			return true;
		}

		private void KMouse_KeyUp(object sender, KeyEventArgs e)
		{
			//Console.WriteLine("KEYUP>>keycode:{0}", e.KeyCode);

			if(e.KeyCode == Keys.NumPad8) { Func_MoveUp_MouseUp(); }
			if(e.KeyCode == Keys.NumPad2) { Func_MoveDown_MouseUp(); }
			if(e.KeyCode == Keys.NumPad4) { Func_MoveLeft_MouseUp(); }
			if(e.KeyCode == Keys.NumPad6) { Func_MoveRight_MouseUp(); }
			if(e.KeyCode == Keys.NumPad1) { Func_RollUp_MouseUp(); }
			if(e.KeyCode == Keys.NumPad3) { Func_RollDown_MouseUp(); }

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
		}
	}
}
