# RabbitMQ - C++
- [여기](https://www.rabbitmq.com/devtools.html)에 가면 C++ 클라이언트 라이브러리 리스트가 있다.
- 현재(2019.08.22) 4개의 라이브러리가 등록 되어 있는데 [RabbitMQ C client](https://github.com/alanxz/rabbitmq-c) 이외는 크로스 플랫폼 지원을 못하거나 Boost나 libuv 등의 또 다른 라이브러리를 필요로 한다.
- RabbitMQ C client는 C 언어로 되어 있어서 사용이 불편하지만 크로스 플랫폼을 지원하고, 이것만으로 RabbitMQ 서버와 통신이 가능하다.
  
    
## RabbitMQ C client
라이브러리는 빌드는 CMake를 사용한다.  
Windows의 경우는 CMake Gui 툴을 사용하여 Visual Studio 솔루션 파일을 생성 후 빌드해서 사용한다.
  
   
사용 방법은 예제 코드를 참고한다



