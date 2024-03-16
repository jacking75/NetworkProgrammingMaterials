using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{
    public class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const Int16 PACKET_HEADER_SIZE_OF_MSGPACK = 8;
        public const Int16 PACKET_HEADER_MSGPACK_START_POS = 3;

        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;

        public const int INVALID_LOBBY_NUMBER = -1;


        static public void SetHeadInfo(byte[] packetData, UInt16 packetId, UInt16 size)
        {
            FastBinaryWrite.UInt16(packetData, PACKET_HEADER_MSGPACK_START_POS, size);
            FastBinaryWrite.UInt16(packetData, (PACKET_HEADER_MSGPACK_START_POS + 2), packetId);
        }
    }
}
