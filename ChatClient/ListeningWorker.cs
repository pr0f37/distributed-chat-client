using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using Newtonsoft.Json;

namespace ChatClient
{
    class ListeningWorker
    {
        public ListeningWorker(TcpClient incoming, int number, ManualResetEvent doneEvent, Dictionary<Dictionary<uint, uint>, Message> talkBuffer, List<Message> buffer, uint MyId, ref Dictionary<uint, Neighbour> Neighbours)
        {
            _workerClient = incoming;
            _workerNumber = number;
            _doneEvent = doneEvent;
            _talkBuffer = talkBuffer;
            _buffer = buffer;
            _myId = MyId;
            _neighbours = Neighbours;            
        }

        
        public void ThreadPoolCallback(object threadContext)
        {
            int threadIndex = (int)threadContext;

            try
            {
                while(_workerClient.Connected)
                {
                    if (_workerClient.Connected)
                    _workerNetworkStream = _workerClient.GetStream();
                    if (_workerClient.Connected)
                    _workerStreamReader = new StreamReader(_workerNetworkStream);
                    if (_workerNetworkStream.DataAvailable) // this will invoke exception if socket will be closed
                    {
                        
                        Message parsedMsg = Parse(_workerStreamReader.ReadLine());
                        if (parsedMsg is NewUserMsg)
                        {
                            Neighbour neigh = new Neighbour(((NewUserMsg)parsedMsg).sender, new ConnHandler(ref _workerClient));
                            _neighbours.Add(((NewUserMsg)parsedMsg).sender, neigh);
                            _buffer.Add(parsedMsg);
                        }
                        else if (parsedMsg is TalkMsg)
                            _talkBuffer.Add(((TalkMsg)parsedMsg).vc, parsedMsg);
                        else if (parsedMsg is UserQuitMsg)
                        {
                            _buffer.Add(parsedMsg);
                            break;
                        }
                        else
                            _buffer.Add(parsedMsg);
                    }
                }
            }
            catch(SocketException e)
            {
                 // reakcja na zamkniecie socketa, czyli wyslanie wywolanie metody przeszukujacej liste neighboursów i wyslanie _user_quit!!
                uint quitId = findDisconnNeighbourId(IPAddress.Parse(_workerClient.Client.RemoteEndPoint.ToString()));
                if (quitId != 0)
                {
                    _buffer.Add(new UserQuitMsg(quitId, _myId, quitId)); // wysylam sam do siebie userQuita o disconnectowanym userze
                }
            }
            _doneEvent.Set();
        }

        public Message Parse(string input)
        {
            Message inputMsg = null;
            if (input == null)
            {
                return null;
            }
            else if (input.Contains("new_user"))
            {
                inputMsg = JsonConvert.DeserializeObject<NewUserMsg>(input);
               
            }
            else if (input.Contains("talk"))
            {
                inputMsg = JsonConvert.DeserializeObject<TalkMsg>(input);
            }
            else if (input.Contains("refresh_vc"))
            {
                inputMsg = JsonConvert.DeserializeObject<RefreshVcMsg>(input);
            }
            else if (input.Contains("user_joined"))
            {
                inputMsg = JsonConvert.DeserializeObject<UserJoinedMsg>(input);
            }
            else if (input.Contains("user_quit"))
            {
                inputMsg = JsonConvert.DeserializeObject<UserQuitMsg>(input);
            }
            else if (input.Contains("users_list"))
            {
                inputMsg = JsonConvert.DeserializeObject<UsersListMsg>(input);
            }
            else if (input.Contains("quit_ack"))
            {
                inputMsg = JsonConvert.DeserializeObject<QuitAck>(input);
            }
            return inputMsg;
        }

        public uint findDisconnNeighbourId(IPAddress quitIp)
        {
            uint quitId = 0;
            foreach (Neighbour i in _neighbours.Values)
            {
                if (i.ConnectionHandler.Address == quitIp)
                    return i.id;
            }
            return quitId;
        }

        private TcpClient _workerClient;
        private int _workerNumber;
        private StreamReader _workerStreamReader;
        private StreamWriter _workerStreamWriter;
        private NetworkStream _workerNetworkStream;
        private ManualResetEvent _doneEvent;
        private Dictionary<Dictionary<uint, uint>, Message> _talkBuffer;
        private List<Message> _buffer;
        private uint _myId;
        private Dictionary<uint, Neighbour> _neighbours;
    }
}
