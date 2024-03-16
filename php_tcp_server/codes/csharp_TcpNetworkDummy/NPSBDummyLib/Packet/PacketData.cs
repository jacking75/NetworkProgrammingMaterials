using System;
using System.Collections.Generic;


namespace CSBaseLib
{
    public class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const int INVALID_ROOM_NUMBER = -1;
    }

    public class ResPacketBase
    {
        public Int16 Ret;
    }

    public class LoginReqJsonPacket
    {
        public string UserID;
        public string UserPW;
    }

    public class LoginResJsonPacket : ResPacketBase
    {
    }


    // 방 입장
    public class RoomEnterReqJsonPacket
    {
        public int RoomNum;
    }

    public class RoomEnterResJsonPacket : ResPacketBase
    {
        public int RoomNum;
    }

    //TODO 새로 들어온 유저 정보 통보, 방에 있는 유저들 정보 통보

    // 방 나가기
    public class RoomLeaveReqJsonPacket
    {
    }

    public class RoomLeaveResJsonPacket : ResPacketBase
    {
    }

    //TODO 방에서 나가는 유저 정보 통보


    // 방 채팅
    public class RoomChatReqJsoPacket
    {
        public string Msg;
    }

    public class RoomChatResJsoPacket : ResPacketBase
    {
    }

    public class RoomChatNtfJsoPacket
    {
        public string UserID;
        public string Msg;
    }
}
