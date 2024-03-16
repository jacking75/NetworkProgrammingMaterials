using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerCommon;


namespace ChatServer
{
    public class PKHandler
    {        
        protected Action<string, byte[], int, int> SendToMQ;

        public UInt16 ServerIndex = 0;

        public void Init(UInt16 serverIndex)
        {
            ServerIndex = serverIndex;
        }
        
        public void SetMq(Action<string, byte[], int, int> sendToMQFunc)
        {
            SendToMQ = sendToMQFunc;
        }
                
    }
}
