<?php

class RedisJobManager
{
    private static $instance;

    //TODO array 이어야 한다
    private $UdpSocket;

    public static function Inst()
    {
        if (null === self::$instance) {
            self::$instance = new RedisJobManager();
        }

        return self::$instance;
    }

            //TODO 복수 입력 가능하도록한다
    public function Init(string $serverAddress) : bool
    {
        $this->UdpSocket = @stream_socket_client($serverAddress, $errno, $errstr);
        if (!$this->UdpSocket)
        {
            printf("[ERROR] %d - %s\n", $errno, $errstr);
            return false;
        }

        @stream_set_blocking($this->UdpSocket, 0);

        return true;
    }

    public function SendRequestJob(string $packet)
    {
        fwrite($this->UdpSocket, $packet);
    }

    public function Update() : ?string
    {
        $length = 2048;
        $packet = fread($this->UdpSocket, $length);

        if($packet == null || (strlen($packet) == 0))
        {
            return null;
        }

        //printf("[RedisJobManager::Update] recv size:%d\n", strlen($packet));
        return $packet;
    }


}