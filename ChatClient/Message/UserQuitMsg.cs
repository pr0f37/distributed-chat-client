using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class UserQuitMsg : ReceiverMessage
    {
        public UserQuitMsg(uint aSender, uint aReciever, uint aQuit_id) : base(aSender, "user_quit", aReciever)
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
