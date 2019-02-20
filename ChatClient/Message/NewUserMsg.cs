using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class NewUserMsg : Message
    {
        public NewUserMsg(uint aSender, string anIp_Address, int aListen_port, string aNick) : base(aSender, "new_user")
        {
            Sender = aSender;
            ip_address = anIp_Address;
            Listen_port = aListen_port;
            Nick = aNick;
        }
        
        public string ip
        {
            get { return ip_address; }
            set { ip_address = value; }
        }

        public int listen_port
        {
            get { return Listen_port; }
            set { Listen_port = value; }
        }

        public string nick
        {
            get { return Nick; }
            set { Nick = value; }
        }

        private string ip_address;
        private int Listen_port;
        private string Nick;
    }
}
