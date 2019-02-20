using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ChatClient
{
    class Neighbour : User
    {
        public Neighbour(uint anId, ConnHandler aConnectionHandler) : base(anId)
        {
            connectionHandler = aConnectionHandler;
        }

        public ConnHandler ConnectionHandler
        {
            get { return connectionHandler; }
            set { connectionHandler = value; }
        }
        private ConnHandler connectionHandler;
    }
}
