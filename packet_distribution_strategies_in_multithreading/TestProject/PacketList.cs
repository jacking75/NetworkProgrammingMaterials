using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject;

public class PacketList
{
    Int64 _capacity = 0;    // 좀 넉넉한 크기여야 한다
    Int64 _useCount = 0;

    List<PacketData> _packets = new List<PacketData>();


    void Init(Int64 capacity, int maxPacketSize)
    {
        _capacity = capacity;

        for(var i = 0; i < _capacity; i++)
        {
            var packet = new PacketData();
            packet.Init(maxPacketSize);

            _packets.Add(packet);
        }
    }

    // 1개의 스레드에서만 호출한다고 가정한다
    PacketData? Alloc()
    {
        if(Interlocked.Read(ref _useCount) == _capacity)
        {
            return null;
        }

        var index = _capacity % _useCount;
        var packet = _packets[(int)index];

        ++_useCount;
        return packet;
    }

    void Free(PacketData packet)
    {
        Interlocked.Decrement(ref _useCount);
    }
}
