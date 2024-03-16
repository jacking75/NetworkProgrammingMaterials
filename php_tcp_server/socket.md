# socket 사용하기
- [PHP Socket 함수](https://www.php.net/manual/ja/book.sockets.php )  

    
## 소켓 확장
- 소켓 확장은 컴파일시 기본적으로 비활성화
    - 컴파일 시에 configure 에 옵션 `--enable-sockets`을 지정한다.
- 소켓 확장으로 만든 소켓 리소스는 socket_xxx 함수에서만 이용 가능
- UNIX 시스템 호출에 가까운 형식의 API
  
    
## 설치하기
소스 컴파일 하기    
```
# cd /usr/local/APM_Setup/php-5.2.9/ext/sockets // php 소스 폴더의 ext/sockets 디렉토리로 이동 
# /usr/local/php/bin/phpize
# ./configure –enable-sockets –with-php-config=/usr/local/php/bin/php-config
# make
# cp -arp modules/soap.so /usr/local/php/modules  // modules 디렉토리는 예시로 만들어준 것
```
     
연동    
php.ini 파일을 열어서 [sockets] 항목에 아래와 같이 sockets.so 파일을 추가해준다.  
```
# vi /usr/local/apache/conf/php.ini

extension=/usr/local/php/modules/sockets.so
```
   



## 참고 글
- [Socket Function](https://rrhh234cm.tistory.com/3    )
- [Socket통신을 하는 방법](https://nowonbun.tistory.com/632)
- PHP를 이용한 다중 연결 소켓 통신 [1](http://www.phpschool.com/class/php_multi_socket_server.html) [2](http://www.phpschool.com/class/php_multi_socket_server2.html)  
  

## 라이브러리
- [clue/php-socket-raw](https://github.com/clue/php-socket-raw  )  
