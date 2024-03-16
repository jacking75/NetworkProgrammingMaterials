#include <iostream>
#include <string>

#include <zmq.hpp>
#include <zhelpers.hpp>

inline void sleep(int n) 
{
	::Sleep(n);
}

// Example Code : http://zguide.zeromq.org/cpp:hwserver

int main()
{
	s_version();

	std::cout << "Init Server" << std::endl;

	zmq::context_t ctx(1);
	zmq::socket_t zmqSock(ctx, ZMQ_REP);
	zmqSock.bind("tcp://*:11021");

	while (true)
	{
		zmq::message_t req;

		// Can receive non-blocking
		auto res = zmqSock.recv(req, zmq::recv_flags::dontwait);

		if (res)
		{
			std::cout << "Received : " << req.str() << std::endl;

			/* Request message parsing
			auto size = req.size();
			zmq::message_t rep(size);
			std::memcpy(rep.data(), req.data(), size);
			*/

			sleep(1); // some work..

			zmqSock.send(req, zmq::send_flags::none); // Send req message itself, echo server
		}
		else 
		{
			sleep(1);
		}
	}

	return 0;
}
