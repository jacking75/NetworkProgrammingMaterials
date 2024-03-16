using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    // 시스템 패킷은 1 ~ 100
    public enum SYS_PACKET_ID : int
    {
        NTF_IN_CONNECT_CLIENT = 11,
        NTF_IN_DISCONNECT_CLIENT = 12,
    }


    // 서버에서 사용하는 패킷 100 ~ 200
    public enum LB_PACKET_ID : int
    {
        NTF_IN_LOBBY_LEAVE = 101,
    }

    

    
    
}
