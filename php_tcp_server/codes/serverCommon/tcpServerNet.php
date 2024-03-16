<?php
require_once("packet.php");

final class TcpServerNet
{
    private $serverSock;
    private $ReadStreams = [];
    private $running = true;

    public $OnNetEvents;


    public function Start($address)
    {
        $this->serverSock = @stream_socket_server($address, $errno, $errstr);
        stream_set_blocking($this->serverSock, 0);

        if ($this->serverSock == false) {
            fwrite(STDERR, "Error connecting to socket: $errno: $errstr\n");
            exit(1);
        }

        $sock_num = (int)$this->serverSock;
        $this->AddReadStream($this->serverSock);
        printf("[Start] Server Network ServerSock:%d\n", $sock_num);
    }

    public function SendPacket(int $sockNum, int $dataLen, string &$data) : bool
    {
        if($this->IsValidSockNum($sockNum) == false)
        {
            printf("[Error] SendPacket - Invalid SockNum: %d", $sockNum);
            return false;
        }

        fwrite($this->ReadStreams[$sockNum], $data, $dataLen);
        return true;
    }

    function OnAccept()
    {
        while(true)
        {
            $client_sock = @stream_socket_accept($this->serverSock, 0, $clientAddress);
            if($client_sock == false)
            {
                return;
            }

            if (is_resource($client_sock) == false)
            {
                printf("[Fail OnAccept] \n");
                fclose($client_sock);
                return;
            }

            $sock_num = (int)$client_sock;
            $this->OnNetEvents[0]($sock_num);

            stream_set_blocking($client_sock, 0);
            $this->AddReadStream($client_sock);
        }
    }

    function OnReadEvent($stream)
    {
        if($this->IsValidStream($stream) == false)
        {
            printf("OnReadEvent ??? 1\n");
        }

        $sock_num = (int)$stream;
        $buffer = fread($stream, 4096);
        $readLen = strlen($buffer);

        if($readLen <= 0)
        {
            if($this->IsValidStream($stream) == false)
            {
                printf("OnReadEvent ??? 2 sockNum%d\n", $sock_num);
            }

            $this->RemoveReadStream($stream);
            $this->OnNetEvents[1]($sock_num);

            stream_socket_shutdown($stream, STREAM_SHUT_RDWR);
            fclose($stream);
        }
        else
        {
            //printf("[OnReadEvent] SockNum:%d, recvDatalen:%d\n", $sock_num, $readLen);

            // 아주 중요: 클라이언트는 아주 큰 패킷을 보내거나, 임의로 패킷을 짤라서 보내지 않아야한다.
            $bufferPos = 0;
            while($readLen >= PacketDesc::HeaderSize)
            {
                PacketDesc::Set($buffer, $bufferPos);
                $this->OnNetEvents[2]($sock_num, PacketDesc::$PacketID, PacketDesc::$BodyDataSize, PacketDesc::$BodyData);

                $readLen -= PacketDesc::$PacketSize;
                $bufferPos += PacketDesc::$PacketSize;
            }
        }
    }

    public function Run()
    {
        $read = $this->ReadStreams;
        $write = null;
        $except = null;

        if ($read)
        {
            $count = @stream_select($read, $write, $except, 0, 100);
            if($count < 1)
            {
                return;
            }
            //printf("[Run] event Count:%d\n", $count);

            foreach ($read as $stream)
            {
                if($stream == $this->serverSock)
                {
                    $this->OnAccept();
                }
                else
                {
                    //printf("onclose\n");
                    $this->OnReadEvent($stream);
                    //printf("onclose - readStreams Count:%d\n", count($this->ReadStreams));
                }
            }
        } else {
            usleep(16000); // 16밀리세컨드
        }
    }


    function AddReadStream($stream)
    {
        $sock_num = (int)$stream;
        //print "[new Client] sock_num: ".$sock_num."\n";

        if (empty($this->ReadStreams[$sock_num]))
        {
            //print "[new Client - Add readStreams]\n";
            $this->ReadStreams[$sock_num] = $stream;
        }
        else
        {
            printf("[Fail addReadStream] sock_num%d\n", $sock_num);
        }
    }

    function RemoveReadStream($stream)
    {
        $sock_num = (int)$stream;
        //print "[remove Client] sock_num: ".$sock_num."\n";
        unset($this->ReadStreams[$sock_num]);
    }

    function IsValidSockNum(int $sock) : bool
    {
        return empty($this->ReadStreams[$sock]) == false ? true:false;
    }

    function IsValidStream($stream) : bool
    {
        $sock = (int)$stream;
        return empty($this->ReadStreams[$sock]) == false ? true:false;
    }



//    function addWriteStream($stream, callable $handler) {
//        $sock_num = (int)$stream;
//
//        if (empty($this->writeStreams[$sock_num])) {
//            $this->writeStreams[$sock_num] = $stream;
//            $this->writeHandlers[$sock_num] = $handler;
//        }
//    }

//    function removeWriteStream($stream)
//    {
//        unset($this->writeStreams[(int) $stream]);
//    }

//    function removeStream($stream)
//    {
//        $this->removeReadStream($stream);
//        //$this->removeWriteStream($stream);
//    }
}