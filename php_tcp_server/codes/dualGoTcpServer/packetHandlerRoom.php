<?php
require_once("packetID.php");
require_once("../serverCommon/packet.php");
require_once("userManager.php");
require_once("roomManager.php");

class PacketHandlerRoom
{
    public $SendPacketFunc;

    public UserManager $UserMgr;
    public RoomManager $RoomMgr;


    public function SetHandler(&$handlerMap)
    {
        $handlerMap[PacketID::REQ_ROOM_ENTER] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestRoomEnter($sockNum, $bodyLen, $bodyData);
        };

        $handlerMap[PacketID::REQ_ROOM_LEAVE] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestRoomLeave($sockNum, $bodyLen, $bodyData);
        };

        $handlerMap[PacketID::REQ_ROOM_CHAT] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestRoomChat($sockNum, $bodyLen, $bodyData);
        };
    }


    function RequestRoomEnter($sockNum, $bodyLen, &$bodyData)
    {
        //printf("[RequestEnterRoom] sockNum:%d\n", $sockNum);

        $user = $this->UserMgr->GetUser($sockNum);
        if($user == null)
        {
            return;
        }

        $request = json_decode($bodyData);
        $roomNum = $request->{'RoomNum'};

        if($user->RoomNumber >= 0)
        {
            $this->SendResponseRoomEnter($sockNum, ErrorCode::ENTER_ROOM_USER_ALREADY, $roomNum);
            return;
        }

        $ret = $this->RoomMgr->EnterRoom($roomNum, $user);
        $this->SendResponseRoomEnter($sockNum, $ret, $roomNum);
    }

    function SendResponseRoomEnter(int $sockNum, int $ret, int $roomNum)
    {
        $response = new RoomEnterResJsonPacket();
        $response->Ret = $ret;
        $response->RoomNum = $roomNum;

        $json_data = json_encode($response);
        $resPacket = PacketDesc::MakePacket(PacketID::RES_ROOM_ENTER, $json_data);
        $packetLen = strlen($resPacket);
        $this->SendPacketFunc[0]($sockNum, $packetLen, $resPacket);

        //printf("[SendResponseRoomEnter- end] ret:%d, sockNum:%d, roomNumber:%d\n", $ret, $sockNum, $roomNum);
    }



    function RequestRoomLeave($sockNum, $bodyLen, &$bodyData)
    {
        //printf("[RequestLeaveRoom] sockNum:%d\n", $sockNum);
        $user = $this->UserMgr->GetUser($sockNum);

        if($user == null)
        {
            printf("[RequestLeaveRoom] fail(not user) sockNum:%d\n", $sockNum);
            return;
        }

        if($user->RoomNumber < 0)
        {
            printf("[RequestLeaveRoom] fail(no room) sockNum:%d\n", $sockNum);
            return;
        }

        $ret = $this->RoomMgr->LeaveRoom(true, $user->RoomNumber, $sockNum);
        if($ret == ErrorCode::NONE) {
            $user->RoomNumber = -1;
        }

        $this->SendResponseRoomLeave($sockNum, $ret);
    }

    function SendResponseRoomLeave(int $sockNum, int $ret)
    {
        $response = new RoomLeaveResJsonPacket();
        $response->Ret = $ret;

        $json_data = json_encode($response);
        $resPacket = PacketDesc::MakePacket(PacketID::RES_ROOM_LEAVE, $json_data);
        $packetLen = strlen($resPacket);
        $this->SendPacketFunc[0]($sockNum, $packetLen, $resPacket);

        //printf("[RequestLeaveRoom - Response!] sockNum:%d\n", $sockNum);
    }



    function RequestRoomChat($sockNum, $bodyLen, &$bodyData)
    {
        printf("[RequestChatRoom] sockNum:%d\n", $sockNum);
        $user = $this->UserMgr->GetUser($sockNum);
        if($user == null || $user->RoomNumber < 0)
        {
            return;
        }

        $request = json_decode($bodyData);

        $this->SendResponseRoomChat($sockNum, ErrorCode::NONE);

        //
        $response = new RoomChatNtfJsoPacket();
        $response->UserID = $user->ID;
        $response->Msg = $request->{'Msg'};

        $json_data = json_encode($response);
        $ntfPacket = PacketDesc::MakePacket(PacketID::NTF_ROOM_CHAT, $json_data);
        $packetLen = strlen($ntfPacket);
        $this->RoomMgr->BroadCastPacket($user->RoomNumber, -1, $packetLen, $ntfPacket);
    }

    function SendResponseRoomChat(int $sockNum, int $ret)
    {
        $response = new RoomChatResJsoPacket();
        $response->Ret = $ret;

        $json_data = json_encode($response);
        $resPacket = PacketDesc::MakePacket(PacketID::RES_ROOM_CHAT, $json_data);
        $packetLen = strlen($resPacket);
        $this->SendPacketFunc[0]($sockNum, $packetLen, $resPacket);
    }
}