<?php

class User
{
    public int $SessionID;
    public string $ID = 'U';
    public bool $IsVerified = false;
    public int $RoomNumber;

    public function IsEnableLogin() : bool { return $this->ID == 'U'; }
    public function SetEnableLogin() { $this->ID = 'U'; }
}

class UserManager
{
    private $UserMapSessionID = [];
    private $UserMapID = [];

    public function AddUser(int $sessionID) : bool
    {
        if(empty($this->UserMapSessionID[$sessionID]) == false)
        {
            return false;
        }

        $user = new User();
        $user->SessionID = $sessionID;
        $user->RoomNumber = -1;

        $this->UserMapSessionID[$sessionID] = $user;
        return true;
    }

    public function VerifiedUser(User &$user)
    {
        $user->IsVerified = true;
        $this->UserMapID[$user->ID] = $user;
    }

    public function RemoveUser(int $sessionID)
    {
        $user = $this->GetUser($sessionID);
        if($user == null) {
            return;
        }

        unset($this->UserMapSessionID[$sessionID]);

        if($user->IsVerified)
        {
            unset($this->UserMapID[$user->ID]);
        }
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
