using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks.Dataflow;

using ServerCommon;

namespace ChatServer
{
    class PacketProcessor
    {
        bool IsThreadRunning = false;
        System.Threading.Thread ProcessThread = null;

        //receive쪽에서 처리하지 않아도 Post에서 블럭킹 되지 않는다. 
        //BufferBlock<T>(DataflowBlockOptions) 에서 DataflowBlockOptions의 BoundedCapacity로 버퍼 가능 수 지정. 
        //BoundedCapacity 보다 크게 쌓이면 블럭킹 된다. default 값은 -1이고 무한대이다.
        BufferBlock<ServerPacketData> MsgBuffer = new BufferBlock<ServerPacketData>();
                
        Tuple<int,int> RoomNumberRange = new Tuple<int, int>(-1, -1);
        List<Room> RoomList = new List<Room>();

        Dictionary<UInt16, Action<ServerPacketData>> PacketHandlerMap = new Dictionary<UInt16, Action<ServerPacketData>>();
        PKHRoom RoomPacketHandler = new PKHRoom();
                

        public void CreateAndStart(List<Room> roomList)
        {
            var maxUserCount = ChatServer.ServerOption.RoomMaxCount * ChatServer.ServerOption.RoomMaxUserCount;
            
            RoomList = roomList;
            var minlobbyNum = RoomList[0].Number;
            var maxlobbyNum = RoomList[0].Number + RoomList.Count() - 1;
            RoomNumberRange = new Tuple<int, int>(minlobbyNum, maxlobbyNum);
            
            RegistPacketHandler((UInt16)ChatServer.ServerOption.Index);

            IsThreadRunning = true;
            ProcessThread = new System.Threading.Thread(this.Process);
            ProcessThread.Start();
        }
        
        public void Destory()
        {
            IsThreadRunning = false;
            MsgBuffer.Complete();
        }

        public void InsertMQMessage(byte[] data)
        {
            var packet = new ServerPacketData();
            packet.Assign(data);

            MsgBuffer.Post(packet);
        }

        //public void InsertPacket(ServerPacketData data)
        //{
        //    MsgBuffer.Post(data);
        //}

        public void SetMQ(Action<string, byte[], int, int> sendToMQ)
        {
            RoomPacketHandler.SetMq(sendToMQ);
        }


        void RegistPacketHandler(UInt16 serverIndex)
        {            
            RoomPacketHandler.Init(serverIndex);
            RoomPacketHandler.SetRoomList(RoomList);
            RoomPacketHandler.RegistPacketHandler(PacketHandlerMap);
        }

        void Process()
        {
            var packetHeader = new PKTBinaryHead();
            var mqHeader = new MQBinaryHeader();

            while (IsThreadRunning)
            {
                try
                {
                    var packet = MsgBuffer.Receive();
                    if(packet == null)
                    {
                        break;
                    }

                    mqHeader.HeaderDecode(packet.Data);
                    var packetId = mqHeader.PacketId;

                    if (packetId == (UInt16)MQ_GATECHAT_DATA_ID.RELAY)
                    {
                        //릴레이 패킷의 데이터 포맷은 msgpack으로 되어 있다고 가정한다.
                        // msgpack의 고유 헤더가 3바이트이다.
                        packetHeader.HeaderDeCode(packet.Data, ServerPacketData.MQHeaderSize);
                        packetId = packetHeader.PacketID;
                    }
                    
                    if (PacketHandlerMap.ContainsKey(packetId))
                    {
                        PacketHandlerMap[packetId](packet);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"세션 번호: {packet.UserNetSessionIndex}, PacketID: {packetId}, 받은 데이터 크기: {packet.Data.Length}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


    }
}
