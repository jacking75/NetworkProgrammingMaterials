# Reliable UDP(RUDP) 설명과 코드 분석
  
# 설명
- PC 온라인 게임 시대에는 P2P 게임에서 사용할 목적으로 이 기술을 사용.
- 그러나 모바일 시대에서는 PC에 비해 좋지 않은 네트워크 환경 때문에 C/S 환경에서 사용한다.
- Reliable UDP는 이름 대로 UDP를 사용한다.
- 여기서 네트워크 프로그래밍의 기초인 UDP, TCP 차이는 설명하지 않음.

## RUDP의 특징
- UDP를 사용하지만 TCP처럼 신뢰성(전송 보장과 패킷 순서)을 가진다.
- 애플리케이션에서 수신 여부와 순서를 보정해야 한다.
  
## RUDP의 패킷 타입
1. 일반적인 UDP 패킷 전송(신뢰성 없음)  
2. 전송은 보장, 그러나 순서는 보장 하지 않음  
3. 전송 보장, 순서 보장  


<br>

**네트워크 성능과 구현의 쉬움 순서는 1 > 2 > 3**
  
<br>  
    
## 패킷 구성
- 전송 타입: 앞의 패킷 타입
- 패킷의 고유 넘버: 수신쪽에서 순서를 판단하기 위해 보내는쪽에서 순서대로 증가하는 번호를 사용한다.
- 에러 검출: crc 코드 등을 사용하여 패킷에 변조가 없는지 조사
- 패킷 데이터: 송신자가 보낼 실 데이터

<br>  
    
## RUDP 패킷 보내기
![](/resource/00001.PNG)
![](/resource/00002.PNG)
![](/resource/00003.PNG)
![](/resource/00004.PNG)
  
<br>  
    
## remote의 정보 관리
```
len = recvfrom(socket, buffer, sizeof(buffer), 
				0, &addr, &addrlen); 
                
packet = packet_decode(buffer, len); 

con = find_connect(packet); 
if (con == NULL) {
	con = connected(addr, buffer, len); 
}

if (con != NULL) {
	con->recv_packet(packet); 
}
```

  
## 패킷 고유 번호
고유하게 1씩 증가하는 경우  
```
if (packet->ackcnt <= recnt_ackcount || 
		find_recvpacket_id(recnt_ackcount) {
    return ;    // 이미 처리한 패킷 
}

if (recnt_ackcount+1 == packet->ackcnt) 
{ 
    recnt_ackcount+=1; 
    while(find_recvpacket_id(recnt_ackcount)) 
    { 
        remove_recvpacket_id(recnt_ackcount); 
        recnt_ackcount++; 
    } 
}    
else 
{ 
    insert_recvpacket_id(recnt_ackcount); 
}
```  
  
  
## 패킷 보내기
```
int rudp_connect::send(char * buff, int len, udp_packet_type type) 
{ 
    rudp_packet packet; 
    int len; 
    len = encode_packet(&packet, buff, len, type); 
    if (ISRECHABLE(type)) 
        store_send_packet(&packet); 
    return sendto(packet, len, & m_addr); 
} 
```
  
  
## 패킷 받기 처리
```
void rudp_connect::recv(char * buff, int len) 
{ 
    rudp_packet packet; 
    int len; 
    len = decode_packet(&packet, buff, len); 
    if (ISINVALIDEPACKET(packet)) 
        return; 
    switch(packet.type) 
    { 
        case udp_packet_type_a : 
            parse_game_packet(packet.data, packet.len); 
            break; 
        case udp_packet_type_b : 
            send_ackpacket(&packet); 
            if (ISOLDPACKET(packet.ackcnt) == false) 
            { 
                update_recv_packetcount(packet.ackcnt); 
                parse_game_packet(packet.data, packet.len); 
            } 
            break; 
        case udp_packet_type_c : 
            send_ackpacket(&packet); 
            if (ISOLDPACKET(packet.ackcnt) == false) 
            { 
                update_recv_packetcount(packet.ackcnt); 
                if (ISRECENTPACKET(packet.seqcnt) == true) 
                { 
                    parse_game_packet(packet.data, packet.len); 
                    for(i=packet.seqcnt+1; 1; i++) 
                    { 
                        p = find_recv_packet(i) 
                        parse_game_packet(p->data, p->len); 
                        remove_recv_packet(p); 
                    } 
                    update_recv_packet_seqcnt(i); 
                }    else 
                    store_recv_packet(&packet, packet.seqcnt); 
            }            break; 
        case udp_packet_ack : 
            remove_send_packet(packet.ackcnt); 
            break; 
            ... 
    } 
} 
```
  
<br>  
  
## 데이터그램을 보낼 때 해야할 일
- 데이터그램의 ID를 부여한다. 이는 보낼 때마다 +1씩 증가시키는 방법으로 처리하면 된다. 
- 데이터그램을 보낸다. 
- 데이터그램에 대한 인증을 받지 못했을 경우, 다음에 전송해야하는 시간을 기록해둔다. 
- 데이터그램에 대한 인증을 받지 못했을 경우, 전송을 시도할 최대 횟수를 기록해둔다. 
- 데이터그램을 보냈지만, 아직 인증받지 못한 데이터그램의 리스트에다가 집어넣어둔다. 
  
  
## 데이터그램을 받았을 때 해야할 일
- 데이터그램이 이미 한번 실행했던 데이터그램이 아닌지 검사한다. 이는 중간에 빠진 데이터그램의 리스트와 현재까지 도착한 데이터그램ID의 최대값을 이용해서 이루어진다. 
- 이미 한번 실행했던 것이라면 그냥 무시하고, 아니라면 정상적으로 실행한다. 
- 현재까지 도착한 데이터그램ID의 최대값과 중간에 빠진 데이터그램의 리스트를 갱신한다. 
- 데이터그램을 "정상적으로 실행했지만, 송신측에 알려주지 않은 리스트"에다가 등록한다. 
- 만일 데이터그램이 수신측이 보내온 인증 데이터그램이라면 보냈지만, 아직 인증받지 못한 데이터그램의 리스트에 있는 해당하는 데이터그램들을 삭제해준다. 

  
## 주기적으로 처리해야할 일
- 정상적으로 실행했지만, 송신측에 알려주지 않은 리스트에 데이터그램ID가 들어있다면 이를 송신측에게 송신한다. (이 데이터그램 역시 데이터그램을 보내는 것이므로, 데이터그램을 보낼 때 해야할 일들을 차례로 수행한다.) 
- 보냈지만, 아직 인증받지 못한 데이터그램의 리스트를 검색하면서 재전송해야하는 데이터그램이 있다면 재전송한다. (재전송할 때마다 다음에 전송해야하는 시간과 전송 시도 횟수를 갱신해야한다.) 
- 최대 재전송 횟수를 초과한 데이터그램은 삭제해버린다. (끝까지 전송되지 않는 데이터그램이 결국 생길 수 있다.) 
  
  
## 타임 아웃에 의한 재전송
타임 아웃과 재전송은 단순히 일정한 타임 아웃 시간을 사용하는 것이 아니라 네트워크의 성질이나 상태, 부하 등을 고려해야 한다. 
- 패킷이 송신자에서 수신자에게 도착해서 다시 돌아올 때까지 왕복 시간(Round-Trip Time:왕복 시간)의 현 시점의 추계치를 통계적으로 구하고, 유지한다. 이 추계치를 토대로 패킷을 네트워크로 보내어, 이것이 너무 빨라서 네트워크를 독점적으로 채우지 않도록 한다.
- 다음 재시도까지의 대기 시간을 계산할 때 "지수적 백 오프"를 사용하여 오류가 자꾸 반복될 때마다 다음 재전송까지 시간이 지수적으로 늘어난다. 이렇게하면, 네트워크 상의 패킷 수가 점점 줄어드니 다음 재전송이 잘 나갈 가능성이 높아진다.
  
  
## RUDP 주의할 점
- 만약 특정 패킷이 계속 전달되지 않을 경우 포기하지 않고 계속 시도하면 네트워크가 멈춘 것 같은 상태가 된다.
- 특정 횟수 이상 실패하면 포기하고 다음 패킷을 보낼 수 있도록 예외 처리가 잘 되어 있어야 한다.

  
## 외부 글
- [Reliable UDP 구현](./Reliable_UDP_구현.pdf) [출처](http://www.slideshare.net/noerror/rudp)
- [Reliable UDP 구현과 활용](./Reliable_UDP_구현과_활용.pdf)  [출처](http://egloos.zum.com/choiwonwoo/v/924906)
- [Reliable 구현](./Reliable_udp_chikicon.pdf)   [출처](http://blog.naver.com/chikicon/60036633811)  
  

<br>  
<br>  
<br>  
  
    
# 참고 글 
- [Unity Technologies의 온라인 대전 FPS 게임 구현을 조사해 보았다](https://docs.google.com/document/d/e/2PACX-1vQVBLOLuCeapeZTjOUxNVNXO3pxAUhr1r7k4qQUAP-CLGaVqZEnNecPWGvCI8YqeZ9MJwL_mw8STCrx/pub )
- [〈카트라이더〉 0.001초 차이의 승부 - 300km/h 물체의 네트워크 동기화 모델 구현기](http://ndcreplay.nexon.com/NDC2019/sessions/NDC2019_0002.html#c=NDC2019&t%5B%5D=%ED%94%84%EB%A1%9C%EA%B7%B8%EB%9E%98%EB%B0%8D&p=3 )
- [QUIC은 데이터 손실시 처리 대기를 어떻게 최소화할까?](https://docs.google.com/document/d/e/2PACX-1vSeUKRkoEVJ0hRTj8Gik4tqclh47p56yJlwjL-7F3JTvvlQ3cJl2dD3RbhnDdANSm_YxTqtaouMvi3w/pub )
- [[draft-ietf-sigtran-reliable-udp](http://crowback.tistory.com/category/Programming/Protocol%20-%20RUDP )     
  