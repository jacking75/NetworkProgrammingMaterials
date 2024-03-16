using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;


namespace DBServer
{
    class DBServer
    {
        MqManager MQMgr = new MqManager();
        
        MqDataProcessManager ReqWorkerMgr = new MqDataProcessManager();

        public void Init(ServerOption option)
        {
            MQMgr.Init(option.MQServerAddress,
                option.MQSubsubject, option.MQSubQueueName,
                ReceivedMQData);

            var myServerIndex = (UInt16)option.Index;
            ReqWorkerMgr.Init(myServerIndex, option.ReqWorkerThreadCount, MQMgr.SendMQ);
        }

        public void Start()
        {
            ReqWorkerMgr.Start();
        }

        public void Stop()
        {
            Console.WriteLine("Server Stop <<<<");

            ReqWorkerMgr.Stop();
            
            MQMgr.Destory();

            Console.WriteLine("Server Stop >>>");
        }


        void ReceivedMQData(byte[] data)
        {
            ReqWorkerMgr.AddReqData(data);
        }


    }
}
