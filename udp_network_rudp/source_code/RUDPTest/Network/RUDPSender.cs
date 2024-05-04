using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPTest.Network
{
    public class RUDPSender
    {
        public static ushort MSS = 1100;
        public uint RetransmissionInterval = 100;//in millisecond
        public ushort MaxWaitingSendLength = 100;

        private Queue<SendingPackage> _sendQueue = new Queue<SendingPackage>();//all input waiting here
        private Queue<byte[]> _sendBuffer = new Queue<byte[]>();//data in stream
        private List<SendingPackage> _waitAckList = new List<SendingPackage>();
        private UInt32 _currentSnedSeq;
        
        byte[] SendStreamBuffer = new byte[1100]; // for final frame construction 

        const float SynReceivedTimeout = 2;
        const float LastAckTimeout = 5;
        const float AckTimeout = 10;

        private const ushort PublicFrameHeaderLength = 48 / 8;

        public Action OnRUDPConnectionDisconnect = null;
        

        public void Clear()
        {
            _sendQueue.Clear();
            _sendBuffer.Clear();
            _waitAckList.Clear();

            _currentSnedSeq = 1;
        }

        public bool Send(byte[] data, int len, UInt32 una)
        {
            byte[] unaBytes = BitConverter.GetBytes(una);
            Array.Copy(unaBytes, 0, data, 2, 4);

            //TODO C# CRCCheckSum 함수 필요
            UInt16 checksum = 0;// CRCCheck.crc16(data, 2, len);
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(checksumBytes);
            Array.Copy(checksumBytes, 0, data, 0, 2);
            return false;
        }

        public void ProcessQueue(float deltaTime, RUDP_STATE state, float stateTimer, UInt32 una)
        {
            if (state == RUDP_STATE.SYN_SEND)
            {
                stateTimer += deltaTime;
                if (stateTimer > SynReceivedTimeout)
                {
                    OnRUDPConnectionDisconnect();
                }
            }
            else if (state == RUDP_STATE.LAST_ACK)
            {
                stateTimer += deltaTime;
                if (stateTimer > LastAckTimeout)
                {
                    SendFINACK();
                    stateTimer = 0;
                }

            }
            else if (state == RUDP_STATE.ESTABLISED)
            {
                //Put available packages to waiting dict
                int readyToSendNum = 0;

                readyToSendNum = Math.Min(MaxWaitingSendLength - _waitAckList.Count, _sendQueue.Count);
                for (int i = 0; i < readyToSendNum; i++)
                {
                    SendingPackage package = _sendQueue.Dequeue();
                    _sendBuffer.Enqueue(package.Content);
                    package.LastSendTimestamp = DateTime.Now;
                    package.FirstSendTimestamp = DateTime.Now;
                    _waitAckList.Add(package);
                }

                //Re-send un-acked packages
                //string pendingList = "";
                for (int i = 0; i < _waitAckList.Count; i++)
                {
                    SendingPackage package = _waitAckList[i];
                    if ((DateTime.Now - package.FirstSendTimestamp).Seconds > AckTimeout)
                    {
                        OnRUDPConnectionDisconnect();
                        return;
                    }

                    if ((DateTime.Now - package.LastSendTimestamp).Milliseconds > RetransmissionInterval || package.fastack >= 2)
                    {
                        _sendBuffer.Enqueue(package.Content);
                        package.LastSendTimestamp = DateTime.Now;
                        package.fastack = 0;
                    }
                }
            }
            //MobaNetworkManager.Instance.waitingSendNum = _sendQueue.Count + _waitAckList.Count;

            int currentPos = PublicFrameHeaderLength;
            //actually send
            while (_sendBuffer.Count > 0)
            {
                byte[] nextSendContent = _sendBuffer.Dequeue();
                if (currentPos + nextSendContent.Length > MSS)
                {
                    Send(SendStreamBuffer, currentPos, una);
                    currentPos = PublicFrameHeaderLength;
                }
                Array.Copy(nextSendContent, 0, SendStreamBuffer, currentPos, nextSendContent.Length);
                currentPos += nextSendContent.Length;
            }
            if (currentPos > PublicFrameHeaderLength)
            {
                Send(SendStreamBuffer, currentPos, una);
            }
        }

        public void SendAck(UInt32 seqNo, UInt32 una)
        {
            if (seqNo < una)
            {
                return;
            }
            byte[] ackFrame = new byte[7];
            MemoryStream ms = new MemoryStream(ackFrame);
            BinaryWriter bw = new BinaryWriter(ms);

            byte[] lenBytes = BitConverter.GetBytes((ushort)5);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lenBytes);
            bw.Write(lenBytes);//0 len
            bw.Write((byte)3); // control 2
            byte[] seqBytes = BitConverter.GetBytes(seqNo);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(seqBytes);
            bw.Write(seqBytes); //seqNo 3

            bw.Close();
            ms.Flush();

            _sendBuffer.Enqueue(ackFrame);
        }

        public void SendSYN(byte[] cookie)
        {
            //Debug.Log("SendSYN");
            byte[] frame = new byte[3 + cookie.Length];
            MemoryStream ms = new MemoryStream(frame);
            BinaryWriter bw = new BinaryWriter(ms);
            byte[] lenBytes = BitConverter.GetBytes((ushort)(1 + cookie.Length));            
            bw.Write(lenBytes);//0 len
            bw.Write((byte)1);//control 2

            bw.Write(cookie);//cookie 3
            bw.Close();

            //Debug.Log("Send SYN: ");
            _sendBuffer.Enqueue(frame);
        }

        public void SendFINACK()
        {
            byte[] frame = new byte[7];
            MemoryStream ms = new MemoryStream(frame);
            BinaryWriter bw = new BinaryWriter(ms);
            byte[] lenBytes = BitConverter.GetBytes((ushort)(5));
            bw.Write(lenBytes);//0 len
            bw.Write((byte)7);//control 2
            byte[] AckSeqBytes = BitConverter.GetBytes(_currentSnedSeq);
            bw.Write(AckSeqBytes);//seqNo 3
            bw.Close();

            _sendBuffer.Enqueue(frame);
        }

        public SendingPackage FindWaitAck(UInt32 seqData)
        {
            return _waitAckList.Find((SendingPackage input) => input.SendingSequenceNo == seqData);
        }
        public void RemoveWaitAck(SendingPackage sendPackage)
        {
            _waitAckList.Remove(sendPackage);
        }

        public void ProcessFastAck(UInt32 maxAckSeq)
        {
            for (int i = 0; i < _waitAckList.Count; i++)
            {
                if (_waitAckList[i].SendingSequenceNo < maxAckSeq)
                {
                    _waitAckList[i].fastack++;
                }
            }
        }

        public void ProcessCoUna(uint coUna)
        {
            for (int i = _waitAckList.Count - 1; i >= 0; i--)
            {
                if (_waitAckList[i].SendingSequenceNo <= coUna)
                {
                    _waitAckList.RemoveAt(i);
                }
            }
        }

        public UInt32 CurSendSeq()
        {
            return _currentSnedSeq;
        }

        protected void SendReliable(byte[] data)
        {
            List<SendingPackage> packages = MakePacketList(data);
            for (int i = 0; i < packages.Count; i++)
            {
                _sendQueue.Enqueue(packages[i]);
            }
        }


        const int DataFrameHeaderLength = 9;

        //이전 이름: Capsule
        private List<SendingPackage> MakePacketList(byte[] data)
        {
            List<SendingPackage> ret = new List<SendingPackage>();
            
            int frameNum = data.Length / MSS;

            ushort currentPosition = 0;
            for (int i = 0; i < frameNum; i++)
            {
                var (dataLength, frame) = MakePacket((UInt16)frameNum, currentPosition, data);
               
                SendingPackage package = new SendingPackage();
                package.Content = frame;
                package.SendingSequenceNo = _currentSnedSeq;
                package.fastack = 0;
                _currentSnedSeq++;
                currentPosition += dataLength;
                ret.Add(package);
            }
            return ret;
        }

        (UInt16, byte[]) MakePacket(UInt16 frameNum, UInt16 currentPosition, byte[] data)
        {
            ushort dataLength = (ushort)Math.Min(MSS, data.Length - currentPosition);
            byte[] frame = new byte[dataLength + DataFrameHeaderLength];

            MemoryStream ms = new MemoryStream(frame);
            BinaryWriter bw = new BinaryWriter(ms);

            byte[] lenBytes = BitConverter.GetBytes((ushort)(dataLength + 8));
            bw.Write(lenBytes);//0 len
            bw.Write((byte)4);//2 control
            byte[] currentSendSeqBytes = BitConverter.GetBytes(_currentSnedSeq);
            bw.Write(currentSendSeqBytes);//3 seqNo
            byte[] maxPieceBytes = BitConverter.GetBytes(frameNum);
            bw.Write(maxPieceBytes);//7 max piece
            bw.Write(data, currentPosition, dataLength);//9 data
            bw.Close();

            return (dataLength, frame);
        }
        /*var packetSize = (UInt16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

        var dataSource = new byte[packetSize];
        Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(pktID), 0, dataSource, 2, 2);
            dataSource[4] = type;*/

    }
}
