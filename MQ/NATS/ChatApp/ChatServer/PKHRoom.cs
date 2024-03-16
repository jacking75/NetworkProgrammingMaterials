using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagePack;

using ServerCommon;


namespace ChatServer
{
    public class PKHRoom : PKHandler
    {
        List<Room> RoomList = null;
        int StartRoomNumber;
        byte[] MQPacketEnCodeBuffer = new byte[1024];

        //UserNetSessionUniqueID는 절대 중복이 되지 않는다!!!
        Dictionary<UInt64, Int32> RoomNumberByUserNetSessionUniqueID = new Dictionary<UInt64, Int32>();


        public void SetRoomList(List<Room> lobbyList)
        {
            RoomList = lobbyList;
            StartRoomNumber = lobbyList[0].Number;
        }

        public void RegistPacketHandler(Dictionary<UInt16, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((UInt16)MQ_GATECHAT_DATA_ID.REQ_ROOM_ENTER, RequestRoomEnter);
            packetHandlerMap.Add((UInt16)MQ_GATECHAT_DATA_ID.REQ_ROOM_LEAVE, RequestRoomLeave);
            packetHandlerMap.Add((UInt16)CL_PACKET_ID.REQ_ROOM_CHAT, RequestRoomChat);            
        }



        #region PacketHandler
        public void RequestRoomEnter(ServerPacketData packetData)
        {
            var sessionIndex = packetData.UserNetSessionIndex;
            var sessionUniqueID = packetData.UserNetSeessionUniqueID;
            Console.WriteLine("RequestLobbyEnter");

            try
            {
                var reqPacket = new MQReqRoomEnter();
                reqPacket.Decode(packetData.Data);

                var roomNumber = reqPacket.RoomNumber;
                var room = GetRoom(roomNumber);
                if (room == null)
                {
                    ResponseRoomEnterToClient(packetData.SenderSubject, sessionIndex, sessionUniqueID, roomNumber, ERROR_CODE.LOBBY_ENTER_INVALID_ROOM_NUMBER);
                    return;
                }

                var userID = reqPacket.UserID.Trim('\0');
                if (room.AddUser(userID, packetData.SenderServerIndex, sessionIndex, sessionUniqueID) == false)
                {
                    ResponseRoomEnterToClient(packetData.SenderSubject, sessionIndex, sessionUniqueID, roomNumber, ERROR_CODE.LOBBY_ENTER_FAIL_ADD_USER);
                    return;
                }

                //TODO 방의 다른 유저에게 새로운 유저가 들어옴을 알린다.

                RoomNumberByUserNetSessionUniqueID.Add(sessionUniqueID, reqPacket.RoomNumber);

                ResponseRoomEnterToClient(packetData.SenderSubject, sessionIndex, sessionUniqueID, roomNumber, ERROR_CODE.NONE);
                Console.WriteLine("RequestLobbyEnter - Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
                

        public void RequestRoomLeave(ServerPacketData packetData)
        {
            var sessionIndex = packetData.UserNetSessionIndex;
            var sessionUniqueID = packetData.UserNetSeessionUniqueID;
            Console.WriteLine("RequestLobbyLeave");

            try
            {
                var reqPacket = new MQReqRoomLeave();
                reqPacket.Decode(packetData.Data);

                if (LeaveRoomUser(sessionUniqueID) == false)
                {
                    return;
                }

                //TODO 방의 다른 유저에게 나감을 알려야 한다

                RoomNumberByUserNetSessionUniqueID.Remove(sessionUniqueID);

                if (reqPacket.IsDisConnected == false)
                {
                    ResponseRoomLeaveToClient(packetData.SenderSubject, sessionIndex, sessionUniqueID, ERROR_CODE.NONE);
                }
                Console.WriteLine("RequestLeave - Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public void RequestRoomChat(ServerPacketData packetData)
        {
            var sessionIndex = packetData.UserNetSessionIndex;
            var sessionUniqueID = packetData.UserNetSeessionUniqueID;
            Console.WriteLine("RequestChat");

            try
            {
                var roomObject = CheckRoomAndRoomUser(sessionUniqueID);
                if(roomObject.Item1 == false)
                {
                    return;
                }

                
                var request = new PKTReqRoomChat();
                request.Decode(packetData.Data, MQBinaryHeader.Size);

                ResponseRoomChatToClient(packetData.SenderSubject, sessionIndex, sessionUniqueID, ERROR_CODE.NONE);

                var userIDBytes = Encoding.UTF8.GetBytes(roomObject.Item3.UserID); 
                var notifyPacket = new PKTNtfRoomChat()
                {
                    IDLen = (UInt16)userIDBytes.Length,
                    UserID = userIDBytes,
                    MsgLen = request.MsgLen,
                    ChatMessage = request.ChatMessage
                };
                var bodySize = notifyPacket.EnCode(MQPacketEnCodeBuffer, MQBinaryHeader.Size + PKTBinaryHead.Size);
                                
                BroadCastRelayPacket(roomObject.Item2.UserList, bodySize);

                Console.WriteLine("RequestChat - Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion


        #region private member
        Room GetRoom(int roomNumber)
        {
            var index = roomNumber - StartRoomNumber;

            if (index < 0 || index >= RoomList.Count())
            {
                return null;
            }

            return RoomList[index];
        }

        (bool, Room, RoomUser) CheckRoomAndRoomUser(UInt64 userNetSessionUniqueID)
        {
            Int32 roomNumber = -1;
            if(RoomNumberByUserNetSessionUniqueID.TryGetValue(userNetSessionUniqueID, out roomNumber) == false)
            {
                return (false, null, null);
            }

            var room = GetRoom(roomNumber);
            if (room == null)
            {
                return (false, null, null);
            }


            var roomUser = room.GetUser(userNetSessionUniqueID);
            if (roomUser == null)
            {
                return (false, room, null);
            }

            return (true, room, roomUser);
        }


        MQBinaryHeader MakeMQPacketHeader(Int32 userNetSessionIndex, UInt64 userNetSessionUniqueID, UInt16 packetId)
        {
            var relayPacket = new MQBinaryHeader();
            relayPacket.SenderInitial = MQSenderInitialHelper.GateWayServerInitialToNumber;
            relayPacket.SenderIndex = ServerIndex;
            relayPacket.UserNetSessionIndex = userNetSessionIndex;
            relayPacket.UserNetSessionUniqueID = userNetSessionUniqueID;
            relayPacket.PacketId = packetId;

            return relayPacket;
        }

        void ResponseRoomEnterToClient(string senderSubject, Int32 userNetSessionIndex, UInt64 userNetSessionUniqueID, Int32 rooomNumber, ERROR_CODE errorCode)
        {
            var mqPacketHeader = MakeMQPacketHeader(userNetSessionIndex, userNetSessionUniqueID, (UInt16)MQ_GATECHAT_DATA_ID.RES_ROOM_ENTER);
            var writePos = mqPacketHeader.HeaderEncode(MQPacketEnCodeBuffer);

            var responsePkt = new MQResRoomEnter();
            responsePkt.Result = (Int16)errorCode;
            responsePkt.RoomNumber = rooomNumber;

            var mqDataSize = responsePkt.EncodeBody(MQPacketEnCodeBuffer, writePos);

            SendToMQ(senderSubject, MQPacketEnCodeBuffer, 0, mqDataSize);
        }

        void ResponseRoomLeaveToClient(string senderSubject, Int32 userNetSessionIndex, UInt64 userNetSessionUniqueID, ERROR_CODE errorCode)
        {
            var mqPacketHeader = MakeMQPacketHeader(userNetSessionIndex, userNetSessionUniqueID, (UInt16)MQ_GATECHAT_DATA_ID.RES_ROOM_LEAVE);
            var writePos = mqPacketHeader.HeaderEncode(MQPacketEnCodeBuffer);

            var responsePkt = new MQResRoomEnter();
            responsePkt.Result = (Int16)errorCode;
            
            var mqDataSize = responsePkt.EncodeBody(MQPacketEnCodeBuffer, writePos);

            SendToMQ(senderSubject, MQPacketEnCodeBuffer, 0, mqDataSize);
        }
        
        void ResponseRoomChatToClient(string senderSubject, Int32 userNetSessionIndex, UInt64 userNetSessionUniqueID, ERROR_CODE errorCode)
        {
            var relayPacket = MakeMQPacketHeader(userNetSessionIndex, userNetSessionUniqueID, (UInt16)MQ_GATECHAT_DATA_ID.RELAY);
            var writePos = relayPacket.HeaderEncode(MQPacketEnCodeBuffer);
            var packetHeadPos = writePos;
            writePos += PKTBinaryHead.Size;

            var responsePkt = new PKTResRoomChat()
            {
                Result = (short)errorCode
            };
            
            var bodySize = responsePkt.EnCode(MQPacketEnCodeBuffer, writePos);

            var header = new PKTBinaryHead();
            header.TotalSize = (UInt16)(PKTBinaryHead.Size + bodySize);
            header.PacketID = (UInt16)CL_PACKET_ID.RES_ROOM_CHAT;
            header.HeaderEnCode(MQPacketEnCodeBuffer, packetHeadPos);

            var mqDataSize = MQBinaryHeader.Size + header.TotalSize;
            SendToMQ(senderSubject, MQPacketEnCodeBuffer, 0, mqDataSize);
        }

                
        bool LeaveRoomUser(UInt64 userNetSessionUniqueID)
        {
            Console.WriteLine($"LeaveLobbyUser. SessionUniqueID:{userNetSessionUniqueID}");

            var roomObject = CheckRoomAndRoomUser(userNetSessionUniqueID);
            if(roomObject.Item1 == false)
            {
                return false;
            }

            roomObject.Item2.RemoveUser(userNetSessionUniqueID);
            return true;
        }
        
        void BroadCastRelayPacket(List<RoomUser> userList, int bodySize)
        {
            foreach(var user in userList)
            {
                var senderSubject = MQSenderSubStringHelper.GateWayServer(user.GateWayServerIndex);

                var relayPacket = MakeMQPacketHeader(user.NetSessionIndex, user.NetSessionUniqueID, (UInt16)MQ_GATECHAT_DATA_ID.RELAY);
                var writePos = relayPacket.HeaderEncode(MQPacketEnCodeBuffer);
                                
                var header = new PKTBinaryHead();
                header.TotalSize = (UInt16)(PKTBinaryHead.Size + bodySize);
                header.PacketID = (UInt16)CL_PACKET_ID.NTF_ROOM_CHAT;
                header.HeaderEnCode(MQPacketEnCodeBuffer, writePos);

                var mqDataSize = MQBinaryHeader.Size + header.TotalSize;
                SendToMQ(senderSubject, MQPacketEnCodeBuffer, 0, mqDataSize);
            }
        }

        #endregion

    }
}
