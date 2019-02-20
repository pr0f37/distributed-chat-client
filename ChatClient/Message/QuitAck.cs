using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class QuitAck : ReceiverMessage
    {
        public QuitAck(uint aSender, uint aReceiver, uint aQuit_id) : base(aSender, "quit_ack", aReceiver)
        {
            Quit_id = aQuit_id;
        }
        public uint quit_id
        {
            get { return Quit_id; }
            set { Quit_id = value; }
        }
        private uint Quit_id;
    }
}
