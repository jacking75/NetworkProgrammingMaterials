using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpDummyClient
{
    class TestSendReceive
    {
        //public Action<string> LogFunc;

        /*
        AsyncTcpSocketClient Client = null;        
                
        public string Connect(string ip, UInt16 port)
        {
            try
            {
                if(Client != null)
                {
                    Client = null;
                }

                var config = new AsyncTcpSocketClientConfiguration();
                config.FrameBuilder = new HeadBodyFrameBuilderBuilder();

                var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

                var msgDisp = new SimpleMessageDispatcher();
                msgDisp.LogFunc = LogFunc;

                Client = new AsyncTcpSocketClient(remoteEP, msgDisp, config);
                Client.Connect().Wait();

                return $"접속 성공: IP{ip}, Port{port}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Close() { Client.Close().Wait(); }

        public void SendData(string text)
        {
            var data = MakePacket(text);
            Client.SendAsync(data);
        }

        public byte[] MakePacket(string text)
        {
            Int16 packetId = 241;
            var textLen = (Int16)Encoding.Unicode.GetBytes(text).Length;
            var bodyLen = (Int16)(textLen+2);

            var sendData = new byte[4 + 2 + textLen];
            Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bodyLen), 0, sendData, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(textLen), 0, sendData, 4, 2);
            Buffer.BlockCopy(Encoding.Unicode.GetBytes(text), 0, sendData, 6, textLen);

            return sendData;
        }


        class SimpleMessageDispatcher : IAsyncTcpSocketClientMessageDispatcher
        {
            public Action<string> LogFunc;


            public async Task OnServerConnected(AsyncTcpSocketClient client)
            {
                LogFunc($"TCP server {client.RemoteEndPoint} has connected.");
                await Task.CompletedTask;
            }

            public async Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] data, int offset, int count)
            {                
                var packet = new TcpDummyClientsLib.Utils.PacketData();
                packet.PacketID = BitConverter.ToInt16(data, offset + 0);
                packet.BodySize = BitConverter.ToInt16(data, offset + 2);
                packet.BodyData = new byte[packet.BodySize];
                Buffer.BlockCopy(data, offset + 4, packet.BodyData, 0, packet.BodySize);

                var errorCode = BitConverter.ToInt16(packet.BodyData, 0);
                var echoDataLen = BitConverter.ToInt16(packet.BodyData, 2);
                var echoData = Encoding.Unicode.GetString(packet.BodyData, 4, echoDataLen);
                LogFunc($"OnServerDataReceived: {echoData}");

                await Task.CompletedTask;
            }

            public async Task OnServerDisconnected(AsyncTcpSocketClient client)
            {
                LogFunc($"TCP server {client.RemoteEndPoint} has disconnected.");
                await Task.CompletedTask;
            }
        }
        */

    }
}
