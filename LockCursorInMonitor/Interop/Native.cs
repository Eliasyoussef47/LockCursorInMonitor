using LockCursorInMonitor.Interop.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LockCursorInMonitor.Interop
{
    /// <summary>
    /// Contains the functions that don't need wrapping and the wrapped functions.
    /// </summary>
    static class Native
    {
        [DllImport("USER32.dll")]
        public static extern short GetKeyState(VirtualKeyStates nVirtKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(RECT rcClip);

        public static POINT GetCursorPos()
        {
            POINT w32Mouse = new POINT();
            bool succes = NativeBase.GetCursorPos(ref w32Mouse);
            if (!succes)
            {
                throw new GetCursorPosFailedException();
            }
            return w32Mouse;
        }

        public static RECT GetClipCursor()
        {
            RectStruct rectStruct = new RectStruct();
            bool succes = NativeBase.GetClipCursor(ref rectStruct);
            if (!succes)
            {
                throw new GetClipCursorFailedException();
            }
            return rectStruct;
        }

    }

    /// <summary>
    /// Contains the functions that need to be wrapped.
    /// </summary>
    static class NativeBase
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetClipCursor(ref RectStruct lprect);
    }
}
