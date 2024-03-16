# NATS

## 개요
- neural autonomic transport system
- GO 언어로 구현 된 오픈 소스 메시징 시스템
- NATS Messaging
    - https://nats.io/
    - https://en.wikipedia.org/wiki/NATS_Messaging
- 메시징 서비스
    - Publish/Subscribe
    - Request/Reply
    - Queueing
- Streaming
    - 인메모리(+외부 저장소)로 메시지를 저장하고, 클라이언트에 최소 1회는 메시지를 보낼 수 있다.
	- At-least-once
	- Publisher/Subscriber Rate Limit NATS 스트리밍을 구현하는 서버가 NATS 스트리밍 서버로 NATS 서버를 베이스로 구현되어 있다. 내부에서는 Protocol Buffers도 사용하고 있다.
- 프로토콜은 텍스트 베이스로 간단
- 느린 클라이언트를 자동으로 접속 끊음(Auto Pruning of Client)
    - Ping-Pong 타임 아웃 또는 패딩 메시지가 한계치를 넘었을 때(10MB)
- 클러스터링
- monitoring
    - http로 엔드 포인트를 가지고 있다( json )
    - top 명령어
    - Prometheus에에서 exporter가 있다
        - NATS Prometheus Exporter
- Lgging
    - 옵션으로 로그 출력이 가능
- 클라이언트 라이브러리는 프로그래밍 언어의 대부분을 지원. https://nats.io/download/ 
- 성숙한 콘텐츠
    
  
## 각 MQ별 성능 비교
![NATS](./images/011.PNG)  
출처: https://nats.io/about/  
  
  
## 실행
- [공식 문서](https://nats-io.github.io/docs/nats_server/running.html )
- 크로스 플랫폼 지원  
- docker로 한방에 실행 가능
- 실행 파일 다운로드 후 실행
- 직접 코드 빌드 후 실행
- 실행 파일 및 코드 [다운로드](https://github.com/nats-io/nats-server/releases/ )
- NATS를 실행하는 서버는 gnatsd 라고 부른다.
- 서버의 port 번호
    - :4222: client port
    - :6222: route port
    - :8222: http port

### 명령어
- 디폴트 서버 실행하기  
	<PRE>
	gnatsd
	</PRE>
- 모니터링 기능 활성화 하기
	<PRE>
	gnatsd -m 8222
	</PRE>  
	8222 포트를 사용해 모니터링 기능을 제공한다
- 단일 사용자/패스워드 설정하기
	<PRE>
	gnatsd -DV --user someuser --pass somepassword
	</PRE> 
- 구성 파일에서 단일 사용자/패스워드 설정하기
	<PRE>
	authorization
	{
		user     : user1
		password : password1
		timeout  : 1
	}
	</PRE>	 
- 구성 파일에서 라우터 연결용 단일 사용자/패스워드 설정하기
	<PRE>
	cluster
	{
		authorization
		{
			user      : user1
			password  : password1
			timeout   : 0.5
		 }
	}
	</PRE>	 
- 구성 파일에서 복수 사용자/패스워드 설정하기
	<PRE>
	authorization
	{
		users =
		[
			{ user : user1, password : password1 }
			{ user : user2, password : password2 }
		]
	}
	</PRE>	 
- 사용자/패스워드를 사용해 서버 연결 문자열 만들기
	<PRE>
	nats://user1:password1@192.168.29.100:4222
	</PRE>	 
- 구성 파일을 사용해 서버 실행하기
	<PRE>
	d:\NATS\Server01\Bin\gnatsd.exe --config d:\NATS\Server01\Config\server.conf
	</PRE>	 
- 서버 권한 설정하기
	<PRE>
	authorization
	{
		ADMIN =
		{
			publish    = ">"
			subscriber = ">"
		}

		REQUESTOR =
		{
			publish    = ["req.foo", "req.bar"]
			subscriber = "_INBOX.>"
		}

		RESPONDER =
		{
			publish    = "_INBOX.>"
			subscriber = ["req.foo", "req.bar"]
		}

		DEFAULT_PERMISSIONS =
		{
			publish    = "SANDBOX.*"
			subscriber = ["PUBLIC.>", "_INBOX.>"]
		}

		PASS : abcdefghijklmnopqrstuvwxyz0123456789

		users =
		{
			{ user : joe    , password : foo  , permissions : $ADMIN     }
			{ user : alice  , password : bar  , permissions : $REQUESTOR }
			{ user : bob    , password : $PASS, permissions : $RESPONDER }
			{ user : charlie, password : bar                             }
		}
	}
	</PRE>	 
	- joe는 ADMIN 권한을 갖는다. 모든 주제를 발행하고 구독할 수 있다. 
	- alice는 REQUESTOR 권한을 갖는다. "req.foo", "req.bar" 주제를 발행할 수 있다. "_INBOX.>" 주제를 구독할 수 있다.  
	- bob는 RESPONDER 권한을 갖는다. "_INBOX.>" 주제를 발행할 수 있다. "req.foo", "req.bar" 주제를 구독할 수 있다.  
	- charlie는 권한이 없어서 디폴트 권한을 갖는다.  
    	  	
  

## 핵심 설명  
[출처](https://www.slideshare.net/hayahitokawamitsu/with-nats-with-kubernetes )    
  
![NATS](./images/001.PNG)    
![NATS](./images/002.PNG)    
![NATS](./images/003.PNG)    
![NATS](./images/004.PNG)    
![NATS](./images/005.PNG)    
![NATS](./images/006.PNG)    
![NATS](./images/007.PNG)    
![NATS](./images/008.PNG)    
![NATS](./images/009.PNG)    
![NATS](./images/010.PNG)    
  

## 벤치 마크에서 성능에 영향을 주는 요소
- 클러스터를 구성하는 NATS 서버의 대수
- 클러스터를 구성하는 VM 시스템 사양
- 클러스터의 네트워크 대역
- 게시자 수
- 구독자 수
- 게시자를 실행하는 VM 시스템 사양
- 구독자가 실행하는 VM 시스템 사양
- 발행되는 메시지의 수
- 발행되는 메시지의 크기  
  
  
## NATS 프로토콜의 기본
### 서브젝트 이름
메시지의 전달 대상을 결정하는 ID 같은 것. 대소 문자를 구분하고 . 로 토큰을 구분한다.  
  
`subj.hello.one` `SUBJ.HELLO.one` 등등.  
  
서브젝트에 와일드 카드를 사용할 수 있다.  
- `*` : 뭐든지 일치한다. `subj.hello.* SUBJ.*`
- `>` : `subj.>`의 경우 `subj.hello` `subj.hoge.fuga`에 매치하지만 subj만으로는 매치하지 않는다.
  
`*` `>` 모두 토큰의 일부에 사용할 수 없다. `su*j.hello`는 사용할 수 없다.(엄밀히 말하면 지정할 수는 있지만)  
  

### 프로토콜 메시지
NATS의 프로토콜에 규정 되어 있는 메시지는 아래와 같다.
  
- INFO : 클라이언트와 서버에서 TCP/IP 연결이 확립된 후에 서버에서 전송 되는 정보
- CONNECT : 클라이언트에서 연결 정보를 보낸다
- PUB : 메시지 전달하기
- SUB : 메시지 구독하기
- UNSUB : 메시지 구독 중지
- MSG : 클라이언트의 실제 전달 메시지
- PING/PONG: 이른바 ping-pong
- +OK: 프로토콜 메시지에 제대로 응답. CONNECT에서 verbose 모드를 false로 결정하는 경우 생략된다.(대부분의 클라이언트에서 기본 OFF로 하고 있다)
- ERR : 오류가 발생했을 때 서버로부터 통지 되는 프로토콜
  
프로토콜에 대한 자세한 내용은 설명을 생략한다.   
매우 단순하기 때문에 [문서](https://nats-io.github.io/docs/nats_protocol/nats-protocol.html )를 읽으면 자세한 것은 알 수있다.  


## 메시징 모델
아래의 예제 코드는 Go를 사용한다.  
[Go 예제 코드](https://github.com/nats-io/nats.go/tree/master/examples )  
[go-nats-examples](https://github.com/nats-io/go-nats-examples)  
  
### Publish/Subscribe 
NATS의 Pub/Sub는 Redis에서 사용하는 일반적인 "토픽 기반"의 Pub/Sub 이다. NATS는 토픽을 Subject 라고 부른다.  
NATS의 Subject는 계층 구조를 취할 수 있고, .(점)로 구분하여 표현한다.   
Subscriber는 이 계층 구조의 일부가 와일드 카드로 *(별표)를 사용할 수 있다.  
또한 > 를 사용하여 하위 계층 모두를 표현할 수 있다.   
  
예를 들어, Subscriber가 foo.bar.*를 구독하는 경우 foo.bar.baz 및 버튼 foo.bar.qux 등의 메시지를 받을 수 있지만 foo.bar.baz.qux은 받을 수 없다. 한편, foo.bar.> 를 구독하는 경우 foo.bar.baz.qux도 받을 수 있다.  
  
**Publisher**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	if err := nc.Publish("subjectFoo", []byte("bodyBar")); err != nil {
		log.Fatal(err)
	}
}
```
  
**Subscriber**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	sub, err := nc.Subscribe("subjectFoo", callback)
	if err != nil {
		log.Fatal(err)
	}
	log.Printf("Subject: %s", sub.Subject)
	log.Printf("Queue: %s", sub.Queue)
	ch := make(chan struct{})
	<-ch
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
}
```
   
동기 처리로 하고 싶은 경우는 *nats.Conn.SubscribeSync를 사용한다  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	log.Printf("Subject: %s", sub.Subject)
	log.Printf("Queue: %s", sub.Queue)
    
    sub, err := nc.Subscribe("subjectFoo", callback)
	if err != nil {
		log.Fatal(err)
	}
    
    for {
        msg, err := sub.NextMsgWithContext(context.Background())
        if err != nil {
            log.Fatal(err)
        }
        callback(msg)
    }
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
}
```
  
### Request/Reply
Request/Reply 모델은 대부분 Pub/Sub 모델이지만 Subscriber 측에서 회신을 기대한다는 점이 다르다.  
메시지를 보낼 때 함께 전달된 Subject에 Subscriber가 답장을 보내는 형태로 구현한다.   
Go 클라이언트의 경우 Subscriber 측은 callback 함수 내에서 회신을 반환하도록 구현하는 것 말고는 다른 큰 차이는 없다.   
한편 Publisher 측은 답장을 기다리기 때문에 *nats.Conn.Request 함수를 사용한다.  
    
**Request**  
```
package main

import (
	"log"
	"time"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	msg, err := nc.Request("subjectFoo", []byte("bodyBar"), 10*time.Second)
	if err != nil {
		log.Fatal(err)
	}
	log.Print(string(msg.Data))
}
```
  
**Reply**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	if _, err := nc.Subscribe("subjectFoo", callback); err != nil {
		log.Fatal(err)
	}
	ch := make(chan struct{})
	<-ch
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()
	nc.Publish(message.Reply, []byte("ReplyBaz"))
}
```
    
이 구현에서는 여러 클라이언트가 있다면 먼저 도착한 회신만 사용된다.   
여러 클라이언트로부터의 회신을 충족시키고 싶다면, 다음과 같이 *nats.Conn.NewRespInbox를 사용한다.    
   
**Request(for multiple client)**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	inbox := nc.NewRespInbox()
	if err := nc.PublishRequest("subjectFoo", inbox, []byte("bodyBar")); err != nil {
		log.Fatal(err)
	}
	if _, err := nc.Subscribe(inbox, callback); err != nil {
		log.Fatal(err)
	}
	ch := make(chan struct{})
	<-ch
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
}
```
  
*nats.Conn.NewRespInbox는 회신용으로 사용할 수 있는 Subject를 생성해서 반환한다.  
이것을 *nats.Conn.PublishRequest 에 반환해서 회신을 대기한다.  
  
  
### Queueing
큐잉(Queueing) 그룹을 지정해서 서브 스크라이브 하면 같은 큐 그룹 내에서 메시지를 나누어서 배신한다.  
Pub/Sub와 다른 점은 Pub/Sub은 같은 서브젝트의 서브 스크라이버가 모두 동일한 메시지를 받고, 큐잉을 사용하면 같은 그룹의 서브 스크라이버는 나누어서 메시지를 받는다.  
  
- Queue-Sub 클라이언트 ([nats-qsub](https://github.com/nats-io/nats/blob/master/examples/nats-qsub.go))
  
Queue-Sub 클라이언트 2개(subject=example.one, queue-group=Q1)와 다른 큐 그룹의 Queue-Sub 클라이언트 2개(subject=example.one, queue-group=Q2)을 실행한다.  
  
<PRE>
$ ./nats-qsub example.one Q1                   # client-A1
$ ./nats-qsub example.one Q1                   # client-A2
$ ./nats-qsub example.one Q2                   # client-B1
$ ./nats-qsub example.one Q2                   # client-B2
</PRE>
  
Pub 클라이언트에서 메시지를 보내본다. 도중 client-B1의 접속을 끊고 이 때의 동작도 본다.  
    
<PRE>  
$ ./nats-pub example.one "Message 1"
$ # 여기에서 client-B1를 정지
$ ./nats-pub example.one "Message 2"
$ ./nats-pub example.one "Message 3"
$ ./nats-pub example.one "Message 4"
$ ./nats-pub example.one "Message 5"
$ ./nats-pub example.one "Message 6"
</PRE>
   
메시지를 보내었을 때 각각의 Queue-Sub 클라이언트의 출력은 아래에.  
  
client-A1  
<PRE>
Listening on [example.one]
[#1] Received on [example.one] Queue[Q1] Pid[73099]: 'Message 1'
[#2] Received on [example.one] Queue[Q1] Pid[73099]: 'Message 6'
</PRE>
  
client-A2  
<PRE>
Listening on [example.one]
[#1] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 2'
[#2] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 3'
[#3] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 4'
[#4] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 5'
</PRE>
  
client-B1  
<PRE>
Listening on [example.one]
[#1] Received on [example.one] Queue[Q2] Pid[73173]: 'Message 1'
^C
</PRE>
  
client-B2  
<PRE>  
Listening on [example.one]
[#1] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 2'
[#2] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 3'
[#3] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 4'
[#4] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 5'
[#5] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 6'
</PRE>
  
같은 큐 그룹 내에서는 어떤 하나의 서브 스크라이버에 메시지를 던지는 것을 알 수 있다.  
client-A1, A2의 동작을 보면 메시지를 분배는 간단한 라운드로빈은 아닌 것 같다.  
  
  
## 보안
### 패스워드 인증
gnatsd 에 `--user` 옵션, `--pass` 옵션 또는 `--auth` 옵션을 붙여서 시작하면 패스워드 인증 or 토큰 인증을 사용하게 된다.  
<PRE>  
$ gnatsd --auth hogetoken
</PRE>  
  
클라이언트를 접속해 본다  
<PRE>  
$ ./nats-sub subj
Can't connect: nats: authorization violation
gnatsd
[93190] 2016/07/15 21:16:48.334616 [ERR] ::1:56626 - cid:1 - Authorization Error
</PRE>  
  
접속 처에 토큰 정보를 붙이면 인증 성공으로 접속할 수 있다.  
<PRE>  
$ ./nats-sub -s "nats://hoge@127.0.0.1:4222" subj
Listening on [subj]
</PRE>  
  
패스워드 인증의 경우는 hoge 부분을 'user:pass' 처럼 하면 된다.  
  
  
### TLS
TLS를 사용하여 서버와 클라이언트의 인증과 통신 내용 자체를 암호화 할 수 있다.  
  
gnatsd의 설정 파일로 아래와 같이 tls.conf를 준비한다.  
CERT 파일 등은 nats-io/gnatsd의 test/configs/certs 디렉토리에 있는 것을 사용한다.  
  
tls.conf  
```
listen: 127.0.0.1:4443

tls {
  cert_file: "./certs/server-cert.pem"
  key_file:  "./certs/server-key.pem"
  ca_file:   "./certs/ca.pem"
  verify:    true
}
```
  
<PRE>
$ gnatsd -D -V -config tls.conf
Starting gnatsd on port 5000
[95447] 2016/07/15 21:48:55.717289 [INF] Starting nats-server version 0.9.0.beta
[95447] 2016/07/15 21:48:55.717450 [DBG] Go build version go1.6.2
[95447] 2016/07/15 21:48:55.717455 [INF] Listening for client connections on 127.0.0.1:4443
[95447] 2016/07/15 21:48:55.717660 [INF] TLS required for client connections
[95447] 2016/07/15 21:48:55.717674 [DBG] Server id is m4w0EmrXB1xPvE5eMxpyX1
[95447] 2016/07/15 21:48:55.717677 [INF] Server is ready
</PRE>
  
TLS를 사용하지 않는 클라이언트를 붙여 보면 실패한다.  
<PRE>
$ ./nats-sub -s "nats://127.0.0.1:4443" subj
Can't connect: nats: secure connection required
</PRE>
  
앞에 사용한 nats-sub.go에 아래의 변경을 더해서 TLS 접속을 해본다.  
  
nats-tls-sub.go.diff  
<PRE>
$ diff -u nats-sub.go nats-tls-sub.go
--- nats-sub.go 2016-07-15 16:09:03.000000000 +0900
+++ nats-tls-sub.go 2016-07-18 00:28:36.000000000 +0900
@@ -4,7 +4,10 @@
 package main

 import (
+   "crypto/tls"
+   "crypto/x509"
    "flag"
+   "io/ioutil"
    "log"
    "runtime"

@@ -33,7 +36,25 @@
        usage()
    }

-   nc, err := nats.Connect(*urls)
+   pool := x509.NewCertPool()
+   pemData, err := ioutil.ReadFile("./certs/ca.pem")
+   if err != nil {
+       log.Fatalf("read error for ca.pem: %v\n", err)
+   }
+   pool.AppendCertsFromPEM(pemData)
+
+   cert, err := tls.LoadX509KeyPair("./certs/client-cert.pem", "./certs/client-key.pem")
+   if err != nil {
+       log.Fatalf("tls.LoadX509KeyPair() error=%v", err)
+   }
+
+   config := &tls.Config{
+       ServerName:   "localhost",
+       Certificates: []tls.Certificate{cert},
+       RootCAs:      pool,
+       MinVersion:   tls.VersionTLS12,
+   }
+   nc, err := nats.Connect(*urls, nats.Secure(config))
    if err != nil {
        log.Fatalf("Can't connect: %v\n", err)
    }
</PRE>
  	
이것을 실행하면 접속할 수 있다.  
<PRE>
$ go build nats-tls-sub.go
$ ./nats-tls-sub -s "tls://127.0.0.1:4443" subj
Listening on [subj]
[99264] 2016/07/18 00:28:53.609262 [DBG] 127.0.0.1:62669 - cid:1 - Client connection created
[99264] 2016/07/18 00:28:53.609327 [DBG] 127.0.0.1:62669 - cid:1 - Starting TLS client connection handshake
[99264] 2016/07/18 00:28:53.670571 [DBG] 127.0.0.1:62669 - cid:1 - TLS handshake complete
[99264] 2016/07/18 00:28:53.670590 [DBG] 127.0.0.1:62669 - cid:1 - TLS version 1.2, cipher suite TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384
[99264] 2016/07/18 00:28:53.670830 [TRC] 127.0.0.1:62669 - cid:1 - ->> [CONNECT {"verbose":false,"pedantic":false,"tls_required":true,"name":"","lang":"go","version":"1.2.2"}]
[99264] 2016/07/18 00:28:53.670919 [TRC] 127.0.0.1:62669 - cid:1 - ->> [PING]
[99264] 2016/07/18 00:28:53.670926 [TRC] 127.0.0.1:62669 - cid:1 - <<- [PONG]
[99264] 2016/07/18 00:28:53.671085 [TRC] 127.0.0.1:62669 - cid:1 - ->> [SUB subj  1]
[99264] 2016/07/18 00:28:53.671108 [TRC] 127.0.0.1:62669 - cid:1 - ->> [PING]
[99264] 2016/07/18 00:28:53.671114 [TRC] 127.0.0.1:62669 - cid:1 - <<- [PONG]
[99264] 2016/07/18 00:28:56.754385 [DBG] 127.0.0.1:62669 - cid:1 - Client connection closed
</PRE>
  
이외에 유저 마다 권한 설정이나 Bcrypt을 사용한 패스워드 해시화도 가능하다.  
  
  
## 모니터링
gnatsd 에 `-m` 포트 번호 옵션을 지정하면 모니터링을 유효화 할 수 있다.  
  
`top` 명령어와 비슷한 `nats-top` 명령어를 설치하면 일단 모니터링 할 수 있다.  
<PRE>
$ nats-top

NATS server version 0.9.0.beta (uptime: 8m25s)
Server:
  Load: CPU:  0.0%  Memory: 9.6M  Slow Consumers: 0
  In:   Msgs: 15  Bytes: 162  Msgs/Sec: 2.0  Bytes/Sec: 14
  Out:  Msgs: 6  Bytes: 54  Msgs/Sec: 1.0  Bytes/Sec: 2

Connections Polled: 2
  HOST                 CID      NAME            SUBS    PENDING     MSGS_TO     MSGS_FROM   BYTES_TO    BYTES_FROM  LANG     VERSION  UPTIME   LAST ACTIVITY
  ::1:61187            3                        0       0           0           6           0           63          go       1.2.2    5s       2016-07-14 12:21:07.254618678 +0900 JST
  ::1:61447            4                        0       0           0           3           0           36          go       1.2.2    2s       2016-07-14 12:21:08.129647296 +0900 JST
</PRE>
  
`nats-top` 이외에도 WebUI를 갖춘 툴 등도 있다.  
  
모니터링 기능 자체는 단순하므로 http://hostname:port/XXX 에 HTTP로 접근하면 JSON을 XXX에 따라서 매트릭스를 돌려준다.  
대상의 엔트리 포인트는 아래와 같다.  
- /vars : NATS 클라이언트의 접속 수와 CPU 사용률 등
- /connz : NATS 클라이언트 수나 이 각 클라이언트의 내용
- /subsz : 서브 스크라이버 수나 매치한 메미지 수, 파괴된 메시지 수 등
- /routez : 클라스터의 루트 관련
  

## 커넥터
다른 미들웨어와 연동 기능을 제공하는 커넥트 라는 기능이 있다.(단순한 미들웨어 간의 브릿지이므로 기능이라고 하기에 애매할 수도....)  

https://nats.io/download/  
   
공식에는 NATS Connector Framework 라는 Java로 구현된 기능이 있고, 이 프레임워크 상에서 동작하는 Redis와 NATS를 연결하는 플러그인도 동시에 제공하고 있다. 
- [fluent-plugin-nats](https://github.com/cloudfoundry-community/fluent-plugin-nats ) : Fluentd와 NATS을 연결하는 Fluentd 플러그인
- [Logstash](https://github.com/r3labs/nats_to_logstash ) : Logstash에 NATS 메시지를 던지는 커넥트
- [nats-proxy](https://github.com/sohlich/nats-proxy ) : HTTP/Websocket으로 NATS과 대화하는 커넥트
  

## NATS Streaming 실행
[여기](https://github.com/nats-io/nats-streaming-server/releases )에서 실행 파일을 다운로드 한다.  
실행 파일 이름은 **nats-streaming-server** 이다.  

클라이언트는 아래처럼 실행한다.  
<PRE>
$ git clone https://github.com/nats-io/go-nats-streaming.git
$ cd go-nats-streaming/examples
$ go build stan-pub.go
$ go build stan-sub.go
</PRE>
  
서버 실행  
<PRE>
$ ./nats-streaming-server -m 8222
[87442] 2016/07/15 20:14:50.605166 [INF] Starting nats-streaming-server[test-cluster] version 0.2.0
[87442] 2016/07/15 20:14:50.605457 [INF] Starting nats-server version 0.9.0.beta
[87442] 2016/07/15 20:14:50.605467 [INF] Listening for client connections on localhost:4222
[87442] 2016/07/15 20:14:50.607194 [INF] Server is ready
[87442] 2016/07/15 20:14:50.944737 [INF] STAN: Message store is MEMORY
[87442] 2016/07/15 20:14:50.944755 [INF] STAN: Maximum of 1000000 will be stored
<//PRE>
  
nats-top을 확인하면 스트리밍 서버가 실행하는 시점에서 내부에서 클라스터용 클라이언트가 있다.  
gnatsd 상에서 돈다는 것이 여기서 알 수 있다.   
  
<PRE>  
NATS server version 0.9.0.beta (uptime: 4m49s)
Server:
  Load: CPU:  0.0%  Memory: 11.0M  Slow Consumers: 0
  In:   Msgs: 1  Bytes: 45  Msgs/Sec: 0.0  Bytes/Sec: 0
  Out:  Msgs: 0  Bytes: 0  Msgs/Sec: 0.0  Bytes/Sec: 0

Connections Polled: 1
  HOST                 CID      NAME            SUBS    PENDING     MSGS_TO     MSGS_FROM   BYTES_TO    BYTES_FROM  LANG     VERSION  UPTIME   LAST ACTIVITY
  127.0.0.1:65386      2        NATS-Streaming-Server-test-cluster 5       0           0           1           0           45          go       1.2.2    4m49s    2016-07-15 20:15:48.18
<//PRE>
    
### Streaming Pub/Sub
Sub 클라이언트가 없는 상태에서 Pub 클라이언트에서 메시지를 보내 본다.  
<PRE>
$ ./stan-pub subject.hoge "Hello NATS Streaming"
Published [subject.hoge] : 'Hello NATS Streaming'
$ ./stan-pub subject.hoge "Hello NATS Streaming 2"
Published [subject.hoge] : 'Hello NATS Streaming 2'
</PRE>
  
여기에서 Sub 클라이언트를 실행한다.  
`--all` 옵션을 붙여서 모든 유효한 메시지를 받도록 한다.  
<PRE>  
$ ./stan-sub -id STANSUB1 --all subject.hoge
Connected to nats://localhost:4222 clusterID: [test-cluster] clientID: [STANSUB1]
subscribing with DeliverAllAvailable
Listening on [subject.hoge], clientID=[STANSUB1], qgroup=[] durable=[]
[#1] Received on [subject.hoge]: 'sequence:1 subject:"subject.hoge" data:"Hello NATS Streaming" timestamp:1468581922036801904 '
[#2] Received on [subject.hoge]: 'sequence:2 subject:"subject.hoge" data:"Hello NATS Streaming 2" timestamp:1468581932488709393 '
</PRE>  
  
NATS에서읭 Pub/Sub 에서는 퍼블리쉬한 시점에서 서브스크라이버 하지 않으면 메시지를 수신 할 수 없었지만 NATS Streaming에서는 수신할 수 있다.  
  
`--last` 옵션을 붙여서 최후의 메시지만을 받을 수도 있다.  
stan-sub.go의 옵션을 보면 메시지의 기한 설정이나 기한 체크해서 유효하지 않으면 수신하지 않도록 하는 것도 가능한 것 같다.  
  

### 메시지 스토어
nats-streaming-server는 default로는 인메모리로 메시지 스토어 하고 있지만 `-store FILE -dir ${DIRNAME}`을 지정하면 DIRNAME 디렉토리를 만들고 그 아래에 파일로 메시지를 스토어 할 수 있다.  
  
<PRE>  
$ ./nats-streaming-server -m 8222 -store FILE -dir streaminglogs
            :
[91285] 2016/07/15 20:45:08.489025 [INF] STAN: Message store is FILE
            :
</PRE>
  			
<PRE>
$ ls -ltr streaminglogs/*
-rw-r--r--  1 hattori-h  staff  195  7 15 20:43 streaminglogs/server.dat
-rw-r--r--  1 hattori-h  staff   71  7 15 20:45 streaminglogs/clients.dat

streaminglogs/subject.hoge:
total 48
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 subs.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.5.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.4.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.3.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.2.dat
-rw-r--r--  1 hattori-h  staff  62  7 15 20:45 msgs.1.dat
</PRE>
  
    

## 참고
- [NATS에 대해서 알아보고 테스트 해 보기](https://qiita.com/hhatto/items/81611940de547ce3bd08)