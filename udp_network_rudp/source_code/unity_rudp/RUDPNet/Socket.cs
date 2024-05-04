using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet
{    
    public class Socket
    {
        System.Net.Sockets.Socket socket = null;


        public Socket() { }
        
        ~Socket() 
        {
            Close();
        }
        
        public bool Open(ushort port)
        {
            socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork,
                                                   SocketType.Dgram, 
                                                   ProtocolType.Udp);
            socket.Blocking = false;
            return true;
        }
        
        public void Close()
        {
            socket.Close();
        }
        
        public bool Send(ref System.Net.IPEndPoint ipep, byte[] data, int size)
        {
            if (socket == null)
            {
                return false;
            }

            int count = socket.SendTo(data, size, System.Net.Sockets.SocketFlags.None, ipep);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Receive(ref System.Net.EndPoint sendor, byte[] data, int size)
        {
            int count = socket.ReceiveFrom(data, ref sendor);
            return count;
        }

        
    }
}
