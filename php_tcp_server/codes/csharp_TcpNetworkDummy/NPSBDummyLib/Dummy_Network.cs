using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Utf8Json;
using CSBaseLib;


namespace NPSBDummyLib
{
    public partial class Dummy
    {
        AsyncSocket ClientSocket; 

        Random ConnRnd = new Random((int)DateTime.Now.Ticks);

        
        public Int64 ConnectCount { get; private set; }

        public void Connected() { ++ConnectCount; }


        public async Task<(bool Result, int ErrorCode, string ErrorStr)> ConnectAsyncAndReTry()
        {
            return await ConnectAsyncAndReTry(DummyManager.Config.RmoteIP, DummyManager.Config.RemotePort);
        }
        public async Task<(bool Result, int ErrorCode, string ErrorStr)> ConnectAsyncAndReTry(string ip, int port)
        {
            var tryMaxTime = 6000; 
            var startTime = DateTime.Now;

            while(true)
            {
                var diff = DateTime.Now - startTime;
                if(diff.TotalMilliseconds >= tryMaxTime)
                {
                    return (false, 1, $"Fail Connect: Max Try");
                }

                var (result, errCode, error) = await ClientSocket.ConnectAsync(ip, port);

                if (result == false)
                {
                    // 서버와 접속 자체가 안되는 경우는 바로 실패
                    if (errCode == 10061)
                    {
                        return (false, 2, $"Fail Connect: {error}");
                    }

                    await Task.Delay(360);
                }
                else
                {
                    Connected();
                    return (result, errCode, error);
                }
            }            
        }


#pragma warning disable 1998
        public Int64 DisConnect()
        {
            Int64 currentCount = 0;
            try
            {
                if (ClientSocket.IsConnected())
                {
                    currentCount = ClientSocket.Close();
                }

            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
            }
            return currentCount;
        }
#pragma warning restore 1998    


        public string SendPacket(int bufferSize, byte[] buffer)
        {
            return ClientSocket.Send(bufferSize, buffer);
        }

        public int NextConnectWaitTimeMilliSec(int min, int max)
        {
            return ConnRnd.Next(min, max);
        }





        RecvPacketInfo RecvPacketInfo = new RecvPacketInfo();                    

        public async Task<(bool, string, List<ReceivePacket>)> ReceivePacketAsync(DateTime untilTime, 
                                                                int untilCount, 
                                                                PACKETID packetID)
        {
            var packetList = new List<ReceivePacket>();
            
            while (ClientSocket.IsConnected() || (DateTime.Now < untilTime))
            {
                await Task.Delay(1);

                // 지정 횟수 이상 패킷을 받았으면 중단한다
                if(untilCount > 0 && (packetList.Count >= untilCount))
                {
                    return (true, "", packetList);
                }

                // 지정 시간 이상 패킷을 받았으면 중단한다.
                if (DateTime.Now >= untilTime)
                {
                    if(packetList.Count == 0)
                    {
                        break;
                    }

                    return (true, "", packetList);
                }


                var (recvSize, recvError) = ClientSocket.Receive(RecvPacketInfo.BufferSize,
                                                                 RecvPacketInfo.RecvBuffer);

                if (string.IsNullOrEmpty(recvError) == false)
                {
                    return (false, recvError, new List<ReceivePacket>());
                }


                var result = RecvProc(recvSize, recvError);
                if(result.Item1 == false)
                {
                    return (result.Item1, result.Item2, result.Item3);
                }

                foreach(var packet in result.Item3)
                {
                    packetList.Add(packet);

                    // 지정한 패킷을 받았으면 중단한다
                    if(packet.PktID == packetID)
                    {
                        return (true, "", packetList);
                    }
                }
            }

            return (false, "Failed to Complete Receive Packet", packetList);
        }

        (bool, string, List<ReceivePacket>) RecvProc(int recvCount, string recvError)
        {
            var pakcetList = new List<ReceivePacket>();

            recvCount = RecvPacketInfo.CombineRemainBuffer(recvCount);
            var readBufPos = 0;
            
            while (recvCount >= PacketUtil.PACKET_HEADER_SIZE)
            {
                var packetSize = BitConverter.ToInt16(RecvPacketInfo.RecvBuffer, readBufPos);

                if(recvCount < packetSize)
                {
                    break;
                }

                if (packetSize > DummyManager.Config.PacketSizeMax)
                {
                    return (false, "RESULT_EXCEED_PACKET_SIZE", pakcetList);
                }

                var packetId = (PACKETID)BitConverter.ToInt16(RecvPacketInfo.RecvBuffer, (readBufPos+2));

                var bodySize = packetSize - PacketUtil.PACKET_HEADER_SIZE;
                byte[] bodyData = null;
                if(bodySize > 0)
                {
                    bodyData = new byte[bodySize];
                    Buffer.BlockCopy(RecvPacketInfo.RecvBuffer, (readBufPos+PacketDef.PACKET_HEADER_SIZE), bodyData, 0, bodySize);
                }
                
                pakcetList.Add(new ReceivePacket { PktID = packetId, Body = bodyData });
                
                recvCount -= packetSize;
                readBufPos += packetSize;
            }

            RecvPacketInfo.SaveRemainBuffer(recvCount);
            return (true, "", pakcetList);
        }

        
    }    
}
