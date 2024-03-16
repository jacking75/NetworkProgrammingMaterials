//=====================================================================
//
// test.cpp - kcp 테스트 사례
//
// 설명：
// gcc test.cpp -o test -lstdc++
//
//=====================================================================

#include <stdio.h>
#include <stdlib.h>

#include "test.h"
#include "ikcp.c"


LatencySimulator *vnet;

// 네트워크 시뮬레이션: udp 패킷 전송 시뮬레이션
int udp_output(const char *buf, int len, ikcpcb *kcp, void *user)
{
	union { int id; void *ptr; } parameter;
	parameter.ptr = user;
	vnet->send(parameter.id, buf, len);
	return 0;
}

// 테스트 사례
void test(int mode)
{
	// 시뮬레이션 네트워크 생성: 패킷 손실률 10%, Rtt 60ms~125ms
	vnet = new LatencySimulator(10, 60, 125);

	// 두 개의 엔드포인트가 있는 kcp 객체를 생성하고, 첫 번째 매개변수 conv는 세션 번호이며, 동일한 세션이어야 한다.
	// 마지막은 식별자를 전달하는 데 사용되는 사용자 매개변수이다.
	ikcpcb *kcp1 = ikcp_create(0x11223344, (void*)0);
	ikcpcb *kcp2 = ikcp_create(0x11223344, (void*)1);

   // udp 네트워크 출력 기능을 에뮬레이트하기 위해 kcp의 하위 레벨 출력(이 경우 udp_output)을 설정한다.
	kcp1->output = udp_output;
	kcp2->output = udp_output;

	IUINT32 current = iclock();
	IUINT32 slap = current + 20;
	IUINT32 index = 0;
	IUINT32 next = 0;
	IINT64 sumrtt = 0;
	int count = 0;
	int maxrtt = 0;

   // 윈도우 크기 구성: 패킷 손실 및 재전송을 고려한 평균 지연 200밀리초, 20밀리초마다 패킷 1개,
   // 최대 송수신 윈도우 128개
	ikcp_wndsize(kcp1, 128, 128);
	ikcp_wndsize(kcp2, 128, 128);

	// 테스트 케이스의 패턴 판단하기
	if (mode == 0) {
		// 기본 모드
		ikcp_nodelay(kcp1, 0, 10, 0, 0);
		ikcp_nodelay(kcp2, 0, 10, 0, 0);
	}
	else if (mode == 1) {
		// 흐름 제어가 꺼진 일반 모드 등
		ikcp_nodelay(kcp1, 0, 10, 0, 1);
		ikcp_nodelay(kcp2, 0, 10, 0, 1);
	}	else {
      // 빠른 모드 시작
		// 두 번째 매개변수인 nodelay - 규칙적인 가속을 활성화한 후 시작된다.
		// 세 번째 파라미터인 interval은 내부 처리 클럭으로, 기본적으로 10ms로 설정되어 있다.
		// 네 번째 파라미터 resend는 빠른 재전송 표시기이며, 2로 설정되어 있다.
		// 다섯 번째 매개 변수는 정기적인 흐름 제어를 비활성화할지 여부이며 여기서는 비활성화되어 있다.
		ikcp_nodelay(kcp1, 2, 10, 2, 1);
		ikcp_nodelay(kcp2, 2, 10, 2, 1);
		kcp1->rx_minrto = 10;
		kcp1->fastresend = 1;
	}


	char buffer[2000];
	int hr;

	IUINT32 ts1 = iclock();

	while (1) {
		isleep(1);
		current = iclock();
		ikcp_update(kcp1, iclock());
		ikcp_update(kcp2, iclock());

		// 20ms마다 kcp1이 데이터를 전송합니다.
		for (; current >= slap; slap += 20) {
			((IUINT32*)buffer)[0] = index++;
			((IUINT32*)buffer)[1] = current;

			// 상위 계층 프로토콜 패킷 전송
			ikcp_send(kcp1, buffer, 8);
		}

		// 가상 네트워크 처리: p1->p2에서 udp 패킷이 있는지 감지하기
		while (1) {
			hr = vnet->recv(1, buffer, 2000);
			if (hr < 0) break;
			// P2가 UDP를 수신하면 하위 계층 프로토콜로 KCP2에 입력된다.
			ikcp_input(kcp2, buffer, hr);
		}

		// 가상 네트워크 처리: P2->P1에서 udp 패킷이 있는지 감지하기
		while (1) {
			hr = vnet->recv(0, buffer, 2000);
			if (hr < 0) break;
			// p1이 udp를 수신하면 하위 계층 프로토콜로 kcp1에 입력된다.
			ikcp_input(kcp1, buffer, hr);
		}

		// kcp2는 수신한 모든 패킷을 반환한다.
		while (1) {
			hr = ikcp_recv(kcp2, buffer, 10);
			// 패키지를 받지 않고 종료
			if (hr < 0) break;
			// 패키지를 받으면 다시 보낸다
			ikcp_send(kcp2, buffer, hr);
		}

		// KCP1은 KCP2가 보낸 데이터를 수신한다.
		while (1) {
			hr = ikcp_recv(kcp1, buffer, 10);
			// 패키지를 받지 않고 종료
			if (hr < 0) break;
			IUINT32 sn = *(IUINT32*)(buffer + 0);
			IUINT32 ts = *(IUINT32*)(buffer + 4);
			IUINT32 rtt = current - ts;

			if (sn != next) {
				// 받은 패키지가 연속적이지 않은 경우
				printf("ERROR sn %d<->%d\n", (int)count, (int)next);
				return;
			}

			next++;
			sumrtt += rtt;
			count++;
			if (rtt > (IUINT32)maxrtt) maxrtt = rtt;

			printf("[RECV] mode=%d sn=%d rtt=%d\n", mode, (int)sn, (int)rtt);
		}
		if (next > 1000) break;
	}

	ts1 = iclock() - ts1;

	ikcp_release(kcp1);
	ikcp_release(kcp2);

	const char *names[3] = { "default", "normal", "fast" };
	printf("%s mode result (%dms):\n", names[mode], (int)ts1);
	printf("avgrtt=%d maxrtt=%d tx=%d\n", (int)(sumrtt / count), (int)maxrtt, (int)vnet->tx1);
	printf("press enter to next ...\n");
	char ch; scanf("%c", &ch);
}

int main()
{
	test(0);	// 기본 모드, TCP와 유사: 일반 모드, 빠른 재전송 없음, 일반 흐름 제어
	test(1);	// 흐름 제어가 꺼진 일반 모드 등
	test(2);	// 고속 모드, 모든 스위치 켜짐 및 흐름 제어 꺼짐
	return 0;
}

/*
default mode result (20917ms):
avgrtt=740 maxrtt=1507

normal mode result (20131ms):
avgrtt=156 maxrtt=571

fast mode result (20207ms):
avgrtt=138 maxrtt=392
*/


