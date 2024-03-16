using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public class RepeatRoomInOut : ScenarioBase
    {        
        public override async Task<bool> TaskAsync(Dummy dummy, TestConfig config)
        {
            bool isSuccess = false;
            config.ScenarioName = "Repeat Echo";

            var testStartTime = DateTime.Now;

            dummy.StartScenario();

            var loginRet = await RequestAndPacketProcess.MTConnectAndLoginAsync(dummy);
            if (loginRet.Ret == false)
            {
                dummy.SetScenarioResult(false, $"Fail - [Login] {loginRet.ErrStr}");
                return isSuccess;
            }

            while (DummyManager.InProgress)
            {
                if (dummy.IsRun == false)
                {
                    return isSuccess;
                }

                dummy.SetScenariActionMaxWaitTime(Utils.CurrentTimeSec() + 8);

                var enterRet = await RequestAndPacketProcess.MTRoomEnterLeaveAsync(dummy, dummy.Number);
                if (enterRet.Ret == false)
                {
                    dummy.SetScenarioResult(false, $"Fail - {enterRet.ErrStr}");
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


    }


    class RoomBaseInfo
    {
        static public int MaxUserCount = 4;
    }
}
