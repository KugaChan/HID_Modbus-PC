//#define SUPPORT_SHOW_FIFO_DATA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Drawing;

namespace KMouse
{
    class Func
    {
        //宏
		public const uint dwAllFF = 0xFFFFFFFF;

        // CRC 高位字节值表 
		static byte[] auchCRCHi = 
		{
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
		};

		// CRC低位字节值表
		static byte[] auchCRCLo =
		{
			0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
			0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
			0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
			0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
			0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
			0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
			0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
			0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
			0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
			0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
			0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
			0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
			0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
			0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
			0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
			0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
			0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
			0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
			0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
			0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
			0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
			0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
			0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
			0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
			0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
			0x43, 0x83, 0x41, 0x81, 0x80, 0x40
		};

		public static ushort Get_CRC(byte[] Data, uint DataLen)
		{
			byte CRCHi = 0xFF;						//高CRC字节初始化 
			byte CRCLo = 0xFF;						//低CRC 字节初始化  
			ushort Res;
			byte uIndex;							//CRC循环中的索引
			byte i = 0;
			while(true)
			{
				DataLen--;
				uIndex = (byte)(CRCHi ^ Data[i]);			//计算CRC
				CRCHi = (byte)(CRCLo ^ auchCRCHi[uIndex]);
				CRCLo = auchCRCLo[uIndex];
				i++;
				if(DataLen == 0)
				{
					break;
				}
			}

			Res = (UInt16)(((UInt16)CRCHi << 8) | (UInt16)CRCLo);
			return Res;
		}

        public enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }

        /// <summary>
        /// Get the system devices information with windows api.
        /// </summary>
        /// <param name="hardType">Device type.</param>
        /// <param name="propKey">the property of the device.</param>
        /// <returns></returns>
        public static string[] GetHarewareInfo(HardwareEnum hardType, string propKey)
        {
            List<string> strs = new List<string>();
            try
            {
                using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach(var hardInfo in hardInfos)
                    {
                        if(hardInfo.Properties[propKey].Value != null)
                        {
                            String str = hardInfo.Properties[propKey].Value.ToString();
                            strs.Add(str);
                        }
                    }
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                strs = null;
            }
        }

		public static void DumpBuffer(byte[] buffer, int length)
        {
            for(int i = 0; i < length; i++)
            {
                if(i % 16 == 0)
                {
                    Console.WriteLine("");
                }
                Console.Write("{0:x2} ", buffer[i]);
            }
            Console.WriteLine("");
        }
		
        public static char GetHexHighLow(byte n, byte mode)
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

            switch(check)
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

		public static byte CharToByte(char n)      //把字符转换为数字
		{
			byte result;
			switch(n)
			{
				case '0': result = 0; break;
				case '1': result = 1; break;
				case '2': result = 2; break;
				case '3': result = 3; break;
				case '4': result = 4; break;
				case '5': result = 5; break;
				case '6': result = 6; break;
				case '7': result = 7; break;
				case '8': result = 8; break;
				case '9': result = 9; break;
				case 'A': result = 10; break;
				case 'B': result = 11; break;
				case 'C': result = 12; break;
				case 'D': result = 13; break;
				case 'E': result = 14; break;
				case 'F': result = 15; break;
				case 'a': result = 10; break;
				case 'b': result = 11; break;
				case 'c': result = 12; break;
				case 'd': result = 13; break;
				case 'e': result = 14; break;
				case 'f': result = 15; break;
				default: result = 0xFF; break;
			}

			return result;
		}

        public static string TextConvert_ASCII_To_Hex(string ascii_text)
        {
            string hex_show = "";                   //不要直接操作textBox的文本，操作内存变量要快很多!

            int n = ascii_text.Length;
            if(n != 0)
            {
                char[] chahArray = new char[n];
                chahArray = ascii_text.ToCharArray();  //将字符串转换为字符数组
                
                for(int i = 0; i < n; i++)
                {
                    char high_char = GetHexHighLow((byte)chahArray[i], 0);
                    char low_char = GetHexHighLow((byte)chahArray[i], 1);
                    //Console.WriteLine("i:{0}|{1} H:{2} L:{3}", i, n, high_char, low_char);

                    hex_show += high_char;
                    hex_show += low_char;
                    hex_show += " ";
                }

                ascii_text = hex_show;
            }

            return hex_show;
        }

        public static string TextConvert_Hex_To_ASCII(string hex_text)
        {
            string ascii_show = hex_text;    //不要直接操作textBox的文本，操作内存变量要快很多!

            int n = ascii_show.Length;
            if(n != 0)
            {
                char[] chahArray = new char[n];
                chahArray = ascii_show.ToCharArray();//将字符串转换为字符数组
                ascii_show = "";

                for(int i = 2; i < n; i++)//找出所有空格，0x3F
                {
                    if(chahArray[i] == ' ')
                    {
                        byte hex_h = Func.CharToByte(chahArray[i - 2]);//3
                        byte hex_l = Func.CharToByte(chahArray[i - 1]);//F	
                        int hex = hex_h << 4 | hex_l;

                        if((hex == 0x00) || (hex > 0x7F))   //不输入ASCII码的，要保留的原始值的，变成'？'不应该
                        {
                            ascii_show += '?';
                        }
                        else
                        {
                            ascii_show += (char)hex;
                        }
                    }
                }                
            }

            return ascii_show;
        }

        public static string Byte_To_String(byte value)
        {
            string str = "";

            switch(value)
            {
                case 0x20: str += " "; break;
                case 0x21: str += "!"; break;
                case 0x22: str += "\""; break;
                case 0x23: str += "#"; break;
                case 0x24: str += "$"; break;
                case 0x25: str += "%"; break;
                case 0x26: str += "&"; break;
                case 0x27: str += "'"; break;
                case 0x28: str += "("; break;
                case 0x29: str += ")"; break;
                case 0x2A: str += "*"; break;
                case 0x2B: str += "+"; break;
                case 0x2C: str += ","; break;
                case 0x2D: str += "-"; break;
                case 0x2E: str += "."; break;
                case 0x2F: str += "/"; break;
                case 0x30: str += "0"; break;
                case 0x31: str += "1"; break;
                case 0x32: str += "2"; break;
                case 0x33: str += "3"; break;
                case 0x34: str += "4"; break;
                case 0x35: str += "5"; break;
                case 0x36: str += "6"; break;
                case 0x37: str += "7"; break;
                case 0x38: str += "8"; break;
                case 0x39: str += "9"; break;
                case 0x3A: str += ":"; break;
                case 0x3B: str += ";"; break;
                case 0x3C: str += "<"; break;
                case 0x3D: str += "="; break;
                case 0x3E: str += ">"; break;
                case 0x3F: str += "?"; break;
                case 0x40: str += "@"; break;
                case 0x41: str += "A"; break;
                case 0x42: str += "B"; break;
                case 0x43: str += "C"; break;
                case 0x44: str += "D"; break;
                case 0x45: str += "E"; break;
                case 0x46: str += "F"; break;
                case 0x47: str += "G"; break;
                case 0x48: str += "H"; break;
                case 0x49: str += "I"; break;
                case 0x4A: str += "J"; break;
                case 0x4B: str += "K"; break;
                case 0x4C: str += "L"; break;
                case 0x4D: str += "M"; break;
                case 0x4E: str += "N"; break;
                case 0x4F: str += "O"; break;
                case 0x50: str += "P"; break;
                case 0x51: str += "Q"; break;
                case 0x52: str += "R"; break;
                case 0x53: str += "S"; break;
                case 0x54: str += "T"; break;
                case 0x55: str += "U"; break;
                case 0x56: str += "V"; break;
                case 0x57: str += "W"; break;
                case 0x58: str += "X"; break;
                case 0x59: str += "Y"; break;
                case 0x5A: str += "Z"; break;
                case 0x5B: str += "["; break;
                case 0x5C: str += "\\"; break;
                case 0x5D: str += "]"; break;
                case 0x5E: str += "^"; break;
                case 0x5F: str += "_"; break;
                case 0x60: str += "`"; break;
                case 0x61: str += "a"; break;
                case 0x62: str += "b"; break;
                case 0x63: str += "c"; break;
                case 0x64: str += "d"; break;
                case 0x65: str += "e"; break;
                case 0x66: str += "f"; break;
                case 0x67: str += "g"; break;
                case 0x68: str += "h"; break;
                case 0x69: str += "i"; break;
                case 0x6A: str += "j"; break;
                case 0x6B: str += "k"; break;
                case 0x6C: str += "l"; break;
                case 0x6D: str += "m"; break;
                case 0x6E: str += "n"; break;
                case 0x6F: str += "o"; break;
                case 0x70: str += "p"; break;
                case 0x71: str += "q"; break;
                case 0x72: str += "r"; break;
                case 0x73: str += "s"; break;
                case 0x74: str += "t"; break;
                case 0x75: str += "u"; break;
                case 0x76: str += "v"; break;
                case 0x77: str += "w"; break;
                case 0x78: str += "x"; break;
                case 0x79: str += "y"; break;
                case 0x7A: str += "z"; break;
                case 0x7B: str += "{"; break;
                case 0x7C: str += "|"; break;
                case 0x7D: str += "}"; break;
                case 0x7E: str += "~"; break;
                default:
                {
                    byte[] array = new byte[1];
                    array[0] = value;
                    str += System.Text.Encoding.ASCII.GetString(array);
                    break;
                }
            }

            return str;
        }

        //截取字符串最后max_size长度的数据
        public static string String_Roll(string str_in, int max_size)
        {
            if(str_in.Length > max_size)
            {
                str_in = str_in.Substring(str_in.Length - max_size, max_size);
            }

            return str_in;
        }

        public static bool Char_String_compare(char[] spec_key_buff, string str, uint length)
        {
            uint i;

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
    }

    /***************************eFIFO Start********************************/
    public class eFIFO
    {
        //readonly object locker = new object();

        public int max_data_len;
        public int max_queue_depth;

        bool is_full;
        int top;
        int bottom;
        List<byte[]> buffer_data;
        List<int> buffer_value;

        public void Reset()
        {
            top = 0;
            bottom = 0;
            is_full = false;
        }

        public void Init(int _max_buffer_len, int _max_queue_depth)
        {
            buffer_data = new List<byte[]>();
            buffer_value = new List<int>();

            max_data_len = _max_buffer_len;
            max_queue_depth = _max_queue_depth;

            for(int i = 0; i < max_queue_depth; i++)
            {
                byte[] x = new byte[max_data_len];
                int y = 0;

                buffer_data.Add(x);
                buffer_value.Add(y);
            }

            Reset();
        }

        public int GetValidNum()
        {
            int num;

            object locker = new object();

            lock(locker)
            {
                if(is_full == true)
                {
                    num = max_queue_depth;
                }
                else
                {
                    if(top < bottom)
                    {
                        num = top + max_queue_depth - bottom;
                    }
                    else
                    {
                        num = top - bottom;
                    }
                }
            }

            return num;
        }

        public byte[] Output(ref int value)
        {
            byte[] data;

            object locker = new object();

            lock(locker)
            {
                data = buffer_data[bottom];
                value = buffer_value[bottom];

#if SUPPORT_SHOW_FIFO_DATA
                Console.WriteLine("out:{0}({1}:{2})", value, top, bottom);
                for(int i = 0; i < buffer_value[bottom]; i++)
                {
                    Console.Write(" {0}", buffer_data[bottom][i]);
                }
                Console.WriteLine("({0}:{1})", top, bottom);
#endif

                is_full = false;
                bottom++;
                if(bottom >= max_queue_depth)
                {
                    bottom = 0;
                }
            }

            return data;
        }

        public byte[] Peek()
        {
            return buffer_data[top];
        }

        public void Input(byte[] data, int value)
        {
            object locker = new object();

            lock(locker)
            {
                if(data != null)    //在peek时已经加入过了
                {
                    buffer_data[top] = data;
                }
                
                buffer_value[top] = value;

#if SUPPORT_SHOW_FIFO_DATA
                Console.WriteLine("in:{0}({1}:{2})", value, top, bottom);
                for(int i = 0; i < buffer_value[top]; i++)
                {
                    Console.Write(" {0}", buffer_data[top][i]);
                }
                Console.WriteLine("({0}:{1})", top, bottom);
#endif
                top++;

                if(top >= max_queue_depth)
                {
                    top = 0;
                }
                if(top == bottom)   //如果头部赶上尾部，则FIFO已满
                {
                    is_full = true;
                }
            }
        }
        /***************************eFIFO End**********************************/
    }

    
    class MyTimer : System.Timers.Timer //集成Timer，可以传递其他参数
    {
        public delegate void tyDelegate_Callback(bool en);

        public object context;
        public FormMain fm;
        public tyDelegate_Callback delegate_callback;
        //public Action action_callback;
    }
}
