using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;

namespace ChatClient
{
    /*
     * Represents a console of the chat application
     */
    class ChatConsole
    {
        public ChatConsole(ConfigurationReader config, uint myId, string myNick, Dictionary<Dictionary<uint, uint>, Message> backup, ref Dictionary<uint, Dictionary<uint, uint>> myMc, ref Program chat)
        {
            _backup = backup;
            _working = false;
            _myId = myId;
            _myNick = myNick;
            _myMc = myMc;
            _client = chat;
        }

        public void Start()
        {
            _working = true;
            string message = string.Empty;
            
            Console.WriteLine("Welcome to chatroom");
            while(!message.ToLower().Equals("quit"))
            {
                message = System.Console.ReadLine();
                if (!message.Equals("quit"))
                {
                     
                    TalkMsg talk = new TalkMsg(_myId, message, _myMc[_myId], Program.ALL);
                    talk.addToList(_myId);
                    _backup.Add(new Dictionary<uint, uint>(_myMc[_myId]), talk);
                    _client.SendTo(talk);
                    _myMc[_myId][_myId]++;
                }
                
                
            }
            UserQuitMsg quitMsg = new UserQuitMsg(_myId, Program.ALL, _myId);
            _client.SendTo(quitMsg);
            _working = false;
        }

        public bool IsWorking()
        {
            return _working;
        }

        private Dictionary<uint, Dictionary<uint, uint>> _myMc;
        private string _myNick;
        private uint _myId;
        private bool _working;
        private Dictionary<Dictionary<uint, uint>, Message> _backup;
        private Program _client;
    }
}
