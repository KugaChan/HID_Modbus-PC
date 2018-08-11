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
		const u8 REG_KEYBOARD = 9;
		const u32 KEY_KEYBOARD_A = 0x04;
		const u32 KEY_KEYBOARD_B = 0x05;
		const u32 KEY_KEYBOARD_C = 0x06;
		const u32 KEY_KEYBOARD_D = 0x07;
		const u32 KEY_KEYBOARD_E = 0x08;
		const u32 KEY_KEYBOARD_F = 0x09;
		const u32 KEY_KEYBOARD_G = 0x0A;
		const u32 KEY_KEYBOARD_H = 0x0B;
		const u32 KEY_KEYBOARD_I = 0x0C;
		const u32 KEY_KEYBOARD_J = 0x0D;
		const u32 KEY_KEYBOARD_K = 0x0E;
		const u32 KEY_KEYBOARD_L = 0x0F;
		const u32 KEY_KEYBOARD_M = 0x10;
		const u32 KEY_KEYBOARD_N = 0x11;
		const u32 KEY_KEYBOARD_O = 0x12;
		const u32 KEY_KEYBOARD_P = 0x13;
		const u32 KEY_KEYBOARD_Q = 0x14;
		const u32 KEY_KEYBOARD_R = 0x15;
		const u32 KEY_KEYBOARD_S = 0x16;
		const u32 KEY_KEYBOARD_T = 0x17;
		const u32 KEY_KEYBOARD_U = 0x18;
		const u32 KEY_KEYBOARD_V = 0x19;
		const u32 KEY_KEYBOARD_W = 0x1A;
		const u32 KEY_KEYBOARD_X = 0x1B;
		const u32 KEY_KEYBOARD_Y = 0x1C;
		const u32 KEY_KEYBOARD_Z = 0x1D;

		const u32 KEY_KEYBOARD_Num1 = 0x1E;
		const u32 KEY_KEYBOARD_Num2 = 0x1F;
		const u32 KEY_KEYBOARD_Num3 = 0x20;
		const u32 KEY_KEYBOARD_Num4 = 0x21;
		const u32 KEY_KEYBOARD_Num5 = 0x22;
		const u32 KEY_KEYBOARD_Num6 = 0x23;
		const u32 KEY_KEYBOARD_Num7 = 0x24;
		const u32 KEY_KEYBOARD_Num8 = 0x25;
		const u32 KEY_KEYBOARD_Num9 = 0x26;
		const u32 KEY_KEYBOARD_Num0 = 0x27;

        const u32 KEY_KEYBOARD_Enter = 0x28;
		const u32 KEY_KEYBOARD_ESC = 0x29;
		const u32 KEY_KEYBOARD_Back = 0x2A;
		const u32 KEY_KEYBOARD_Tab = 0x2B;
		const u32 KEY_KEYBOARD_Space = 0x2C;

		const u32 KEY_KEYBOARD_Min = 0x2D;
		const u32 KEY_KEYBOARD_Add = 0x2E;
		const u32 KEY_KEYBOARD_Kuo1 = 0x2F;
		const u32 KEY_KEYBOARD_Kuo2 = 0x30;
		const u32 KEY_KEYBOARD_Or = 0x31;
		const u32 KEY_KEYBOARD_Mao = 0x33;
		const u32 KEY_KEYBOARD_Fen = 0x34;
		const u32 KEY_KEYBOARD_Dou = 0x35;
		const u32 KEY_KEYBOARD_Xiao = 0x36;
		const u32 KEY_KEYBOARD_Da = 0x37;
		const u32 KEY_KEYBOARD_Wen = 0x38;

		const u32 KEY_KEYBOARD_Caps = 0x39;
		const u32 KEY_KEYBOARD_F1 = 0x3A;
		const u32 KEY_KEYBOARD_F2 = 0x3B;
		const u32 KEY_KEYBOARD_F3 = 0x3C;
		const u32 KEY_KEYBOARD_F4 = 0x3D;
		const u32 KEY_KEYBOARD_F5 = 0x3E;
		const u32 KEY_KEYBOARD_F6 = 0x3F;
		const u32 KEY_KEYBOARD_F7 = 0x40;
		const u32 KEY_KEYBOARD_F8 = 0x41;
		const u32 KEY_KEYBOARD_F9 = 0x42;
		const u32 KEY_KEYBOARD_F10 = 0x43;
		const u32 KEY_KEYBOARD_F11 = 0x44;
		const u32 KEY_KEYBOARD_F12 = 0x45;
		const u32 KEY_KEYBOARD_PrintScreen = 0x46;

		const u32 KEY_KEYBOARD_Home = 0x4A;
		const u32 KEY_KEYBOARD_PageUp = 0x4B;
		const u32 KEY_KEYBOARD_Del = 0x4C;
		const u32 KEY_KEYBOARD_End = 0x4D;
		const u32 KEY_KEYBOARD_PageDown = 0x4E;

		const u32 KEY_KEYBOARD_Right = 0x4F;
		const u32 KEY_KEYBOARD_Left = 0x50;
		const u32 KEY_KEYBOARD_Down = 0x51;
		const u32 KEY_KEYBOARD_Up = 0x52;

        const u32 KEY_KEYBOARD_Shift = 1u << 16;
		const u32 KEY_KEYBOARD_Ctrl = 1u << 17;		
		const u32 KEY_KEYBOARD_Alt = 1u << 18;

        const u16 KEY_KEYBOARD_NULL = 0xFFFF;

		bool modbus_send_cmd_is_busy = false;

        const u32 MODBUS_KB_WAITING_MAX = 1024;

        u32[] modbus_kb_fifo = new u32[MODBUS_KB_WAITING_MAX];
        u32 modbus_kb_input = 0;
        u32 modbus_kb_output = 0;

        private bool Func_KB_FIFO_Input(u32 KEY)
        {
            u32 KEY_Add = KEY;
            if (button_Ctrl.BackColor == System.Drawing.Color.Yellow)
            {
                button_Ctrl.BackColor = System.Drawing.Color.Gainsboro;
                KEY_Add |= KEY_KEYBOARD_Ctrl;
            }

            if (button_Shift.BackColor == System.Drawing.Color.Yellow)
            {
                button_Shift.BackColor = System.Drawing.Color.Gainsboro;
                KEY_Add |= KEY_KEYBOARD_Shift;
            }

            if (button_Alt.BackColor == System.Drawing.Color.Yellow)
            {
                button_Alt.BackColor = System.Drawing.Color.Gainsboro;
                KEY_Add |= KEY_KEYBOARD_Alt;
            }

            if (modbus_kb_input - modbus_kb_output < MODBUS_KB_WAITING_MAX)
            {
                modbus_kb_fifo[modbus_kb_input] = KEY_Add;
                modbus_kb_input++;

                return true;
            }
            else
            {
                MessageBox.Show("FIFO已满", "提示");
                return false;
            }
        }

        private bool Func_KB_FIFO_HasData()
        {
            if (modbus_kb_input - modbus_kb_output > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private u32 Func_KB_FIFO_Output()
        {
            u32 KEY;
            if (Func_KB_FIFO_HasData() == true)
            {
                KEY = modbus_kb_fifo[modbus_kb_output];
                modbus_kb_output++;

                return KEY;
            }
            else
            {
                MessageBox.Show("FIFO已空", "提示");
                return dwAllFF;
            }
        }

		private void Func_KB_Click(u32 KEY)
		{
            Func_KB_FIFO_Input(KEY);           
		}

		/* 鼠标单击Shift, Ctrl和Alt时使用，由于部分组合键会与本机冲突，所以需要用 */
        private void Func_KB_Set(u8 REG)
        {
            if (modbus_send_cmd_is_busy == false)
            {
                modbus_send_cmd_is_busy = true;
                Func_Modbus_Send_03(REG, 1, 0);
            }
        }

		private void button_Ctrl_Click(object sender, EventArgs e)
		{
            if (button_Ctrl.BackColor == System.Drawing.Color.Yellow)
            {
                button_Ctrl.BackColor = System.Drawing.Color.Gainsboro;
                Func_KB_Click(KEY_KEYBOARD_NULL + KEY_KEYBOARD_Ctrl);
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
                button_Shift.BackColor = System.Drawing.Color.Gainsboro;
                Func_KB_Click(KEY_KEYBOARD_NULL + KEY_KEYBOARD_Shift);
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
                button_Alt.BackColor = System.Drawing.Color.Gainsboro;
                Func_KB_Click(KEY_KEYBOARD_NULL + KEY_KEYBOARD_Alt);
            }
            else
            {
                button_Alt.BackColor = System.Drawing.Color.Yellow;
            }
		}

		/**********************鼠标单击键盘按钮，用得少，主要用热键***********************/
        private void button_Win_Click(object sender, EventArgs e)
        {
            Func_KB_Click(KEY_KEYBOARD_NULL);
        }

		private void button_A_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_A);
		}

		private void button_B_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_B);
		}

		private void button_C_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_C);
		}

		private void button_D_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_D);
		}

		private void button_E_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_E);
		}

		private void button_F_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F);
		}

		private void button_G_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_G);
		}

		private void button_H_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_H);
		}

		private void button_I_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_I);
		}

		private void button_J_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_J);
		}
		private void button_K_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_K);
		}

		private void button_L_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_L);
		}

		private void button_M_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_M);
		}

		private void button_N_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_N);
		}

		private void button_O_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_O);
		}

		private void button_P_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_P);
		}

		private void button_Q_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Q);
		}

		private void button_R_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_R);
		}

		private void button_S_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_S);
		}

		private void button_T_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_T);
		}

		private void button_U_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_U);
		}

		private void button_V_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_V);
		}

		private void button_W_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_W);
		}

		private void button_X_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_X);
		}

		private void button_Y_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Y);
		}

		private void button_Z_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Z);
		}
		/******************************************************************/
		private void button_Num1_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num1);
		}

		private void button_Num2_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num2);
		}

		private void button_Num3_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num3);
		}

		private void button_Num4_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num4);
		}

		private void button_Num5_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num5);
		}

		private void button_Num6_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num6);
		}

		private void button_Num7_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num7);
		}

		private void button_Num8_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num8);
		}

		private void button_Num9_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num9);
		}
		private void button_Num0_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Num0);
		}
		/******************************************************************/
		private void button_Enter_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Enter);
		}
		private void button_ESC_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_ESC);
		}

		private void button_BackSpace_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Back);
		}

		private void button_Tab_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Tab);
		}

		private void button_Space_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Space);
		}
		/******************************************************************/
		private void button_VV_Min_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Min);
		}

		private void button_VV_Add_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Add);
		}

		private void button_VV_kuo_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Kuo1);
		}

		private void button_VV_Kuo2_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Kuo2);
		}

		private void button_VV_Or_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Or);
		}

		private void button_VV_Mao_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Mao);
		}

		private void button_VV_Fen_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Fen);
		}

		private void button_VV_Dou_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Dou);
		}

		private void button_VV_Xiaoyu_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Xiao);
		}

		private void button_VV_Dayu_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Da);
		}

		private void button_WenHao_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Wen);
		}
		/******************************************************************/
		bool caps_is_enable = false;
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
			Func_KB_Click(KEY_KEYBOARD_Caps);
		}

		private void button_F1_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F1);
		}

		private void button_F2_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F2);
		}

		private void button_F3_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F3);
		}

		private void button_F4_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F4);
		}

		private void button_F5_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F5);
		}

		private void button_F6_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F6);
		}

		private void button_F7_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F7);
		}

		private void button_F8_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F8);
		}

		private void button_F9_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F9);
		}

		private void button_F10_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F10);
		}

		private void button_F11_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F11);
		}

		private void button_F12_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_F12);
		}

		private void button_Screen_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_PrintScreen);
		}
		/******************************************************************/
		private void button_Home_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Home);
		}
		private void button_PageUp_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_PageUp);
		}
		private void button_Del_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Del);
		}
		private void button_End_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_End);
		}
		private void button_PageDwon_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_PageDown);
		}
		/******************************************************************/
		private void button_Right_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Right);
		}
		private void button_Left_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Left);
		}
		private void button_Down_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Down);
		}
		private void button_Up_Click(object sender, EventArgs e)
		{
			Func_KB_Click(KEY_KEYBOARD_Up);
		}
		/******************************************************************/
	}
}

