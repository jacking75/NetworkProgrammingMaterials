<?php
require_once("userManager.php");

class Room
{
    public $SendPacketFunc;

    const MaxUserCount = 4;

    private int $Index;
    private int $Number;
    private $RoomUsers = [];

    public function __construct(int $index, int $number)
    {
        $this->Index = $index;
        $this->Number = $number;
    }


    function AddUser(User &$user) : int
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

    function RemoveUser(int $sessionID)
    {
        $user = $this->GetUser($sessionID);
        if($user != null) {
            $user->RoomNumber = 0;
            unset($this->RoomUsers[$sessionID]);
        }
    }


    function BroadCastPacket(int $exceptUserSessionID, int $packetLen, string &$packet)
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


    function GetUser(int $sessionID) : ?User
    {
        if(empty($this->RoomUsers[$sessionID]))
        {
            return null;
        }

        return $this->RoomUsers[$sessionID];
    }
}