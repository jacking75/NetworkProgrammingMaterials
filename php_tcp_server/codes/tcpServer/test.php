<?php
require_once("../serverCommon/packet.php");

//$ttt = "\x0"."\x8"."\x0"."\xB"."\x0"."aaa";
//$len = strlen($ttt);
//print $ttt."  ".$len."\n";
//
//PacketDesc::Set($ttt, 0);
//print "Body ".PacketDesc::$BodyData."\n";

$response = new LoginResPacket();
$response->Ret = 11;

$json_data = json_encode($response);
$resPacket = PacketDesc::MakePacket(23, $json_data);

PacketDesc::Set($resPacket, 0);
printf("packetID:%d, Body:%s", PacketDesc::$PacketID, PacketDesc::$BodyData);
