<?php
require_once("packetID.php");
require_once("userManager.php");
require_once("../serverCommon/packet.php");

class PacketHandlerCommon
{
    public $SendPacketFunc;
    public UserManager $UserMgr;

    public function SetHandler(&$handlerMap)
    {
        $handlerMap[PacketID::REQ_LOGIN] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestLogin($sockNum, $bodyLen, $bodyData);
        };
    }


    function RequestLogin($sockNum, $bodyLen, &$bodyData)
    {
        $request = json_decode($bodyData);
        $userID = $request->{'UserID'};

        //printf("[RequestLogin] UserID:%s, PW:%s\n", $userID, $request->{'UserPW'});

        if($this->UserMgr->AddUser($sockNum, $userID) == false)
        {
            $this->SendResponseLogin($sockNum, ErrorCode::USER_ADD_FAIL);
            return;
        }

        $this->SendResponseLogin($sockNum, ErrorCode::NONE);
    }

    function SendResponseLogin($sockNum, int $ret)
    {
        $response = new LoginResPacket();
        $response->Ret = $ret;

        $json_data = json_encode($response);
        $resPacket = PacketDesc::MakePacket(PacketID::RES_LOGIN, $json_data);
        $packetLen = strlen($resPacket);
        $this->SendPacketFunc[0]($sockNum, $packetLen, $resPacket);
    }
}