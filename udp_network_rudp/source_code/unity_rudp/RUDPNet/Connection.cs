using System.Collections.Generic;
using System.Net;

namespace RUDPNet
{

    public class Connection
    {
        
        Mode _mode;
        uint _protocolID;
        float _timeout;
        bool _running;
    
        State _state;
        float _timeoutAccumulator;
        Socket _socket = new Socket();
        System.Net.IPEndPoint _address = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1912);

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnConnect() { }
        protected virtual void OnDisconnect() { }


        void ClearData()
        {
            _state = State.Disconnected;
            _timeoutAccumulator = 0.0f;
        }

        
        public Connection(uint pID, float to)
        {
            this._protocolID = pID;
            this._timeout = to;
            _mode = Mode.None;
            _running = false;
            
            ClearData();
        }

        public bool Start(ushort port)
        {
            //Debug.Log("start connection on port " + port);
            if (!_socket.Open(port))
            {
                return false;
            }

            _running = true;
            OnStart();
            return true;
        }

        public void Stop()
        {
            //Debug.Log("stop connection");
            bool connected = IsConnected();
            
            ClearData();
            _socket.Close();
            _running = false;

            if (connected)
            {
                OnDisconnect();
            }

            OnStop();
        }

        public bool IsRunning()
        {
            return _running;
        }
        
        public void Listen()
        {
            //Debug.Log("server listening for connection");
            bool connected = IsConnected();
            
            ClearData();
            
            if (connected)
            {
                OnDisconnect();
            }

            _mode = Mode.Server;
            _state = State.Listening;
        }

        public void Connect(ref System.Net.IPEndPoint addr)
        {
            //print("client connecting to %hhu.%hhu.%hhu.%hhu:%d\n",
            //        addr.GetA(), addr.GetB(), addr.GetC(), addr.GetD(), addr.GetPort());
            bool connected = IsConnected();
            ClearData();

            if (connected)
            {
                OnDisconnect();
            }

            _mode = Mode.Client;
            _state = State.Connecting;
            _address = addr;
            //Debug.Log("(0)address = " + address.ToString());
        }

        public bool IsConnecting()
        {
            return _state == State.Connecting;
        }

        public bool ConnectFailed()
        {
            return _state == State.ConnectFail;
        }

        public bool IsConnected()
        {
            return _state == State.Connected;
        }

        public bool IsListening()
        {
            return _state == State.Listening;
        }

        public Mode GetMode()
        {
            return _mode;
        }

        public virtual void Update(float deltaTime)
        {
            _timeoutAccumulator += deltaTime;

            if (_timeoutAccumulator <= _timeout)
            {
                return;
            }
                
            
            if (_state == State.Connecting)
            {
                //Debug.Log("connect timed out");
                ClearData();
                _state = State.ConnectFail;
                OnDisconnect();
            }
            else if (_state == State.Connected)
            {
                //Debug.Log("connnection timed out");
                ClearData();
                if (_state == State.Connecting)
                {
                    _state = State.ConnectFail;
                }

                OnDisconnect();
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = System.BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
        
        public virtual bool SendPacket(byte[] data, int size)
        {
            byte[] packet = new byte[size + 4];
            HelperFunc.WriteInteger(ref data, _protocolID, 0);            
            System.Buffer.BlockCopy(data, 0, packet, 4, data.Length);

            //Debug.Log(ByteArrayToString(packet));
            //Debug.Log("IP = " + address.ToString());
            return _socket.Send(ref _address, packet, size + 4);

        }

        public virtual int ReceivePacket(ref byte[] data, int size)
        {            
            var remoteIpEp = new IPEndPoint(IPAddress.Any, 0);
            var remoteEp = (EndPoint)remoteIpEp;

            byte[] packet = new byte[size + UDPPacketMeta.HeaderSize];

            int count = _socket.Receive(ref remoteEp, packet, size + 4);
            if (count == 0)
            {
                return 0;
            }
            
            if (count <= 4)
            {
                return 0;
            }

            UInt32 recvProtocolID = 0;
            HelperFunc.ReadInteger(ref packet, ref recvProtocolID, 0);
            if ( _protocolID != recvProtocolID)
            {
                return 0;
            }
            
            if (_mode == Mode.Server && !IsConnected())
            {
                //Debug.Log("server accepts connection from client ");
                _state = State.Connected;
                _address = remoteIpEp;
                OnConnect();
            }
            
            // TODO
            if (_address.Equals(remoteIpEp))
            {
                if (_state == State.Connecting)
                {
                    _state = State.Connected;
                    OnConnect();
                }

                var header = UDPPacketMeta.HeaderSize;
                _timeoutAccumulator = 0.0f;
                System.Buffer.BlockCopy(packet, header, data, 0, count - header);
                return count - header;
            }
            return 0;
        }
        
        public virtual int GetHeaderSize()
        {
            return UDPPacketMeta.HeaderSize;
        }
    }

    





}