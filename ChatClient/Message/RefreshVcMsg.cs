using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class RefreshVcMsg : ReceiverMessage
    {
        public RefreshVcMsg(uint aSender, Dictionary<uint, uint> aVc, uint aReciever) : base(aSender, "refresh_vc", aReciever)
        {
            Vc = aVc;
        }

        public Dictionary<uint, uint> vc
        {
            get { return Vc; }
            set { Vc = value; }
        }

        private Dictionary<uint, uint> Vc;
    }
}
