using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace RUDPTest.Network
{
    public class RUDPConnection : RUDP
    {
        private Socket _socket;

        readonly byte[] _recvBuffer = new byte[MTU];

        public event Action OnDisconnect;

        HashSet<string> _socketSendErrorSet = new HashSet<string>();
        HashSet<string> _socketRecvErrorSet = new HashSet<string>();

        public override bool Send(byte[] f_data, int len)
        {
            base.Send(f_data, len);
            if (_socket == null)
                return false;
            try
            {
                /*StringBuilder sb = new StringBuilder();
                sb.Append("Send: ");
                for(int i = 0; i < len; i++)
                {
                    sb.Append(f_data[i] + ", ");
                }
                Debug.Log(sb.ToString());*/
                _socket.Send(f_data, len, SocketFlags.None);
                
            }
            catch(SocketException e)
            {
                if (!_socketSendErrorSet.Contains(e.SocketErrorCode.ToString()))
                {
                    //Debug.Log("RUDP Send Exception: " + e.SocketErrorCode.ToString());
                    _socketSendErrorSet.Add(e.SocketErrorCode.ToString());
                }

                if (e.SocketErrorCode != SocketError.WouldBlock)
                {
                    OnRUDPConnectionDisconnect();
                    return false;
                }
                    
            }
            return true;
        }

        public override void Recv(ref byte[] data, ref int len)
        {
            data = null;
            len = 0;
            if (_socket == null)
                return;
            try
            {
                int n = _socket.Receive(_recvBuffer);
                
                data = _recvBuffer;
                len = n;
            }
            catch(SocketException e)
            {
                if (!_socketRecvErrorSet.Contains(e.SocketErrorCode.ToString()))
                {
                    //Debug.Log("RUDP Recv Exception: " + e.SocketErrorCode.ToString());
                    _socketRecvErrorSet.Add(e.SocketErrorCode.ToString());
                }

                if (e.SocketErrorCode != SocketError.WouldBlock && e.SocketErrorCode != SocketError.ConnectionReset)
                {
                    OnRUDPConnectionDisconnect();
                }
            }
        }

        protected override void OnRUDPConnectionDisconnect()
        {
            base.OnRUDPConnectionDisconnect();
            OnDisconnect();
        }

        public override void Close()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        protected override void Connect(IPEndPoint remoteIp)
        {
            Close();
            Socket newSocket = null;
            
            try
            {
                newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                newSocket.Blocking = false;
                newSocket.DontFragment = true;
                //newSocket.SendBufferSize = 81920;
                //newSocket.ReceiveBufferSize = 81920;
                newSocket.Connect(remoteIp);

                _socket = newSocket;
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.WouldBlock)
                {
                    return;
                }
            }
        }

    }
}