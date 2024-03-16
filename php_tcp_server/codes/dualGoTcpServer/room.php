<?php
require_once("userManager.php");
require_once("gameLogic.php");



class Room
{
    public $SendPacketFunc;

    public DualGoGame $GameLogic;

    const MaxUserCount = 4;

    private int $Index;
    private int $Number;
    private $RoomUsers = [];


    public function GetIndex() { return $this->Index; }

    public function GetNumber() { return $this->Number; }


    public function Init(int $index, int $number)
    {
        $this->Index = $index;
        $this->Number = $number;
        $this->GameLogic = new DualGoGame();

        $this->GameLogic->SendPacketFunc = $this->SendPacketFunc;

        $this->GameLogic->BroadcastSendPacketFunc[0] = function (int $exceptUserSessionID, int $packetLen, string &$packet) {
            return $this->BroadCastPacket($exceptUserSessionID, $packetLen, $packet);
        };
    }

    public function AddUser(User &$user) : int
    {
        if(empty($this->RoomUsers[$user->SessionID]) == false)
        {
            return ErrorCode::ENTER_ROOM_DUP_USER;
        }

        $userCount = count($this->RoomUsers);
        if($userCount >= Room::MaxUserCount)
        {
            return ErrorCode::ENTER_ROOM_FULL_USER;
        }

        $this->RoomUsers[$user->SessionID] = $user;
        return ErrorCode::NONE;
    }

    public function RemoveUser(int $sessionID)
    {
        $user = $this->GetUser($sessionID);
        if($user != null) {
            $user->RoomNumber = 0;
            unset($this->RoomUsers[$sessionID]);
        }
    }

    public function BroadCastPacket(int $exceptUserSessionID, int $packetLen, string &$packet)
    {
        foreach($this->RoomUsers as $user) // $this->RoomUsers에는 User 객체의 참조가 들어가 있다.
        {
            if($user->SessionID == $exceptUserSessionID)
            {
                continue;
            }

            $this->SendPacketFunc[0]($user->SessionID, $packetLen, $packet);
        }
    }

    public function EnableGameStart() : int
    {
        if($this->GameLogic->EnableGameStart() == false)
        {
            return ErrorCode::GAME_START_INVALID_ROOM_STATE;
        }

        if(count($this->RoomUser) != 2)
        {
            return ErrorCode::GAME_START_INVALID_ROOM_USER_COUNT;
        }

        return ErrorCode::NONE;
    }

    public function GetPvPUserID() : array
    {
        return array($this->RoomUser[0]->ID, $this->RoomUser[1]->ID);
    }


    function GetUser(int $sessionID) : ?User
    {
        if(empty($this->RoomUsers[$sessionID]))
        {
            return null;
        }

        return $this->RoomUsers[$sessionID];
    }
}