# Toxiproxy 
- 네트워크 상태를 시뮬레이션 하기 위한 프레임워크.
- Toxiproxy는 테스트를 통해 애플리케이션의 단일 실패 지점이 없음을 증명하는데 필요한 도구이다. 
- 테스트, CI 및 개발 환경에서 작동하도록 특별히 제작되어 연결에 대한 결정적 변조를 지원하지만 무작위 혼돈 및 사용자 지정을 지원한다. 
- 2014년 10월부터 Shopify의 모든 개발 및 테스트 환경에서 성공적으로 사용하고 있다. 자세한 내용 [여기](https://shopifyengineering.myshopify.com/blogs/engineering/building-and-testing-resilient-ruby-on-rails-applications )  
  

## Toxiproxy 사용법은 두 부분으로 구성
- Go로 작성된 TCP 프록시(이 저장소에 포함됨) 
- 클라이언트 라이브러리로 조정하기. 
- 모든 테스트 연결이 Toxiproxy를 통과하도록 애플리케이션을 구성한 다음 HTTP를 통해 상태를 조작할 수 있다. 
  
예를 들어 Ruby 클라이언트의 MySQL 응답에 1000ms의 지연 시간을 추가하려면 아래처럼한다.  
```
Toxiproxy[:mysql_master].downstream(:latency, latency: 1000).apply do
  Shop.first # this takes at least 1s
end
```
  
모든 Redis 인스턴스 중단 시키기  
```
Toxiproxy[/redis/].down do
  Shop.first # this will throw an exception
end
```  
  

## 지원하는 클라이언트 라이브러리 
- toxiproxy-ruby
- toxiproxy-go
- toxiproxy-python
- toxiproxy.net
- toxiproxy-php-client
- toxiproxy-node-client
- toxiproxy-java
- toxiproxy-haskell
- toxiproxy-rust
- toxiproxy-elixir  


## 사용하기
### 설치하기
우분투: 최신 버전을 설치한다.  
```
$ wget -O toxiproxy-2.1.4.deb https://github.com/Shopify/toxiproxy/releases/download/v2.1.4/toxiproxy_2.1.4_amd64.deb
$ sudo dpkg -i toxiproxy-2.1.4.deb
$ sudo service toxiproxy start
```  
  
OS X: 홈브루로 설치한다  
```
$ brew tap shopify/shopify
$ brew install toxiproxy
```
  
Windows: [릴리즈 페이지](https://github.com/Shopify/toxiproxy/releases)에서 최신 버전의 `toxiproxy-server-windows-amd64.exe`를 설치한다  
  
Docker: Toxiproxy is available on Github container registry. Old versions <= 2.1.4 are available on on Docker Hub.  
```
$ docker pull ghcr.io/shopify/toxiproxy
$ docker run --rm -it ghcr.io/shopify/toxiproxy
```  
If using Toxiproxy from the host rather than other containers, enable host networking with --net=host.  
```
$ docker run --rm --entrypoint="/toxiproxy-cli" -it ghcr.io/shopify/toxiproxy list
```
  

### Toxiproxy 설정
라이브러리에서 사용할 때 애플리케이션 초기 단계에서 네트워크 주소를 맵핑해 놓아야 한다. 아래는 루비의 예이다.  
```
# Make sure `shopify_test_redis_master` and `shopify_test_mysql_master` are
# present in Toxiproxy
Toxiproxy.populate([
  {
    name: "shopify_test_redis_master",
    listen: "127.0.0.1:22220",
    upstream: "127.0.0.1:6379"
  },
  {
    name: "shopify_test_mysql_master",
    listen: "127.0.0.1:24220",
    upstream: "127.0.0.1:3306"
  }
])
```
    
또는 CLI로 아래처럼 맵핑이 가능하다  
```
toxiproxy-cli create -l localhost:26379 -u localhost:6379 shopify_test_redis_master
```  
대규모 프로젝트의 경우 위처럼 직접 입력하는 것보다는 json 형식의 옵션 파일을 사용하는 것이 좋다  
예) config/toxiproxy.json  
```
[
  {
    "name": "web_dev_frontend_1",
    "listen": "[::]:18080",
    "upstream": "webapp.domain:8080",
    "enabled": true
  },
  {
    "name": "web_dev_mysql_1",
    "listen": "[::]:13306",
    "upstream": "database.domain:3306",
    "enabled": true
  }
]
```

### Toxiproxy 사용하기
1. 라이브러리를 사용하여 Redis 접속하기  
  
```
redis = Redis.new(port: 22220)

Toxiproxy[:shopify_test_redis_master].downstream(:latency, latency: 1000).apply do
  redis.get("test") # will take 1s
end
```
  
2. CLI 사용  
```
toxiproxy-cli toxic add -t latency -a latency=1000 shopify_test_redis_master
```
  
CLI 파라미터    
- toxicName(약어 n)
- type(약어 t)
    - latency, jitter, down, bandwidth, slow_close, timeout, reset_peer, slicer, limit_data    
- toxicity(약어 tox)
    - 랜덤 테스트를 할 때 사용. 예를들어 50% 확률로 지연 발생을 원할 때
    - 1.0 이 100% 이다.
- attribute(약어 a)
    - key-value. 예)레이턴시 `-a latency=1000`
- upstream(약어 u)
- downstream(약어 d)
    
<br>  
      
latency  
Add a delay to all data going through the proxy. The delay is equal to latency +/- jitter.  
Attributes:  
- latency: time in milliseconds  
- jitter: time in milliseconds  
  
down  
Bringing a service down is not technically a toxic in the implementation of Toxiproxy. This is done by POSTing to /proxies/{proxy} and setting the enabled field to false.  
  
bandwidth  
Limit a connection to a maximum number of kilobytes per second.  
Attributes:  
- rate: rate in KB/s
  
slow_close  
Delay the TCP socket from closing until delay has elapsed.  
Attributes:  
- delay: time in milliseconds  
  
timeout  
Stops all data from getting through, and closes the connection after timeout. If timeout is 0, the connection won't close, and data will be delayed until the toxic is removed.  
Attributes:  
- timeout: time in milliseconds  
  
reset_peer  
Simulate TCP RESET (Connection reset by peer) on the connections by closing the stub Input immediately or after a timeout.  
Attributes:  
- timeout: time in milliseconds  
  
slicer  
Slices TCP data up into small bits, optionally adding a delay between each sliced "packet".  
Attributes:  
- average_size: size in bytes of an average packet     
- size_variation: variation in bytes of an average packet (should be smaller than average_size)  
- delay: time in microseconds to delay each packet by  
  
limit_data  
Closes connection when transmitted data exceeded limit.  
- bytes: number of bytes it should transmit before connection is closed  
 
<br>  
<br>  
  
####  Toxics
[HTTP api](https://github.com/Shopify/toxiproxy#toxics )로 조작할 수 있고, 기능을 새로 만들 수도 있다. [CREATING_TOXICS.md](https://github.com/Shopify/toxiproxy/blob/master/CREATING_TOXICS.md)
  

#### HTTP API 
클라이언트에서 Toxiproxy daemon와 통신을 할 때는 HTTP로 통신하고, 8474 포트를 사용한다.  
https://github.com/Shopify/toxiproxy#http-api  
  
  
#### [CLI Example](https://github.com/Shopify/toxiproxy#cli-example )
  

## 테스트 시나리오 
    
### 에코서버에서 다운스트림 지연 발생 시키기
- SuperSocketLite의 Tutorial에 있는 테스트클라이언트와 EchoServer를 사용한다
- `toxiproxy-server.exe`를 실행한다
- `toxiproxy-cli.exe`를 실행한다
    - 프록시를 만든다. 클라이언트는 11021로 접속하면 toxiproxy-server가 32452로 중계한다.
    - `toxiproxy-cli create -l localhost:11021 -u localhost:32452 echoServer`
- EchoServer를 실행한다. 포트번호는 32452
- 테스트클라이언트를 실행한다. 접속 후 에코가 잘 되는지 확인한다
- toxiproxy에 지연을 만든다. 1초 지연을 만든다
    - `toxiproxy-cli toxic add -t latency -a latency=1000 echoServer`
- 테스트클라이언트에서 에코를 보내면 받는데 1초가 걸린다.
- 지연을 제거 하고 싶다면 아래처럼 한다.
    - `toxiproxy-cli toxic remove -n latency_downstream echoServer`
- 업스트림 지연: 만약 지연으로 서버쪽에서 늦게 받게 하고 싶다면 아래처럼 한다
    - `toxiproxy-cli toxic add -u -t latency -a latency=1000 echoServer`
  





