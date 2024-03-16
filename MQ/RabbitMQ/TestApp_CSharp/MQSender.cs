using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{
    public class MQSender
    {
        RabbitMQ.Client.IConnection Connection = null;
        RabbitMQ.Client.IModel Channel = null;

        public string QueueName { get; private set; }


        public void Init(string ip, string queueName)
        {
            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = ip };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: true,
                                 arguments: null);

            QueueName = queueName;
        }

        public void Destory()
        {
            Channel?.Dispose();
            Connection?.Dispose();
        }

        public void Send(byte[] data)
        {
            // BasicPublish를 호출하면 바로 소켓 write를 한다. 만약 연결이 끊어진 상태이거나 혹은 소켓의 sendBuffer가 다 찬 상태라면 대기가 발생할 수 있다.
            Channel.BasicPublish(exchange: "",
                                    routingKey: QueueName,
                                    basicProperties: null,
                                    body: data);       
        }
    }
}
