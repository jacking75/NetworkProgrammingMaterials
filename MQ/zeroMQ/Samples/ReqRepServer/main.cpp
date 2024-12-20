#define ZMQ_STATIC

#include "zmq.hpp"

#include <string>
#include <iostream>

#ifndef _WIN32
	#include <unistd.h>
#else
	#include <windows.h>
	#define sleep(n)	Sleep(n)
#endif



int main() {
	//  Prepare our context and socket
	zmq::context_t context(1);
	zmq::socket_t socket(context, ZMQ_REP);
	socket.bind("tcp://*:32452");

	while (true) {
		zmq::message_t request;

		//  Wait for next request from client
		socket.recv(&request);
		std::cout << "Received Hello" << std::endl;

		//  Do some 'work'
		sleep(1);

		//  Send reply back to client
		zmq::message_t reply(5);
		memcpy(reply.data(), "World", 5);
		socket.send(reply);
	}
	return 0;
}