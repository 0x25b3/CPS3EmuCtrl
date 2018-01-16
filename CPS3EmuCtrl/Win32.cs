// Stolen from the web mostly

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPS3EmuCtrl
{
    public static class Win32
    {
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        #pragma warning disable 649
        public struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        public struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }
        #pragma warning restore 649

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        
        public static (int X, int Y) GetPosition(Process Emulator)
        {
            if (Emulator == null)
                return (-1, -1);

            IntPtr ptr = Emulator.MainWindowHandle;
            Rect EmulatorRect = new Rect();
            GetWindowRect(ptr, ref EmulatorRect);

            return (EmulatorRect.Left, EmulatorRect.Top);
        }

        public static void ClickOnPoint(Process Emulator, Point Position, bool DoubleClick = false)
        {
            IntPtr Handle = Emulator.MainWindowHandle;
            var OldPos = Cursor.Position;
            ClientToScreen(Handle, ref Position);
            
            Cursor.Position = new Point(Position.X, Position.Y);

            var MouseDown = new INPUT()
            {
                Type = 0,
                Data = new MOUSEKEYBDHARDWAREINPUT()
                {
                    Mouse = new MOUSEINPUT()
                    {
                        Flags = 0x0002
                    }
                }
            };
            var MouseUp = new INPUT()
            {
                Type = 0,
                Data = new MOUSEKEYBDHARDWAREINPUT()
                {
                    Mouse = new MOUSEINPUT()
                    {
                        Flags = 0x0004
                    }
                }
            };

            var inputs = new INPUT[] { MouseDown, MouseUp };

            if(DoubleClick)
                inputs = new INPUT[] { MouseDown, MouseUp, MouseDown, MouseUp };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            
            Cursor.Position = OldPos;
        }
    }
}
