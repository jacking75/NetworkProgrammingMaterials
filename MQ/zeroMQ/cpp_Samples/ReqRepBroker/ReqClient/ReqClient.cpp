#include <iostream>
#include <string>

#include <zmq.hpp>
#include <zhelpers.hpp>

int main()
{
	std::cout << "Init Client" << std::endl;

	zmq::context_t ctx(1);
	zmq::socket_t clientSock(ctx, ZMQ_REQ);
	clientSock.connect("tcp://localhost:11021");

	while (true)
	{
		std::string input;
		std::cout << "Write message : ";
		std::cin >> input;

		s_send(clientSock, input);

		auto rc = s_recv(clientSock);

		std::cout << "Receive : " << rc << std::endl;
	}

	return 0;
}