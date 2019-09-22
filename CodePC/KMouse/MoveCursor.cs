using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KMouse
{
    class MoveCursor
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        const int MOUSEEVENTF_MOVE = 0x1;//模拟鼠标移动
        const int MOUSEEVENTF_LEFTDOWN = 0x2;//
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        public void Mouse_AbsoluteMove(int x_rate, int y_rate)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, 65536 * x_rate / 1000, 65536 * y_rate / 1000, 0, 0);
        }

        public void Mouse_LeftDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public void Mouse_LeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void Mouse_RightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public void Mouse_RightDown()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }

        public void Mouse_MiddleUp()
        {
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }

        public void Mouse_MiddleDown()
        {
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
        }

        public void Mouse_WheelMove(int count)
        {
            //控制鼠标滑轮滚动，count代表滚动的值，负数代表向下，正数代表向上，如-100代表向下滚动100的y坐标
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, count, 0);
        }

        public void Mouse_Single_LeftClick()
        {
            Mouse_LeftDown();
            Mouse_LeftUp();
        }

        public void Mouse_Single_DoubleClick()
        {
            Mouse_LeftDown();
            Mouse_LeftUp();
            Mouse_LeftDown();
            Mouse_LeftUp();
        }

        public void Mouse_RightClick()
        {
            Mouse_RightDown();
            Mouse_RightUp();
        }

        public void Mouse_MiddleClick()
        {
            Mouse_MiddleDown();
            Mouse_MiddleUp();
        }

        public struct tyLoc
        {
            public bool inside;

            public int x_min;
            public int x_max;
            public int y_min;
            public int y_max;

            public int cur_x;
            public int cur_y;

            public double rate_x;
            public double rate_y;
            public double rate_cur;
        }

        public tyLoc Cursor_Inside_Form(PictureBox pic, int left_offs, int top_offs)
        {
            tyLoc loc;
            
            int x_min = left_offs + pic.Location.X;
            int x_max = left_offs + pic.Location.X + pic.Size.Width - 1;
            int y_min = top_offs + pic.Location.Y;
            int y_max = top_offs + pic.Location.Y + pic.Size.Height - 1;

            //画出鼠标
            int cur_x, cur_y;
            Cursor cur = Func.GetCursor(out cur_x, out cur_y);

            loc.inside = false;

            if(    (cur_x > x_min)
                && (cur_x < x_max)
                && (cur_y > y_min)
                && (cur_y < y_max))
            {
                loc.inside = true;
            }

            loc.x_min = x_min;
            loc.x_max = x_max;
            loc.y_min = y_min;
            loc.y_max = y_max;

            loc.cur_x = cur_x;
            loc.cur_y = cur_y;

            loc.rate_x = 1000 * (loc.cur_x - loc.x_min) / (loc.x_max - loc.x_min);
            if(loc.rate_x > 1000)
            {
                loc.rate_x = 1000;
            }
            if(loc.rate_x < 0)
            {
                loc.rate_x = 0;
            }
            loc.rate_y = 1000 * (loc.cur_y - loc.y_min) / (loc.y_max - loc.y_min);
            if(loc.rate_y > 1000)
            {
                loc.rate_y = 1000;
            }
            if(loc.rate_y < 0)
            {
                loc.rate_y = 0;
            }

            loc.rate_cur = loc.rate_x + loc.rate_y / 1000;

#if false
            Dbg.WriteLine("inside:%", loc.inside);
            Dbg.WriteLine("cur_x:% x_min:% x_max:%", loc.cur_x, loc.x_min, loc.x_max);
            Dbg.WriteLine("cur_y:% y_min:% y_max:%", loc.cur_y, loc.y_min, loc.y_max);
            Dbg.WriteLine("rate_x:% rate_y:%", loc.rate_x, loc.rate_y);
            Dbg.WriteLine("left_offs:% top_offs:%", left_offs, top_offs);
#endif

            return loc;
        }

    }
}
