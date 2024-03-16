# Priority Queue
출처: https://qiita.com/tamikura@github/items/0d10a37c674d5f912fdd  
  
RabbitMQ3.5.0 에서 도입된 기능  
보통의常MQ 에서는 메시지는 넣은 순서대로만 얻을 수 있다.  
그러나 우선도 큐를 사용하면 메시지에 우선도를 설정하여 우선도 높은 순서로 얻을 수 있다.  
  

## 환경
- Amazon Linux AMI 2016.03.3
- RabbitMQ 3.5.4
- Erlang R14B04
- bunny 2.4.0


## RabbitMQ 시작
```
sudo service rabbitmq-server start
#=>
#Starting rabbitmq-server: SUCCESS
#rabbitmq-server.
```
  
## bunny 설치
```
gem install bunny
```
  
    

## 우선도 큐를 만들고, 큐에 메시지 등록하기
queue 작성 시에 x-max-priority를 설정하면 우선도 큐가 된다.  
  
publish_with_priority.rb  
```
# -*- coding: utf-8 -*-
require "bunny"

# RabbitMQ에 접속
conn = Bunny.new(:host => "localhost", :vhost => "/", :user => "guest", :pass => "guest")
conn.start

# channel을 만든다
ch = conn.create_channel

# queue1 이라는 큐를 만든다
# 우선도 큐를 설정
q  = ch.queue("queue1", :durable => true, :arguments => {"x-max-priority" => 10 })

# 우선도를 설정하고 queue1에 메시지를 추가
q.publish("priority 5" , { :priority => 5} )
q.publish("priority 1" , { :priority => 1} )
q.publish("priority 3" , { :priority => 3} )
q.publish("priority 9" , { :priority => 9} )
q.publish("priority 10", { :priority => 10} )
q.publish("priority none" )   # 우선도 설정 없음

# close
conn.stop
```
  
  
## 우선도 큐에서 메시지 얻기
subscribe_with_priority.rb  
```
# -*- coding: utf-8 -*-
require "bunny"

# RabbitMQ에 접속
conn = Bunny.new(:host => "localhost", :vhost => "/", :user => "guest", :pass => "guest")
conn.start

# channel을 만든다
ch = conn.create_channel

# queue1 큐 얻기
q  = ch.queue("queue1", :durable => true, :arguments => {"x-max-priority" => 10 })

# 메시지를 취득
# 취득하면 큐에서 메시지는 삭제된다
q.subscribe do |delivery_info, properties, msg|
  p "queue  = #{q.name}, msg = #{msg}"
end

# close
conn.stop
```
  
실행 결과    
ruby subscribe_with_priority.rb  
<pre>
#=>
#"queue  = queue1, msg = priority 10"
#"queue  = queue1, msg = priority 9"
#"queue  = queue1, msg = priority 5"
#"queue  = queue1, msg = priority 3"
#"queue  = queue1, msg = priority 1"
#"queue  = queue1, msg = priority none"
</pre>
  
priority가 높은 순서대로 취득함(priority를 설정하지 않는 경우는 0(최저)이 된다)  
  

## 여러가지 설정해 보기
### 기존의 큐에 x-max-priority를 변경한 경우
```
# "x-max-priority" => 10 으로 설정되어 있는 큐에 대해서 20을 설정
q  = ch.queue("queue1", :durable => true, :arguments => {"x-max-priority" => 20 })
```
  
에러가 발생  
<pre>
/home/ec2-user/.gem/ruby/2.0/gems/bunny-2.4.0/lib/bunny/channel.rb:1927:in `raise_if_continuation_resulted_in_a_channel_error!':
 PRECONDITION_FAILED - inequivalent arg 'x-max-priority' for queue 'queue1' in vhost '/': received '20' but current is '10' (Bunny::PreconditionFailed)
 </pre>
   

### x-max-priority를 넘은 값을 설정한 경우
```
q  = ch.queue("queue1", :durable => true, :arguments => {"x-max-priority" => 10 })

q.publish("priority 10" , { :priority => 10} )
q.publish("priority 30" , { :priority => 30} )
q.publish("priority 20" , { :priority => 20} )
q.publish("priority 100", { :priority => 100} )
```
  
아래처럼 되었다  
<pre>
"queue  = queue1, msg = priority 10"
"queue  = queue1, msg = priority 20"
"queue  = queue1, msg = priority 30"
"queue  = queue1, msg = priority 100"
</pre>  
  
### 같은 priority를 설정한 경우
```
q.publish("priority 1-1", { :priority => 1} )
q.publish("priority 1-2", { :priority => 1} )
q.publish("priority 1-3", { :priority => 1} )
q.publish("priority 2-1", { :priority => 2} )
q.publish("priority 2-2", { :priority => 2} )
q.publish("priority 2-3", { :priority => 2} )
```
  
먼저 넣었던 메시지를 취득하였다  
<pre>
"queue  = queue1, msg = priority 2-1"
"queue  = queue1, msg = priority 2-2"
"queue  = queue1, msg = priority 2-3"
"queue  = queue1, msg = priority 1-1"
"queue  = queue1, msg = priority 1-2"
"queue  = queue1, msg = priority 1-3"
</pre>
  
### 마이너스, 소수점을 설정한 경우
```
q.publish("priority 10" , { :priority => 10} )
q.publish("priority -1" , { :priority => -1} )
q.publish("priority 9.5", { :priority => 9.5} )
q.publish("priority 1"  , { :priority => 1} )
```
  
아래처럼 되었다  
<pre>
"queue  = queue1, msg = priority 10"
"queue  = queue1, msg = priority -1"
"queue  = queue1, msg = priority 9.5"
"queue  = queue1, msg = priority 1"
</pre>
  