using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{    
    // 클라이언트 - 로비 패킷 ID
    // 1 ~ 1000
    public enum CL_PACKET_ID : int
    {
        REQ_RES_TEST_ECHO = 101,
                      
        // 클라이언트
        CS_BEGIN        = 1001,

        REQ_ROOM_ENTER = 721,
        RES_ROOM_ENTER = 722,
        
        REQ_ROOM_LEAVE = 726,
        RES_ROOM_LEAVE = 727,
        
        REQ_ROOM_CHAT = 731,
        RES_ROOM_CHAT = 732,
        NTF_ROOM_CHAT = 733,

        CS_END          = 100,        
    }

    
    
}
