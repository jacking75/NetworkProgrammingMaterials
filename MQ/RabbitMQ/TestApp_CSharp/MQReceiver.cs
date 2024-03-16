using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace ServerCommon
{
    public class MQReceiver
    {
        RabbitMQ.Client.IConnection Connection = null;
        RabbitMQ.Client.IModel Channel = null;

        public string QueueName { get; private set; }

        public Action<string, byte[]> ReceivedMQData;

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

            // EventingBasicConsumer는 비동기로 동작한다
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += ReceiveEvent;

            Channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            QueueName = queueName;
        }



        //비동기로 호출된다
        private void ReceiveEvent(object m, BasicDeliverEventArgs ea)
        {
            ReceivedMQData(QueueName, ea.Body);
        }

        public void Destory()
        {
            Channel?.Dispose();
            Connection?.Dispose();
        }
    }
}
