using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class RoomManager
    {
        List<Room> RoomList = new List<Room>();

        public void CreateRooms()
        {
            var maxRoomCount = ChatServer.ServerOption.RoomMaxCount;
            var startNumber = ChatServer.ServerOption.RoomStartNumber;
            var maxUserCount = ChatServer.ServerOption.RoomMaxUserCount;

            for(int i = 0; i < maxRoomCount; ++i)
            {
                var roomNumber = (startNumber + i);
                var room = new Room();
                room.Init(i, roomNumber, maxUserCount);

                RoomList.Add(room);
            }                                   
        }


        public List<Room> GetRoomList() { return RoomList; }
    }
}
