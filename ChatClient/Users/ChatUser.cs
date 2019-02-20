using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ChatClient
{
    class ChatUser : User
    {
        public ChatUser(uint anId, string aNick, string anIp, int aPort) : base(anId)
        {
            
            Nick = aNick;
            Ip = anIp;
            Port = aPort;
        }

        public string nick
        {
            get { return Nick; }
            set { Nick = value; }
        }

        public string ip
        {
            get { return Ip; }
            set { Ip = value; }
        }

        public int port
        {
            get { return Port; }
            set { Port = value; }
        }

        private string Nick;
        private string Ip;
        private int Port;
    }
}
