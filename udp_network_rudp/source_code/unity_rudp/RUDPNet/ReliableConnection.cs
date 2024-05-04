using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet;

public class ReliableConnection : Connection
{
    ReliabilitySystem reliabilitySystem;


    public ReliableConnection(uint protocolId, float timeout, uint max_sequence = 0xffffffff)
        : base(protocolId, timeout)
    {
        reliabilitySystem = new ReliabilitySystem(max_sequence);
        ClearData();
    }

    public void Dispose()
    {
        if (base.IsRunning())
        {
            base.Stop();
        }
    }
        
    public bool SendPacket(byte[] data, int size)
    {
        const int header = RUDPPacketMeta.HeaderSize;
        byte[] packet = new byte[header + size];

        uint seq = reliabilitySystem.GetLocalSequence();
        uint ack = reliabilitySystem.GetRemoteSequence();
        uint ack_bits = reliabilitySystem.GenerateAckBits(); // 지금까지 받은 ack 리스트이다.
        WriteHeader(ref packet, seq, ack, ack_bits);
        
        System.Buffer.BlockCopy(data, 0, packet, header, data.Length);
        
        if (!base.SendPacket(packet, size + header))
        {
            return false;
        }
        
        reliabilitySystem.PacketSent(size);
        return true;
    }

    public override int ReceivePacket(ref byte[] data, int size)
    {
        const int header = RUDPPacketMeta.HeaderSize;
        
        if (size <= header)
        {
            return 0;
        }
        
        
        byte[] packet = new byte[header + size];
        
        int count = base.ReceivePacket(ref packet, size + header);
        if (count == 0)
        {
            return 0;
        }

        if (count <= header)
        {
            return 0;
        }


        uint packet_sequence = 0;
        uint packet_ack = 0;
        uint packet_ack_bits = 0;
        ReadHeader(ref packet, ref packet_sequence, ref packet_ack, ref packet_ack_bits);
        
        reliabilitySystem.PacketReceived(packet_sequence, count - header);
        reliabilitySystem.ProcessAck(packet_ack, packet_ack_bits);
        
        Buffer.BlockCopy(packet, header, data, 0, count - header);
        
        return count - header;
    }
            
    public void WriteHeader(ref byte[] header, uint sequence, uint ack, uint ack_bits)
    {
        HelperFunc.WriteInteger(ref header, sequence, 0);
        HelperFunc.WriteInteger(ref header, ack, 4);
        HelperFunc.WriteInteger(ref header, ack_bits, 8);
    }
            
    public void ReadHeader(ref byte[] header, ref uint sequence, ref uint ack, ref uint ack_bits)
    {
        HelperFunc.ReadInteger(ref header, ref sequence, 0);
        HelperFunc.ReadInteger(ref header, ref ack, 4);
        HelperFunc.ReadInteger(ref header, ref ack_bits, 8);
    }
    
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        reliabilitySystem.Update(deltaTime);
    }
    
    public override int GetHeaderSize()
    {
        return base.GetHeaderSize() + reliabilitySystem.GetHeaderSize();
    }
    
    public ReliabilitySystem GetReliableSystem()
    {
        return reliabilitySystem;
    }
    
    protected override void OnStop()
    {
        ClearData();
    }

    protected override void OnDisconnect()
    {
        ClearData();
    }
    
    void ClearData()
    {
        reliabilitySystem.Reset();
    }
    
}
