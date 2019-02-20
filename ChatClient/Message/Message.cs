using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class Message
    {
        public Message(uint aSender, string aName)
        {
            Sender = aSender;
            type = aName;
            DontSendTo = new List<uint>();
        }
        
        public uint sender
        {
            get { return Sender; }
            set { Sender = value; }
        }

        public string msg_type
        {
            get { return type; }
            set { type = value; }
        }

        public List<uint> dont_send_to{
           get { return DontSendTo; }
           set { DontSendTo = value; }
        }

        public void addToList(uint id)
        {
            if (!DontSendTo.Contains(id))
                DontSendTo.Add(id);
        }

        protected string type;
        protected uint Sender;
        protected List<uint> DontSendTo;
    }
}
