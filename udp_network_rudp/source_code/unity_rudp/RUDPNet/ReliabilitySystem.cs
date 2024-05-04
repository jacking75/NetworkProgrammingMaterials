using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet;

public class ReliabilitySystem
{
    uint max_sequence;
    uint local_sequence;
    uint remote_sequence;

    uint sent_packets;
    uint recv_packets;
    uint lost_packets;
    uint acked_packets;

    float sent_bandwidth;
    float acked_bandwidth;
    float rtt;
    float rtt_maximum;

    List<uint> acks;

    PacketQueue sentQueue = new PacketQueue();
    PacketQueue pendingAckQueue = new PacketQueue();
    PacketQueue receivedQueue = new PacketQueue();
    PacketQueue ackedQueue = new PacketQueue();


    public ReliabilitySystem(uint max_sequence = 0xffffffff)
    {
        this.max_sequence = max_sequence;
        Reset();
    }

    public void Reset()
    {
        local_sequence = 0;
        remote_sequence = 0;
        sentQueue.Clear();
        receivedQueue.Clear();
        pendingAckQueue.Clear();
        ackedQueue.Clear();
        sent_packets = 0;
        recv_packets = 0;
        lost_packets = 0;
        acked_packets = 0;
        sent_bandwidth = 0.0f;
        acked_bandwidth = 0.0f;
        rtt = 0.0f;
        rtt_maximum = 0.0f;
    }
    
    public void PacketSent(int size)
    {
        if (sentQueue.Exists(local_sequence))
        {
            //Debug.Log("local sequence exists " + local_sequence);
        }

        PacketData data;
        data.Sequence = local_sequence;
        data.Size = size;
        data.Time = 0.0f;
        
        sentQueue.AddLast(data);
        pendingAckQueue.AddLast(data);
        
        sent_packets++;
        local_sequence++;
        
        if (local_sequence > max_sequence)
        {
            local_sequence = 0;
        }
    }

    static bool SequenceMoreRecent(uint s1, uint s2, uint max_sequence)
    {
        return ((s1 > s2) && (s1 - s2 <= max_sequence / 2)) || ((s2 > s1) && (s2 - s1 > max_sequence / 2));
    }

    public void PacketReceived(uint sequence, int size)
    {
        recv_packets++;
        
        if (receivedQueue.Exists(sequence))
        {
            return;
        }

        PacketData data;
        data.Sequence = sequence;
        data.Size = size;
        data.Time = 0.0f;
        receivedQueue.AddLast(data);
        
        if (SequenceMoreRecent(sequence, remote_sequence, max_sequence))
        {
            remote_sequence = sequence;
        }
    }

    public uint GenerateAckBits()
    {
        return generate_ack_bits(GetRemoteSequence(), ref receivedQueue, max_sequence);
    }
    public void ProcessAck(uint ack, uint ack_bits)
    {
        process_ack(ack, ack_bits, ref pendingAckQueue, ref ackedQueue, ref acks, acked_packets, rtt, max_sequence);
    }
    
    public void Update(float deltaTime)
    {
        acks.Clear();
        AdvanceQueueTime(deltaTime);
        UpdateQueues();
        UpdateStatus();
    }
    
    public static uint bit_index_for_sequence(uint sequence, uint ack, uint max_sequence)
    {
        if (sequence > ack)
        {
            return ack + (max_sequence - sequence);
        }
        else
        {
            return ack - 1 - sequence;
        }
    }
    
    public static uint generate_ack_bits(uint ack, ref PacketQueue received_queue, uint max_sequence)
    {
        uint ack_bits = 0;
        foreach (PacketData received in received_queue)
        {
            if (received.Sequence == ack || SequenceMoreRecent(received.Sequence, ack, max_sequence))
            {
                break;
            }

            uint bit_index = bit_index_for_sequence(received.Sequence, ack, max_sequence);
            if (bit_index <= 31)
            {
                ack_bits |= 1U << (int)bit_index;
            }
        }
        return ack_bits;
    }
    
    public static void process_ack(uint ack, uint ack_bits, ref PacketQueue pending_ack_queue,
                                    ref PacketQueue acked_queue, ref List<uint> acks,
                                    uint acked_packets, float rtt, uint max_sequence)
    {
        if (pending_ack_queue.Count == 0)
        {
            return;
        }
        
        var node = pending_ack_queue.First;
        while (node != null)
        {
            var nextnode = node.Next;

            bool acked = false;
            if (node.Value.Sequence == ack)
            {
                acked = true;
            }
            else if (!SequenceMoreRecent(node.Value.Sequence, ack, max_sequence))
            {
                uint bit_index = bit_index_for_sequence(node.Value.Sequence, ack, max_sequence);
                if (bit_index <= 31)
                {
                    acked = System.Convert.ToBoolean((ack_bits >> (int)bit_index) & 1);
                }
            }
            if (acked)
            {
                rtt += (node.Value.Time - rtt) * 0.1f;

                acked_queue.InsertSorted(node.Value, max_sequence);
                acks.Add(node.Value.Sequence);
                acked_packets++;
                pending_ack_queue.Remove(node);
                node = nextnode;
            }
            else
            {
                node = nextnode;
            }
        }

    }

    public uint GetLocalSequence()
    {
        return local_sequence;
    }

    public uint GetRemoteSequence()
    {
        return remote_sequence;
    }
    
    public uint GetMaxSequence()
    {
        return max_sequence;
    }
    
    public void GetAcks()
    {
    }

    public uint GetSentPackets()
    {
        return sent_packets;
    }
    public uint GetReceivePackets()
    {
        return recv_packets;
    }
    public uint GetLostPackets()
    {
        return lost_packets;
    }
    public uint GetAckedPackets()
    {
        return acked_packets;
    }
    public float GetSentBandwidth()
    {
        return sent_bandwidth;
    }
    public float GetAckedBandwidth()
    {
        return acked_bandwidth;
    }

    public float GetRoundTripTime()
    {
        return rtt;
    }
    
    public int GetHeaderSize()
    {
        return 12;
    }
    
    protected void AdvanceQueueTime(float deltaTime)
    {
        PacketData data;

        for (var node = sentQueue.First; node != sentQueue.Last.Next; node = node.Next)
        {
            data.Sequence = node.Value.Sequence;
            data.Size = node.Value.Size;
            data.Time = node.Value.Time + deltaTime;
            node.Value = data;
        }
        
        for (LinkedListNode<PacketData> node = receivedQueue.First; node != receivedQueue.Last.Next; node = node.Next)
        {
            data.Sequence = node.Value.Sequence;
            data.Size = node.Value.Size;
            data.Time = node.Value.Time + deltaTime;
            node.Value = data;
        }
        
        for (LinkedListNode<PacketData> node = pendingAckQueue.First; node != pendingAckQueue.Last.Next; node = node.Next)
        {
            data.Sequence = node.Value.Sequence;
            data.Size = node.Value.Size;
            data.Time = node.Value.Time + deltaTime;
            node.Value = data;
        }
        
        for (LinkedListNode<PacketData> node = ackedQueue.First; node != ackedQueue.Last.Next; node = node.Next)
        {
            data.Sequence = node.Value.Sequence;
            data.Size = node.Value.Size;
            data.Time = node.Value.Time + deltaTime;
            node.Value = data;
        }
    }

    protected void UpdateQueues()
    {
        const float epsilon = 0.001f;
        
        while ((sentQueue.Count > 0) && 
                (sentQueue.First.Value.Time > (rtt_maximum + epsilon))
                )
        {
            sentQueue.RemoveFirst();
        }

        if (receivedQueue.Count > 0)
        {
            uint latest_sequence = receivedQueue.Last.Value.Sequence;
            uint minimum_sequence = latest_sequence >= 34 ? (latest_sequence - 34) : max_sequence - (34 - latest_sequence);
            
            while ((receivedQueue.Count > 0) && !SequenceMoreRecent(receivedQueue.First.Value.Sequence, minimum_sequence, max_sequence))
            {
                receivedQueue.RemoveFirst();
            }
        }

        while (ackedQueue.Count > 0 && ackedQueue.First.Value.Time > rtt_maximum * 2 - epsilon)
        {
            ackedQueue.RemoveFirst();
        }
        
        while (pendingAckQueue.Count > 0 && pendingAckQueue.First.Value.Time > rtt_maximum + epsilon)
        {
            pendingAckQueue.RemoveFirst();
            lost_packets++;
        }
    }

    protected void UpdateStatus()
    {
        int sent_bytes_per_second = 0;
        foreach (PacketData data in sentQueue)
        {
            sent_bytes_per_second += data.Size;
        }
        
        int acked_packets_per_second = 0;
        int acked_bytes_per_second = 0;
        foreach (PacketData data in ackedQueue)
        {
            if (data.Time >= rtt_maximum)
            {
                acked_packets_per_second++;
                acked_bytes_per_second += data.Size;
            }
        }
        
        sent_bytes_per_second = (int)(sent_bytes_per_second / rtt_maximum);
        acked_bytes_per_second = (int)(acked_bytes_per_second / rtt_maximum);
        sent_bandwidth = sent_bytes_per_second * (8 / 1000.0f);
        acked_bandwidth = acked_bytes_per_second * (8 / 1000.0f);
    }
    
}
