# RabbitMQ - PHP
출처: https://gomiba.co.in/blog/archives/1296  
    
가볍게 테스트 하기 위해 docker를 사용한다.  
```
$ sudo docker run -d -p 5672:5672  --name rabbit-server rabbitmq:3
...

$ sudo docker ps
CONTAINER ID        IMAGE               COMMAND                CREATED             STATUS              PORTS                                                   NAMES
92547dbde579        rabbitmq:3          "docker-entrypoint.s   5 seconds ago       Up 4 seconds        4369/tcp, 5671/tcp, 0.0.0.0:5672->5672/tcp, 25672/tcp   rabbit-server


// 모듈이 필요하므로 넣는다（mbstring은 원래 있었음）
$ sudo yum install php-bcmath --enablerepo=remi-php70

// 모듈을 이동하지 않으면 안된다(패스 설정하지 않ㅇ아서 그런듯)
$ sudo mv /etc/php.d/20-bcmath.ini /etc/opt/remi/php70/php.d/
$ sudo mv /usr/lib64/php/modules/bcmath.so /opt/remi/php70/root/usr/lib64/php/modules/

$ php -m | grep bcmath
bcmath


// 앱을 구축한다
$ mkdir php-amqplib-test
$ cd php-amqplib-test

$ composer init
...

$ composer require php-amqplib/php-amqplib:2.7.*
...
```  
  
코딩하기  
```
<?php
// send.php

require('vendor/autoload.php');

use PhpAmqpLib\Connection\AMQPStreamConnection;
use PhpAmqpLib\Message\AMQPMessage;

// 기본 ID/PW = guest/guest
$connection = new AMQPStreamConnection('localhost', 5672, 'guest', 'guest');

// 송신할 채널을 설정
$channel = $connection->channel();

// https://github.com/php-amqplib/php-amqplib/blob/master/PhpAmqpLib/Channel/AMQPChannel.php#L597
// string queue = ''
// bool passive = false
// bool durable = false
// bool exclusive = false
// bool auto_delete = true
// bool nowait = false
$channel->queue_declare('hello', false, false, false, false);


// 메시지 만들기
$msg = new AMQPMessage('Hello World!');

// 메시지를 보낸다
// https://github.com/php-amqplib/php-amqplib/blob/master/PhpAmqpLib/Channel/AMQPChannel.php#L1086
// string msg
// string exchange = ''
// string routing_key = ''
$channel->basic_publish($msg, '', 'hello');
```
  
```  
<?php
// receive.php

require('vendor/autoload.php');

use PhpAmqpLib\Connection\AMQPStreamConnection;

// 기본 ID/PW = guest/guest
$connection = new AMQPStreamConnection('localhost', 5672, 'guest', 'guest');

// 채널을 설정
$channel = $connection->channel();

// https://github.com/php-amqplib/php-amqplib/blob/master/PhpAmqpLib/Channel/AMQPChannel.php#L597
// string queue = ''
// bool passive = false
// bool durable = false
// bool exclusive = false
// bool auto_delete = true
// bool nowait = false
$channel->queue_declare('hello', false, false, false, false);


// 콜백 함수 준비
$callback = function ($msg) {
        echo $msg->body . "\n";
};

// 메시지 수신
// https://github.com/php-amqplib/php-amqplib/blob/master/PhpAmqpLib/Channel/AMQPChannel.php#L901
// string queue = ''
// string consumer_tag = ''
// bool no_local = false
// bool no_ack = false
// bool exclusive = false
// bool nowait = false
// function callback = null
$channel->basic_consume('hello', '', false, true, false, false, $callback);

// 메시지 수신 대기 루프
while(count($channel->callbacks)) {
    $channel->wait();
}
```
   
터미널에 2개를 열어서 실행한다.  
```
// 보내는쪽
$ php send.php
$ php send.php
$ php send.php
$ php send.php
```  
```
// 받는쪽
$ php receive.php
Hello World!
Hello World!
Hello World!
Hello World!
```
  
아래는 RabbitMQ가 종료되어도 메시지가 저장되도록 한다.  
```
$ php send.php
$ sudo docker restart rabbit-server
$ php receive.php
```  
  
공식 anstjdp Message durability 항목이 있지만 송신 시에 인수를 설정해야 한다.  
[RabbitMQ - RabbitMQ tutorial - Work Queues](http://www.rabbitmq.com/tutorials/tutorial-two-php.html)  
  
```  
<?php
require('vendor/autoload.php');

use PhpAmqpLib\Connection\AMQPStreamConnection;
use PhpAmqpLib\Message\AMQPMessage;

$connection = new AMQPStreamConnection('localhost', 5672, 'guest', 'guest');

$channel = $connection->channel();

// https://github.com/php-amqplib/php-amqplib/blob/master/PhpAmqpLib/Channel/AMQPChannel.php#L597
// string queue = ''
// bool passive = false
// bool durable = false
// bool exclusive = false
// bool auto_delete = true
// bool nowait = false
$channel->queue_declare('hello', false, true, false, false);


// 메시지 만들기
// 딜리버리 모드 추가
$msg = new AMQPMessage('Hello World!', ['delivery_mode' => AMQPMessage::DELIVERY_MODE_PERSISTENT]);

// 메시지 보내기
// https://github.com/php-amqplib/php-amqplib/blob/master/PhpAmqpLib/Channel/AMQPChannel.php#L1086
// string msg
// string exchange = ''
// string routing_key = ''
$channel->basic_publish($msg, '', 'hello');
```
  
```  
// 일단 재 기동한다
$ sudo docker restart rabbit-server

// 보내기, 재기동, 받기
$ php send.php
$ sudo docker restart rabbit-server
$ php receive.php
Hello World!
```
   
  
Laravel의 큐 드라이버로 RabbitMQ를 사용하는 라이브러리가 이미 있어서 이것을 사용한다.  
[vyuldashev/laravel-queue-rabbitmq: RabbitMQ driver for Laravel Queue](https://github.com/vyuldashev/laravel-queue-rabbitmq )  
    





