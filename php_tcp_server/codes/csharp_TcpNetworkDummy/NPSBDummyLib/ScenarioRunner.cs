using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NPSBDummyLib
{
    public class ScenarioRunner
    {
        Thread RunThread;

        ScenarioCase RunScenario;

        TestConfig Config;

        DummyManager DummyMgr;
        Action<string> WriteLogFunc;

        
        public void Init(DummyManager dummyMgr, Action<string> logFunc)
        {
            DummyMgr = dummyMgr;
            WriteLogFunc = logFunc;
        }

        public void Start(ScenarioCase scenario, TestConfig config)
        {
            RunScenario = scenario;
            Config = config;

            RunThread = new Thread(Run);
            RunThread.Start();
        }

        void Run()
        {
            switch (RunScenario)
            {
                case ScenarioCase.OnlyConnect:
                    DummyMgr.StartScenario<NPSBDummyLib.Scenario.OnlyConnect>(Config).Wait();
                    break;
                case ScenarioCase.RepeatConnectDisconnect:
                    DummyMgr.StartScenario<NPSBDummyLib.Scenario.RepeatConnect>(Config).Wait();
                    break;
                case ScenarioCase.RepeatEcho:
                    DummyMgr.StartScenario<NPSBDummyLib.Scenario.RepeatEcho>(Config).Wait();
                    DummyMgr.EndTest();
                    break;
                case ScenarioCase.RepeatLogInOut:
                    DummyMgr.StartScenario<NPSBDummyLib.Scenario.RepeatLogInOut>(Config).Wait();
                    DummyMgr.EndTest();
                    break;
                case ScenarioCase.RepeatRoomInOut:
                    DummyMgr.StartScenario<NPSBDummyLib.Scenario.RepeatRoomInOut>(Config).Wait();
                    DummyMgr.EndTest();
                    break;
                case ScenarioCase.RepeatRoomChat:
                    DummyMgr.StartScenario<NPSBDummyLib.Scenario.RepeatRoomChat>(Config).Wait();
                    DummyMgr.EndTest();
                    break;
                default:
                    WriteLogFunc("[ERROR] Unknown Scenario");
                    break;
            }

            WriteTestResultToLog(DummyMgr);
        }

        void WriteTestResultToLog(DummyManager DummyMgr)
        {
            var testConfig = DummyManager.Config;

            UInt64 allDummyActionCount = 0;
            int failDummyCount = 0;
            int successDummyCount = 0;

            // 실패한 더미 출력하기
            foreach (var dummy in DummyMgr.DummyList)
            {
                if (dummy.IsSuccessScenarioResult() == false)
                {
                    ++failDummyCount;
                    WriteLogFunc($"Fail. Dummy Index: {dummy.Index}, ActionCount: {dummy.CurActionRepeatCount}, Error:{dummy.GetScenarioResult()}");
                }
                else
                {
                    ++successDummyCount;
                }

                allDummyActionCount += dummy.CurActionRepeatCount;
            }

            var spantTime = DateTime.Now - DummyMgr.TestStartTime;
            var testResult = $"[{DateTime.Now}][{spantTime.TotalSeconds} s][{testConfig.ScenarioName}] Success:{successDummyCount}, Fail:{failDummyCount}";
            WriteLogFunc(testResult);
            WriteLogFunc($"All Dummy Action Count: {allDummyActionCount}");
        }      
    }


    public enum ScenarioCase
    {
        NONE = 0,
        OnlyConnect = 1,
        RepeatConnectDisconnect = 2,
        RepeatEcho = 3,
        RepeatLogInOut = 5,
        RepeatRoomInOut = 6,
        RepeatRoomChat = 7,
    }
}
