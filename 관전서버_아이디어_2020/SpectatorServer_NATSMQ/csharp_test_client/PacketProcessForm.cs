using MessagePack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_test_client
{
    public partial class mainForm
    {
        Dictionary<CSPacketID, Action<byte[]>> PacketFuncDic = new Dictionary<CSPacketID, Action<byte[]>>();

        void SetPacketHandler()
        {
            PacketFuncDic.Add(CSPacketID.PACKET_ID_ERROR_NTF, PacketProcess_ErrorNotify);
            PacketFuncDic.Add(CSPacketID.ResponseLogin, PacketProcess_LoginResponse);
            PacketFuncDic.Add(CSPacketID.ResponseRoomEnter, PacketProcess_RoomEnterResponse);
            //PacketFuncDic.Add(CSPacketID.PACKET_ID_ROOM_USER_LIST_NTF, PacketProcess_RoomUserListNotify);
            //PacketFuncDic.Add(CSPacketID.PACKET_ID_ROOM_NEW_USER_NTF, PacketProcess_RoomNewUserNotify);
            PacketFuncDic.Add(CSPacketID.ResponseRoomLeave, PacketProcess_RoomLeaveResponse);
            //PacketFuncDic.Add(CSPacketID.PACKET_ID_ROOM_LEAVE_USER_NTF, PacketProcess_RoomLeaveUserNotify);
            PacketFuncDic.Add(CSPacketID.NotifyRoomChat, PacketProcess_RoomChatNotify);
            //PacketFuncDic.Add(CSPacketID.PACKET_ID_ROOM_RELAY_NTF, PacketProcess_RoomRelayNotify);
        }

        void PacketProcess(byte[] packetData)
        {
            var packetID = (CSPacketID)MsgPackPacketHeaderInfo.ReadPacketID(packetData, 0);
            //DevLog.Write("Packet Error:  PacketID:{packet.PacketID.ToString()},  Error: {(ERROR_CODE)packet.Result}");
            //DevLog.Write("RawPacket: " + packet.PacketID.ToString() + ", " + PacketDump.Bytes(packet.BodyData));

            if (PacketFuncDic.ContainsKey(packetID))
            {
                PacketFuncDic[packetID](packetData);
            }
            else
            {
                DevLog.Write("Unknown Packet Id: " + packetID.ToString());
            }         
        }

       
        void PacketProcess_ErrorNotify(byte[] bodyData)
        {
            var notifyPkt = new ErrorNtfPacket();
            notifyPkt.FromBytes(bodyData);

            DevLog.Write($"에러 통보 받음:  {notifyPkt.Error}");
        }


        void PacketProcess_LoginResponse(byte[] packetData)
        {
            var responsePkt = new LoginResPacket();
            responsePkt.FromBytes(packetData);

            DevLog.Write($"로그인 결과:  {(ERROR_CODE)responsePkt.Result}");
        }


        void PacketProcess_RoomEnterResponse(byte[] packetData)
        {
            var responsePkt = new RoomEnterResPacket();
            responsePkt.FromBytes(packetData);

            DevLog.Write($"방 입장 결과:  {(ERROR_CODE)responsePkt.Result}");
        }

        void PacketProcess_RoomUserListNotify(byte[] packetData)
        {
            var notifyPkt = new RoomUserListNtfPacket();
            notifyPkt.FromBytes(packetData);

            for (int i = 0; i < notifyPkt.UserCount; ++i)
            {
                AddRoomUserList(notifyPkt.UserUniqueIdList[i], notifyPkt.UserIDList[i]);
            }

            DevLog.Write($"방의 기존 유저 리스트 받음");
        }

        void PacketProcess_RoomNewUserNotify(byte[] packetData)
        {
            var notifyPkt = new RoomNewUserNtfPacket();
            notifyPkt.FromBytes(packetData);

            AddRoomUserList(notifyPkt.UserUniqueId, notifyPkt.UserID);
            
            DevLog.Write($"방에 새로 들어온 유저 받음");
        }


        void PacketProcess_RoomLeaveResponse(byte[] packetData)
        {
            var responsePkt = new RoomLeaveResPacket();
            responsePkt.FromBytes(packetData);

            DevLog.Write($"방 나가기 결과:  {(ERROR_CODE)responsePkt.Result}");
        }

        void PacketProcess_RoomLeaveUserNotify(byte[] packetData)
        {
            var notifyPkt = new RoomLeaveUserNtfPacket();
            notifyPkt.FromBytes(packetData);

            RemoveRoomUserList(notifyPkt.UserUniqueId);

            DevLog.Write($"방에서 나간 유저 받음");
        }                       

        void PacketProcess_RoomChatNotify(byte[] packetData)
        {
            var responsePkt = MessagePackSerializer.Deserialize< RoomChatNtfPacket>(packetData);

            AddRoomChatMessageList(responsePkt.UserID, responsePkt.Message);
        }

        void AddRoomChatMessageList(string userID, string msgssage)
        {
            var msg = $"{userID}:  {msgssage}";

            if (listBoxRoomChatMsg.Items.Count > 512)
            {
                listBoxRoomChatMsg.Items.Clear();
            }

            listBoxRoomChatMsg.Items.Add(msg);
            listBoxRoomChatMsg.SelectedIndex = listBoxRoomChatMsg.Items.Count - 1;
        }


        void PacketProcess_RoomRelayNotify(byte[] packetData)
        {
            var notifyPkt = new RoomRelayNtfPacket();
            notifyPkt.FromBytes(packetData);

            var stringData = Encoding.UTF8.GetString(notifyPkt.RelayData);
            DevLog.Write($"방에서 릴레이 받음. {notifyPkt.UserUniqueId} - {stringData}");
        }
    }
}
