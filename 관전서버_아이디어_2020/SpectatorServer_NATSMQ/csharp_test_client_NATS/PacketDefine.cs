using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_test_client
{
    class PacketDef
    {
        public const UInt16 PACKET_HEADER_SIZE = 8;
        public const UInt16 PACKET_HEADER_MSGPACK_HEADER = 3;
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;
    }

    public enum CSPacketID : ushort
    {
        PACKET_ID_ECHO = 101,

        // Ping(Heart-beat)
        PACKET_ID_PING_REQ = 201,
        PACKET_ID_PING_RES = 202,

        PACKET_ID_ERROR_NTF = 203,


        // 로그인
        RequestLogin = 701,
        ResponseLogin = 702,
                

        RequestRoomEnter = 721,
        ResponseRoomEnter = 722,
        //PACKET_ID_ROOM_USER_LIST_NTF = 723,
        //PACKET_ID_ROOM_NEW_USER_NTF = 724,

         RequestRoomLeave = 726,
         ResponseRoomLeave = 727,
         //PACKET_ID_ROOM_LEAVE_USER_NTF = 728,

         RequestRoomChat = 761,
         NotifyRoomChat = 762,

    }


    public enum ERROR_CODE : Int16
    {
        ERROR_NONE = 0,



        ERROR_CODE_USER_MGR_INVALID_USER_UNIQUEID = 112,

        ERROR_CODE_PUBLIC_CHANNEL_IN_USER = 114,

        ERROR_CODE_PUBLIC_CHANNEL_INVALIDE_NUMBER = 115,
    }
}
