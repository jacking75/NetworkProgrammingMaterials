<?php


class RedisClient
{
    private $Connector;


    public function Init(string $hostIP, string $port)
    {
        try
        {
            $this->Connector = new Redis();
            $this->Connector->connect($hostIP,$port);
        } catch(RedisException $e) {
            var_dump($e);
        }
        //$redis->close();
    }

    public function SetKeyValue(string $key, string &$value, int $expire)
    {
        $this->Connector->setex( $key, $expire, $value );
    }

    public function GetKeyValue(string $key) :?string
    {
        $value = $this->Connector->get($key);
        if(!$value)
        {
            return null;
        }
        return $value;
    }

    public function AddQueue(string $key, string $value)
    {
        $this->Connector->RPush($key, $value);
    }

}