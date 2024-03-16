<?php
require_once("room.php");
require_once("errorCode.php");

class RoomManager
{
    private int $MaxRoomCount;
    private int $StartRoomNumber;

    private $Rooms = [];
    private $GamingRoomIndexList = [];

    public $SendPacketFunc;


    public function Init(int $maxRoomCount, int $roomStartNum)
    {
        $this->MaxRoomCount = $maxRoomCount;
        $this->StartRoomNumber = $roomStartNum;

        for($i = 0; $i < $maxRoomCount; ++$i)
        {
            $room = new Room($i, ($i+$roomStartNum));
            $room->SendPacketFunc = $this->SendPacketFunc;
            $room->Init($i, ($i+$roomStartNum));
            $this->Rooms[$i] = $room;
        }
    }

    public function Update(int $curTime)
    {
        foreach ($this->GamingRoomIndexList as $index => $number)
        {
            $this->CheckRoomState($curTime, $this->Rooms[$index]);
        }
    }

    function CheckRoomState(int $curTime, Room &$room)
    {
        $room->GameLogic->Update($curTime);
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

    public function GetRoom(int $roomNumber) : ?Room
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

    public function AddGamingRoomIndex(int $roomIndex, int $roomNum)
    {
        $this->GamingRoomIndexList[$roomIndex] = $roomNum;
    }



}