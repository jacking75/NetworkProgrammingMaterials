using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet
{
    public class PacketQueue : LinkedList<PacketData>
    {
        bool SequenceMoreRecent(uint s1, uint s2, uint max_sequence)
        {
            return ((s1 > s2) && (s1 - s2 <= max_sequence / 2)) || ((s2 > s1) && (s2 - s1 > max_sequence / 2));
        }

        public bool Exists(uint sequence)
        {
            foreach (PacketData pd in this)
            {
                if (pd.Sequence == sequence)
                {
                    return true;
                }
            }
            return false;
        }

        public void InsertSorted(PacketData p, uint max_sequence)
        {
            if (this.Count == 0)
            {
                this.AddLast(p);
            }
            else
            {
                if (!SequenceMoreRecent(p.Sequence, this.First.Value.Sequence, max_sequence))
                {
                    this.AddFirst(p);
                }
                else if (SequenceMoreRecent(p.Sequence, this.Last.Value.Sequence, max_sequence))
                {
                    this.AddLast(p);
                }
                else
                {
                    for (var node = this.First; node != this.Last.Next; node = node.Next)
                    {
                        if (SequenceMoreRecent(node.Value.Sequence, p.Sequence, max_sequence))
                        {
                            this.AddAfter(node, p);
                            break;
                        }
                    }
                }
            }
        }

        public void VerifySorted(uint max_sequence)
        {
            if (this.Count < 2)
            {
                //Debug.LogError("queue is too short");
            }
            for (LinkedListNode<PacketData> node = this.First; node != this.Last.Next; node = node.Next)
            {
                if (node.Next != null)
                {
                    if (SequenceMoreRecent(node.Value.Sequence, node.Next.Value.Sequence, max_sequence))
                    {
                        //Debug.LogError("verify_sorted fail!");
                    }
                }

            }
        }

    }
}
