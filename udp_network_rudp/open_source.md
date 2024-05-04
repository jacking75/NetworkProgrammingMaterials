# C++
  
## cloudwu/rudp
- https://github.com/cloudwu/rudp
  

## C++ UDP Networking Library
- https://github.com/jivibounty/Packet
- Packet-master.zip
- Visual C++ 지원.
- 서버/클라이언트.
- 이런 저런 기능이 많이 구현 되어 있음.
- 설계 측면에서는 좀 바꾸고 싶음.
  

## tinynet 
- https://github.com/RandyGaul/tinyheaders
- header only 라이브러리. 여기에 RUDP 코드가 있음(1개의 헤더 파일)
- 너무 많이 복잡하지 않은 듯.
- 그러나 사용 예제가 없어서 사용 방법은 좀 헷갈리듯

    
## A UDP-Only Winsock RIO C++ Network Library
- https://github.com/kklouzal/PeerNet 
  
  
## netcode.io : A simple protocol for creating secure client/server connections over UDP
- https://github.com/networkprotocol/netcode.io
  

## reliable.io. A simple reliability layer for UDP-based protocols
- https://github.com/networkprotocol/reliable.io
  

## Launch of libyojimbo
- http://gafferongames.com/2016/07/21/launch-of-libyojimbo/
- https://github.com/networkprotocol/yojimbo
  
  
## enet(UDP 네트워크 라이브러리)
- https://github.com/lsalzman/enet
- http://enet.bespin.org/
  
  
## NanoSockets
- https://github.com/nxrighthere/NanoSockets    
  
  
## ValveSoftware/GameNetworkingSockets. Reliable & unreliable messages over UDP. Robust message fragmentation & reassembly. Encryption.
- https://github.com/ValveSoftware/GameNetworkingSockets
  

## KCP
- UDP 베이스의 스루풋을 희생하고 저 지연을 위한 프로토콜(A Fast and Reliable ARQ Protocol)
- https://github.com/skywind3000/kcp 
   


<br>    
  

# C#
  
## unity_rudp
- https://github.com/linuxaged/unity_rudp
- unity_rudp-master.zip
- 구현은 1개의 파일로 되어 있음.
- C#으로 유니티3D에서 사용하는 것을 전제로.
- 테스트 코드도 있음.
- `/source_code/unity_rudp`에 코드를 정리해 놓았다
  
  
## houshuo/RUDP
- https://github.com/houshuo/RUDP
- RUDP-master.zip
- 1개의 파일로 되어 있음.
- 코드 분석이 쉬울 듯.
- `/source_code/RUDPTest`에 코드를 정리해 놓았다
  

## SharpRUDP
- https://github.com/whl0070179/SharpRUDP
- `/source_code/SharpRUDP`에 코드를 정리해 놓았다
- Thread-safe.
- Channel-based communication.
- Keeps the connection alive using tiny keepalive packets.
- Retransmission of unacknowledged packets in the next send/reset iteration.
- Different serialization options (JSON and Binary. Binary is default, as it's WAY faster than JSON!)
- Packet data can be in JSON format, so the protocol can be ported to other languages (Node.js anyone?) without much issue.
- Pure concise, clean C# code. Avoids C++ wrappers and obscure BS. Most of the code is in RUDPConnection.cs and RUDPChannel.cs and they're < 700 lines long together!.
- Long data can be sent and will be retrieved sequentially, while keeping packet size to a safe MTU value (8Kb, since Android has 16Kb and Windows has 64Kb, safe spot is 8Kb). However, it will reserve 20% of that size for packet data just in case.
  
  
## nxrighthere/ENet-CSharp
- https://github.com/nxrighthere/ENet-CSharp  
   
    
## FinalSpeed-RUDP-CSharp
- https://github.com/wspl/FinalSpeed-RUDP-CSharp
- FinalSpeed-RUDP-CSharp-master.zip
- 구현이 잘 되어 있는듯.
  
  
## - KCP
- UDP 베이스의 스루풋을 희생하고 저 지연을 위한 프로토콜(A Fast and Reliable ARQ Protocol)
- https://github.com/KumoKyaku/KCP  
    
    
## Netcode.IO.NET
- https://github.com/KillaMaaki/Netcode.IO.NET
  

## **LiteNetLib**
- [LiteNetLib - Lite reliable UDP library for Mono and .NET](https://github.com/RevenantX/LiteNetLib )
- [LiteNetLibとMessagePackで行うリアルタイム通信](https://www.amusement-creators.info/articles/advent_calendar/2020/15/ )  
- [MirrorとLiteNetLib4Mirrorを使って、UnityでのNetwork Discoveryを行うメモ](https://qiita.com/yanosen_jp/items/4f9b951af68a1c156f74 )
  
   
  
## Mirror KCP: reliable UDP
- https://github.com/vis2k/kcp2k
  

## P2P.NET
- https://github.com/Phylliida/P2P.NET
- WebRTC
  
  
## MidLevel/Ruffles 
- https://github.com/MidLevel/Ruffles
- Unity  

  
<br>  
  
  
# Golang
  
## go-stun
- https://github.com/ccding/go-stun
  

## libp2p/go-libp2p
- https://github.com/libp2p/go-libp2p


## Fast RFC 5389 STUN implementation in go
- https://github.com/gortc/stun
  
  
## xtaci/kcp-go 
- https://devhub.io/repos/xtaci-kcp-go
- https://github.com/xtaci/kcp-go 
  



  
  
# Rust 
  
## quilkin - UDP proxy, Rust
- https://news.hada.io/topic?id=4876 
- https://github.com/googleforgames/quilkin 
