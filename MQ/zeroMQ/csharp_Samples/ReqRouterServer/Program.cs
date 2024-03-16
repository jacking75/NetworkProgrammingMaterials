using System;
using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;


namespace ReqRouterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Init Server");

            var server = new RouterSocket("@tcp://localhost:11021");

            while(true)
            {
                List<List<string>> msgList = new List<List<string>>();

                for (int i = 0; i < 3; ++i)
                {
                    var msg = server.ReceiveMultipartStrings();
                    msgList.Add(msg);
                }


                server.SendMoreFrame(msgList[2][0]);
                server.SendMoreFrame("");
                server.SendFrame(msgList[2][2]);

                server.SendMoreFrame(msgList[0][0]);
                server.SendMoreFrame("");
                server.SendFrame(msgList[0][2]);

                server.SendMoreFrame(msgList[1][0]);
                server.SendMoreFrame("");
                server.SendFrame(msgList[1][2]);
            }
        }
    }
}
