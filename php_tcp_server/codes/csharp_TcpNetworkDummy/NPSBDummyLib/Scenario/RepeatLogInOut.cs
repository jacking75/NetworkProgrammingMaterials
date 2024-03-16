using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public class RepeatLogInOut : ScenarioBase
    {
        public override async Task<bool> TaskAsync(Dummy dummy, TestConfig config)
        {
            bool isSuccess = false;
            config.ScenarioName = "Repeat Echo";
            
            var testStartTime = DateTime.Now;

            dummy.StartScenario();
            
            while (DummyManager.InProgress)
            {
                if(dummy.IsRun == false)
                {
                    return isSuccess;
                }

                // 1 루프 사이에 최대 대기 시간을 걸어 놓도록 한다. 외부에서 이 대기 시간을 넘으면 자동 취소 시킨다
                dummy.SetScenariActionMaxWaitTime(Utils.CurrentTimeSec() + 8);


                var connRet = await dummy.ConnectAsyncAndReTry();
                if (connRet.Result == false)
                {
                    dummy.SetScenarioResult(false, $"Fail - [Connect] Code({connRet.ErrorCode}), {connRet.ErrorStr}");
                    return isSuccess;
                }

                var loginRet = await RequestAndPacketProcess.LoginAsync(dummy);
                if (loginRet.Ret == false)
                {
                    dummy.SetScenarioResult(false, $"Fail - [Login] {loginRet.ErrStr}");
                    return isSuccess;
                }

                dummy.DisConnect();

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
