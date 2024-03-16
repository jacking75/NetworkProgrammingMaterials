// TestApp_Cpp.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib,"mswsock.lib")

#include <iostream>

#include "UseCase_Queue.h"
#include "UseCase_RoutingKey.h"


int main()
{
    std::cout << "MQ Test!\n";

	//UseCase1On1Queue::Tester test1;
	//test1.Test();

	UseCaseRoutingKey::Tester test2;
	test2.Test();


	getchar();
	return 0;
}

