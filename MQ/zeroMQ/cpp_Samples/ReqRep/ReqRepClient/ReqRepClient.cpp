#include <iostream>
#include <string>

#include <zmq.hpp>

// Example code : http://zguide.zeromq.org/cpp:hwclient

int main()
{
	std::cout << "Init Client" << std::endl;

	zmq::context_t ctx(1);
	zmq::socket_t zmqSock(ctx, ZMQ_REQ);
	zmqSock.connect("tcp://localhost:11021");

	while (true)
	{
		std::string input;
		std::cout << "Write message : ";
		std::cin >> input;

		auto size = input.size();

		zmq::message_t req(size);
		std::memcpy(req.data(), input.c_str(), size);

		zmqSock.send(req, zmq::send_flags::none);

		zmq::message_t rep;
		zmqSock.recv(rep);

		std::cout << "Echo : " << rep.str() << std::endl;
	}

	return 0;
}