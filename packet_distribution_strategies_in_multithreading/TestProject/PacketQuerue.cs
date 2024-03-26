using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class PacketQuerue
    {
        ConcurrentBag<PacketData> packetDatas = new ConcurrentBag<PacketData>();

        PacketData Alloc()
        {
            if(packetDatas.TryTake(out var packetData))
            {
                return packetData;
            }

            return null;
        }

        void Free(PacketData packetData)
        {
            packetDatas.Add(packetData);
        }
    }
}
