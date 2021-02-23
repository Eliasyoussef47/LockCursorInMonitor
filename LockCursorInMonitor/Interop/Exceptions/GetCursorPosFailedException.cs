using System;
using System.Collections.Generic;
using System.Text;

namespace LockCursorInMonitor.Interop.Exceptions
{
    class GetCursorPosFailedException : Exception
    {
        public GetCursorPosFailedException() : base("GetCursorPos has failed.")
        {
        }
    }
}
