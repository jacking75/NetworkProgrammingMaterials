# ZeroMq - C++
=========  
  
## 빌드
1. Corelib https://github.com/zeromq/libzmq 에서 소스를 다운로드 한다.
2. 압축을 푼 후 builds 디렉토리로 이동, 다시 msvc 디렉토리로 이동한다.  
3. Visual Studio 버전에 맞는 디렉토리로 이동하여 솔루션 파일을 연다.
4. 일괄빌드에서 원하는 프로젝트를 빌드한다.  
   static lib의 경우 런타임 라이브러리가 'MT', 'MTD'로 되어 있으므로 가능하면 'MD', 'MDD'로 변경한다.
5. bin 디렉토리에 가면 라이브러리가 만들어져 있다.


## Visual C++ 에서 사용하기
1. zeromq는 C 라이브러리이므로 C++로 사용하고 싶다면 cppzmq 라이브러리를 사용한다.  
   [Github cppzmp](https://github.com/zeromq/cppzmq)
2. cppzmq를 사용한다면 zmq.hpp, zmq_addon.hpp를 include 한다.
3. 솔루션 설정에 libzmq의 포함 디렉토리와 라이브러리 디렉토리를 추가한다.  
   static lib의 경우 디버그/릴리즈 버전은 디렉토리만 다르고 파일이름은 같다.
4. 솔루션 설정의 링크 부분에 ws2_32.lib, IPHLPAPI.lib, libzmq.lib을 추가한다.
5. static lib을 사용한다면 코드 상단에 #define ZMQ_STATIC 을 추가한다.  
  
예제 코드  
```
#define ZMQ_STATIC

#include "zmq.hpp"

#include <string>
#include <iostream>

#ifndef _WIN32
    #include <unistd.h>
#else
    #include <windows.h>
    #define sleep(n)    Sleep(n)
#endif



int main() {
    //  Prepare our context and socket
    zmq::context_t context(1);
    zmq::socket_t socket(context, ZMQ_REP);
    socket.bind("tcp://*:5555");

    while (true) {
        zmq::message_t request;

        //  Wait for next request from client
        socket.recv(request);
        std::cout << "Received Hello" << std::endl;

        //  Do some 'work'
        sleep(1);

        //  Send reply back to client
        zmq::message_t reply(5);
        memcpy(reply.data(), "World", 5);
        socket.send(reply, zmq::send_flags::none);
    }
    return 0;
}
```
     
  
## send 함수의 반환 값을 확인해야 하나?
확인해야 한다.  
 zmq_send가 오류를 EAGAIN으로 설정한 경우 send는 false를 돌려준다.  
 이것이 언제 일어날지를 이해하기 위해 zmq_socket과 zmq_setsockopt에 대한 문서를 읽어 보기 바란다.  
 사용하는 소켓이 EAGAIN을 발생시킬 가능성이 있고, 이 상태를 처리하고 싶은 경우는 반환 값을 확인해야한다. 예를 들어, 메시지를 큐에 넣어 두고 나중에 다시 시도하여 해결할 수 있다
   
```
inline bool send (message_t &msg_, int flags_ = 0)
{
    int rc = zmq_send (ptr, &msg_, flags_);
    if (rc == 0)
        return true;
    if (rc == -1 && zmq_errno () == EAGAIN)
        return false;
    throw error_t ();
}
```
  
  
## 스트림 소켓 
- ZeroMQ를 사용하지 않는 소켓과 통신할 수 있다.
  
```
bool GWSAgent::CreateStreamSocket(unsigned short port)
{
	if (_socket[2] != 0)
		return false;

	_socket[2] = zmq_socket(_ctx, ZMQ_STREAM);

	int option = GWS_ZMQ_BACKLOG_COUNT;
	if (zmq_setsockopt(_socket[2], ZMQ_BACKLOG, &option, sizeof(option)) != 0)
	{
		_LOG4(ERR, "$$Start Failed : set ZMQ_BACKLOG error");
		return false;
	}

	if (Bind(_socket[2], port, port) == 0)
	{
		_LOG4(ERR, "$$Start Failed : Bind Client Socket error");
		return false;
	}

	return true;
}

unsigned short GWSAgent::Bind(void* socket, unsigned short start_port, unsigned short end_port)
{
	// 소켓 타입 확인
	int type = 0;
	size_t type_size = sizeof(type);
	if (zmq_getsockopt(socket, ZMQ_TYPE, &type, &type_size) != 0)
		return 0;

	// 라우터 소켓인 경우 이름설정 , Connect/Disconnect Event , Handover
	if (type == ZMQ_ROUTER)
	{
		unsigned long long sid = HostToNetwork<unsigned long long>(_sid.UN.Value);
		if (zmq_setsockopt(socket, ZMQ_IDENTITY, &sid, sizeof(sid)) != 0)
			return 0;

		int option = 1;
		if (zmq_setsockopt(socket, ZMQ_PROBE_ROUTER, &option, sizeof(option)) != 0)
			return 0;

		if (zmq_setsockopt(socket, ZMQ_ROUTER_MANDATORY, &option, sizeof(option)) != 0) // 대상이 없을때 Block
			return 0;

		if (zmq_setsockopt(socket, ZMQ_ROUTER_HANDOVER, &option, sizeof(option)) != 0) // 동일 이름일때 두번째가 덮어 씀
			return 0;

		if (zmq_setsockopt(socket, ZMQ_IMMEDIATE, &option, sizeof(option)) != 0) // 연결이 끊어져 있을때 Drop
			return 0;

		option = 1000;
		if (zmq_setsockopt(socket, ZMQ_HANDSHAKE_IVL, &option, sizeof(option)) != 0) // 접속성립 대기 시간 (밀리초)
			return 0;

		option = 100;
		if (zmq_setsockopt(socket, ZMQ_RECONNECT_IVL_MAX, &option, sizeof(option)) != 0) // 재접속 주기 최대 값 (밀리초)
			return 0;

		option = 1;
		if (zmq_setsockopt(socket, ZMQ_TCP_KEEPALIVE, &option, sizeof(option)) != 0) // TCP Heartbeat 처리 사용
			return 0;

		option = 1;
		if (zmq_setsockopt(socket, ZMQ_TCP_KEEPALIVE_INTVL, &option, sizeof(option)) != 0)// TCP Heartbeat 전송 주기
			return 0;

		option = GWS_MAX_ZMQ_SNDHWM_SZIE;
		if (zmq_setsockopt(socket, ZMQ_SNDHWM, &option, sizeof(option)) != 0)// 전송큐
			return 0;

		option = GWS_MAX_ZMQ_RCVHWM_SZIE;
		if (zmq_setsockopt(socket, ZMQ_RCVHWM, &option, sizeof(option)) != 0)// 수신큐
			return 0;

		// 윈도우에서는 무시됨
		{
			option = 5;
			if (zmq_setsockopt(socket, ZMQ_TCP_KEEPALIVE_CNT, &option, sizeof(option)) != 0) // TCP Heartbeat 끊어지기전 시도 횟수 (윈도우 10회)
				return 0;

			option = 1;
			if (zmq_setsockopt(socket, ZMQ_TCP_KEEPALIVE_IDLE, &option, sizeof(option)) != 0) // TCP Heartbeat 끊어지는 시간 (초)
				return 0;
		}
	}

	// 범위 바인딩
	char address[256] = { 0, };
	for (int i = start_port; i <= end_port; i++)
	{
		sprintf(address, "tcp://*:%d", i);
		if (zmq_bind(socket, address) == 0)
		{
			return i;
		}
	}
	return 0;
}


void MainWorker::Stream()
{
	//_LOG4(INF, "Stream");

	statistics_check = std::chrono::steady_clock::now();
	stream_nowork = true;
	workcnt = 0;
	while (workcnt < GWS_MAXWORK_COUNT)
	{
		id_size = zmq_recv(agent->GetSocket(GWS::SOCKETTYPE_STREAM), id, GWS_IDBUFFER_SIZE, ZMQ_DONTWAIT);// Identify
		if (id_size > 0)
		{
			workcnt++;
			stream_nowork = false;

			zmq_msg_t* raw = new zmq_msg_t();
			zmq_msg_init(raw);
			unsigned int idnum = UserSocket::ConvertToSocketID(id);
			auto ctx = GetUserSocket(idnum);
			if (ctx == 0)
			{
				new_connection = true;
			}
			else
			{
				new_connection = false;
				ctx->SetLastAccess();
			}

			raw_size = zmq_msg_recv(raw, agent->GetSocket(GWS::SOCKETTYPE_STREAM), 0);// Body

			_LOG4(DBG, "Perform STREAM Recv, " << idnum << " - " << raw_size << "(" << new_connection<<")");

			if (raw_size < 0)
			{
				_LOG4(ERR, "Perform STREAM Error, " << idnum);

				if (ctx)
					ctx->Disconnect(false);
			}
			else if (raw_size == 0)
			{
				if (new_connection == true)
				{
					ctx = std::make_shared<UserSocket>(id, idnum);
					MainWorker::AddUserSocket(idnum, ctx);
					ctx->OnUserAccept();

					agent->AddTimer(new GWS::CheckGatewayPostee(idnum), agent->_session_timeout);
				}
				else
					ctx->Disconnect(false);
			}
			else if (new_connection == false)
			{
				ctx->_input_stream.Write((unsigned char*)zmq_msg_data(raw), (int)raw_size);
				ctx->UserProcess(ctx->_input_stream);
			}
			else
			{
				if (ctx)
					ctx->Disconnect(false);

				_LOG4(INF, "Perform STREAM Recv After Close, " << idnum);
			}

			zmq_msg_close(raw);
			delete raw;
		}
		else
		{
			break;
		}
	}

	while (g_mainWorker->_deleteList.size() > 0)
	{
		g_mainWorker->_deleteList.pop();
	}

	if (workcnt == GWS_MAXWORK_COUNT)
		statistics_stream_workmax++;

	statistics_stream_workcnt += workcnt;
	statistics_stream_cnt++;
	statistics_stream_total += (double)std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now() - statistics_check).count();
}


void UserSocket::SendRawToClient(unsigned char *buffer, int size)
{
	if (-1 == zmq_send(GWS::GWSAgent::Instance().GetSocket(GWS::SOCKETTYPE_STREAM), &_id_raw, sizeof(_id_raw), ZMQ_SNDMORE | ZMQ_DONTWAIT))
	{
		_LOG4(ERR, "SendRawToClient Packet Unknown Socket " << GetSocketID() << " - " << zmq_strerror(zmq_errno()));
		Disconnect(false);
		return;
	}

	if (-1 == zmq_send(GWS::GWSAgent::Instance().GetSocket(GWS::SOCKETTYPE_STREAM), buffer, size, ZMQ_SNDMORE | ZMQ_DONTWAIT))
	{
		_LOG4(ERR, "SendRawToClient Packet Failed " << GetSocketID() << " - " << zmq_strerror(zmq_errno()));
		Disconnect(false);
	}
}
```
  
    
## 라이브러리 
- [0mq 'highlevel' C++ bindings](http://zeromq.github.io/zmqpp )
    - https://github.com/zeromq/zmqpp  
- [Integrating Boost Asio with ZeroMQ](http://rodgert.github.io/2014/12/24/boost-asio-and-zeromq-pt1/  )
- [C++ language binding library integrating ZeroMQ with Boost Asio](https://github.com/zeromq/azmq  )
  