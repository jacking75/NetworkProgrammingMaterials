using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class DummyManager
    {
        static public int DummyCount { get; private set; } = 0;
        public List<Dummy> DummyList { get; private set;} = new List<Dummy>();

        static public TestConfig Config { get; private set; }
         
        static public bool InProgress { get; private set; }
                
        public DateTime TestStartTime { get; private set; }

        public Action<string> LogFunc; //[진행중] [완료] [실패]

        // 전체 더미들의 반복 횟수
        static private Int64 CurDummyIterationCount = 0;
        static public Int64 CurAllDummyIterationCount() => CurDummyIterationCount;
        static public Int64 AddDummyIteration() => Interlocked.Increment(ref CurDummyIterationCount);


        static private Int64 CurrentConnectingCount = 0;
        static public Int64 ConnectedDummyCount() => CurrentConnectingCount;
        static public Int64 DummyConnected() => Interlocked.Increment(ref CurrentConnectingCount);
        static public Int64 DummyDisConnected() => Interlocked.Decrement(ref CurrentConnectingCount);

        public void SetConfigure(TestConfig config) => Config = config;

        

        public Dummy GetDummy(int index)
        {
            if (index < 0 || index >= DummyList.Count)
            {
                return null;
            }

            return DummyList[index];
        }
        
        public bool Prepare()
        {
            if(DummyManager.InProgress)
            {
                return false;
            }

            CurDummyIterationCount = 0;
            CurrentConnectingCount = 0;
            DummyList.Clear();

            var config = DummyManager.Config;
            DummyManager.DummyCount = config.DummyCount;

            for (int i = 0; i < config.DummyCount; ++i)
            {
                var dummy = new Dummy();
                dummy.Init(i, (config.DummyStartNumber + i));
                DummyList.Add(dummy);   
            }
            InProgress = true;

            StartDummyActionMaxWaitCheckThread();

            return true;
        }

        public async Task StartScenario<T>(TestConfig config) where T : Scenario.ScenarioBase, new()
        {
            SetConfigure(config);

            Prepare();

            TestStartTime = DateTime.Now;
            
            List<Task<bool>> dummyResultList = new List<Task<bool>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];
                var scenario = new T();
                dummyResultList.Add(scenario.TaskAsync(dummy, config));
            }

            await Task.WhenAll(dummyResultList.ToArray());
        }

  
        public void EndTest()
        {
            InProgress = false;

            Task.Delay(600).Wait();

            Clear();
            
            TestStartTime = new DateTime(1, 1, 1);            
        }

        void Clear()
        {
            for (int i = 0; i < DummyList.Count; ++i)
            {
                if (DummyList[i] == null)
                {
                    continue;
                }

                DummyList[i].DisConnect();
            }

            CurrentConnectingCount = 0;
        }




        Thread DummyActionMaxWaitCheckThread;

        void StartDummyActionMaxWaitCheckThread()
        {
            DummyActionMaxWaitCheckThread = new Thread(CheckAllDummyActionMaxWait);
        }

        void CheckAllDummyActionMaxWait()
        {
            while(InProgress)
            {
                var timeSec = Utils.CurrentTimeSec();

                foreach (var dummy in DummyList)
                {
                    if(InProgress == false)
                    {
                        break;
                    }

                    if(dummy.IsRun == false)
                    {
                        continue;
                    }

                    dummy.IsOverScenariActionMaxWaitTimeThenStop(timeSec);                    
                }
            }
        }
    }
}
