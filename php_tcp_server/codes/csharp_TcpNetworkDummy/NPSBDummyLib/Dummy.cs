using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class Dummy
    {
        public Int32 Index { get; private set; }

        public Int32 Number { get; private set; }

        public bool IsRun { get; private set; }
                  
        public UInt64 CurActionRepeatCount = 0;

        string LastExceptionMessage;

        DummyScenarioResult ScenarioResult = new DummyScenarioResult();

        List<string> ActionHistory = new List<string>(); // 더미의 행동 이력


        public string GetUserID() => $"User_{Number}";

        public bool IsSuccessScenarioResult() => ScenarioResult.IsSuccess;

        public string GetScenarioResult() => ScenarioResult.Result;

        public void Init(Int32 index, Int32 number)
        {
            Index = index;
            Number = number;
            ClientSocket = new AsyncSocket();
            RecvPacketInfo.Init(DummyManager.Config.PacketSizeMax);
        }
                
   
        public void AddActionDesc(string desc) => ActionHistory.Add($"[{DateTime.Now}] {desc}");
  
        public void SetScenarioResult(bool isSuccess, string result)
        {
            ScenarioResult.IsSuccess = isSuccess;
            ScenarioResult.Result = result;
        }

        public void StartScenario()
        {
            ActionHistory.Clear();
            CurActionRepeatCount = 0;
            ScenariActionMaxWaitTimeSec = 0;
            IsRun = true;            
        }

        

        Int64 ScenariActionMaxWaitTimeSec = 0;

        public void SetScenariActionMaxWaitTime(Int64 timeSec) => ScenariActionMaxWaitTimeSec = timeSec;

        public bool IsOverScenariActionMaxWaitTimeThenStop(Int64 curTimeSec)
        {
            // 단순하게 변수의 값을 읽어서 체크 하는 것이라서 스레드 세이프하지 않아도 괜찮다
            if (ScenariActionMaxWaitTimeSec == 0 || curTimeSec < ScenariActionMaxWaitTimeSec)
            {
                return false;
            }

            AddActionDesc($"[실패] 시나리오 액션의 최대 대기 시간을 넘었음");
            IsRun = false;
            return true;
        }
    }
             

    public class DummyScenarioResult
    {
        public bool IsSuccess;
        public string Result;
    }
   
    
}
