using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace DBServer
{
    class MqDataProcessManager
    {
        bool IsRunning = false;

        Int32 RunningThreadCount = 0;

        ConcurrentQueue<byte[]> WorkQueue = new ConcurrentQueue<byte[]>();
        List<System.Threading.Thread> ThreadList = new List<System.Threading.Thread>();

        //TODO 스레드 수만큼 DB와 Redis 객체가 있어야 한다

        MqDataProcess ReqProcess = new MqDataProcess();

        

        public void Init(UInt16 myServerIndex, int threadCount, Action<string, byte[]> mqSendFunc)
        {
            ReqProcess.Init(myServerIndex, mqSendFunc);

            for (int i = 0; i < threadCount; ++i)
            {
                ThreadList.Add(new System.Threading.Thread(this.ThreadFunc));
            }
        }

        public void Start()
        {
            IsRunning = true;

            foreach (var thread in ThreadList)
            {
                ++RunningThreadCount;
                thread.Start();
            }
        }

        public void Stop()
        {
            IsRunning = false;

            while (true)
            {
                if (RunningThreadCount == 0)
                {
                    break;
                }

                System.Threading.Thread.Yield();
           }
        }

        public void AddReqData(byte[] data)
        {
            WorkQueue.Enqueue(data);
        }


        void ThreadFunc()
        {
            while (IsRunning)
            {
                if (WorkQueue.TryDequeue(out var work))
                {
                    ReqProcess.ReqProcess(work);
                }
                else
                {
                    System.Threading.Thread.Sleep(1);
                }
            }

            System.Threading.Interlocked.Decrement(ref RunningThreadCount);
        }

        
    }
}
