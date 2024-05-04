using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPTest.Network
{
    public class RUDPReceiver
    {
        public static ushort MTU = 1464;
        public uint RetransmissionInterval = 100;//in millisecond

        static readonly public int msgIdSize = 4;
        public bool reverseByte = true;
        public ushort MaxWaitingSendLength = 100;

        public UInt32 UNA { get; private set; }

        private List<RecvingPackage> _recvQueue = null;//data in stream
        private Queue<RecvingPackage> _recvBuffer = null;//wait to get
        private const int MaxRecvWindSize = 100;
        private UInt32 _lastRecvSeqNo = UInt32.MaxValue;

        private const ushort DataFrameHeaderLength = 72 / 8;
        private const ushort PublicFrameHeaderLength = 48 / 8;
                      
        private RUDPSender _sender = null;

        Action _onRUDPConnectionDisconnect = null;
        Action _rudpReset = null;


        public RUDPReceiver()
        {
            _recvQueue = new List<RecvingPackage>(MaxWaitingSendLength);//data in stream
            _recvBuffer = new Queue<RecvingPackage>();//wait to get
        }
        
        public void Init(RUDPSender sender, Action onRUDPConnectionDisconnect, Action rudpReset)
        {
            _sender = sender;
            _onRUDPConnectionDisconnect = onRUDPConnectionDisconnect;
            _rudpReset = rudpReset;
        }

        public void Clear()
        {
            _recvQueue.Clear();
            for (int i = 0; i < MaxWaitingSendLength; i++)
            {
                _recvQueue.Add(null);
            }
            _recvBuffer.Clear();

            UNA = 0;            
        }

        public RUDP_STATE ProcessQueue(byte[] rawData, int len, RUDP_STATE state)
        {
            RUDP_STATE changeState = state;

            MemoryStream msgStream = new MemoryStream(rawData);
            BinaryReader reader = new BinaryReader(msgStream);
            
            //CRC
            byte[] checksumBytes = reader.ReadBytes(2);            
            UInt16 checksum = BitConverter.ToUInt16(checksumBytes, 0);
            UInt16 calChecksum = Helper.CRC16(rawData, 2);
            
            if (checksum != calChecksum)
            {
                //Debug.LogWarning("Checksum Failed!!!!!!");
                return changeState;
            }


            //UNA
            byte[] unaBytes = reader.ReadBytes(4);
            UInt32 coUna = BitConverter.ToUInt32(unaBytes, 0);

            len -= PublicFrameHeaderLength;

            uint maxAckSeq = 0;

            while (len > 0)
            {
                //len
                byte[] lenBytes = reader.ReadBytes(2);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBytes);
                ushort currentLen = BitConverter.ToUInt16(lenBytes, 0);
                len -= (currentLen + 2);

                //Get controll bits
                byte control = reader.ReadByte();
                if (control == (byte)4)
                {
                    if (state != RUDP_STATE.ESTABLISED) {
                        return changeState;
                    }

                    //this is a data frame
                    byte[] seqDataBytes = reader.ReadBytes(4);
                    UInt32 seqData = BitConverter.ToUInt32(seqDataBytes, 0);
                    byte[] maxPieceBytes = reader.ReadBytes(2);
                    ushort maxPiece = BitConverter.ToUInt16(maxPieceBytes, 0);
                    byte[] data = reader.ReadBytes(currentLen - DataFrameHeaderLength + 2);
                    if (seqData > UNA && seqData < UNA + MaxRecvWindSize)
                    {
                        int recvQueuePos = (int)(seqData - UNA - 1);
                        if (_recvQueue[recvQueuePos] == null)
                        {
                            //replace dummy packages
                            RecvingPackage recvPackage = new RecvingPackage();
                            recvPackage.Data = data;
                            recvPackage.MaxPiece = maxPiece;
                            recvPackage.RecvingSequenceNo = seqData;
                            _recvQueue[recvQueuePos] = recvPackage;

                            //Calculate una
                            int i = 0;
                            for (; i < _recvQueue.Count; i++, UNA++)
                            {
                                if (_recvQueue[i] == null)
                                {
                                    break;
                                }
                                else
                                {
                                    _recvBuffer.Enqueue(_recvQueue[i]);
                                    _recvQueue.Add(null);
                                }
                            }
                            _recvQueue.RemoveRange(0, i);
                        }
                        _sender.SendAck(seqData, UNA);//SendAck(seqData);
                    }

                }
                else if (control == (byte)3) //ACK, FIN+ACK
                {
                    byte[] seqDataBytes = reader.ReadBytes(4);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(seqDataBytes);
                    UInt32 seqData = BitConverter.ToUInt32(seqDataBytes, 0);
                    //Debug.Log("Recv Ack SeqNo: " + seqData.ToString());
                    if (state == RUDP_STATE.ESTABLISED)
                    {
                        SendingPackage sendPackage = _sender.FindWaitAck(seqData);
                        if (sendPackage != null)
                        {
                            //if (MobaNetworkManager.Instance.pingQueue.Count > 10)
                            //{
                            //int oldPing = MobaNetworkManager.Instance.pingQueue.Dequeue();
                            //MobaNetworkManager.Instance.ping -= oldPing;
                            //}
                            int newPing = (int)(DateTime.Now - sendPackage.FirstSendTimestamp).TotalMilliseconds;
                            //MobaNetworkManager.Instance.pingQueue.Enqueue(newPing);
                            //MobaNetworkManager.Instance.ping += newPing;
                            _sender.RemoveWaitAck(sendPackage);

                            if (maxAckSeq < seqData)
                            {
                                maxAckSeq = seqData;
                            }
                        }
                    }
                    else if (state == RUDP_STATE.LAST_ACK)
                    {
                        if (seqData == _sender.CurSendSeq())
                            _rudpReset();
                    }
                }
                else if (control == (byte)2)//SYN+ACK
                {
                    byte[] seqDataBytes = reader.ReadBytes(4);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(seqDataBytes);
                    UInt32 seqData = BitConverter.ToUInt32(seqDataBytes, 0);
                    _sender.SendAck(seqData, UNA);
                    UNA++;
                    
                    changeState = RUDP_STATE.ESTABLISED;
                    state = changeState;

                    //lastTime = DateTime.Now;
                    _lastRecvSeqNo = UInt32.MaxValue;
                }
                else if (control == (byte)6)//FIN
                {
                    byte[] seqDataBytes = reader.ReadBytes(4);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(seqDataBytes);
                    _lastRecvSeqNo = BitConverter.ToUInt32(seqDataBytes, 0);
                }
                else if (control == (byte)5)//RST
                {
                    _onRUDPConnectionDisconnect();
                }
                else
                {
                    //Debug.LogError("Receive Illegal Package");
                }
            }

            if (state == RUDP_STATE.ESTABLISED)
            {
                //fastack
                _sender.ProcessFastAck(maxAckSeq);

                //process correspondance's una
                _sender.ProcessCoUna(coUna);
            }

            return changeState;
        }

        public byte[]? GetReliableMsg(ref RUDP_STATE state)
        {
            if (UNA == _lastRecvSeqNo)
            {
                _sender.SendFINACK();
                state = RUDP_STATE.LAST_ACK;
            }

            if (_recvBuffer == null)
                return null;

            //Queue is empty
            if (_recvBuffer.Count <= 0)
            {
                return null;
            }

            int MaxPiece = _recvBuffer.Peek().MaxPiece;
            //No enough pieces
            if (_recvBuffer.Count < MaxPiece)
            {
                //Debug.Log(string.Format("Not Enough Packet, Need: {0}, Queue: {1}, Assembling: {2}", package.MaxPiece, _assmblingPackages.Count, _recvQueue.Count));
                return null;
            }

            int dataLength = 0;
            List<byte[]> resultData = new List<byte[]>();
            //Debug.Log("MaxPiece: " + MaxPiece.ToString());
            for (int i = 0; i < MaxPiece; i++)
            {
                RecvingPackage apackage = _recvBuffer.Dequeue();
                /*StringBuilder sb = new StringBuilder();
                sb.Append("getPackage: ");
                for (int j = 0; j < apackage.Data.Length; j++)
                {
                    sb.Append(apackage.Data[j] + ", ");
                }
                Debug.Log(sb.ToString());*/
                resultData.Add(apackage.Data);
                dataLength += apackage.Data.Length;
            }

            byte[] ret = new byte[dataLength];
            int currentPos = 0;
            for (int i = 0; i < resultData.Count; i++)
            {
                byte[] Data = resultData[i];
                Data.CopyTo(ret, currentPos);
                currentPos += Data.Length;
            }
            return ret;
        }
    }
}
