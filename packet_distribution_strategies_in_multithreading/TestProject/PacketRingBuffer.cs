using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject;

public class PacketRingBuffer
{
    byte[] _buffer;


    void Init(Int32 capacity)
    {
        _buffer = new byte[capacity];
    }
}
