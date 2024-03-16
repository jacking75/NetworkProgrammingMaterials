#include <iostream>

#include <zmq.hpp>
#include <zhelpers.hpp>

int main()
{
	std::cout << "Init broker" << std::endl;

	zmq::context_t ctx(1);
	zmq::socket_t frontend(ctx, ZMQ_ROUTER);
	zmq::socket_t backend(ctx, ZMQ_DEALER);

	frontend.bind("tcp://*:11021");
	backend.bind("tcp://*:11022");

	/* proxy raw code
	zmq::pollitem_t items[] = {
		{ static_cast<void*>(frontend), 0, ZMQ_POLLIN, 0 },
		{ static_cast<void*>(backend), 0, ZMQ_POLLIN, 0 },
	};

	while (true)
	{
		zmq::message_t msg;
		int more;

		zmq::poll(items, 2, -1);

		if (items[0].revents & ZMQ_POLLIN)
		{
			while (true)
			{
				frontend.recv(msg);
				std::cout << "Received from client : " << msg.str() << std::endl;

				auto moreSize = sizeof(more);
				frontend.getsockopt(ZMQ_RCVMORE, &more, &moreSize);
				auto flag = more ? zmq::send_flags::sndmore : zmq::send_flags::none;
				backend.send(msg, flag);

				if (!more)
				{
					break;
				}
			}
		}

		if (items[1].revents & ZMQ_POLLIN)
		{
			while (true)
			{
				backend.recv(msg);
				std::cout << "Received from worker : " << msg.str() << std::endl;

				auto moreSize = sizeof(more);
				backend.getsockopt(ZMQ_RCVMORE, &more, &moreSize);
				auto flag = more ? zmq::send_flags::sndmore : zmq::send_flags::none;
				frontend.send(msg, flag);

				if (!more)
				{
					break;
				}
			}
		}
	}
	*/

	zmq::proxy(frontend, backend);

	return 0;
}