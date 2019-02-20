using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
namespace ChatClient
{
    class UserQuitTimer
    {

        public UserQuitTimer(ref Dictionary<uint, ChatUser> usersList, double time, uint myId, uint timerId)
        {
            ackAwaitingList = new Dictionary<uint, User>();
            copyList(ref usersList);
            timer = new Timer(time);
            timer.Enabled = true;
            timer.AutoReset = false;
            MyId = myId;
            TimerId = timerId;
        }

        public void copyList( ref Dictionary<uint, ChatUser> usersList)
        {
            foreach (ChatUser i in usersList.Values)
            {
                if (i.online && i.id != MyId)
                {
                    ackAwaitingList.Add(i.id, i);
                }
            }
        }

        public void deleteAwaiting(uint id)
        {
            if (ackAwaitingList.ContainsKey(id))
                ackAwaitingList.Remove(id);
        }

        public Dictionary<uint, User> AckAwaitingList
        {
            get { return ackAwaitingList; }
            set { ackAwaitingList = value; }
        }

        public Timer Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public uint TimerId
        {
            get { return timerId; }
            set { timerId = value; }
        }

        private Timer timer;
        private Dictionary<uint, User> ackAwaitingList;
        private uint MyId;
        private uint timerId;
    }
}

