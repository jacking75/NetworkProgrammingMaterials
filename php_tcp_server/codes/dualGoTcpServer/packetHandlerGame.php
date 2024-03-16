<?php
require_once("packetID.php");
require_once("../serverCommon/packet.php");
require_once("userManager.php");
require_once("room.php");
require_once("roomManager.php");

class PacketHandlerGame
{
    public $SendPacketFunc;

    public UserManager $UserMgr;
    public RoomManager $RoomMgr;


    public function SetHandler(&$handlerMap)
    {
        $handlerMap[PacketID::REQ_GAME_START] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestGameStart($sockNum, $bodyLen, $bodyData);
        };

        $handlerMap[PacketID::REQ_GAME_PLAYER_TURN_THROW_PAE] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestGamePlayerTurnThrowPae($sockNum, $bodyLen, $bodyData);
        };
    }


    // 패킷 핸들러 - 게임 시작 요청
    function RequestGameStart($sockNum, $bodyLen, &$bodyData)
    {
        //printf("[RequestGameStart] sockNum:%d\n", $sockNum);

        $room = $this->GetRoom($sockNum);
        if($room == null)
        {
            $this->SendResponseGameStart($sockNum, ErrorCode::GAME_START_INVALID_USER);
            return;
        }

        $ret = $room->EnableGameStart();
        if($ret != ErrorCode::NONE)
        {
            $this->SendResponseGameStart($sockNum, $ret);
            return $ret;
        }

        // 게임 관련 데이터를 셋팅한다
        $pvpUserIDList = $room->GetPvPUserID();
        $room->GameLogic->StartGame($pvpUserIDList[0], $pvpUserIDList[1]);

        // 모두에게 게임 시작 패킷 보내기
        $ntfPacketSt = $room->GameLogic->MakeGameStartNotifyPacket();
        $ntfBodyData = json_encode($ntfPacketSt);
        $ntfPacket = PacketDesc::MakePacket(PacketID::NTF_GAME_START, $ntfBodyData);
        $packetLen = strlen($ntfPacket);
        $room->BroadCastPacket(-1, $packetLen, $ntfPacket);

        $this->RoomMgr->AddGamingRoomIndex($room->GetIndex(), $room->GetNumber());
    }

    function SendResponseGameStart(int $sockNum, int $ret)
    {
        $response = new GameStartResErrorJsoPacket();
        $response->Ret = $ret;

        $json_data = json_encode($response);
        $resPacket = PacketDesc::MakePacket(PacketID::RES_GAME_START_ERROR, $json_data);
        $packetLen = strlen($resPacket);
        $this->SendPacketFunc[0]($sockNum, $packetLen, $resPacket);
    }



    // 패킷 핸들러 - 플레이어가 패를 던짐
    function RequestGamePlayerTurnThrowPae($sockNum, $bodyLen, &$bodyData)
    {
        //printf("[RequestGamePlayerTurnThrowPae] sockNum:%d\n", $sockNum);
        if($this->IsValidUser($sockNum) == false)
        {
            printf("[Error RequestGamePlayerTurnThrowPae - InValid User] sockNum:%d\n", $sockNum);
            return;
        }

        $user = $this->UserMgr->GetUser($sockNum);
        $room = $this->RoomMgr->GetRoom($user->RoomNumber);

        // 패를 던질 수 있나?
        $ret = $room->GameLogic->CheckPlayerTurnThrowPae($user->ID);
        if($ret != ErrorCode::NONE)
        {
            $this->SendResponseGamePlayerTurnThrowPae($sockNum, $ret);
            return;
        }

        $request = json_decode($bodyData);
        $throwPaeNum = $request->{'ThrowPaeNum'};
        $selBadagPaeNum = $request->{'SelBadagPaeNum'};

        $ret = $room->GameLogic->ProcessPlayerTurnThrowPae($throwPaeNum, $selBadagPaeNum);
        if($ret == false)
        {
            $this->SendResponseGamePlayerTurnThrowPae($sockNum, $ret);
            return;
        }
    }

    function SendResponseGamePlayerTurnThrowPae(int $sockNum, int $ret)
    {
        $response = new GamePlayerTurnThrowPaeErrorJsoPacket();
        $response->Ret = $ret;

        $json_data = json_encode($response);
        $resPacket = PacketDesc::MakePacket(PacketID::RES_GAME_PLAYER_TURN_THROW_PAE_ERROR, $json_data);
        $packetLen = strlen($resPacket);
        $this->SendPacketFunc[0]($sockNum, $packetLen, $resPacket);
    }




    ///<<<< private function ////////////////////////
    function IsValidUser(int $sockNum) : bool
    {
        $user = $this->UserMgr->GetUser($sockNum);

        if($user == null || $user->RoomNumber == -1)
        {
            return false;
        }

        $room = $this->RoomMgr->GetRoom($user->RoomNumber);
        if($room == null)
        {
            return false;
        }

        return true;
    }

    function GetUserAndRoom(int $sockNum)
    {
        $user = $this->UserMgr->GetUser($sockNum);

        if($user == null || $user->RoomNumber == -1)
        {
            return array(null, null);
        }

        $room = $this->RoomMgr->GetRoom($user->RoomNumber);

        return array($user, $room);
    }

    function GetRoom(int $sockNum) : ?Room
    {
        $user = $this->UserMgr->GetUser($sockNum);
        if($user == null || $user->RoomNumber == -1)
        {
            return null;
        }

        return $this->RoomMgr->GetRoom($user->RoomNumber);
    }
}