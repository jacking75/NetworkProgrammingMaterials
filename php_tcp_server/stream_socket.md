# 스트림 소켓
- 스트림 확장은 PHP 5부터 기본적으로 활성화
- PHP 5부터 스트림(Stream) API 소켓을 다룬다
- 스트림 API로 만든 소켓 리소스는 파일 계의 함수로 취급한다
- 소켓의 낮은 수준에서 제어 할 수 없다
- 원칙적으로, 스트림 API를 사용하여 소켓을 처리해야한다
- [PHP Stream_Socket 함수](https://www.php.net/manual/ja/ref.stream.php )    
    
    
## PHP Socket Programming Handbook
[(영문)PHP 소켓 프로그래밍 책](https://www.christophh.net/php-socket-programming-handbook/ ) 을 [요약한 글](https://ryo511.info/archives/3882 ) 정리  
  

## 시작하기
Windows 에서는 UNIX 소켓의 일부 기능은 사용할 수 없다.  
PHP에서 소켓 프로그래밍에는 2가지 방법이 있다  
   

### 소켓 확장
- 소켓 확장은 컴파일시 기본적으로 비활성화
- 소켓 확장으로 만든 소켓 리소스는 socket_xxx 함수에서만 이용 가능
- UNIX 시스템 호출에 가까운 형식의 API
- (이 책의 저자는) 사용을 권장하지 않는다
    
  
## 에코 서버의 예
고전적인 서버 프로그램의 처리는 다음과 같다:  
1. 소켓에 bind하기  
2. 소켓을 listen 하고, 연결을 기다린다  
3. 연결을 accept 한다  
4. 클라이언트가 전송한 데이터를 수신한다  
5. 응답을 보낸다  
6. 연결을 끊거나 클라이언트가 종료 될 때까지 기다린다  
7. 3으로 돌아가서 반복한다
  

### 구현
  
```
<?php

// echo.php

$server = @stream_socket_server('tcp://0.0.0.0:1337', $errno, $errstr);

if (false === $server) {
    fwrite(STDERR, "Error: $errno: $errstr\n");
    exit(1);
}

$timeout = -1; // タイムアウトしないようにする
$readLength = 4096;

for (;;) {
    $conn = @stream_socket_accept($server, $timeout, $peer);

    if ($conn) {
        while ($buf = fread($conn, $readLength)) {
            fwrite($conn, $buf);
        }
        fclose($conn);
    }
}
```
  
- stream_socket_server는 1번과 2번을 실행한다
- `for (;;)` 에서 무한 루프를 만들고, 내부에서 stream_socket_accept로 연결을 기다린다(3번)
- fread로 데이터를 읽는(4번)
- fwrite로 데이터를 쓴다(5번)
- fclose로 소켓을 닫는다(6번)
  

### 서버의 실행
- php echo.php 로 시작한다
- 처음 시작할 때 포트 개방이 필요하다
- 클라이언트는 127.0.0.1 1337로 접속할 수 있다.  
  
  

## 1 프로세스로 복수의 클라이언트 처리하기
두 가지 방법이 있다  
- 복수의 프로세스를 사용한다(UNIX 계에서만 사용 가능)
- stream_select를 사용한다(모든 OS에서 동작 가능)
  
### 구현
https://github.com/phpsphb/book-examples/blob/master/multi-connect/listing2.php  
  
```
<?php

$server = @stream_socket_server('tcp://0.0.0.0:9000', $errno, $errstr);
stream_set_blocking($server, 0);

if (false === $server) {
    fwrite(STDERR, "Error connecting to socket: $errno: $errstr\n");
    exit(1);
}

$connections = [];
$buffers = [];

for (;;) {
    $readable = $connections;
    array_unshift($readable, $server);
    $writable = $connections;
    $except = null;

    if (stream_select($readable, $writable, $except, 0, 500) > 0) {
        // Some streams have data to read
        foreach ((array) $readable as $stream) {
            // When the server is readable this means that a client
            // connection is available. Let's accept the connection and store it
            if ($stream === $server) {
                $client = @stream_socket_accept($stream, 0, $clientAddress);
                $key = (int) $client;
                if (is_resource($client)) {
                    printf("Client %s connected\n", $clientAddress);
                    stream_set_blocking($client, 0);
                    $connections[$key] = $client;
                }
            } else {
                // One of the clients sent data, read it in a client specific buffer
                $key = (int) $stream;

                if (!isset($buffers[$key])) {
                    $buffers[$key] = '';
                }

                $buffers[$key] .= fread($stream, 4096);
            }
        }

        // Some streams are waiting for data
        foreach ((array) $writable as $stream) {
            $key = (int) $stream;

            // Try to write 4096 bytes, look how many bytes were really written,
            // and subtract the written bytes from this client's buffer
            if (isset($buffers[$key]) && strlen($buffers[$key]) > 0) {
                $bytesWritten = fwrite($stream, $buffers[$key], 4096);
                $buffers[$key] = substr($buffers[$key], $bytesWritten);
            }
        }

        // Out of band data, usually not handled.
        foreach ((array) $except as $stream) {
            // Can't happen, we haven't set $except to anything
        }
    }

    // House keeping
    // Purge connections which were closed by the peer
    foreach ($connections as $key => $conn) {
        if (feof($conn)) {
            printf("Client %s closed the connection\n", stream_socket_get_name($conn, true));
            unset($connections[$key]);
            fclose($conn);
        }
    }
}
```
  
- stream_set_blocking($server, 0) 로 서버가 논블럭킹 모드가 된다
- stream_select 스트림에 대해서 동작하는 select(2)
  


## Reactor 패턴
https://github.com/phpsphb/book-examples/blob/master/reactor/reactor.php  
```
namespace SocketProgrammingHandbook;

final class StreamSelectLoop {
    private $readStreams = [];
    private $readHandlers = [];
    private $writeStreams = [];
    private $writeHandlers = [];
    private $running = true;

    function addReadStream($stream, callable $handler) {
        if (empty($this->readStreams[(int) $stream])) {
            $this->readStreams[(int) $stream] = $stream;
            $this->readHandlers[(int) $stream] = $handler;
        }
    }

    function addWriteStream($stream, callable $handler) {
        if (empty($this->writeStreams[(int) $stream])) {
            $this->writeStreams[(int) $stream] = $stream;
            $this->writeHandlers[(int) $stream] = $handler;
        }
    }

    function removeReadStream($stream) {
        unset($this->readStreams[(int) $stream]);
    }

    function removeWriteStream($stream) {
        unset($this->writeStreams[(int) $stream]);
    }

    function removeStream($stream) {
        $this->removeReadStream($stream);
        $this->removeWriteStream($stream);
    }

    /**
     * Runs the event loop, which blocks the current process. Make sure you do
     * any necessary setup before running this.
     */
    function run() {
        while ($this->running) {
            $read = $this->readStreams;
            $write = $this->writeStreams;
            $except = null;

            if ($read || $write) {
                @stream_select($read, $write, $except, 0, 100);

                foreach ($read as $stream) {
                    $this->readHandlers[(int) $stream]($stream);
                }

                foreach ($write as $stream) {
                    $this->writeHandlers[(int) $stream]($stream);
                }
            } else {
                usleep(100);
            }
        }
    }
}
```
    
Reactor 패턴을 사용하는 경우:  
- 여러 클라이언트 연결을 동시에 처리해야 하지만, 서버에서 복잡한 연결 관리를 하고 싶지 않은 경우
- 메모리가 한정 되어 있고, 시스템에서 하나의 서버 프로세스만 실행할 수 있는 경우
- fork가 느리거나 메모리를 많이 사용하는 플랫폼(주로 Windows)에서 실행해야 하는 경우
- 클라이언트를 처리하는 동안 코드가 크래쉬 되어도 새로운 서버 프로세스를 다시 시작하면 사는 경우
  

Reactor 패턴을 사용하면 안되는 경우:  
- fork가 빠르고 메모리 효율적인 플랫폼(주로 UNIX, Linux)에서 실행하는 경우
- 정말 병렬 처리를 할 필요가 있는 경우(Reactor는 완전한 병렬이 아니다)
- 메모리가 윤택한 경우
  

### PHP 라이브러리
PHP는 Reactor 패턴의 구현으로 유명한 것으로 [ReactPHP](https://github.com/reactphp/react ) 라이브러리가 있다.  
또한 stream_select는아 select(2)에 의존하고 있지만, 더 높은 성능의 시스템 호출에 epoll(2 )및 kqueue(2)등이 있다.  
PHP에서 epoll(2) 등을 사용하고 싶은 경우, 이 시스템 호출을 추상화한 라이브러리인 libevent의 PHP용 인터페이스 [Libevent](http://php.net/manual/ja/book.libevent.php ) 등을 사용할 수 있다.
   
  
  
## fork(2)
프로세스 실행 중에 다른 프로세스를 실행할 때는 fork(2)를 사용한다  
PHP에서 fork(2)를 사용하려면 pcntl 확장을 사용한다  
pcntl_fork를 사용하여 fork를 할 수 있다  
  
https://github.com/phpsphb/book-examples/blob/master/multiprocess/limited_forking_echo_server.php  
```
<?php

$server = stream_socket_server('tcp://127.0.0.1:'.(getenv('PORT') ?: 1234), $errno, $errstr);

if (false === $server) {
    fwrite(STDERR, "Failed creating socket server: $errstr\n");
    exit(1);
}

echo "Waiting…\n";

const MAX_PROCS = 2;
$children = [];

for (;;) {
    $read = [$server];
    $write = null;
    $except = null;

    stream_select($read, $write, $except, 0, 500);

    foreach ($read as $stream) {
        if ($stream === $server && count($children) < MAX_PROCS) {
            $conn = @stream_socket_accept($server, -1, $peer);

            if (!is_resource($conn)) {
                continue;
            }

            echo "Starting a new child process for $peer\n";

            $pid = pcntl_fork();

            if ($pid > 0) {
                $children[] = $pid;
            } elseif ($pid === 0) {
                // Child process, implement our echo server
                $childPid = posix_getpid();
                fwrite($conn, "You are connected to process $childPid\n");

                while ($buf = fread($conn, 4096)) {
                    fwrite($conn, $buf);
                }
                fclose($conn);

                // We are done, quit.
                exit(0);
            }
        }
    }

    // Do housekeeping on exited childs
    foreach ($children as $i => $child) {
        $result = pcntl_waitpid($child, $status, WNOHANG);

        if ($result > 0 && pcntl_wifexited($status)) {
            unset($children[$i]);
        }
    }

    echo "\t".count($children)." connected\r";
}
```
  
- pcntl_fork의 반환 값은 부모 프로세스의 경우는 자식 프로세스의 PID, 자식 프로세스의 경우는 0, fork에 실패한 경우는 -1
- [pcntl_waitpid](http://php.net/manual/ja/function.pcntl-waitpid.php )는 종료한 자식 프로세스의 프로세스 ID를 반환한다
    - WNOHANG 옵션을 지정하면 자식 프로세스가 종료하지 않은 경우에 바로 처리를 반환한다
- [pcntl_wifexited](http://php.net/manual/ja/function.pcntl-wifexited.php )는 상태가 정상 종료했는지 조사한다
    


## 시그널
Linux 계열에서만 사용할 수 있다.  
  
OS는 아래와 같은 시그널을 실행 중의 프로세스에 보낸다
- SIGINT: 프로그램을 강제 종료한다
- SIGQUIT: 프로그램의 종료(SIGINT 보다 부드러운 방법)
- SIGCHLD: 자식 프로세스가 종료했을 때에 부모 프로세스에 보낸다
- SIGHUP: 프로세스를 제어하고 있는 터미널이 종료했을 때 보내고, 설정 파일의 재 로딩에 사용되는 경우가 많다
- SIGTTIN, SIGTTOU: 백 그라운드에서 TTY로 읽으려고 하는 프로세스에 보내진다 
    
PHP에서는 [pcntl_signal](http://php.net/manual/ja/function.pcntl-signal.php )을 사용하여 시그널 제어를 한다  
```
<?php

pcntl_signal(SIGINT, function () {
    echo 'Received SIGINT. Cleaning up and terminationg' . PHP_EOL;
    exit;
});

echo 'Waiting for signals...' . PHP_EOL;

for (;;) {
    pcntl_signal_dispatch();
    usleep(100);
}
```
  
pcntl_signal로 시그널 수신 시 콜백을 등록한다.  
[pcntl_dispatch](https://ryo511.info/archives/php.net/manual/ja/function.pcntl-signal-dispatch.php )를 실행하면 시그널 수신 확인을 한다.  
  


## 참고
- [(일어)PHP로 배우는 소켓 프로그래밍 입문](http://slides.com/ryoutsunomiya/php-socket-programming#/   )  

