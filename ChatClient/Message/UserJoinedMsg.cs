using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class UserJoinedMsg : ReceiverMessage
    {
        public UserJoinedMsg(uint aSender, uint aReceiver, string anIpAddress, int aPort, string aNew_nick, uint aNew_id) : base(aSender, "user_joined", aReceiver) 
        {
            ip_Address = anIpAddress;
            Listen_port = aPort;
            New_id = aNew_id;
            New_nick = aNew_nick;
        }

        public string new_ip
        {
            get { return ip_Address; }
            set { ip_Address = value; }
        }
        
        public int new_port
        {
            get { return Listen_port; }
            set { Listen_port = value; }
        }

        public uint new_id
        {
            get { return New_id; }
            set { New_id = value; }
        }

        public string new_nick
        {
            get { return New_nick; }
            set { New_nick = value; }
        }

        private string ip_Address;
        private int Listen_port;
        private uint New_id;
        private string New_nick;
    }
}
