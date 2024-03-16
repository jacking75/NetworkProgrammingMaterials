<?php

class PacketDesc
{
    const HeaderSize = 5;
    static public int $PacketSize;
    static public int $PacketID;
    static public int $BodyDataSize;
    static public string $BodyData;

    static function Set(&$packetData, int $bufferPos)
    {
        $size = unpack("v", substr($packetData, $bufferPos, 2));
        self::$PacketSize = $size[1];
        //print "PacketSize ".self::$PacketSize."\n";

        $bufferPos += 2;
        $id = unpack("v", substr($packetData,$bufferPos,2));
        self::$PacketID = $id[1];
        //print "PacketID ".self::$PacketID."\n";

        self::$BodyDataSize = self::$PacketSize - self::HeaderSize;
        //print "BodyDataLen ".self::$BodyDataLen."\n";

        $bufferPos += (2 + 1); // 헤더는 총 5바이트인데 마지막 1바이트는 아직 미사용
        if(self::$BodyDataSize != 0)
        {
            //TODO 보디 크기만큼 복사해도 버퍼 영역을 벗어나지 않는지 조사가 필요하다
            self::$BodyData = substr($packetData, $bufferPos, self::$BodyDataSize);
        }
    }

    static public function MakePacket(int $packetID, string &$json_data) : string
    {
        $packetSize = strlen($json_data) + PacketDesc::HeaderSize;

        $packet = sprintf("%s%s%s%s", pack("v",$packetSize), pack("v", $packetID), pack("C",0), $json_data);
        return $packet;
    }
}

class Packet
{
    static public function Make(int $packetID, string &$json_data) : string
    {
        $packetSize = strlen($json_data) + PacketDesc::HeaderSize;

        $packet = sprintf("%s%s%s%s", pack("v",$packetSize), pack("v", $packetID), pack("C",0), $json_data);
        return $packet;
    }
}

class LoginReqPacket
{
    public string $UserID;
    public string $userPW;
}

class LoginResPacket
{
    public int $Ret;
}


// 방 입장
class RoomEnterReqJsonPacket
{
    public int $RoomNum;
}

class RoomEnterResJsonPacket
{
    public int $Ret;
    public int $RoomNum;
}


// 방 나가기
class RoomLeaveResJsonPacket
{
    public int $Ret;
}


// 방 채팅
class RoomChatReqJsoPacket
{
    public string $Msg;
}

class RoomChatResJsoPacket
{
    public int $Ret;
}

class RoomChatNtfJsoPacket
{
    public string $UserID;
    public string $Msg;
}



// 게임 시작 요청
class GameStartReqJsoPacket
{
}

class GameStartResErrorJsoPacket
{
    public int $Ret;
}

class GameStartNtfJsoPacket
{
    public string $TurnUserID;
    public array $P1PaeNums;
    public array $P2PaeNums;
    public array $BadagPaeNums;
}


// 플레이어 턴 - 화투 패 던짐
class GamePlayerTurnThrowPaeReqJsoPacket
{
    public int $ThrowPaeNum;
    public int $SelBadagPaeNum;
}

class GamePlayerTurnThrowPaeErrorJsoPacket
{
    public int $Ret;
}

class GamePlayerTurnThrowPaeNtfJsoPacket
{
    public int $TakePaeNumThrow = -1; //플레이어가 던진 패
    public int $TakePaeNumBadag1 = -1; //던진 패로 먹은 바닥 패
    public int $TakePaeNumNew = -1; //새로 받은 패
    public int $TakePaeNumBadag2 = -1;//새로 받은 패로 먹은 바닥 패
    public int $NewPaeNum = -1;
    public bool $IsSeolsa = false;
    public string $NextTurnUserID;
}


// 턴을 받은 플레이어가 패가 하나도 없어서 더미 패에서 하나 준다
class GameAddPaeToTurnPlayerNtfJsonPacket
{
    public int $PaeNum;
}


// 게임 끝
class GameEndNtfJsonPacket
{
    public bool $IsDraw;
    public string $WinPlayerUserID;
    public int $Score;
}