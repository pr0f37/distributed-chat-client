using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class ReceiverMessage : Message
    {
        public ReceiverMessage(uint aSender, string aName, uint aReciever) : base(aSender, aName)
        {
            Reciever = aReciever;
        }

        public uint receiver
        {
            get { return Reciever; }
            set { Reciever = value; }
        }
        protected uint Reciever;
    }
}
