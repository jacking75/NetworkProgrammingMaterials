using System;

namespace DBServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverOption = ParseCommandLine(args);
            if (serverOption == null)
            {
                return;
            }
            
            var serverApp = new DBServer();
            serverApp.Init(serverOption);
            
            serverApp.Start();

            Console.WriteLine("Start DBServer !");
            Console.WriteLine("Press q to shut down the server");

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                {
                    Console.WriteLine("Server Stop ~~~");
                    serverApp.Stop();
                    break;
                }
                else
                {
                    Console.WriteLine($"Preessed key:{key.KeyChar}");
                }
            }
            
            Console.WriteLine("Server Terminate ~~~");
        }


        static ServerOption ParseCommandLine(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<ServerOption>(args) as CommandLine.Parsed<ServerOption>;

            if (result == null)
            {
                System.Console.WriteLine("Failed Command Line");
                return null;
            }

            return result.Value;
        }
    }
}
