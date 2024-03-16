using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerCommon;
using MessagePack;

namespace ChatServer
{
    public class ServerPacketData
    {
        public const int MQHeaderSize = 17;

        public string SenderSubject;
        public UInt16 SenderServerIndex;
        public Int32 UserNetSessionIndex;
        public UInt64 UserNetSeessionUniqueID;
        public byte[] Data;
                
        
        public void Assign(byte[] mqData)
        {
            var mqHead = new MQBinaryHeader();
            mqHead.HeaderDecode(mqData);

            if(mqHead.SenderInitial == MQSenderInitialHelper.GateWayServerInitialToNumber)
            {
                SenderSubject = MQSenderSubStringHelper.GateWayServer(mqHead.SenderIndex);
            }

            SenderServerIndex = mqHead.SenderIndex;
            UserNetSessionIndex = mqHead.UserNetSessionIndex;
            UserNetSeessionUniqueID = mqHead.UserNetSessionUniqueID;
            Data = mqData;
        }
                       
        
    }



    //[MessagePackObject]
    //public class PKTInternalReqRoomEnter
    //{
    //    [Key(0)]
    //    public int RoomNumber;

    //    [Key(1)]
    //    public string UserID;        
    //}

    //[MessagePackObject]
    //public class PKTInternalResRoomEnter
    //{
    //    [Key(0)]
    //    public ERROR_CODE Result;

    //    [Key(1)]
    //    public int RoomNumber;

    //    [Key(2)]
    //    public string UserID;
    //}


    //[MessagePackObject]
    //public class PKTInternalNtfLobbyLeave
    //{
    //    [Key(0)]
    //    public int LobbyNumber;

    //    [Key(1)]
    //    public string UserID;
    //}

}
