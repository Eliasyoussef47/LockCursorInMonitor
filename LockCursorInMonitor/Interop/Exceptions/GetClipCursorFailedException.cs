using System;
using System.Collections.Generic;
using System.Text;

namespace LockCursorInMonitor.Interop.Exceptions
{
    class GetClipCursorFailedException : Exception
    {
        public GetClipCursorFailedException() : base("GetClipCursor has failed.")
        {
        }
    }
}
