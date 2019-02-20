using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class User
    {
        public User(uint anId)
        {
            Id = anId;
            Online = true;
        }
        
        public uint id
        {
            get { return Id; }
            set { Id = value; }
        }

        public bool online
        {
            get { return Online; }
            set { Online = value; }
        }

        protected uint Id;
        protected bool Online;
    }
}
