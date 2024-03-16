<?php

// 101 ~ 199까지
abstract class RedisJobID
{
    const REQ_LOGIN = 111;
    const RES_LOGIN = 112;

    const NTF_GAME_RESULT = 115;
}

class LoginRedisJobReq
{
    public $JobID;
    public $SID; // 관련 유저의 세션 인덱스
    public $UserID;
    public $UserPW;
}

class LoginRedisJobRes
{
    public $JobID;
    public $SID;
    public $UserID;
    public $Ret;
}

