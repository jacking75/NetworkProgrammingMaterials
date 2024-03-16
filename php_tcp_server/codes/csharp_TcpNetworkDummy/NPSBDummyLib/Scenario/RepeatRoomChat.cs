using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public class RepeatRoomChat : ScenarioBase
    {
        public override async Task<bool> TaskAsync(Dummy dummy, TestConfig config)
        {
            bool isSuccess = false;
            config.ScenarioName = "Repeat Echo";

            var testStartTime = DateTime.Now;

            dummy.StartScenario();

            var roomNumber = EnterRoomNumber(RoomBaseInfo.MaxUserCount, dummy.Number);
            var enterRet = await RequestAndPacketProcess.MTConnetToRoomEnter(dummy, roomNumber);
            if (enterRet.Ret == false)
            {
                dummy.SetScenarioResult(false, $"Fail - [MTConnetToRoomEnter] {enterRet.ErrStr}");
                return isSuccess;
            }


            while (DummyManager.InProgress)
            {
                if (dummy.IsRun == false)
                {
                    return isSuccess;
                }

                dummy.SetScenariActionMaxWaitTime(Utils.CurrentTimeSec() + 8);
                
                
                var chatRet = await RequestAndPacketProcess.RoomChatAsync(dummy);
                if (chatRet.Ret == false)
                {
                    dummy.SetScenarioResult(false, $"Fail - [RoomChat] {chatRet.ErrStr}");
                    break;
                }

                DummyManager.AddDummyIteration();

                var elapsedTime = DateTime.Now - testStartTime;
                if (Utils.IsCompletedRepeatTask(ref dummy.CurActionRepeatCount, config, elapsedTime.TotalSeconds))
                {
                    isSuccess = true;
                    break;
                }
            }


            dummy.DisConnect();

            if (isSuccess)
            {
                dummy.SetScenarioResult(true, "Success");
            }

            return isSuccess;
        }


        int EnterRoomNumber(int maxRoomUserNum, int dummyNuber)
        {
            // 풀방으로 채운다
            var roomNum = dummyNuber / maxRoomUserNum;
            return roomNum;
        }

    }
}
