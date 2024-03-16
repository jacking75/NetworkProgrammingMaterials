<?php
require_once("redisClient.php");
require_once("packet.php");
require_once("errorCode.php");


final class RedisJobWorker
{
    private $RedisClient;
    private $UdpSocket;

    public function __construct()
    {
        $this->RedisClient = new RedisClient();
        $this->RedisClient->Init('127.0.0.1', '6379');
    }

    public function Init(string $address)
    {
        $this->UdpSocket = @stream_socket_server($address, $errno, $errstr, STREAM_SERVER_BIND);
        if (!$this->UdpSocket)
        {
            die("$errstr ($errno)");
        }

        printf("[RedisJobWorker] Success Init\n");
    }

    public function Update()
    {
        $curTime = time();

        $length = 2048;
        $packet = stream_socket_recvfrom($this->UdpSocket, $length, 0, $peer);
        $this->ProcessRequestJob($packet, $peer);
    }


    function ProcessRequestJob(string $packet, string $peer)
    {
        $request = json_decode($packet);

        $jobID = $request->{'JobID'};
        $sid = $request->{'SID'};

        switch ($jobID)
        {
            case RedisJobID::REQ_LOGIN:
                $userID = $request->{'UserID'};
                $userPW = $request->{'UserPW'};
                $this->ProcessJobLogin($sid, $userID, $userPW, $peer);
                break;
            case RedisJobID::NTF_GAME_RESULT:
                break;
            default:
                printf("Unknown Request - %d\n", $jobID);
        }
    }

    function ProcessJobLogin(int $sid, string $userID, string $userPW, string $peer)
    {
        $response = new LoginRedisJobRes();
        $response->JobID = RedisJobID::RES_LOGIN;
        $response->SID = $sid;
        $response->UserID = $userID;
        $response->Ret = ErrorCode::NONE;

        $dbValue = $this->RedisClient->GetKeyValue('auth_'.$response->UserID);
        if($dbValue == null || $dbValue != $userPW)
        {
            $response->Ret = ErrorCode::LOGIN_INVALID_PW;
        }

        $json_data = json_encode($response);
        @stream_socket_sendto($this->UdpSocket, $json_data, 0, $peer);
    }


}



