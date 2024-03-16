using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace csharp_test_client
{
    public partial class mainForm
    {
        Dictionary<PACKET_ID, Action<byte[]>> PacketFuncDic = new Dictionary<PACKET_ID, Action<byte[]>>();

        void SetPacketHandler()
        {
            PacketFuncDic.Add(PACKET_ID.PACKET_ID_ECHO, PacketProcess_Echo);
            PacketFuncDic.Add(PACKET_ID.PACKET_ID_SIMPLE_CHAT, PacketProcess_SimpleChat);

            PacketFuncDic.Add(PACKET_ID.RES_LOGIN, PacketProcess_LoginRes);            
            PacketFuncDic.Add(PACKET_ID.RES_ROOM_ENTER, PacketProcess_RoomEnterRes);
            PacketFuncDic.Add(PACKET_ID.RES_ROOM_LEAVE, PacketProcess_RoomLeaveRes);
            PacketFuncDic.Add(PACKET_ID.RES_ROOM_CHAT, PacketProcess_RoomChatRes);          
            PacketFuncDic.Add(PACKET_ID.NTF_ROOM_CHAT, PacketProcess_RoomChatNotify);            
        }

        void PacketProcess(ClientNetLib.PacketData packet)
        {
            var packetType = (PACKET_ID)packet.PacketID;
            //DevLog.Write("Packet Error:  PacketID:{packet.PacketID.ToString()},  Error: {(ERROR_CODE)packet.Result}");
            //DevLog.Write("RawPacket: " + packet.PacketID.ToString() + ", " + PacketDump.Bytes(packet.BodyData));

            if (PacketFuncDic.ContainsKey(packetType))
            {
                PacketFuncDic[packetType](packet.BodyData);
            }
            else
            {
                DevLog.Write("Unknown Packet Id: " + packet.PacketID.ToString());
            }         
        }

        void PacketProcess_Echo(byte[] bodyData)
        {
            DevLog.Write($"Echo 받음:  {bodyData.Length}");
        }

        void PacketProcess_SimpleChat(byte[] bodyData)
        {
            var stringData = Encoding.UTF8.GetString(bodyData);
            DevLog.Write($"SimpleChat 받음: {stringData}");
        }

        void PacketProcess_ErrorNotify(byte[] bodyData)
        {
            var notifyPkt = new ErrorNtfPacket();
            notifyPkt.FromBytes(bodyData);

            DevLog.Write($"에러 통보 받음:  {notifyPkt.Error}");
        }


        void PacketProcess_LoginRes(byte[] bodyData)
        {
            var responsePkt = JsonSerializer.Deserialize<LoginResJsonPacket>(bodyData);
            
            DevLog.Write($"로그인 결과:  {(ERROR_CODE)responsePkt.Ret}");

            if ((ERROR_CODE)responsePkt.Ret == ERROR_CODE.NONE)
            {
                labelStatus.Text = "로비에 로그인 완료";
            }
        }

        
        void PacketProcess_RoomEnterRes(byte[] bodyData)
        {
            var responsePkt = JsonSerializer.Deserialize<RoomEnterResJsonPacket>(bodyData);

            DevLog.Write($"게임 서버 방 입장 요청 결과:  {(ERROR_CODE)responsePkt.Ret}");

            if ((ERROR_CODE)responsePkt.Ret == ERROR_CODE.NONE)
            {
                labelStatus.Text = "게임 방에 입장 완료";
            }
        }
        
        
        void PacketProcess_RoomLeaveRes(byte[] bodyData)
        {
            var responsePkt = JsonSerializer.Deserialize<RoomLeaveResJsonPacket>(bodyData);

            DevLog.Write($"게임 서버 방 나가기 요청 결과:  {(ERROR_CODE)responsePkt.Ret}");

            if ((ERROR_CODE)responsePkt.Ret == ERROR_CODE.NONE)
            {
                labelStatus.Text = "게임 방을 나간 상태";
            }
        }
        
        
        void PacketProcess_RoomChatRes(byte[] bodyData)
        {
            var responsePkt = JsonSerializer.Deserialize<RoomChatResJsoPacket>(bodyData);

            DevLog.Write($"게임 서버 방 채팅 요청 결과:  {(ERROR_CODE)responsePkt.Ret}");            
        }
        

        void PacketProcess_RoomChatNotify(byte[] bodyData)
        {
            var notifyPkt = JsonSerializer.Deserialize<RoomChatNtfJsoPacket>(bodyData);
            
            AddRoomChatMessageList($"[{notifyPkt.UserID}] {notifyPkt.Msg}");
        }
        
        void AddRoomChatMessageList(string msgssage)
        {
            if (listBoxRoomChatMsg.Items.Count > 512)
            {
                listBoxRoomChatMsg.Items.Clear();
            }

            listBoxRoomChatMsg.Items.Add(msgssage);
            listBoxRoomChatMsg.SelectedIndex = listBoxRoomChatMsg.Items.Count - 1;
        }
    }
}
