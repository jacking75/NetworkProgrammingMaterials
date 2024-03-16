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
    // etc.
}