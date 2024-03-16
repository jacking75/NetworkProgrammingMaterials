using System;
using NetMQ;
using NetMQ.Sockets;


namespace ReqReqServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Init Server");

            var server = new ResponseSocket("@tcp://localhost:11021");

            while(true)
            {
                var recvMsg = server.ReceiveFrameString();

                System.Threading.Thread.Sleep(3000);

                server.SendFrame("Re: " + recvMsg);
            }
        }
    }
}
