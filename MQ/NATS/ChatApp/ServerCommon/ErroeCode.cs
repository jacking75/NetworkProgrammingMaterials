using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    // 0 ~ 9999
    public enum ERROR_CODE : Int16
    {
        NONE = 0, // 에러가 아니다

        // 서버 초기화 에라
        REDIS_INIT_FAIL = 1,    // Redis 초기화 에러

        // 로그인 
        LOGIN_INVALID_AUTHTOKEN = 1001, // 로그인 실패: 잘못된 인증 토큰
        REMOVE_USER_SEARCH_FAILURE_USER_ID = 1003,
        USER_AUTH_SEARCH_FAILURE_USER_ID = 1004,
        USER_AUTH_ALREADY_SET_AUTH = 1005,
        LOGIN_PRE_AUTHENTICATING = 1006,
        LOGIN_FULL_USER_COUNT = 1007,
        ADD_USER_NET_SESSION_ID_DUPLICATION = 1008,
        //ADD_USER_ID_DUPLICATION = 1009,

        DB_LOGIN_INVALID_PASSWORD = 1011,
        DB_LOGIN_EMPTY_USER = 1012,
        DB_LOGIN_EXCEPTION = 1013,

        LOBBY_ENTER_INVALID_STATE = 1021,
        LOBBY_ENTER_INVALID_USER = 1022,
        ROOM_ENTER_ERROR_SYSTEM = 1023,
        LOBBY_ENTER_INVALID_ROOM_NUMBER = 1024,
        LOBBY_ENTER_FAIL_ADD_USER = 1025,
    }   

    
    
}
