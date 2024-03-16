# KCP C#
[출처](https://github.com/KumoKyaku/KCP )
DeepL로 번역한 것을 정리한 것이다.  
  
## 기능: 
- 비동기 API 표준 인터페이스 IKcpIO.cs
    - ValueTask Recv(IBufferWriter writer, object option = null);
	- ValueTask Output(IBufferWriter writer, object option = null).
	- 기본 구현과 함께 제공된다. kcpIO.cs
- 사용자 정의 고성능 구현을 위한 kcpSegment 일반화.
    - KcpCore<Segment> 여기서 세그먼트 : IKcpSegment
	- KcpIO<Segment>: KcpCore<Segment>, IKcpIO where Segment : IKcpSegment
	- Kcp<Segment>: KcpCore<Segment> where Segment:IKcpSegment
  
  
## 설명: 
- 내부적으로 안전하지 않은 코드와 관리되지 않는 메모리를 사용하므로 GC에 스트레스를 주지 않는다.
- 사용자 정의 메모리 관리를 지원하며, 안전하지 않은 모드를 사용하지 않으려면 메모리 풀을 사용할 수 있다.
- 출력 콜백 및 TryRecv 함수의 경우. 렌트버퍼 콜백을 사용하여 외부에 메모리를 할당한다. IMemoryOwner 사용법을 참조한다.
- Spna<byte> 지원
  
  
## 스레드 안전 
간단히 말해:  
스레드 1에서 스레드 2가 Recv/Update를 호출하는 동안에는 스레드 2에서 Recv/Update를 호출할 수 없다. 이 함수는 내부적으로 많은 공유 데이터 구조를 사용하므로 함수에 잠금을 설정하면 성능에 심각한 영향을 미치게 된다.  
스레드 2가 Send/Input을 호출하는 동안 스레드 1에서 Send/Input을 호출할 수 있다. 함수 내부에 잠금이 있다.  
- Send와 Input은 스레드 수에 상관없이 동시에 호출할 수 있다.  
  여러 스레드에서 동시에 메시지를 보내도 안전하므로 비동기 함수에서 메시지를 안전하게 보낼 수 있다.  
- 그러나 Recv와 Update는 여러 스레드에서 동시에 호출할 수 없다.  
  이름이 같은 메서드는 한 번에 하나의 스레드에서만 호출할 수 있으며, 그렇지 않으면 다중 스레드 오류가 발생한다.  
  
   
## C 버전과 관련된 일부 변경 사항이 있다: 
<pre>
분산 변화	   버전 C	      			C# 버전
데이터 구조		
acklist	 			배열				동시 대기열
snd_queue	양방향 링크 목록	동시 대기열
snd_buf		양방향 링크 목록	LinkedList
rcv_buf		양방향 링크 목록	LinkedList
rcv_queue	양방향 링크 목록	목록
--------------	--------------	--------------
콜백 함수							KCP가 메모리를 필요로 할 때 외부에서 요청할 수 있도록 RentBuffer 콜백을 추가했다.
멀티 스레드							스레드 안전성이 추가되었습니다.
스트리밍 모드						데이터 구조 변경으로 인해 스트리밍 모드가 제거되었다.
간격 최소 간격	10ms				0ms(특수 조건에서 CPU 풀로드 허용)
--------------	--------------	--------------
API 변경 사항		
										스몰엔드 및 라지엔드 인코딩 설정을 추가한다. 기본 스몰 엔드 인코딩이다.  
										Recv가 가능할 때 한 번만 엿볼 수 있도록 TryRecv 함수를 추가한다.  
				IKCP_ACK_PUSH		이 기능 제거(인라인)
				IKCP_ACK_GET		이 기능 제거(인라인)
</pre>  
  