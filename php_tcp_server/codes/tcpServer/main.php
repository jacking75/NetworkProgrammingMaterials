<?php
require_once("chatServer.php");

// function ctrl_handler(int $event)
// {
    // switch ($event) {
        // case PHP_WINDOWS_EVENT_CTRL_C:
            // echo "You have pressed CTRL+C\n";
            // SimplePacketRqsResCounter::PrintLogFile();
            // exit();
            // break;
        // case PHP_WINDOWS_EVENT_CTRL_BREAK:
            // echo "You have pressed CTRL+BREAK\n";
            // exit();
            // break;
    // }
// }
// sapi_windows_set_ctrl_handler('ctrl_handler');

$serverOpt = new ServerOption();
$serverOpt->IP = "127.0.0.1";
$serverOpt->Port = 32452;
$serverOpt->MaxSessionCount = 100;
$serverOpt->MaxRoomCount = 500;
$serverOpt->StartRoomNumber = 0;
$serverOpt->MaxRoomUserCount = 4;

$server = new ChatServer();
$server->Init($serverOpt);

$server->Start();

while(true)
{
    $server->Update();
}


//print "\n";
//while(true)
//{
//    sleep(1);
//
//    $stdin = trim(fgets(STDIN));
//
//    if($stdin == "q") {
//        print "Server End!!!";
//        exit;
//    }
//}

