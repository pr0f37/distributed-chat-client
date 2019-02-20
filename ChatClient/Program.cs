using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;
using Newtonsoft;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.IO;
using System.Timers;


namespace ChatClient
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Program chatClient = new Program();
            // create configuration reader
            ConfigurationReader config = new ConfigurationReader("config.xml");
            // parse an XML file
            config.ParseFile();
            // get port number from the xml file
            chatClient.MyPort = config.Port;
            chatClient.MyIp = config.IpAddress;
            chatClient.UsersList[chatClient.MyId] = new ChatUser(chatClient.MyId, chatClient.MyNick, chatClient.MyIp, chatClient.MyPort);

            // create tcp port listener
            TCPPortListener portListener = new TCPPortListener(config, chatClient.Inbox, chatClient.InboxBuffer, chatClient.MyId, ref chatClient.Neighbours);
            // create and start tcp port listener thread
            Thread portListenerThread = new Thread(portListener.Start);
            portListenerThread.Start();
            // give 1 second to port listener thread to start listening
            System.Threading.Thread.Sleep(1000);

            // check if connected to net
            if ((chatClient.MyState & CONNECTED) != CONNECTED)
            {
                // if not, try to connect
                string ipAddress = string.Empty;
                int port;
                Console.WriteLine("Please write IP to connect to:");
                ipAddress = Console.ReadLine();
//                ipAddress = "127.0.0.1";
                Console.WriteLine("Now please write port number to connect to:");
                port = int.Parse(Console.ReadLine());
                chatClient.ConnectTo(ipAddress, port, ref portListener);

            }
            // create console 
            ChatConsole console = new ChatConsole(config, chatClient.MyId, chatClient.MyNick, chatClient.Backup, ref chatClient.MClocks, ref chatClient);
            // create and start console thread
            Thread consoleThread = new Thread(console.Start);
            consoleThread.Start();
            // console is working until user won't type 'quit' command
            // main thread needs to wait 2 seconds to ensure console starts properly
            System.Threading.Thread.Sleep(2000);
            int j = 0;
            while(console.IsWorking() == true)
            {
                int i = 0;
                while (i < chatClient.InboxBuffer.Count && i >= 0)
                {
                     if(chatClient.toMeOrNotToMe(chatClient.InboxBuffer[i]) == 1)
                     {
                         chatClient.InboxBuffer.RemoveAt(i);
                         i--;
                     }
                    i++;
                }
                i = 0;
                
                while (i < chatClient.Inbox.Count && i >= 0)
                {
                    if (chatClient.toMeOrNotToMe(chatClient.Inbox.ElementAt(i).Value) == 1)
                    {
                        chatClient.Backup.Add(chatClient.Inbox.ElementAt(i).Key, chatClient.Inbox.ElementAt(i).Value);
                        chatClient.Inbox.Remove(chatClient.Inbox.ElementAt(i).Key);
                        i--;
                    }
                    else if (chatClient.toMeOrNotToMe(chatClient.Inbox.ElementAt(i).Value) == 2)
                    {
                        chatClient.Inbox.Remove(chatClient.Inbox.ElementAt(i).Key);
                        i--;
                    }
                    i++;
                }
               
               
                // check the inbox buffer per every 2 seconds

                if (j == 50)
                {
               //     chatClient.CleanBackup();
//                     Console.WriteLine("****DEBUG****");
//                     Console.WriteLine("CleanBackup. Backup size after cleaning: " + chatClient.Backup.Count);
//                     Console.WriteLine("****DEBUG****");
                }
                System.Threading.Thread.Sleep(500);
                j++;
                j %= 20;
            }
            // shut down tcp port listener
            portListener.StopWorking();
            // join all threads
            consoleThread.Join();
            portListenerThread.Join();
            
        }
        

        public Program()
        {
            Backup = new Dictionary<Dictionary<uint, uint>, Message>();
            Inbox = new Dictionary<Dictionary<uint, uint>, Message>();
            MClocks = new Dictionary<uint, Dictionary<uint, uint>>();
            Dictionary<uint, uint> dictionary = new Dictionary<uint, uint>();
            InboxBuffer = new List<Message>();
            TimerMap = new Dictionary<uint, UserQuitTimer>();
            Console.WriteLine("Please give your nick:");
            MyNick = Console.ReadLine();
            Console.WriteLine("Please give your id:");
            MyId = (uint.Parse(Console.ReadLine()) * 10) + 19;
            if (MyId > 200)
                MyId = 199;
            dictionary.Add(MyId, 0);
            MClocks.Add(MyId, dictionary);
          
            UsersList = new Dictionary<uint, ChatUser>();
            Neighbours = new Dictionary<uint, Neighbour>();
            MyState = 0;
            _writer = new TCPPortWriter();
        }

        public int Parse(Message input)
        {
//             Console.WriteLine("****DEBUG****");
//             Console.WriteLine("Otrzymalem: " + JsonConvert.SerializeObject(input));
//             Console.WriteLine("****DEBUG****");
            if (input == null)
            {
                return 0;
            }
            else if (input is NewUserMsg)
            {
               
                NewUserMsg inputMsg = (NewUserMsg)input;
                
                // after broadcasting information about new user adds it to neighbours and to UserList
                UsersList.Add(inputMsg.sender, new ChatUser(inputMsg.sender, inputMsg.nick, inputMsg.ip, inputMsg.listen_port));
                
                
                // Updating MClocks Dictionary
                foreach (Dictionary<uint, uint> i in MClocks.Values)
                {
                    if (!i.ContainsKey(inputMsg.sender))
                        i.Add(inputMsg.sender, 0); // have to create new clock for every user
                    else
                    {
                        i.Remove(inputMsg.sender);
                        i.Add(inputMsg.sender, 0);
                    }
                }
                Dictionary<uint, uint> senderDictionary = new Dictionary<uint, uint>();
                foreach (User i in UsersList.Values)
                {
                    senderDictionary.Add(i.id, 0);
                }
                MClocks.Add(inputMsg.sender, senderDictionary);

                // after updating users' list, send it to new joined user
                SendTo(Neighbours[inputMsg.sender], new UsersListMsg(MyId, UsersListMsg.toJson(UsersList)));
                    //Console.WriteLine("wysłałem do: " + Neighbours[inputMsg.sender].ConnectionHandler.Port + " " + JsonConvert.SerializeObject(new UsersListMsg(MyId, UsersListMsg.toJson(UsersList))));
                // try to let to know everybody about new user
                SendTo(new UserJoinedMsg(MyId, ALL, inputMsg.ip, inputMsg.listen_port, inputMsg.nick, inputMsg.sender));

                return 1;
            }
            else if (input is TalkMsg)
            {
                TalkMsg inputMsg = (TalkMsg)input;

                MClocks[inputMsg.dont_send_to.First()] = inputMsg.vc;
                return ProcessTalkMessage(inputMsg);
            }
            else if (input is RefreshVcMsg)
            {
                RefreshVcMsg inputMsg = (RefreshVcMsg)input;
                uint realSender = inputMsg.dont_send_to[0];
                MClocks.Remove(realSender);
                MClocks.Add(realSender, inputMsg.vc);
                
                return 1;
            }
            else if (input is UserJoinedMsg)
            {
                UserJoinedMsg inputMsg = (UserJoinedMsg)input;
                
                // check if User already exist on Users List
                // if not, or if his nick has changed update your list
                if (UsersList.ContainsKey(inputMsg.new_id))
                    return 1;
                UsersList.Add(inputMsg.new_id, new ChatUser(inputMsg.new_id, inputMsg.new_nick, inputMsg.new_ip, inputMsg.new_port));
                
                foreach (Dictionary<uint, uint> i in MClocks.Values)
                {
                    if (!i.ContainsKey(inputMsg.new_id))
                        i.Add(inputMsg.new_id, 0); // have to create new clock for every user
                    else
                    {
                        i.Remove(inputMsg.new_id);
                        i.Add(inputMsg.new_id, 0);
                    }
                }
                Dictionary<uint, uint> senderDictionary = new Dictionary<uint, uint>();
                foreach (User i in UsersList.Values)
                {
                    senderDictionary.Add(i.id, 0);
                }
                MClocks.Add(inputMsg.new_id, senderDictionary);

                // if massage is to be broadcasted, broadcast it
                return 1;
            }
            
            else if (input is UsersListMsg)
            {
                UsersListMsg inputMsg = (UsersListMsg)input;
                UsersList = UsersListMsg.fromJson(inputMsg.users_list);
                MyState &= CONNECTED;
                firstNeighbour.id = inputMsg.sender;
                Neighbours.Add(inputMsg.sender, firstNeighbour);
                Console.WriteLine("Dodalem: " + firstNeighbour.id + " " + firstNeighbour.ConnectionHandler.Port + " do sasiadow");

                foreach (User i in UsersList.Values)
                {
                    Dictionary<uint, uint> userDictionary = new Dictionary<uint, uint>();
                    if(!MClocks[MyId].ContainsKey(i.id))
                        MClocks[MyId].Add(i.id,0); // MClocks, my knowledge
                    foreach (User j in UsersList.Values)
                    {
                        userDictionary.Add(j.id, 0);
                    }
                    if(!MClocks.ContainsKey(i.id))
                        MClocks.Add(i.id, userDictionary);    
                }
                return 1;
            }
            else if (input is UserQuitMsg)
            {
                UserQuitMsg inputMsg = (UserQuitMsg)input;
                if (Neighbours.ContainsKey(inputMsg.quit_id) && UsersList[inputMsg.quit_id].online == true)
                {
                    Neighbours[inputMsg.quit_id].ConnectionHandler.NeighbourNetworkStream.Close();
                    Neighbours[inputMsg.quit_id].ConnectionHandler.NeighbourClient.Close();
                    Neighbours[inputMsg.quit_id] = null;
                    if (inputMsg.dont_send_to.Last() == inputMsg.quit_id || inputMsg.receiver == MyId)
                    {
                        UsersList[inputMsg.quit_id].online = false;
                        UserQuitTimer newTimer = new UserQuitTimer(ref UsersList, 2000, MyId, inputMsg.quit_id);
                        newTimer.Timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
                        TimerMap.Add(inputMsg.quit_id, newTimer);
                        SendTo(inputMsg);
                    }
                    else
                    {
                        uint source = inputMsg.dont_send_to.First();
                        QuitAck msg = new QuitAck(MyId, source, inputMsg.quit_id);
                        SendTo(Neighbours[source], msg);
                    }
                }
                return 1;
            }
            else if (input is QuitAck)
            {
                QuitAck inputMsg = (QuitAck)input;
                uint whoSentAck = inputMsg.dont_send_to.First();
                if (TimerMap.ContainsKey(inputMsg.quit_id))
                    TimerMap[inputMsg.quit_id].deleteAwaiting(whoSentAck);
                // sprawdzamy czy istnieje jeszcze timer dla  whoSentAck i rozlaczonego quit_id
                // jesli tak to zatrzymaj go i usun
                // jesli nie to nic nie rob

            }
            return 0;
        }

        public bool SendTo(Neighbour receiver, Message message)
        {
            bool state = false;
            message.addToList(MyId);
            message.sender = MyId;
            if (receiver != null)
                return SendTo(receiver, JsonConvert.SerializeObject(message)); // if there is direct connection to neighbour
            else
            {
                if (SendTo(message)) // If there is no direct connection, broadcast the message, MyId will be written twice in don't send to list, but it doesn't change anything
                    state = true;
            }
            return state;
        }

        public bool SendTo(Message message)
        {
            bool state = false;
            message.addToList(MyId);
            message.sender = MyId;
            
            foreach (uint i in Neighbours.Keys)
            {
                if (!message.dont_send_to.Contains(i))
                    if (SendTo(Neighbours[i], JsonConvert.SerializeObject(message)))
                        state = true;

            }
            return state;
        }

        public bool SendTo(Neighbour receiver, string message)
        {
            if (receiver != null)
                return _writer.Connect(receiver.ConnectionHandler, message); // if we've got direct connection to our neighbour
            return false;
        }

        public bool ConnectTo(string ipAddress, int port, ref TCPPortListener portListener)
        {
            if ((MyState & CONNECTED) == CONNECTED)
                return false; // cannot make another connection if is already connected
            ConnHandler connectionHandler = new ConnHandler(ipAddress, port);
            firstNeighbour = new Neighbour(0, connectionHandler);
            portListener.doneListeners[portListener.workerCounter] = new ManualResetEvent(false);

            ListeningWorker newWorker = new ListeningWorker(connectionHandler.NeighbourClient, portListener.workerCounter, portListener.doneListeners[portListener.workerCounter], Inbox, InboxBuffer, MyId, ref Neighbours);
            ThreadPool.QueueUserWorkItem(newWorker.ThreadPoolCallback, portListener.workerCounter);
            portListener.workerCounter = (++portListener.workerCounter) % 10;

            NewUserMsg message = new NewUserMsg(UsersList[MyId].id, UsersList[MyId].ip, UsersList[MyId].port, UsersList[MyId].nick);
            //Console.WriteLine("wysylam do:" + firstNeighbour.ConnectionHandler.Address + " " + firstNeighbour.ConnectionHandler.Port + ": " + JsonConvert.SerializeObject(message));
            if ( SendTo(firstNeighbour, message)) 
            {
                MyState = CONNECTED;
                return true;
            }
            return false;
        }

        public void Disconnect()
        {
            foreach (Neighbour i in Neighbours.Values)
            {
                UserQuitMsg msg = new UserQuitMsg(MyId, i.id, MyId);
                SendTo(i, msg);
            }
            System.Threading.Thread.Sleep(1000);
            foreach (Neighbour i in Neighbours.Values)
            {
                i.ConnectionHandler.NeighbourNetworkStream.Close();
                i.ConnectionHandler.NeighbourClient.Close();
            }
        }

        public void CleanBackup()
        {
            List<Dictionary<uint, uint>> toBeRemoved = new List<Dictionary<uint, uint>>();
            foreach (Dictionary<uint, uint> i in Backup.Keys)
            {
                uint realSender = Backup[i].dont_send_to.First();
                bool ok = true;
                foreach (uint j in MClocks.Keys)
                {
                    if (i[realSender] > MClocks[j][realSender])
                    {
                        ok = false;
                        List<uint> dst = new List<uint>();
                        dst.Add(realSender);
                        dst.Add(MyId);
                        Backup[i].dont_send_to = dst;
                        ((TalkMsg)Backup[i]).receiver = j;
                        Backup[i].addToList(MyId); 
                        if (Neighbours.ContainsKey(j))
                            SendTo(Neighbours[j], Backup[i]);
                        else
                            SendTo(Backup[i]);
                    }
                }
                if (ok == true)
                    toBeRemoved.Add(i);
            }
            foreach (Dictionary<uint, uint> i in toBeRemoved)
            {
                Backup.Remove(i);
            }
        }
        
        public int toMeOrNotToMe(Message input)
        {
            bool isMessageToMe = false;
            if ( input is NewUserMsg || input is UsersListMsg || (input is ReceiverMessage &&  ((ReceiverMessage)input).receiver == MyId))
            {
                isMessageToMe = true; // message only to me, process the message
            }
            else if (input is ReceiverMessage)
            {
                ReceiverMessage inputMsg = (ReceiverMessage) input;
                if ( inputMsg.receiver != ALL)
                {
                    return SendTo(Neighbours[inputMsg.receiver], input)?1:0;
                } 
                else
                {
                    if (input is TalkMsg)
                    {
                        bool isOld = false; ;
                        if (((TalkMsg)input).vc[((TalkMsg)input).dont_send_to.First()] < MClocks[MyId][((TalkMsg)input).dont_send_to.First()])
                            isOld = true;
                        if (!isOld)
                        {
                            isMessageToMe = true;
                            SendTo(input);
                        }
                        else
                            return 2;
                    }
                    else
                    {
                        isMessageToMe = true;
                        SendTo(input);
                    }
                }
            }
            if (isMessageToMe)
            {
                return Parse(input);
            }
            return 1;
        }

        public bool CanBeShown(TalkMsg msg)
        {
            uint realSender = msg.dont_send_to.First();
            bool ok = true;
            foreach (uint i in msg.vc.Keys)
            {
                if (i == MyId)
                    continue;
                if (!MClocks[MyId].ContainsKey(i))
                    MClocks[MyId].Add(i, 0);
                if (MClocks[MyId][i] < msg.vc[i])
                {
                    ok = false;
                    break;
                }
            }
            return ok;
        }

        public int ProcessTalkMessage(TalkMsg msg)
        {
            uint realSender = msg.dont_send_to.First();
            if (CanBeShown(msg))
            {
                
                    MClocks[MyId][realSender]++;
                    Console.WriteLine("{0} said: {1}", UsersList[msg.dont_send_to.First()].nick, msg.chat_msg);
                    return 1;
                
            }
            return 0;
        }

        public void OnTimerEvent(object source, ElapsedEventArgs e)
        {
           // Console.WriteLine("Timer event received");
            foreach (UserQuitTimer qt in TimerMap.Values)
            {
                if (qt.Timer.Equals(source))
                {
                    if (qt.AckAwaitingList.Count != 0)
                    {
                        foreach (ChatUser user in qt.AckAwaitingList.Values)
                        {
                            if (user.online)
                            {
                                InboxBuffer.Add(new UserQuitMsg(user.id, MyId, user.id));
                            }
                        }
                    }
                    TimerMap.Remove(qt.TimerId);
                    break;
                }
                
            }
        }

        public const int USERS_NUMBER = 200;
        public const int CONNECTED = 1;
        public const uint ALL = 0;

        private TCPPortWriter _writer;
        private Dictionary<Dictionary<uint, uint>, Message> Inbox;
        private Dictionary<Dictionary<uint, uint>, Message> Backup;
        private List<Message> InboxBuffer;
        private Dictionary<uint, ChatUser> UsersList;
        private Dictionary<uint, Neighbour> Neighbours;
        private Dictionary<uint, Dictionary<uint,uint>> MClocks;
        private int MyPort;
        private string MyNick;
        private uint MyId;
        private string MyIp;
        private int MyState;
        private Dictionary<uint, UserQuitTimer> TimerMap;
        private Neighbour firstNeighbour;
        
    }
}
