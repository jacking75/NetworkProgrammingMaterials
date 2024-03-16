#include <iostream>
#include <string>

#include <zmq.hpp>
#include <zhelpers.hpp>

int main()
{
	std::cout << "Init Client" << std::endl;

	std::cout << "Input UID : ";
	intptr_t uid;
	std::cin >> uid;

	zmq::context_t ctx(1);
	zmq::socket_t worker(ctx, ZMQ_REQ);

	auto id = s_set_id(worker, uid);

	std::cout << "Your socket UID : " << id << std::endl;

	worker.connect("tcp://localhost:11021");

	auto total = 1;
	while (true)
	{
		std::string input;
		std::cout << "Write message : ";
		std::cin >> input;
		s_send(worker, input);

		std::string res = s_recv(worker);
		std::cout << "Received : " << res << std::endl;
	}

	return 0;
}