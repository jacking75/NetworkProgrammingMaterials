using MessagePack; //https://github.com/neuecc/MessagePack-CSharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{    
    [MessagePackObject]
    public class PKTMsgPackHead
    {
        public void SetInfo(UInt16 totalSize, UInt16 packetID, SByte type)
        {
            FastBinaryWrite.UInt16(Head, 0, totalSize);
            FastBinaryWrite.UInt16(Head, 2, packetID);
            FastBinaryWrite.SByte(Head, 4, type);
        }

        [Key(0)]
        public Byte[] Head = new Byte[5];       
    }

    public class PKTBinaryHead
    {
        public const UInt16 Size = 5;

        public UInt16 TotalSize;
        public UInt16 PacketID;
        public SByte Type;

        public int HeaderDeCode(byte[] buffer, int startPos)
        {
            var pos = startPos;

            TotalSize = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            PacketID = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            Type = FastBinaryRead.SByte(buffer, pos);
            pos += 1;

            return pos;
        }

        public int HeaderEnCode(byte[] buffer, int startPos)
        {
            var pos = startPos;

            FastBinaryWrite.UInt16(buffer, pos, TotalSize);
            pos += 2;

            FastBinaryWrite.UInt16(buffer, pos, PacketID);
            pos += 2;

            FastBinaryWrite.SByte(buffer, pos, Type);
            pos += 1;

            return pos;
        }

    }
        

    public class PKTReqRoomEnter : PKTBinaryHead
    {
        public int RoomNumber;

        public int Decode(byte[] buffer, int startPos)
        {
            var pos = HeaderDeCode(buffer, startPos);

            RoomNumber = FastBinaryRead.Int32(buffer, pos);
            pos += 4;

            return pos;
        }

        public int EnCode(byte[] buffer, int startPos)
        {
            var pos = HeaderEnCode(buffer, startPos);

            FastBinaryWrite.Int32(buffer, pos, RoomNumber);
            pos += 4;

            return pos;
        }
    }

    public class PKTResRoomEnter : PKTBinaryHead
    {
        public short Result;
        public Int32 RoomNumber;

        public int Decode(byte[] buffer, int startPos)
        {
            var pos = HeaderDeCode(buffer, startPos);

            Result = FastBinaryRead.Int16(buffer, pos);
            pos += 2;

            RoomNumber = FastBinaryRead.Int32(buffer, pos);
            pos += 4;

            return pos;
        }

        public int EnCode(byte[] buffer, int startPos)
        {
            var pos = HeaderEnCode(buffer, startPos);

            FastBinaryWrite.Int16(buffer, pos, Result);
            pos += 2;

            FastBinaryWrite.Int32(buffer, pos, RoomNumber);
            pos += 4;

            return pos;
        }
    }



    public class PKTReqRoomLeave
    {
    }

    public class PKTResRoomLeave : PKTBinaryHead
    {
        public short Result;

        public int Decode(byte[] buffer, int startPos)
        {
            var pos = HeaderDeCode(buffer, startPos);

            Result = FastBinaryRead.Int16(buffer, pos);
            pos += 2;

            return pos;
        }

        
    }



    public class PKTReqRoomChat : PKTBinaryHead
    {
        public UInt16 MsgLen;
        public byte[] ChatMessage;

        public int Decode(byte[] buffer, int startPos)
        {
            var pos = HeaderDeCode(buffer, startPos);

            MsgLen = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            ChatMessage = FastBinaryRead.Bytes(buffer, pos, MsgLen);
            pos += MsgLen;
            return pos;
        }
    }

    public class PKTResRoomChat : PKTBinaryHead
    {
        public short Result;

        public int EnCode(byte[] buffer, int startPos)
        {
            var pos = startPos;

            FastBinaryWrite.Int16(buffer, pos, Result);
            pos += 2;

            return pos - startPos;
        }
    }

    public class PKTNtfRoomChat : PKTBinaryHead
    {
        public UInt16 IDLen;
        public byte[] UserID;

        public UInt16 MsgLen;
        public byte[] ChatMessage;


        public int EnCode(byte[] buffer, int startPos)
        {
            var pos = startPos;

            FastBinaryWrite.UInt16(buffer, pos, IDLen);
            pos += 2;

            FastBinaryWrite.Bytes(buffer, pos, UserID, IDLen);
            pos += IDLen;

            FastBinaryWrite.UInt16(buffer, pos, MsgLen);
            pos += 2;

            FastBinaryWrite.Bytes(buffer, pos, ChatMessage, MsgLen);
            pos += MsgLen;

            return pos - startPos;
        }
    }


}
