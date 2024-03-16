<?php
require_once("serverOption.php");
require_once("../serverCommon/tcpServerNet.php");
require_once("packetHandlerCommon.php");
require_once("packetHandlerRoom.php");
require_once("packetHandlerGame.php");
require_once("userManager.php");
require_once("roomManager.php");
require_once("redisJobManager.php");
require_once("../serverCommon/simplePacketRqsResCounter.php");
require_once ("../serverCommon/serverStatusView.php");

final class GameServer
{
    private $serverNet;
    private $serverOpt;

    private UserManager $UserMgr;
    private RoomManager $RoomMgr;

    private $HandlerMap;
    private PacketHandlerCommon $HandlerCommon;
    private PacketHandlerRoom $HandlerRoom;
    private PacketHandlerGame $HandlerGame;

    private int $ServerStartTimeSec = 0;


    public function __construct()
    {
        $this->serverNet = new TcpServerNet();

        $this->serverNet->OnNetEvents[0] = function ($sockNum) {
            $this->OnConnect($sockNum);
        };
        $this->serverNet->OnNetEvents[1] = function ($sockNum) {
            $this->OnClose($sockNum);
        };
        $this->serverNet->OnNetEvents[2] = function ($sockNum, $packetId, $bodyLen, &$bodyData) {
            $this->OnReceive($sockNum, $packetId, $bodyLen, $bodyData);
        };
    }

    public function Init($serveropt)
    {
        $this->serverOpt = $serveropt;

        $this->CreateComponent();

        $this->CreatePacketHandler();

        print "Init ChatServer !\n";

        //SimplePacketRqsResCounter::Init();
    }

    public function Start()
    {
        $this->ServerStartTimeSec = time();

        $redisJobMgrAddress = "udp://" . "127.0.0.1". ":" . 50001;
        print "RedisJobWorker Address: " . $redisJobMgrAddress . "\n";
        if(RedisJobManager::Inst()->Init($redisJobMgrAddress) == false)
        {
            printf("[Error] RedisJobWorker Connect. Address:%s", $redisJobMgrAddress);
            return;
        }


        $address = "tcp://" . $this->serverOpt->IP . ":" . $this->serverOpt->Port;
        print "Server Address: " . $address . "\n";
        $this->serverNet->Start($address);

        $logfileName = sprintf("./serverStatus_%d.log", $this->ServerStartTimeSec);
        ServerStatusView::Init($logfileName);
    }

    public function Update()
    {
        $curTime = time();

        $this->serverNet->Run();

        $this->RoomMgr->Update($curTime);

        $packet = RedisJobManager::Inst()->Update();
        if($packet != null)
        {
            $request = json_decode($packet);
            $jobID = $request->{'JobID'};
            $this->HandlerMap[$jobID]($packet);
        }

        ServerStatusView::Update();
    }


    function CreateComponent()
    {
        $this->UserMgr = new UserManager();

        $this->RoomMgr = new RoomManager();
        $this->RoomMgr->SendPacketFunc[0] = function (int $sockNum, int $dataLen, string &$data) : bool {
            return $this->serverNet->SendPacket($sockNum, $dataLen, $data);
        };
        $this->RoomMgr->Init($this->serverOpt->MaxRoomCount,
                            $this->serverOpt->StartRoomNumber);
}

    function CreatePacketHandler()
    {
        $this->HandlerCommon = new PacketHandlerCommon();
        $this->HandlerCommon->SendPacketFunc[0] = function (int $sockNum, int $dataLen, string &$data) : bool {
            return $this->serverNet->SendPacket($sockNum, $dataLen, $data);
        };
        $this->HandlerCommon->UserMgr = $this->UserMgr;
        $this->HandlerCommon->SetHandler($this->HandlerMap);


        $this->HandlerRoom = new PacketHandlerRoom();
        $this->HandlerRoom->SendPacketFunc = $this->HandlerCommon->SendPacketFunc;
        $this->HandlerRoom->UserMgr = $this->UserMgr;
        $this->HandlerRoom->RoomMgr = $this->RoomMgr;
        $this->HandlerRoom->SetHandler($this->HandlerMap);


        $this->HandlerGame = new PacketHandlerGame();
        $this->HandlerGame->SendPacketFunc = $this->HandlerCommon->SendPacketFunc;
        $this->HandlerGame->UserMgr = $this->UserMgr;
        $this->HandlerGame->RoomMgr = $this->RoomMgr;
        $this->HandlerGame->SetHandler($this->HandlerMap);
    }


    function OnConnect($sockNum)
    {
        //printf("[OnConnect] SockNum:%d\n", $sockNum);
        $this->UserMgr->AddUser($sockNum);

        ServerStatusView::IncrementConnected();
    }

    function OnClose($sockNum)
    {
        //printf("[OnClose] SockNum:%d\n", $sockNum);
        ServerStatusView::DecrementConnected();

        $user = $this->UserMgr->GetUser($sockNum);

        if($user != null)
        {
            $this->RoomMgr->LeaveRoom(false, $user->RoomNumber, $sockNum);
            $this->UserMgr->RemoveUser($sockNum);
        }
    }

    function OnReceive($sockNum, $packetId, $bodyLen, &$bodyData)
    {
        //printf("[OnReceive] SockNum:%d, packetId:%d\n", $sockNum, $packetId);
        ServerStatusView::IncrementReq();

        $this->HandlerMap[$packetId]($sockNum, $bodyLen, $bodyData);

        // 1 요청마다 시간 측정과 완료 개수를 계산하므로 조금 비효율적임
        SimplePacketRqsResCounter::CountRes(time(),1);
    }
}



