using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatClient
{
    class UsersListMsg : Message
    {
        public UsersListMsg(uint aSender, Dictionary<string, ChatUser> aUsersList) : base(aSender, "users_list")
        {
            if (aUsersList == null)
                return;
            UsersList = fromJson(aUsersList);
        }
        public Dictionary<string, ChatUser> users_list
        {
            get { return toJson(UsersList); }
            set { UsersList = fromJson(value); }
        }
        private Dictionary<uint, ChatUser> UsersList;

        public static Dictionary<string, ChatUser> toJson(Dictionary<uint, ChatUser> UsersList)
        {
            Dictionary<string, ChatUser> ret = new Dictionary<string, ChatUser>();
            try
            {
                foreach (ChatUser cu in UsersList.Values)
                {
                    ret.Add(cu.id.ToString(), cu);
                }
            }
            catch (Exception e)
            {
            }
            return ret;
        }

        public static Dictionary<uint, ChatUser> fromJson(Dictionary<string, ChatUser> val)
        {
            Dictionary<uint, ChatUser> ret = new Dictionary<uint, ChatUser>();
            foreach (ChatUser cu in val.Values)
            {
                ret.Add(cu.id, cu);
            }
            return ret;
        }
    }
}
