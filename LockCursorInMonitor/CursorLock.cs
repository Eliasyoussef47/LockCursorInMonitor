using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LockCursorInMonitor
{
    static class CursorLock
    {

        private static int traceCounter = 0;
        public static int TraceCounter 
        {
            get
            {
                traceCounter++;
                return traceCounter;
            }
        }


        //TODO: Bug: if you press ctrl and then press the title bar of a window the cursor clipt resets
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool ClipCursor(RECT rcClip);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClipCursor(ref RECT lprect);

        private static RECT rect = new RECT();
        private static bool locked = false;

        public static bool Locked 
        { 
            get => locked;
            set 
            {
                Trace.WriteLine(CursorLock.TraceCounter + ") Locked: " + value);
                locked = value; 
            } 
        }


        public static void LockCursor()
        {
            Trace.WriteLine(CursorLock.TraceCounter + ") LockCursor");
            System.Drawing.Point MousePoint = GetMousePosition();
            RECT bounds = Screen.GetWorkingArea(MousePoint);
            ClipCursor(bounds);
            Locked = true;
        }

        public static void UnlockCursor()
        {
            Trace.WriteLine(CursorLock.TraceCounter + ") UnlockCursor");
            ClipCursor(null);
            Locked = false;
        }

        private static System.Drawing.Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new System.Drawing.Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
