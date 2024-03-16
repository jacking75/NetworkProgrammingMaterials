using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DBServer
{
    class MqManager
    {
        ServerCommon.MQReceiver MQReceiver = new ServerCommon.MQReceiver();
        ServerCommon.MQSender MQSender = new ServerCommon.MQSender();
                

        public void Init(string serverAddress, string recvSubject, string qGroup, 
                        Action<byte[]> receivedMQDataEvent)
        {
            MQReceiver.Init(serverAddress, recvSubject, qGroup);
            MQReceiver.ReceivedMQData = receivedMQDataEvent;

            MQSender.Init(serverAddress);
        }
                
        public void Destory()
        {
            MQReceiver.Destory();
            MQSender.Destory();
        }

        public void SendMQ(string subject, byte[] payload)
        {
            MQSender.Send(subject, payload);
        }

        
    }
}
