using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{    
    public class MQSenderInitialHelper
    {
        public static SByte DBServerInitialToNumber = Convert.ToSByte('D');
        public static SByte GateWayServerInitialToNumber = Convert.ToSByte('W');   
    }

    public class MQSenderSubStringHelper
    {
        public static string DBServer()
        {
            return "DB";
        }

        public static string GateWayServer(int index)
        {
            return $"GATE.{index}";
        }
    }


    #region LobbyServer-DBServer
    [MessagePackObject]
    public class MQMsgPackHead
    {
        [Key(0)]
        public UInt16 ID;
        [Key(1)]
        public string Sender;
    }

    // 로비의 로그인 요청
    [MessagePackObject]
    public class MQReqLBLogin : MQMsgPackHead
    {
        [Key(2)]
        public string UserNetSessionID;
        [Key(3)]
        public string UserID;
        [Key(4)]
        public string AuthToken;
    }

    [MessagePackObject]
    public class MQResLBLogin : MQMsgPackHead
    {
        [Key(2)]
        public Int16 Result;
        [Key(3)]
        public string UserNetSessionID;
        [Key(4)]
        public string UserID;
    }
    #endregion




    public class MQBinaryHeader
    {
        public const Int32 Size = 17;

        public sbyte SenderInitial;
        public UInt16 SenderIndex;
        public Int32 UserNetSessionIndex;
        public UInt64 UserNetSessionUniqueID;
        public UInt16 PacketId;

        public int HeaderDecode(byte[] buffer)
        {
            var pos = 0;

            SenderInitial = FastBinaryRead.SByte(buffer, pos);
            pos += 1;

            SenderIndex = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            UserNetSessionIndex = FastBinaryRead.Int32(buffer, pos);
            pos += 4;

            UserNetSessionUniqueID = FastBinaryRead.UInt64(buffer, pos);
            pos += 8;

            PacketId = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            return pos;
        }

        public int HeaderEncode(byte[] buffer)
        {
            var pos = 0;

            FastBinaryWrite.SByte(buffer, pos, SenderInitial);
            pos += 1;

            FastBinaryWrite.UInt16(buffer, pos, SenderIndex);
            pos += 2;

            FastBinaryWrite.Int32(buffer, pos, UserNetSessionIndex);
            pos += 4;

            FastBinaryWrite.UInt64(buffer, pos, UserNetSessionUniqueID);
            pos += 8;

            FastBinaryWrite.UInt16(buffer, pos, PacketId);
            pos += 2;

            return pos;
        }
    }

    #region GateWayServer-CHAT
    public class MQReqRoomEnter : MQBinaryHeader
    {
        const int MAX_USER_ID_BYTE_LENGTH = 16;
        
        public string UserID;
        public Int32 RoomNumber;


        public void Decode(byte[] buffer)
        {
            var pos = MQBinaryHeader.Size;

            UserID = FastBinaryRead.String(buffer, pos, MAX_USER_ID_BYTE_LENGTH);
            pos += MAX_USER_ID_BYTE_LENGTH;

            RoomNumber = FastBinaryRead.Int32(buffer, pos);
            pos += 4;
        }
    }
    
    public class MQResRoomEnter : MQBinaryHeader
    {
        public Int16 Result;
        public Int32 RoomNumber;


        public int EncodeBody(byte[] buffer, int writePos)
        {
            FastBinaryWrite.Int16(buffer, writePos, Result);
            writePos += 2;

            FastBinaryWrite.Int32(buffer, writePos, RoomNumber);
            writePos += 4;

            return writePos;
        }
    }


    public class MQReqRoomLeave : MQBinaryHeader
    {
        public bool IsDisConnected = false;


        public void Decode(byte[] buffer)
        {
            var pos = MQBinaryHeader.Size;

            var isDisConnected = FastBinaryRead.Byte(buffer, pos);
            pos += 1;

            if (isDisConnected == 1)
            {
                IsDisConnected = true;
            }
        }
    }

    public class MQResRoomLeave : MQBinaryHeader
    {
        public Int16 Result;
        
        public int EncodeBody(byte[] buffer, int writePos)
        {
            FastBinaryWrite.Int16(buffer, writePos, Result);
            writePos += 2;

            return writePos;
        }
    }


    //public class MQReqRelay : MQBinaryHeader
    //{
    //    public UInt16 DataSize;
    //    public byte[] RelayPacket = new byte[2024]; // 최대 패킷 크기로 한다


    //    public void Decode(byte[] buffer)
    //    {
    //        var pos = MQBinaryHeader.Size + PKTBinaryHead.Size;

    //        DataSize = FastBinaryRead.UInt16(buffer, pos);
    //        pos += 2;

    //        Buffer.BlockCopy(buffer, pos, RelayPacket, 0, DataSize);
    //        pos += DataSize;            
    //    }

    //    public int EncodeBody(byte[] buffer, int writePos)
    //    {
    //        var pos = writePos;

    //        FastBinaryWrite.UInt16(buffer, pos, DataSize);
    //        pos += 2;

    //        FastBinaryWrite.Bytes(buffer, pos, RelayPacket, DataSize);
    //        pos += DataSize;

    //        return pos;
    //    }
    //}
    #endregion

    #region GateWayServer-DB
    public class MQReqLogin : MQBinaryHeader
    {
        const int MAX_USER_ID_BYTE_LENGTH = 16;
        const int MAX_USER_PW_BYTE_LENGTH = 16;
                
        public string UserID;
        public string UserPW;


        public void Decode(byte[] buffer)
        {
            var pos = HeaderDecode(buffer);

            UserID = FastBinaryRead.String(buffer, pos, MAX_USER_ID_BYTE_LENGTH);
            pos += MAX_USER_ID_BYTE_LENGTH;

            UserPW = FastBinaryRead.String(buffer, pos, MAX_USER_PW_BYTE_LENGTH);
            pos += MAX_USER_PW_BYTE_LENGTH;
        }        
    }

    public class MQResLogin : MQBinaryHeader
    {
        public Int16 Result;
                
        public int Encode(byte[] buffer)
        {
            var pos = HeaderEncode(buffer);

            FastBinaryWrite.Int16(buffer, pos, Result);
            pos += 2;

            return pos;
        }
    }


    //public class MQNtfGAMatchingInfo : MQBinaryHeader
    //{
    //    public string UserID1;
    //    public string UserID2;
    //    public Int32 RoomNumber;

    //    public int Encode(byte[] buffer)
    //    {
    //        var userID1Bytes = Encoding.UTF8.GetBytes(UserID1);
    //        var userID2Bytes = Encoding.UTF8.GetBytes(UserID2);


    //        var pos = HeaderEncoding(buffer);

    //        FastBinaryWrite.Int16(buffer, pos, (Int16)userID1Bytes.Length);
    //        pos += 2;

    //        FastBinaryWrite.Bytes(buffer, pos, userID1Bytes);
    //        pos += userID1Bytes.Length;

    //        FastBinaryWrite.Int16(buffer, pos, (Int16)userID2Bytes.Length);
    //        pos += 2;

    //        FastBinaryWrite.Bytes(buffer, pos, userID2Bytes);
    //        pos += userID2Bytes.Length;

    //        FastBinaryWrite.Int32(buffer, pos, RoomNumber);
    //        pos += 4;

    //        return pos;
    //    }

    //    public void Decode(byte[] buffer)
    //    {
    //        var pos = HeaderDecode(buffer);

    //        var len = FastBinaryRead.UInt16(buffer, pos);
    //        pos += 2;
    //        UserID1 = FastBinaryRead.String(buffer, pos, len);
    //        pos += len;

    //        len = FastBinaryRead.UInt16(buffer, pos);
    //        pos += 2;
    //        UserID2 = FastBinaryRead.String(buffer, pos, len);
    //        pos += len;

    //        RoomNumber = FastBinaryRead.Int32(buffer, pos);
    //        pos += 4;
    //    }
    //}


    //public class MQNtfGAUnUsedRoom : MQBinaryHeader
    //{
    //    public Int32 RoomNumber;

    //    public void Decode(byte[] buffer)
    //    {
    //        var pos = HeaderDecode(buffer);

    //        RoomNumber = FastBinaryRead.Int32(buffer, pos);
    //        pos += 4;
    //    }
    //}
    #endregion
}
