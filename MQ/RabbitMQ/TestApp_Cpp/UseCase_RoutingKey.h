#pragma once



#include <chrono>
#include <thread>
#include <string>

#include <amqp.h>
#include <amqp_tcp_socket.h>

// 익스체인지에 할당된 라우팅키를 통해서 메시지를 주고 받는다
namespace UseCaseRoutingKey
{
	class MqBase
	{
	public:
		MqBase() = default;
		~MqBase() = default;

		void Init(amqp_channel_t channelNum, const char* queueName)
		{
			m_ChannelNum = channelNum;
			m_QueueName = amqp_bytes_malloc_dup(amqp_cstring_bytes(queueName));
		}

		void Destory()
		{
			amqp_channel_close(m_pConnState, m_ChannelNum, AMQP_REPLY_SUCCESS);
			amqp_connection_close(m_pConnState, AMQP_REPLY_SUCCESS);
			amqp_destroy_connection(m_pConnState);
		}

		int Connect(char const* hostname, const int port, const char* userID, const char* userPW, const char* vhost, const char* exchangeName)
		{
			m_pConnState = amqp_new_connection();

			auto pSocket = amqp_tcp_socket_new(m_pConnState);
			if (pSocket == nullptr) {
				return 1;
			}

			int status = amqp_socket_open(pSocket, hostname, port);
			if (status) {
				return 2;
			}

			auto loginRet = amqp_login(m_pConnState, vhost, 0, 131072, 0, AMQP_SASL_METHOD_PLAIN, userID, userPW);
			if (loginRet.reply_type != AMQP_RESPONSE_NORMAL)
			{
				return 3;
			}

			amqp_channel_open(m_pConnState, m_ChannelNum);
			auto openChannelRet = amqp_get_rpc_reply(m_pConnState);
			if (openChannelRet.reply_type != AMQP_RESPONSE_NORMAL)
			{
				return 5;
			}


			amqp_exchange_declare(m_pConnState, 1, amqp_cstring_bytes(exchangeName),
				amqp_cstring_bytes("direct"), 0, 0, 0, 0,
				amqp_empty_table);
			auto ret = amqp_get_rpc_reply(m_pConnState);
			if (ret.reply_type != AMQP_RESPONSE_NORMAL)
			{
				return 6;
			}
			
			return 0;
		}

		bool Send(const char* exchangeName, const char* routingKey, const size_t dataSize, char* pData)
		{
			amqp_bytes_t message_bytes;
			message_bytes.len = dataSize;
			message_bytes.bytes = pData;

			auto result = amqp_basic_publish(m_pConnState, m_ChannelNum, amqp_cstring_bytes(exchangeName),
				amqp_cstring_bytes(routingKey), 0, 0, NULL,
				message_bytes);

			if (result < 0)
			{
				return false;
			}

			return true;
		}

		int BindAndComsume(const char* exchangeName, const char* routingKey)
		{
			amqp_table_t amqpEmptyTable; amqpEmptyTable.num_entries = 0;
			amqp_bytes_t amqpEmptyBytes; amqpEmptyBytes.len = 0;

			amqp_queue_declare_ok_t* r = amqp_queue_declare(
				m_pConnState, m_ChannelNum, m_QueueName, 0, 0, 0, 1, amqpEmptyTable);
			auto rpcGetRet = amqp_get_rpc_reply(m_pConnState);
			if (rpcGetRet.reply_type != AMQP_RESPONSE_NORMAL)
			{
				return 10;
			}

			amqp_queue_bind(m_pConnState, 1, m_QueueName,
								amqp_cstring_bytes(exchangeName), amqp_cstring_bytes(routingKey),
								amqp_empty_table);
			auto ret = amqp_get_rpc_reply(m_pConnState);
			if (ret.reply_type != AMQP_RESPONSE_NORMAL)
			{
				return 11;
			}


			amqp_basic_consume(m_pConnState, m_ChannelNum, m_QueueName, amqpEmptyBytes, 0, 1, 0,
				amqpEmptyTable);//AMQP_EMPTY_TABLE  AMQP_EMPTY_BYTES

			auto bindRet = amqp_get_rpc_reply(m_pConnState);
			if (rpcGetRet.reply_type != AMQP_RESPONSE_NORMAL)
			{
				return 12;
			}

			return 0;
		}

		amqp_connection_state_t GetAmqpConnection() { return m_pConnState; }


	protected:
		amqp_connection_state_t m_pConnState = nullptr;
		amqp_channel_t m_ChannelNum = 0;
		amqp_bytes_t m_QueueName;
	};


	class Tester
	{
	public:
		Tester() = default;

		~Tester()
		{

		}


		void Test()
		{
			// 큐 이름보다 익스체인지 이름과 라운팅키 이름이 중요
			auto result = Init("127.0.0.1", "A_TO_B", "B_TO_A", "DBResponse", "DB0_TO_LB0", m_SenderA, m_ReceiverA);
			if (result > 0)
			{
				printf("Error Init 1. %d\n", result);
				return;
			}

			result = Init("127.0.0.1", "C_TO_D", "D_TO_C", "DBRequest", "LB0_TO_DB0", m_SenderB, m_ReceiverB);
			if (result > 0)
			{
				printf("Error Init 2. %d\n", result);
				return;
			}

			printf("TestStart.\n");
			m_IsReceiveThreadRun = true;
			auto thread1 = std::thread(&Tester::ThreadFunc, this, &m_ReceiverA);
			auto thread2 = std::thread(&Tester::ThreadFunc, this, &m_ReceiverB);

			char msg1[] = "AAA";
			m_SenderA.Send("DBRequest", "LB0_TO_DB0", 3, msg1);

			char msg2[] = "Re:AAA";
			m_SenderB.Send("DBResponse", "DB0_TO_LB0", 6, msg2);


			std::this_thread::sleep_for(std::chrono::duration<int>(3));


			printf("TestEnd.\n");
			Destory();

			m_IsReceiveThreadRun = false;

			thread1.join();
			thread2.join();
		}

	private:
		int Init(const char* pHostName, const char* pAToBSendQueueName, const char* pReceivFromMatchQueueName, 
				const char* exchangeName, const char* routingKey, 
				MqBase& sender, MqBase& receiver)
		{
			int result = 0;

			sender.Init(1, pAToBSendQueueName);

			result = sender.Connect(pHostName, 5672, "guest", "guest", "/", exchangeName);
			if (result != 0)
			{
				return result;
			}


			receiver.Init(1, pReceivFromMatchQueueName);

			result = receiver.Connect(pHostName, 5672, "guest", "guest", "/", exchangeName);
			if (result != 0)
			{
				return result;
			}

			result = receiver.BindAndComsume(exchangeName, routingKey);
			if (result != 0)
			{
				return result;
			}


			return 0;
		}

		void Destory()
		{
			m_SenderA.Destory();
			m_ReceiverA.Destory();
			m_SenderB.Destory();
			m_ReceiverB.Destory();
		}

		void ThreadFunc(MqBase* pReceiver)
		{
			while (m_IsReceiveThreadRun)
			{
				amqp_rpc_reply_t res;
				amqp_envelope_t envelope;

				amqp_maybe_release_buffers(pReceiver->GetAmqpConnection());

				res = amqp_consume_message(pReceiver->GetAmqpConnection(), &envelope, NULL, 0);

				if (AMQP_RESPONSE_NORMAL != res.reply_type) {
					break;
				}

				//printf("Delivery %u, exchange %.*s routingkey %.*s\n", (unsigned)envelope.delivery_tag, (int)envelope.exchange.len, (char*)envelope.exchange.bytes, (int)envelope.routing_key.len, (char*)envelope.routing_key.bytes);

				//if (envelope.message.properties._flags & AMQP_BASIC_CONTENT_TYPE_FLAG) {
				//printf("Content-type: %.*s\n", (int)envelope.message.properties.content_type.len, (char*)envelope.message.properties.content_type.bytes);
				//}
				//printf("Body. size: %d\n", (int)envelope.message.body.len);
				//printf("----\n");

				printf("Body: %s\n", (char*)envelope.message.body.bytes);

				amqp_destroy_envelope(&envelope);
			}
		}


		bool m_IsReceiveThreadRun = false;
		MqBase m_SenderA;
		MqBase m_ReceiverA;
		MqBase m_SenderB;
		MqBase m_ReceiverB;
	};
}