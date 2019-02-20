using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ChatClient
{
    class TCPPortWriter
    {
        public TCPPortWriter()
        {
            _port = 0;
            _address = IPAddress.Parse("0.0.0.0");
        }

        public TCPPortWriter(string address, int port)
        {
            _port = port;
            _address = IPAddress.Parse(address);
        }

        public bool Connect(ConnHandler connHandler, string message)
        {
            try
            {
                if (connHandler.NeighbourClient.Connected)
                {
//                     Console.WriteLine("****DEBUG****");
//                     Console.WriteLine("wysylam do:" + connHandler.Port + " " + message);
//                     Console.WriteLine("****DEBUG****");
//                     //StreamWriter clientStreamWriter = new StreamWriter(connHandler.NeighbourNetworkStream);
                    //StreamReader clientStreamReader = new StreamReader(connHandler.NeighbourNetworkStream);
                    connHandler.NeighbourStreamWriter.WriteLine(message);
                    connHandler.NeighbourStreamWriter.Flush();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                return false;
            }
        }


        private IPAddress _address;
        private int _port;
    }
}
