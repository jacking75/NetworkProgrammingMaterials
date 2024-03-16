using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{
    // 패킷 범위는 201 ~ 301

    // GatewayServer와 DBServer간에 주고 받은 MQ 데이터의 Id 정의
    #region MQ_GATEDB_DATA_ID
    public enum MQ_GATEDB_DATA_ID : UInt16
    {
        REQ_LOGIN = 1001,
        RES_LOGIN = 1002,

        

    }
    #endregion

    // GatewayServer와 ChatServer 간에 주고 받은 MQ 데이터의 Id 정의
    #region MQ_GATECHAT_DATA_ID
    public enum MQ_GATECHAT_DATA_ID : UInt16
    {
        REQ_ROOM_ENTER = 1021,
        RES_ROOM_ENTER = 1022,

        REQ_ROOM_LEAVE = 1026,
        RES_ROOM_LEAVE = 1027,

        RELAY = 1031,
        

    }
    #endregion



}
