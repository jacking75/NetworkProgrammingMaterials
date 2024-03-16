using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class ServerOption
    {
        [Option( "serverIndex", Required = true, HelpText = "Server Index")]
        public int Index { get; set; }

        [Option("name", Required = true, HelpText = "Server Name")]
        public string Name { get; set; }
                
        [Option("roomMaxCount", Required = true, HelpText = "Max Room Count")]
        public int RoomMaxCount { get; set; } = 0;

        [Option("roomMaxUserCount", Required = true, HelpText = "Max Room User Count")]
        public int RoomMaxUserCount { get; set; } = 0;

        [Option("roomStartNumber", Required = true, HelpText = "Start Room Number")]
        public int RoomStartNumber { get; set; } = 0;


        [Option("mqServerAddress", Required = true, HelpText = "MQ Server Address")]
        public string MQServerAddress { get; set; }

        [Option("subject", Required = true, HelpText = "MQ Sub Subject")]
        public string MQSubsubject { get; set; }



    }    
}
