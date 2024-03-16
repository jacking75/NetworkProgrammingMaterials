<?php
require_once("redisJobWorker.php");

$worker = new RedisJobWorker();
$worker->Init("udp://127.0.0.1:50001");

while(true)
{
    $worker->Update();
}

