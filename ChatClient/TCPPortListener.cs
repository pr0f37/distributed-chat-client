using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatClient
{
    class TCPPortListener
    {
        public TCPPortListener(ConfigurationReader config, Dictionary<Dictionary<uint, uint>, Message> talkBuffer, List<Message> buffer, uint MyId, ref Dictionary<uint, Neighbour> Neighbours)
        {
            _port = config.Port;
            _address = IPAddress.Parse(config.IpAddress);
            _listener = new TcpListener(_address, _port);
            _talkBuffer = talkBuffer;
            _buffer = buffer;
            _working = false;
            _myId = MyId;
            _neighbours = Neighbours;
        }

        public void Start()
        {
            _working = true;
            _listener.Start();
            // start listening on port
            Console.WriteLine("Listening\n  on port: {0}\n  IP: {1}\nStarted", _port, _address.ToString());
            // thread work pool declaration
            doneListeners = new ManualResetEvent[10];
            workerCounter = 0;
            while(_working == true)
            {
                // check if there are any connections waiting to be handled
                if (_listener.Pending())
                {
                    
                    // accept the connection
                    //Socket listeningSocket = _listener.AcceptSocket();
                    TcpClient client = _listener.AcceptTcpClient();
                    // new worker to handle the connection
                    doneListeners[workerCounter] = new ManualResetEvent(false);
                    ListeningWorker newWorker = new ListeningWorker(client, workerCounter, doneListeners[workerCounter], _talkBuffer, _buffer, _myId, ref _neighbours);
                    ThreadPool.QueueUserWorkItem(newWorker.ThreadPoolCallback, workerCounter);
                    workerCounter = (++workerCounter) % 10;
                }
            }
            
            //WaitHandle.WaitAll(doneListeners);
            _listener.Stop();

        }

        public void StopWorking()
        {
            _working = false;
        }

        private TcpListener _listener;
        private IPAddress _address;
        private int _port;
        private Dictionary<Dictionary<uint, uint>, Message> _talkBuffer;
        private List<Message> _buffer;
        private bool _working;
        private uint _myId;
        private Dictionary<uint, Neighbour> _neighbours;
        public int workerCounter;
        public ManualResetEvent[] doneListeners;
    }
}
