using System;
using NetMQ;
using NetMQ.Sockets;


namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Worker");

            const string WorkerEndpoint = "tcp://127.0.0.1:11022";

            using (var worker = new ResponseSocket())
            {
                worker.Connect(WorkerEndpoint);

                while (true)
                {
                    var msg = worker.ReceiveMultipartMessage();
                    Console.WriteLine("Processing Message {0}", msg.Last.ConvertToString());

                    System.Threading.Thread.Sleep(500);

                    worker.SendFrame(msg.Last.ConvertToString());
                }
            }
        }
    }
}
