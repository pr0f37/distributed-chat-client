using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class TalkMsg : ReceiverMessage
    {
        public TalkMsg(uint aSender, string aValue, Dictionary<uint, uint> aVc, uint aReciever) : base(aSender, "talk", aReciever)
        {
          //  Stamp = aStamp;
            Value = aValue;
            Vc = aVc;
        }

        /*public uint[] stamp
        {
            get { return Stamp; }
            set { Stamp = value; }
        }
        */
        public string chat_msg
        {
            get { return Value; }
            set { Value = value; }
        }

        public Dictionary<uint,uint> vc
        {
            get { return Vc; }
            set { Vc = value; }
        }

        //private uint[] Stamp;
        private string Value;
        private Dictionary<uint,uint> Vc;
    }
}
