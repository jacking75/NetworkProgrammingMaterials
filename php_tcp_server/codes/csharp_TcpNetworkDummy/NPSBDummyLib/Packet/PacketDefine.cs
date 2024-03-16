namespace CSBaseLib
{
    // 0 ~ 9999
    public enum ERROR_CODE : short
    {
        NONE = 0,

        USER_ADD_FAIL = 11,

        ENTER_ROOM_INVALID_ROOM_NUMBER = 21,
        ENTER_ROOM_DUP_USER = 22,
        ENTER_ROOM_FULL_USER = 23,

        LEAVE_ROOM_INVALID_ROOM_NUMBER = 31,
    }

    // 1 ~ 10000
    public enum PACKETID : int
    {
        PACKET_ID_ECHO = 101,
        PACKET_ID_SIMPLE_CHAT = 103,

        REQ_LOGIN = 202,
        RES_LOGIN = 203,

        REQ_ROOM_ENTER = 206,
        RES_ROOM_ENTER = 207,

        REQ_ROOM_LEAVE = 211,
        RES_ROOM_LEAVE = 212,

        REQ_ROOM_CHAT = 216,
        RES_ROOM_CHAT = 217,
        NTF_ROOM_CHAT = 218,

        CS_END = 300,
    }



}
