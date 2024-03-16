using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{
    public class MQRoutingKeySender
    {
        RabbitMQ.Client.IConnection Connection = null;
        RabbitMQ.Client.IModel Channel = null;

        string ExchangeName;

        public void Init(string ip, string exchangeName)
        {
            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = ip };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();                        
            Channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

            ExchangeName = exchangeName;
        }

        public void Destory()
        {
            Channel?.Dispose();
            Connection?.Dispose();            
        }

        public void Send(string routingKey, byte[] data)
        {
            // BasicPublish를 호출하면 바로 소켓 write를 한다. 만약 연결이 끊어진 상태이거나 혹은 소켓의 sendBuffer가 다 찬 상태라면 대기가 발생할 수 있다.
            Channel.BasicPublish(exchange: ExchangeName,
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: data);
        } 


    }
}
