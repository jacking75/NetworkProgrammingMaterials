using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBServer
{
    class ServerOption
    {
        [Option("serverIndex", Required = true, HelpText = "Server Index")]
        public int Index { get; set; }

        [Option("name", Required = true, HelpText = "Server Name")]
        public string Name { get; set; }


        [Option("reqWorkerThreadCount", Required = true, HelpText = "Req Worker Thread Count")]
        public int ReqWorkerThreadCount { get; set; } = 0;

        [Option("dbAddres", Required = true, HelpText = "DB Addres")]
        public string DBAddres { get; set; }
                                

        [Option("mqServerAddress", Required = true, HelpText = "MQ Server Address")]
        public string MQServerAddress { get; set; }

        [Option("subject", Required = true, HelpText = "MQ Sub Subject")]
        public string MQSubsubject { get; set; }

        [Option("qGroup", Required = true, HelpText = "MQ Sub Queue Group Name")]
        public string MQSubQueueName { get; set; }

    }
}
