<?php
require_once("packetID.php");
require_once("userManager.php");
require_once("roomManager.php");
require_once("redisJobPacket.php");

class PacketHandlerCommon
{
    public $SendPacketFunc;
    public UserManager $UserMgr;

    public function SetHandler(&$handlerMap)
    {
        $handlerMap[PacketID::REQ_LOGIN] = function($sockNum, $bodyLen, &$bodyData) {
            $this->RequestLogin($sockNum, $bodyLen, $bodyData);
        };

        $handlerMap[RedisJobID::RES_LOGIN] = function(string $packet) {
            $this->ResponseRedisJobLogin($packet);
        };
    }


    function RequestLogin($sockNum, $bodyLen, &$bodyData)
    {
        $request = json_decode($bodyData);
        $userID = $request->{'UserID'};
        $userPW = $request->{'UserPW'};

        //printf("[RequestLogin] UserID:%s, PW:%s\n", $userID, $request->{'UserPW'});

        $user = $this->UserMgr->GetUser($sockNum);
        if($user == null || $user->IsEnableLogin() == false)
        {
            return;
        }
        else
        {
            $user->ID = $userID;
        }

        $redisJobReq = new LoginRedisJobReq();
        $redisJobReq->JobID = RedisJobID::REQ_LOGIN;
        $redisJobReq->SID = $sockNum;
        $redisJobReq->UserID = $userID;
        $redisJobReq->UserPW = $userPW;
        $redisJobReq_json_data = json_encode($redisJobReq);
        RedisJobManager::Inst()->SendRequestJob($redisJobReq_json_data);
        //$this->SendResponseLogin($sockNum, ErrorCode::NONE);
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


    function ResponseRedisJobLogin(string $packet)
    {
        $response = json_decode($packet);
        $sid = $response->{'SID'};
        $userID = $response->{'UserID'};
        $ret = $response->{'Ret'};


        $user = $this->UserMgr->GetUser($sid);
        if($user == null || $user->ID != $userID)
        {
            printf("[ResponseRedisJobLogin] fail. null user or invalid userID\n");
            return;
        }

        if($ret == ErrorCode::NONE)
        {
            $this->UserMgr->VerifiedUser($user);
        }
        else
        {
            $user->SetEnableLogin();
        }

        $this->SendResponseLogin($sid, $ret);
    }
}