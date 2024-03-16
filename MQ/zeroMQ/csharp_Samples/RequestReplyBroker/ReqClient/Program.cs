using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;


namespace ReqClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ReqClient");

            var taskList = new List<Task>();
            for (int i = 0; i < 7; ++i)
            {
                var task = Task.Run(() =>
                {
                    RequestToServer(i);
                });

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            Console.WriteLine("Complete~~~");
        }

        static void RequestToServer(int index)
        {
            using (var client = new RequestSocket("tcp://127.0.0.1:11021"))
            {
                var msg = new NetMQMessage();
                msg.Append("Message_" + index);
                client.SendMultipartMessage(msg);
                Console.WriteLine("Sent Message {0}", msg.Last.ConvertToString());

                var response = client.ReceiveMultipartMessage();
                Console.WriteLine("Received Message {0}", response.Last.ConvertToString());
            }
        }
    }
}
