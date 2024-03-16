<?php
require_once("room.php");
require_once("errorCode.php");

class RoomManager
{
    private int $MaxRoomCount;
    private int $StartRoomNumber;
    private $Rooms = [];
    public $SendPacketFunc;

    public function Init(int $maxRoomCount, int $roomStartNum)
    {
        $this->MaxRoomCount = $maxRoomCount;
        $this->StartRoomNumber = $roomStartNum;

        for($i = 0; $i < $maxRoomCount; ++$i)
        {
            $room = new Room($i, ($i+$roomStartNum));
            $room->SendPacketFunc = $this->SendPacketFunc;
            $this->Rooms[$i] = $room;
        }
    }


    public function EnterRoom(int $roomNumber, User &$user) : int
    {
        $room = $this->GetRoom($roomNumber);

        if($room == null)
        {
            return ErrorCode::ENTER_ROOM_INVALID_ROOM_NUMBER;
        }

        $errorCode = $room->AddUser($user);
        if($errorCode != ErrorCode::NONE)
        {
            return $errorCode;
        }

        $user->RoomNumber = $roomNumber;
        return ErrorCode::NONE;
    }

    public function LeaveRoom(bool $isNormal, int $roomNumber, $sessionIndex) : int
    {
        $room = $this->GetRoom($roomNumber);

        if($room == null)
        {
            return ErrorCode::LEAVE_ROOM_INVALID_ROOM_NUMBER;
        }

        $room->RemoveUser($sessionIndex);

        return ErrorCode::NONE;
    }


    public function BroadCastPacket(int $roomNumber, int $exceptUserSessionID, int $packetLen, string &$packet) : int
    {
        $room = $this->GetRoom($roomNumber);

        if($room == null)
        {
            return ErrorCode::CHAT_ROOM_INVALID_ROOM_NUMBER;
        }

        $room->BroadCastPacket($exceptUserSessionID, $packetLen, $packet);

        return ErrorCode::NONE;
    }



    function GetRoom(int $roomNumber) : ?Room
    {
        $index = $roomNumber - $this->StartRoomNumber;

        if( $index < 0 || $index >= $this->MaxRoomCount)
        {
            return null;
        }

        if(empty($this->Rooms[$index]))
        {
            return null;
        }

        $room = $this->Rooms[$index];
        return $room;
    }
}