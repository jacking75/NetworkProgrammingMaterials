using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public class RepeatEcho : ScenarioBase
    {        
        public override async Task<bool> TaskAsync(Dummy dummy, TestConfig config)
        {
            bool isSuccess = false;
            config.ScenarioName = "Repeat Echo";


            var testStartTime = DateTime.Now;
            dummy.StartScenario();

            var result = await dummy.ConnectAsyncAndReTry();
            if (result.Result == false)
            {
                dummy.SetScenarioResult(false, $"Fail - [Connect] ErrCode:{result.ErrorCode}, {result.ErrorStr}");
                return isSuccess;
            }


            var sendPacketInfo = new SendEchoPacketInfo();
            sendPacketInfo.Init(DummyManager.Config.PacketSizeMax);

            while (DummyManager.InProgress)
            {
                // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                //Utils.Logger.Debug($"Echo-Send. ClientIndex: {dummy.Index}");
                sendPacketInfo.SetData(config.EchoPacketSizeMin, config.EchoPacketSizeMax);

                var sendError = dummy.SendPacket(sendPacketInfo.BufferSize, sendPacketInfo.BufferData);
                if (sendError != "")
                {
                    dummy.SetScenarioResult(false, $"Fail - [Send] sendError");
                    return isSuccess;
                }


                // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                //Utils.Logger.Debug($"Echo-Recv. ClientIndex: {dummy.Index}");

                var waitUntilTime = DateTime.Now.AddSeconds(2);
                var (recvCount, recvError, packetList) = await dummy.ReceivePacketAsync(waitUntilTime, 1, CSBaseLib.PACKETID.PACKET_ID_ECHO);
                if (recvError != "" || packetList.Count == 0)
                {
                    dummy.SetScenarioResult(false, $"Fail - [Receive] {recvError}");
                    return isSuccess;
                }

                
                if (sendPacketInfo.BodyData() != System.Text.Encoding.UTF8.GetString(packetList[0].Body))
                {
                    dummy.SetScenarioResult(false, $"Fail - [Receive] Data received is different from sent");
                    return isSuccess;
                }

                DummyManager.AddDummyIteration();

                var elapsedTime = DateTime.Now - testStartTime;
                if (Utils.IsCompletedRepeatTask(ref dummy.CurActionRepeatCount, config, elapsedTime.TotalSeconds))
                {
                    isSuccess = true;
                    break;
                }                
            }


            if (isSuccess)
            {
                dummy.SetScenarioResult(true, "Success");
            }
            return isSuccess;
        }
               
    }    
}
