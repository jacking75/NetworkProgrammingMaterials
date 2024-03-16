using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{
    public class MQRoutingKeyReceiver
    {
        //https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html

        RabbitMQ.Client.IConnection Connection = null;
        RabbitMQ.Client.IModel Channel = null;

        string ExchangeName;

        public Action<string, byte[]> ReceivedMQData;

        public void Init(string ip, string exchangeName, List<string> routingKeyList)
        {
            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = ip };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

            ExchangeName = exchangeName;

            var consumer = new EventingBasicConsumer(Channel);

            // 1개의 워스스레드로 Received 이벤트가 호출된다.
            consumer.Received += ReceiveEvent;
  
            // 라우팅 키 별로 queue를 만든다
            foreach (var severity in routingKeyList)
            {
                var queueName = Channel.QueueDeclare().QueueName;
                Channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: severity);

                Channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

                Console.WriteLine($"ExchangeName:{exchangeName}, routingKey:{severity}");
            }

            // 아래는 Queue 하나 만들어서 같이 사용한다
            //var queueName = Channel.QueueDeclare().QueueName; 
            
            //foreach (var severity in routingKeyList)
            //{
            //    Channel.QueueBind(queue: (queueName + i),
            //                      exchange: exchangeName,
            //                      routingKey: severity);
            //}

            //var consumer = new EventingBasicConsumer(Channel);
            //consumer.Received += ReceiveEvent;

            //Channel.BasicConsume(queue: queueName,
            //                     autoAck: true,
            //                     consumer: consumer);
        }

        //비동기로 호출된다
        private void ReceiveEvent(object m, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var routingKey = ea.RoutingKey;

            ReceivedMQData(routingKey, body);
        }

        public void Destory()
        {
            Channel?.Dispose();
            Connection?.Dispose();            
        }

    }
}
