using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Scenario
{
    public abstract class ScenarioBase
    {
        public abstract Task<bool> TaskAsync(Dummy dummy, TestConfig config);
    }

    public class OnlyConnect : ScenarioBase
    {        
        public override async Task<bool> TaskAsync(Dummy dummy, TestConfig config)
        {
            bool IsSuccess = false;
            config.ScenarioName = "OnlyConnect";
            
            var testStartTime = DateTime.Now;
            dummy.StartScenario();

            var result = await dummy.ConnectAsyncAndReTry();
            if (result.Result == false)
            {
                dummy.SetScenarioResult(false, "Fail");
            }
            else
            {
                IsSuccess = true;
                dummy.Connected();
                dummy.SetScenarioResult(true, "Success");

                DummyManager.AddDummyIteration();
            }

            return IsSuccess;
        }
    }
}
