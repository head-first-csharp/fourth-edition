using System;
using System.Collections.Generic;
using System.Text;

namespace MultipleEventHandlers
{
    class TalkEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public TalkEventArgs(string message) => Message = message;
    }
}
