using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

// TCP 연결-끊기 상태 전이도  https://hanaldo.tistory.com/50

namespace RUDPTest.Network
{
    public class RUDP
    {
        public static ushort MTU = 1464;
        
        protected RUDP_STATE _state;
        float _stateTimer;

        private RUDPSender _sender = new();
        private RUDPReceiver _receiver = new();


        public RUDP()
        {
            _receiver.Init(_sender, OnRUDPConnectionDisconnect, RUDPReset);

            RUDPReset();
        }

        protected virtual void Connect(IPEndPoint remoteIp) { }

        public virtual bool Send(byte[] data, int len)
        {
            var una = _receiver.UNA;
            return _sender.Send(data, len, una);
        }

        public virtual void Recv(ref byte[] data, ref int len) { }
        
        public virtual void Close() { }

        protected virtual void OnRUDPConnectionDisconnect()
        {
            //Debug.Log("OnRUDPConnectionDisconnect");
            RUDPReset();
        }

        public void RUDPReset()
        {
            _sender.Clear();

            _receiver.Clear();

            _state = RUDP_STATE.CLOSED;
            _stateTimer = 0;

            Close();
        }

        public bool RUDPConnect(IPEndPoint remoteIp, byte[] cookie)
        {
            if (_state == RUDP_STATE.SYN_SEND || _state == RUDP_STATE.CLOSED) {
                if (_state == RUDP_STATE.SYN_SEND) {
                    RUDPReset();
                }

                Connect(remoteIp);
                _sender.SendSYN(cookie);
                _state = RUDP_STATE.SYN_SEND;
                _stateTimer = 0;
                return true;
            }
            else {
                return false;
            }
        }

        public void Update(float deltaTime)
        {
            byte[] rawData = null;
            int len = 0;
            
            Recv(ref rawData, ref len);
            
            while (rawData != null && len > 0)
            {
                _state = _receiver.ProcessQueue(rawData, len, _state);   
                Recv(ref rawData, ref len);
            }
            //MobaNetworkManager.Instance.waitingRecvNum = _recvQueue.Count + _recvBuffer.Count;

            var una = _receiver.UNA;
            _sender.ProcessQueue(deltaTime, _state, _stateTimer,una);
        }
       
        
        #region IsConnected
        public bool IsConnected()
        {
            return _state == RUDP_STATE.ESTABLISED;
        }
        #endregion

        #region GetMsg
        /*public MsgObject GetMsg()
        {
            byte[] thepacket = GetReliableMsg();
            if (thepacket == null)
                return null;
            return ProcessData(thepacket);
        }*/

        /*MsgObject ProcessData(byte[] thepacket)
        {
            MemoryStream msgStream = new MemoryStream(thepacket, 0, thepacket.Length);
            BinaryReader reader = new BinaryReader(msgStream, Encoding.Unicode);

            byte[] data = reader.ReadBytes(msgIdSize);
            uint msgID;
            if (reverseByte)
                msgID = NetUtilites.ByteReverse_uint(data);
            else
                msgID = BitConverter.ToUInt32(data, 0);
            data = reader.ReadBytes(thepacket.Length - msgIdSize);
            MsgObject msg = new MsgObject((OpCode)msgID, data);
            return msg;
            

        }*/

        
        #endregion

        #region SendMessage
        public void SendMessage(uint f_id, byte[] f_buf)
        {
            SendMsg((uint)f_id, f_buf);
        }

        private void SendMsg(uint f_id, byte[] f_buf)
        {
            /*byte[] data = new byte[f_buf.Length + msgIdSize];
            BitConverter.GetBytes(f_id).CopyTo(data, 0);

            if (f_buf != null)
                f_buf.CopyTo(data, msgIdSize);
            SendReliable(data);*/
        }

       

        
        #endregion

        
        
    }

    
}