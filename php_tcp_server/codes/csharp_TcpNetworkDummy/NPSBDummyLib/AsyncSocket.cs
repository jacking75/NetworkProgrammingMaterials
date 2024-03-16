using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class AsyncSocket
    {
        Socket Conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
       
        bool IsConnectToDisconnected = false;

        string LastExceptionMessage;


        public bool IsConnected() { return Conn != null && Conn.Connected; }
                

        public async Task<(bool,int,string)> ConnectAsync(string ip, int port)
        {
            try
            {
                Conn.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                await Conn.ConnectAsync(ip, port);
                
                Conn.NoDelay = true;
                
                DummyManager.DummyConnected();

                IsConnectToDisconnected = false;
                return (true, 0, "");
            }
            catch (Exception ex)
            {
                var sockEx = (SocketException)ex;
                LastExceptionMessage = sockEx.Message;
                return (false, sockEx.ErrorCode, sockEx.Message);
            }
        }
        
        public (int, string) Receive(int bufferSize, byte[] buffer)
        {
            // 대기조건. 시간, 원하는 패킷, 횟수
            try
            {
                if (Conn.Poll(100, SelectMode.SelectRead) == false)
                {                    
                    return (0, "");
                }

                var size = Conn.Receive(buffer);
                return (size, "");                    
            }
            catch (Exception ex)
            {
                // 더미가 끊은 것이므로 에러가 아니다
                if (IsConnectToDisconnected)
                {
                    return (0, "");
                }

                LastExceptionMessage = ex.Message;
                Close();
                return (-1, ex.Message);
            }
        }
                
        public string Send(int bufferSize, byte[] buffer)
        {
            try
            {
                Conn.Send(buffer);
                return "";
            }
            catch (Exception ex)
            {
                LastExceptionMessage = ex.Message;
                Close();
                return ex.Message;
            }            
        }

        public Int64 Close()
        {
            Int64 currentCount = 0;

            try
            {
                IsConnectToDisconnected = true;

                Conn.LingerState = new LingerOption(false, 0);
                //Conn.Close();
                Conn.Disconnect(true);
                Conn.Dispose();

                Conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch(Exception ex)
            {
                LastExceptionMessage = ex.Message;
            }
            finally
            {
                currentCount = DummyManager.DummyDisConnected();
            }

            return currentCount;
        }
    }
}
