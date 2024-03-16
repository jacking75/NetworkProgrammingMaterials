#include <iostream>
#include <string>

#include <zmq.hpp>
#include <zhelpers.hpp>

// Exmaple Code : http://zguide.zeromq.org/cpp:rtreq

int main()
{
	std::cout << "Init Server" << std::endl;

	zmq::context_t ctx(1);
	zmq::socket_t broker(ctx, ZMQ_ROUTER);
	broker.bind("tcp://*:11021");

	auto endTime = s_clock() + 5000;

	while (true)
	{
		auto identity = s_recv(broker);
		auto del = s_recv(broker);	// Envelope delimiter
		auto res = s_recv(broker); // Response from worker

		std::cout << "id : " << identity << ", delimiter : " << del << ", res : " << res << std::endl;

		s_sendmore(broker, identity);
		s_sendmore(broker, "");
		s_send(broker, res);
	}

	return 0;
}