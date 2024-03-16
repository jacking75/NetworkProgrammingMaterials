using ServerCommon;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Room
    {
        public int Index { get; private set; }
        public int Number { get; private set; }

        int MaxUserCount = 0;

        public List<RoomUser> UserList { get; private set; } = new List<RoomUser>();

        public static Func<string, byte[], int, int, bool> NetSendFunc;


        public void Init(int index, int number, int maxUserCount)
        {
            Index = index;
            Number = number;
            MaxUserCount = maxUserCount;
        }

        public bool AddUser(string userID, UInt16 gateWaySereverIndex, Int32 netSessionIndex, UInt64 netSessionUniqueID)
        {
            if(GetUser(netSessionUniqueID) != null)
            {
                return false;
            }

            var roomUser = new RoomUser();
            roomUser.Set(userID, gateWaySereverIndex, netSessionIndex, netSessionUniqueID);
            UserList.Add(roomUser);

            return true;
        }

        public void RemoveUser(UInt64 netSessionUniqueID)
        {
            var index = UserList.FindIndex(x => x.NetSessionUniqueID == netSessionUniqueID);
            UserList.RemoveAt(index);
        }

        public bool RemoveUser(RoomUser user)
        {
            return UserList.Remove(user);
        }

        public RoomUser GetUserByID(string userID)
        {
            return UserList.Find(x => x.UserID == userID);
        }

        public RoomUser GetUser(UInt64 netSessionUniqueID)
        {
            return UserList.Find(x => x.NetSessionUniqueID == netSessionUniqueID);
        }

        public int CurrentUserCount()
        {
            return UserList.Count();
        }            
        
    }


    public class RoomUser
    {
        public string UserID { get; private set; }
        public UInt16 GateWayServerIndex { get; private set; }
        public Int32 NetSessionIndex { get; private set; }
        public UInt64 NetSessionUniqueID { get; private set; }

        public void Set(string userID, UInt16 gateWayServerIndex, Int32 netSessionIndex, UInt64 netSessionUniqueID)
        {
            UserID = userID;
            GateWayServerIndex = gateWayServerIndex;
            NetSessionIndex = netSessionIndex;
            NetSessionUniqueID = netSessionUniqueID;
        }
    }
}
