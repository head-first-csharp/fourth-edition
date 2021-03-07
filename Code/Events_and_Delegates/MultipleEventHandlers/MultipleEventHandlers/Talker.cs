using System;
using System.Collections.Generic;
using System.Text;

namespace MultipleEventHandlers
{
    class Talker
    {
        public event EventHandler<TalkEventArgs> TalkToMe;

        public void OnTalkToMe(string message) =>
                         TalkToMe?.Invoke(this, new TalkEventArgs(message));
    }
}
