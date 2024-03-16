using CSBaseLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public class RequestAndPacketProcess
    {
        static public async Task<(bool Ret, string ErrStr)> MTConnectAndLoginAsync(Dummy dummy)
        {
            var connRet = await dummy.ConnectAsyncAndReTry();
            if (connRet.Result == false)
            {
                return (connRet.Result, connRet.ErrorStr);
            }

            var loginRet = await LoginAsync(dummy);
            if (loginRet.Ret == false)
            {
                return (loginRet.Ret, loginRet.ErrStr);
            }

            return (true, "");
        }

        static public async Task<(bool Ret, string ErrStr)> MTRoomEnterLeaveAsync(Dummy dummy, int roomNumber)
        {
            var enterRet = await RoomEnterAsync(dummy, dummy.Number);
            if (enterRet.Ret == false)
            {
                return enterRet;
            }

            var LeaveRet = await RoomLeaveAsync(dummy);
            if (LeaveRet.Ret == false)
            {
                return LeaveRet;
            }

            return (true, "");
        }

        static public async Task<(bool Ret, string ErrStr)> MTConnetToRoomEnter(Dummy dummy, int roomNumber)
        {
            var enterRet = await MTConnectAndLoginAsync(dummy);
            if (enterRet.Ret == false)
            {
                return enterRet;
            }

            var LeaveRet = await RoomEnterAsync(dummy, dummy.Number);
            if (LeaveRet.Ret == false)
            {
                return LeaveRet;
            }

            return (true, "");
        }
        


        static public async Task<(bool Ret, string ErrStr)> LoginAsync(Dummy dummy)
        {
            var packet = new LoginReqJsonPacket()
            {
                UserID = dummy.GetUserID(),
                UserPW = "abcde",
            };

            var sendData = PacketToBytes.Make(PACKETID.REQ_LOGIN, packet);
            var sendError = dummy.SendPacket(sendData.Length, sendData);
            if (sendError != "")
            {
                return (false, sendError);
            }

            var (ret, errorStr, packetList) = await ReceivePacketAndCheck<LoginResJsonPacket>(
                                           dummy, 2, 1, PACKETID.RES_LOGIN);
            return (ret, $"[Login - Res] {errorStr}");
        }

        static public async Task<(bool Ret, string ErrStr)> RoomEnterAsync(Dummy dummy, int roomNumber)
        {
            var reqPacket = new RoomEnterReqJsonPacket();
            reqPacket.RoomNum = roomNumber;

            var sendData = PacketToBytes.Make(PACKETID.REQ_ROOM_ENTER, reqPacket);
            var sendError = dummy.SendPacket(sendData.Length, sendData);
            if (sendError != "")
            {
                return (false, $"[RoomEnter - Request] {sendError}");
            }

            var (ret, errorStr, packetList) = await ReceivePacketAndCheck<RoomEnterResJsonPacket>(
                                           dummy, 2, 4, PACKETID.RES_ROOM_ENTER);
            return (ret, $"[RoomEnter - Res] {errorStr}");
        }


        static public async Task<(bool Ret, string ErrStr)> RoomLeaveAsync(Dummy dummy)
        {
            var temp = new RoomLeaveReqJsonPacket();

            var sendData = PacketToBytes.Make(PACKETID.REQ_ROOM_LEAVE, temp);
            var sendError = dummy.SendPacket(sendData.Length, sendData);
            if (sendError != "")
            {
                return (false, $"[RoomLeave - Request] {sendError}");
            }


            var (ret, errorStr, packetList) = await ReceivePacketAndCheck<RoomLeaveResJsonPacket>(
                                            dummy, 2, 4, PACKETID.RES_ROOM_LEAVE);
            return (ret, $"[RoomLeave - Res] {errorStr}");
        }


        static public async Task<(bool Ret, string ErrStr)> RoomChatAsync(Dummy dummy)
        {
            var ChatMessage = "dsdsd"; //TODO 랜덤한 문자로 바꾸기
            var packet = new RoomChatReqJsoPacket()
            {
                Msg = ChatMessage,
            };

            var sendData = PacketToBytes.Make(PACKETID.REQ_ROOM_CHAT, packet);
            var sendError = dummy.SendPacket(sendData.Length, sendData);
            if (sendError != "")
            {
                return (false, $"[RoomChat - Req] {sendError}");
            }

            var (ret, errorStr, packetList) = await ReceivePacketAndCheck<RoomChatResJsoPacket>(
                                            dummy, 2, 4, PACKETID.RES_ROOM_CHAT);            
            return (ret, $"[RoomChat - Res] {errorStr}");
        }



        static async Task<(bool, string, List<ReceivePacket>)> ReceivePacketAndCheck<T>(Dummy dummy, 
                                                        int waitTimeSec, 
                                                        int maxReceiveCount, 
                                                        PACKETID packetID) 
                                                            where T:ResPacketBase
        {
            var waitUntilTime = DateTime.Now.AddSeconds(waitTimeSec);

            var (recvCount, recvError, packetList) = await dummy.ReceivePacketAsync(waitUntilTime,
                                            maxReceiveCount,
                                            packetID);
            if (recvError != "")
            {
                return (false, recvError, packetList);
            }

            foreach (var packet in packetList)
            {
                if (packet.PktID == packetID)
                {
                    var body = Utf8Json.JsonSerializer.Deserialize<T>(packet.Body);
                    if ((ERROR_CODE)body.Ret == ERROR_CODE.NONE)
                    {
                        return (true, "", packetList);
                    }
                    else
                    {
                        return (false, $"{((ERROR_CODE)body.Ret).ToString()}", packetList);
                    }
                }
            }

            return (false, "Not Receive Packet", packetList);
        }


    }
}
