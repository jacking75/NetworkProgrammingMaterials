using CSBaseLib;
using System;
using System.Text;
using Utf8Json;

namespace NPSBDummyLib
{
    public class PacketUtil
    {
        public const Int16 PACKET_HEADER_SIZE = 5;

        public static byte[] StringToBytes(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            return data;
        }

        public static string FromPacketBodyData(byte[] packetData, Int32 bodySize)
        {
            return System.Text.Encoding.UTF8.GetString(packetData, PACKET_HEADER_SIZE, bodySize);
        }
    }

    public class SendEchoPacketInfo
    {
        Random RandDataSize = new Random();
    
        public Int16 BufferSize;
        public byte[] BufferData;
        public Int16 BodySize;
        
        public void Init(int maxBodySize)
        {
            BufferData = new byte[PacketUtil.PACKET_HEADER_SIZE + maxBodySize];
        }

        public void SetData(int minBodySize, int maxBodySize)
        {
            try
            {
                var length = RandDataSize.Next(minBodySize, maxBodySize);
                var bodyData = PacketUtil.StringToBytes(Utils.RandomString(length));
                var bodySize = (Int16)bodyData.Length;


                // 패킷 전체 크기(2), 패킷id(2), 패킷타입(1), Body(padding)
                Int16 packetId = 101;

                BufferSize = (Int16)(PacketUtil.PACKET_HEADER_SIZE + bodySize);
                BodySize = bodySize;

                Buffer.BlockCopy(BitConverter.GetBytes(BufferSize), 0, BufferData, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, BufferData, 2, 2);
                Buffer.BlockCopy(bodyData, 0, BufferData, 5, bodySize);
            }
            catch(Exception ex)
            {
                Utils.Logger.Error(ex.Message);
            }
        }

        public string BodyData()
        {
            return PacketUtil.FromPacketBodyData(BufferData, BodySize);
        }
    }


    public class RecvPacketInfo
    {
        public int BufferSize;
        public byte[] RecvBuffer;
        public byte[] RemainBuffer;

        public int BodySize;
        public int RemainLength;
        

        public void Init(int maxBodySize)
        {
            BufferSize = PacketUtil.PACKET_HEADER_SIZE + maxBodySize;
            RecvBuffer = new byte[BufferSize * 2];  // 합쳐지는 것 때문에 크기를 최대 두배로 잡음
        }

        //public void Received(int recvSize)
        //{
        //    Int16 packetSize = BitConverter.ToInt16(RecvBuffer, 0);
        //    Int16 packetId = BitConverter.ToInt16(RecvBuffer, 2);

        //    BodySize = packetSize - PacketUtil.PACKET_HEADER_SIZE;
        //}

        //public string BodyData()
        //{
        //    return PacketUtil.FromPacketBodyData(RecvBuffer, BodySize);
        //}

        public void SaveRemainBuffer(int remainSize)
        {

            if (remainSize > 0)
            {
                RemainBuffer = new byte[remainSize];
                RemainLength = remainSize;
                Buffer.BlockCopy(RecvBuffer, 0, RemainBuffer, 0, remainSize);
            }
        }

        public int CombineRemainBuffer(int recvCount)
        {
            if (RemainLength > 0)
            {
                var result = recvCount + RemainLength;
                Buffer.BlockCopy(RecvBuffer, 0, RecvBuffer, RemainLength, recvCount);
                Buffer.BlockCopy(RemainBuffer, 0, RecvBuffer, 0, RemainLength);
                BufferSize += RemainLength;
                RemainLength = 0;

                return result;
            }
            return recvCount;
        }

        public void Reset()
        {
            RemainLength = 0;
        }
    }

    public class PacketToBytes
    {
        public static byte[] Make<T>(PACKETID packetID, T packet)
            where T : class
        {
            byte[] bodyData = JsonSerializer.Serialize(packet);
            byte type = 0;
            var pktID = (Int16)packetID;
            Int16 bodyDataSize = 0;
            if (bodyData != null && bodyData.Length > 2) // json 포맷이므로 클래스에 멤버가 없으면 {} 가 만들어져서 최소 2바이트 만들어진다
            {
                bodyDataSize = (Int16)bodyData.Length;
            }
            var packetSize = (Int16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

            var dataSource = new byte[packetSize];
            Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(pktID), 0, dataSource, 2, 2);
            dataSource[4] = type;

            if (bodyDataSize > 0 && bodyData != null)
            {
                Buffer.BlockCopy(bodyData, 0, dataSource, 5, bodyDataSize);
            }

            return dataSource;
        }

        //public static (int, T) ClientReceiveData<T>(int recvLength, RecvPacketInfo recvData)
        //    where T : class
        //{
        //    var packetID = BitConverter.ToInt16(recvData.RecvBuffer, 2);
        //    recvData.BufferSize = BitConverter.ToInt16(recvData.RecvBuffer, 0);
        //    recvData.BodySize = recvData.BufferSize - PacketDef.PACKET_HEADER_SIZE;

        //    var packetBody = new byte[recvData.BodySize];
        //    Buffer.BlockCopy(recvData.RecvBuffer, PacketDef.PACKET_HEADER_SIZE, packetBody, 0, recvData.BodySize);

        //    if (recvLength > recvData.BufferSize)
        //    {
        //        Buffer.BlockCopy(recvData.RecvBuffer, recvData.BufferSize, recvData.RecvBuffer, 0, recvLength - recvData.BufferSize);
        //    }

        //    var body = JsonSerializer.Deserialize<T>(packetBody);
        //    return (packetID, body);
        //}            

    }

    public class ReceivePacket
    {
        public PACKETID PktID;
        public byte[] Body;
    }
}
