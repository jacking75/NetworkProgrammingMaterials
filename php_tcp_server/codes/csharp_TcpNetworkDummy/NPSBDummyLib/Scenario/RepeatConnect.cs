using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public class RepeatConnect : ScenarioBase
    {
        public override async Task<bool> TaskAsync(Dummy dummy, TestConfig config)
        {
            bool IsSuccess = false;
            config.ScenarioName = "Repeat Connect-DiscConnect";

            var testStartTime = DateTime.Now;
            dummy.StartScenario();

            while (DummyManager.InProgress)
            {
                var ret = await dummy.ConnectAsyncAndReTry();
                if (ret.Result == false)
                {
                    IsSuccess = false;
                    dummy.SetScenarioResult(false, $"Fail - ErrCode:{ret.ErrorCode}, {ret.ErrorStr}");
                    break;
                }
                else
                {
                    IsSuccess = true;
                    dummy.Connected();

                    dummy.DisConnect();                    
                }

                DummyManager.AddDummyIteration();

                var waitTimeMilli = dummy.NextConnectWaitTimeMilliSec(32, 640);
                await Task.Delay(waitTimeMilli);
            }


            if (IsSuccess)
            {
                dummy.SetScenarioResult(true, "Success");
            }
            return IsSuccess;
        }
    }
}
