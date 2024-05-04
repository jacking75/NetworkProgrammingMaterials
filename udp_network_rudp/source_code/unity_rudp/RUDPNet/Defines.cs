using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet;

public enum Mode
{
    None,
    Server,
    Client
};

enum State
{
    Disconnected,
    Listening,
    Connecting,
    ConnectFail,
    Connected
};

public struct PacketData
{
    public uint Sequence;
    public int Size;
    public float Time;
}

public class UDPPacketMeta
{
    // protocolID(4)
    public const int HeaderSize = 4;
}

public class RUDPPacketMeta
{
    // seq(4), ack(4), ack_bits(4)
    public const int HeaderSize = 12;
}
