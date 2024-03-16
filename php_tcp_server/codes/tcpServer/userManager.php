<?php

class User
{
    public int $SessionID;
    public string $UserID;
    public int $RoomNumber;
}

class UserManager
{
    private $UserMapSessionID = [];
    private $UserMapID = [];

    public function AddUser(int $sessionID, string $userID) : bool
    {
        if(empty($this->UserMapSessionID[$sessionID]) == false)
        {
            return false;
        }

        if(empty($this->UserMapID[$userID]) == false)
        {
            return false;
        }

        $user = new User();
        $user->SessionID = $sessionID;
        $user->UserID = $userID;
        $user->RoomNumber = -1;

        $this->UserMapSessionID[$sessionID] = $user;
        $this->UserMapID[$userID] = $user;
        return true;
    }

    public function RemoveUser(int $sessionID)
    {
        $user = $this->GetUser($sessionID);
        if($user == null) {
            return;
        }

        unset($this->UserMapSessionID[$sessionID]);
        unset($this->UserMapID[$user->UserID]);
    }


    public function GetUser(int $sessionID) : ?User
    {
        if(empty($this->UserMapSessionID[$sessionID]))
        {
            return null;
        }

        return $this->UserMapSessionID[$sessionID];
    }
}
