using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPTest.Network
{
    public class SendingPackage
    {
        public byte[] Content;
        public UInt32 SendingSequenceNo;
        public DateTime LastSendTimestamp;
        public DateTime FirstSendTimestamp;
        public int fastack = 0;
    }

    public class RecvingPackage
    {
        public byte[] Data;
        public ushort MaxPiece;
        public UInt32 RecvingSequenceNo;
    }

    public enum RUDP_STATE
    {
        CLOSED,
        SYN_SEND,
        ESTABLISED,
        LAST_ACK
    }

    public class UDPPacketFrame
    {
        public UInt16 CheckSum = 0;
        public UInt32 UNA = 0; //지금까지 받은 시퀸스
    }

    public enum CONNECT_HANDSHAKE
    {
        SYN = 0,
        SYN_ACK = 1,
        ACK = 2,
    }

    public enum CloseHandShake
    {
        FIN = 6,
        FIN_ACK = 4,
        ACK = 3,
        FIN_ACK = 2,
        FIN = 6,
        RST = 5
    }
}
