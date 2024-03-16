# KCP - A Fast and Reliable ARQ Protocol
[출처](https://github.com/skywind3000/kcp/blob/master/README.en.md )


# Introduction
KCP는 빠르고 안정적인 프로토콜로, TCP 보다 대역폭을 10~20% 더 낭비하는 대신 평균 지연 시간을 30~40% 감소시키고, 최대 지연 시간을 3배까지 줄이는 전송 효과를 얻을 수 있다. 순수 알고리즘을 사용하여 구현되며, 기반 프로토콜(예: UDP)의 송수신에 대한 책임이 없으므로 사용자가 기반 데이터 패킷에 대한 전송 모드를 직접 정의하고 이를 콜백 방식으로 KCP에 제공해야 한다. 심지어 내부 시스템 호출 없이 외부에서 클럭을 전달받아야 한다.

전체 프로토콜에는 ikcp.h, ikcp.c의 소스 파일 두 개만 있고 사용자의 자체 프로토콜 스택에 쉽게 통합할 수 있다. P2P 또는 UDP 기반 프로토콜을 구현했지만 완벽한 ARQ 프로토콜 구현이 부족하다면 두 파일을 기존 프로젝트에 복사하고 몇 줄의 코드를 작성하기만 하면 사용할 수 있다.



# 기술 사양
TCP는 대역폭을 최대한 활용하는 데 중점을 둔 트래픽(초당 전송할 수 있는 데이터의 양)을 위해 설계 되었다. 반면 KCP는 유량(하나의 패킷을 한쪽 끝에서 다른 쪽 끝으로 전송하는 데 걸리는 시간)을 위해 설계 되었으며, 전송 속도가 TCP 보다 30~40% 빠른 대신 10~20%의 대역폭 낭비가 발생한다. TCP 채널은 유속은 매우 느리지만 초당 흐름이 매우 큰 대운하인 반면, KCP는 빠른 흐름을 가진 작은 급류이다. KCP는 일반 모드와 고속 모드를 모두 가지고 있으며, 다음과 같은 전략을 통해 유속 증가라는 결과를 달성한다:


## RTO 두 배 대 두 배 없음:
TCP 타임아웃 계산은 RTOx2 이므로 3번 연속 패킷 손실이 발생하면 RTOx8 이 되어 매우 끔찍한 반면, KCP 고속 모드가 활성화된 후에는 x2 가 아니라 x1.5(실험 결과 1.5의 값이 상대적으로 양호함)로 전송 속도가 향상 되었다.


## 선택적 재전송과 전체 재전송 비교:
TCP에서 패킷 손실이 발생하면 손실된 패킷 이후의 모든 데이터를 재전송하는 반면, KCP는 선택적 재전송으로 실제로 손실된 데이터 패킷만 재전송한다.


## 빠른 재전송:
송신 단말기는 1, 2, 3, 4, 5 패킷을 전송한 다음, 원격 ACK : 1, 3, 4, 5를 수신하고 ACK3을 수신 할 때 KCP는 2가 한 번 건너 뛴 것을 알고, ACK4를 수신 할 때 2가 두 번 건너 뛴 것을 알고 이 시점에서 2가 손실 된 것으로 간주하고 타임 아웃까지 기다리지 않고 패킷 2를 직접 재전송하여 패킷 손실이 발생할 때 전송 속도를 크게 향상시킬 수 있다.


## 지연 ACK와 비지연 ACK 비교:
TCP는 대역폭을 최대한 활용하기 위해 ACK 전송을 지연(NODELAY도 작동하지 않음)하여 타임아웃 계산이 상대적으로 높은 RTT로 나오기 때문에 패킷 손실이 발생했을 때 판단 과정이 길어졌다. 반면 KCP의 경우 ACK 전송 지연 여부를 조정할 수 있다.


## UNA와 ACK+UNA:
ARQ 모델 응답에는 두 가지 종류가 있다: UNA(TCP와 같이 이 번호 이전의 모든 패킷 수신)와 ACK(이 번호의 패킷 수신)이다. UNA만 사용하면 전체 재전송이 발생하고, ACK만 사용하면 패킷 손실 비용이 너무 커서 기존 프로토콜에서는 둘 중 하나를 선택했지만, KCP 프로토콜에서는 하나의 ACK 패킷을 제외한 모든 패킷에 UNA 정보가 있다.

## 비양보적 흐름 제어:
KCP 일반 모드는 TCP와 동일한 공정한 양보 규칙, 즉 전송 캐시 크기, 수신 측의 수신 버퍼 크기, 패킷 손실 양보, 슬로우 스타트 등 4가지 요소에 의해 전송 창 크기가 결정된다. 그러나 적시성이 요구되는 작은 데이터를 전송할 때는 설정을 통해 후자의 두 단계를 건너뛰고 앞의 두 항목만 사용하여 전송 주파수를 제어할 수 있으며, BT가 열려 있어도 전송이 원활해지는 효과를 얻는 대신 공정성과 대역폭 활용도를 일부 희생할 수 있다.


# Quick Install
You can download and install kcp using the [vcpkg](https://github.com/Microsoft/vcpkg ) dependency manager:
```
git clone https://github.com/Microsoft/vcpkg.git
cd vcpkg
./bootstrap-vcpkg.sh
./vcpkg integrate install
./vcpkg install kcp
```

vcpkg의 kcp 포트는 Microsoft 팀원 및 커뮤니티 기여자에 의해 최신 상태로 유지된다. 버전이 오래된 경우 vcpkg 리포지토리에 이슈 또는 풀 리퀘스트를 만들어 주기 바란다.



# Basic Usage
1. Create KCP object:
```
// kcp 객체를 초기화한다. conv는 세션 번호를 나타내는 정수이다.
// tcp의 conv와 동일하며, 양측 통신은 동일한 conv를 보장해야 한다,
// 상호 데이터 패킷을 인식할 수 있도록 user는 콜백 함수에 전달될 포인터이다.
// 포인터를 콜백 함수에 전달한다.
ikcpcb *kcp = ikcp_create(conv, user);
```

2. Set the callback function:
```
// KCP가 데이터를 전송해야 할 때 호출하는 KCP 하위 계층 프로토콜 출력 함수이며, buf/len 은 버퍼와 데이터 길이를 나타낸다.
// user는 여러 KCP 객체를 구분하기 위해 KCP 객체가 생성될 때 수신되는 값을 나타낸다.
int udp_output(const char *buf, int len, ikcpcb *kcp, void *user)
{
  ....
}

// Set the callback function
kcp->output = udp_output;
```

3. Call update in an interval:
```
// 특정 주기로 ikcp_update를 호출하여 kcp 상태를 업데이트하고, 현재 시계(밀리초 단위)를 전달한다. 10ms 마다 호출을 실행하거나 ikcp_check를 사용하여 다음 업데이트 호출 시간을 결정하는 경우, 매번 호출할 필요가 없다.
ikcp_update(kcp, millisec);
```

4. Input a lower layer data packet:
```
// 하위 계층 데이터 패킷(예: UDP 패킷)을 수신할 때 호출해야 한다:
ikcp_input(kcp, received_udp_packet, received_udp_size);
```

하위 계층 프로토콜의 출력/입력을 처리한 후 KCP 프로토콜은 정상적으로 작동할 수 있으며, ikcp_send를 사용하여 원격 측으로 데이터를 전송한다. 상대방은 ikcp_recv(kcp, ptr, size)를 사용하여 데이터를 수신한다.



# 프로토콜 구성
프로토콜 기본 모드는 표준 ARQ 이며, 설정에 따라 다양한 가속 스위치를 활성화할 수 있다:

1. 작업 모드:
```
int ikcp_nodelay(ikcpcb *kcp, int nodelay, int interval, int resend, int nc)
```

- nodelay : 노딜레이 모드 사용 여부, 0은 사용 안함, 1은 사용함.
- interval ：프로토콜 내부 작업 간격(밀리초 단위, 예: 10ms 또는 20ms).
- 재전송 ：빠른 재전송 모드, 0은 default 꺼짐을 나타내며, 2를 설정할 수 있다(2 ACK 스팬은 직접 재전송으로 이어짐).
- 흐름 제어 끄기 여부, 0은 default "끄지 않음", 1은 "켜기"를 나타낸다.
- 일반 모드: ikcp_nodelay(kcp, 0, 40, 0, 0);
- 터보 모드: ikcp_nodelay(kcp, 1, 10, 2, 1);

2. Window Size:
```
int ikcp_wndsize(ikcpcb *kcp, int sndwnd, int rcvwnd);
```
이 호출은 프로코톨의 최대 송신 윈도우와 최대 수신 윈도우 크기를 설정하며, 기본값은 32 이다. 이는 TCP의 SND_BUF와 RCV_BUF로 이해할 수 있지만 단위는 같지 않으며, SND/RCV_BUF 단위는 바이트이고 이 단위는 패킷이다.

3. 최대 전송 단위:
순수 알고리즘 프로토콜은 MTU 감지를 담당하지 않으며, 기본 MTU는 1400 바이트이며, ikcp_setmtu를 사용하여 설정할 수 있다. 이 값은 데이터 패킷 병합 및 조각화 시 최대 전송 단위에 영향을 미친다.

4. 최소 RTO:
TCP나 KCP에 상관없이 RTO를 계산할 때 최소 RTO에 대한 제한이 있으며, 계산된 RTO가 40ms 이더라도 기본 RTO가 100ms 이므로 프로토콜은 고속 모드에서 30ms인 100ms 이후에만 패킷 손실을 감지할 수 있으며, 이 값은 수동으로 변경할 수 있다:
```
kcp->rx_minrto = 10;
```


# 문서 인덱싱
프로토콜의 사용과 구성은 매우 간단하며, 대부분의 경우 위의 내용을 읽은 후 기본적으로 사용할 수 있습니다. KCP 메모리 할당자를 변경하는 등 더 세밀한 제어가 필요하거나, 3,500개 이상의 링크와 같이 보다 효율적인 대규모 KCP 링크 스케줄링이 필요하거나, TCP와 더 잘 결합해야 하는 경우, 자세한 내용을 계속 읽으셔도 됩니다:
- [KCP Best Practice](./KCP-Best-Practice.md )
- [Integration with the Existing TCP Server]( )
- [Benchmarks]( )



# [Related Applications](https://github.com/skywind3000/kcp/blob/master/README.en.md#related-applications )



# 프로토콜 비교
네트워크가 혼잡하지 않은 경우 KCP/TCP 성능은 비슷하지만 네트워크 자체가 안정적이지 않으면 패킷 손실과 지터가 불가피할 수 있다(그렇지 않은 경우 다양한 안정적 프로토콜이 존재하는 이유). 거의 이상적인 인트라넷 환경에서는 비슷한 성능을 보이지만 공용 인터넷, 3G / 4G 네트워크 상황 또는 인트라넷 패킷 손실 시뮬레이션을 사용하면 격차가 분명하다. 공용 네트워크는 피크 시간대에 평균 10%에 가까운 패킷 손실이 발생하며, 이는 Wi-Fi / 3G / 4G 네트워크에서 더욱 심해져 전송 정체를 유발한다.

KCP, enet 및 udt에 대한 수평적 평가에 대해 [asio-kcp](https://github.com/libinzhangyuan/asio_kcp )의 저자 장위안에게 감사드리며, 결론은 다음과 같다:

- ASIO-KCP는 와이파이와 전화 네트워크(3G, 4G)에서 좋은 성능을 보인다.
- 실시간 대전 게임에서 가장 먼저 선택되는 것은 KCP 이다.
- 네트워크 지연이 발생할 때 지연이 1초 미만이다. 지연이 발생할 때 enet 보다 3 배 더 좋다.
- 게임이 2초 지연을 허용하는 경우 enet이 좋은 선택이다.
- UDT는 나쁜 생각이다. 항상 여러 초 이상의 지연이 발생하는 나쁜 상황에 빠지게 된다. 그리고 복구가 예상되지 않는다.
- ENET에는 문서가 부족하다는 문제가 있다. 그리고 그것은 당신이 관심을 가질만한 많은 기능을 가지고 있다.
- KCP의 문서는 중국어와 영어가 모두 있다. 좋은 점은 코드로 작성된 함수 세부 사항이 영어라는 것이다. 그리고 좋은 랩퍼인 asio_kcp를 사용할 수 있다.
- kcp는 간단하다. 더 많은 기능을 원한다면 더 많은 코드를 작성할 것이다.
- UDT에는 완벽한 문서가 있다. 내가 느끼기에는 UDT가 다른 것보다 버그가 많을 수 있다.

