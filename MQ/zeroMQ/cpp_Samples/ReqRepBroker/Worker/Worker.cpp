#include <iostream>
#include <string>

#include <zmq.hpp>
#include <zhelpers.hpp>

inline void sleep(int n) 
{
	::Sleep(n); 
}

int main()
{
	std::cout << "Init worker" << std::endl;

	zmq::context_t ctx(1);
	zmq::socket_t worker(ctx, ZMQ_REP);
	worker.connect("tcp://localhost:11022");

	while (true)
	{
		auto rc = s_recv(worker);

		std::cout << "Receive : " << rc << std::endl;

		sleep(1);

		s_send(worker, rc);
	}

	return 0;
}