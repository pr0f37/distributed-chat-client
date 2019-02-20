using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ChatClient
{
    class ConnHandler
    {
        public ConnHandler(ref TcpClient client)
        {
            try
            {
                _address = IPAddress.None;
                _port = 0;
                _neighbourClient = client;
               
                _neighbourNetworkStream = new NetworkStream(client.Client);
                _neighbourStreamReader = new StreamReader(_neighbourNetworkStream);
                _neighbourStreamWriter = new StreamWriter(_neighbourNetworkStream);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Net not detected. Listening mode on.");
            }
        }

        public ConnHandler(string address, int port)
        {
            _address = IPAddress.Parse(address);
            _port = port;
            try
            {
	            _neighbourClient = new TcpClient();
	            _neighbourClient.Connect(_address, _port);
                _neighbourNetworkStream = _neighbourClient.GetStream();
                _neighbourStreamReader = new StreamReader(_neighbourNetworkStream);
                _neighbourStreamWriter = new StreamWriter(_neighbourNetworkStream);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Net not detected. Listening mode on.");
            }
        }

        public ConnHandler(IPAddress address, int port)
        {
            _address = address;
            _port = port;
        }
        
        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public TcpClient NeighbourClient
        {
            get { return _neighbourClient; }
            set { _neighbourClient = value; }
        }

        public NetworkStream NeighbourNetworkStream
        {
            get { return _neighbourNetworkStream; }
            set { _neighbourNetworkStream = value; }
        }

        public StreamWriter NeighbourStreamWriter
        {
            get { return _neighbourStreamWriter; }
            set { _neighbourStreamWriter = value; }
        }

        public StreamReader NeighbourStreamReader
        {
            get { return _neighbourStreamReader; }
            set { _neighbourStreamReader = value; }
        }
        
        private IPAddress _address;
        private int _port;
        private TcpClient _neighbourClient;
        private NetworkStream _neighbourNetworkStream;
        private StreamWriter _neighbourStreamWriter;
        private StreamReader _neighbourStreamReader;
    }
}
