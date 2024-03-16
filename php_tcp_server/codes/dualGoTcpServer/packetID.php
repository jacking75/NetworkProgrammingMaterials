<?php
abstract class PacketID
{
    const REQ_LOGIN = 202;
    const RES_LOGIN = 203;

    const REQ_ROOM_ENTER = 206;
    const RES_ROOM_ENTER = 207;

    const REQ_ROOM_LEAVE = 211;
    const RES_ROOM_LEAVE = 212;

    const REQ_ROOM_CHAT = 216;
    const RES_ROOM_CHAT = 217;
    const NTF_ROOM_CHAT = 218;

    const REQ_GAME_START = 231;
    const RES_GAME_START_ERROR = 232;
    const NTF_GAME_START = 233;

    const REQ_GAME_PLAYER_TURN_THROW_PAE = 236;
    const RES_GAME_PLAYER_TURN_THROW_PAE_ERROR = 237;
    const NTF_GAME_PLAYER_TURN_THROW_PAE = 238;

    const NTF_GAME_ADD_PAE_TO_TURN_PLAYER = 241; // 턴을 받은 플레이어가 패가 하나도 없어서 더미 패에서 하나 준다

    const NTF_GAME_END = 251;
}