using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using LockCursorInMonitor.Interop;

namespace LockCursorInMonitor
{
    class CursorLock
    {
        private static bool locked = false;

        public static bool Locked 
        { 
            get => locked;
            set 
            {
                locked = value; 
            } 
        }

        public static void LockCursor()
        {
            RECT bounds = GetCursorWorkingArea();
            // Confine the cursor to that monitor
            Native.ClipCursor(bounds);
            Locked = true;
        }

        public static void UnlockCursor()
        {
            Native.ClipCursor(null);
            Locked = false;
        }

        public static RECT GetCursorWorkingArea()
        {
            // Get the position of the cursor
            System.Drawing.Point MousePoint = Native.GetCursorPos();
            // Get the bounds of the screen that the cursor is on
            return Screen.GetWorkingArea(MousePoint);
        }
    }
}
