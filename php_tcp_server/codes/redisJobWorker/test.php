<?php
//require_once("redisJobWorker.php");

$redis = new Redis();
try
{
    $redis->connect('127.0.0.1','6379');
    $key = 'myKey';
    $value = ['val1' => 'myValue1', 'val2' => 'Value2'];
    $ttl = 3600;
    $redis->setex( $key, $ttl, $value );
    $value = $redis->get($key);
    var_dump($value);
} catch(RedisException $e) {
    var_dump($e);
}
$redis->close();


