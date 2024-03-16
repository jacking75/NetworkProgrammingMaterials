# ZeroMq - C#
  
닷넷용 ZeroMQ 라이브러리는 아래의 링크에서 소개하고 있다.  
http://zeromq.org/bindings:clr  

clrzmq4, NetMQ,  Castle.Zmq  
위 라이브러리 중 clrzmq4는 zeromq lib 파일을 C#으로 랩핑한 것으로 .NET Core는 지원하지 않는다.  
NetMQ는 ZeroMQ의 C 코드를 C# 코드로 포팅한 것이다. .NET Core를 지원한다.  
NetMQ 사용을 추천한다.  
  

## NetMQ
- .NetCore 지원
- Windows에서는 [AsyncIO](https://github.com/somdoron/AsyncIO) 라는 라이브러리를 사용하여 비동기IO를 사용(즉 IOCP)
- .NetCore에서는 Socket.Select 사용하는 듯
- C++ 버전의 ZeroMQ와 통신 가능
  

### multipart 메시지 조작
multipart 메시지를 분해 및 추가한다  
  
메시지 빼기    
```
private static byte[] Unwrap(NetMQMessage msg)
{
    var idFrame = msg.Pop();
    // throw away the empty frame
    msg.Pop();

    return idFrame.Buffer;
}
```
  
메시지 조합      
```
/// <summary>
///     wraps the message with the identity and includes an empty frame
/// </summary>
/// <returns>[socket.Identity][empty][old message]</returns>
private static NetMQMessage Wrap(byte[] identity, NetMQMessage msg)
{
    var result = new NetMQMessage(msg);

    result.Push(NetMQFrame.Empty);
    result.Push(identity);

    return result;
}
```  

  
### 옵션
- Affinity
- BackLog
- CopyMessages
- DelayAttachOnConnect
- Endian
- GetLastEndpoint
- IPv4Only
- Identity
- Linger
- MaxMsgSize
- MulticastHops
- MulticastRate
- MulticastRecoveryInterval
- ReceiveHighWaterMark
- ReceiveMore
- ReceiveTimeout
- ReceiveBuffer
- ReconnectInterval
- ReconnectIntervalMax
- SendHighWaterMark
- SendTimeout
- SendBuffer
- TcpAcceptFilter
- TcpKeepAlive
- TcpKeepaliveCnt
- TcpKeepaliveIdle
- TcpKeepaliveInterval
- XPubVerbose
  
  
### 소스, 문서, 샘플
- [NetMQ - Github](https://github.com/zeromq/netmq)
- [NetMQ - Doc](https://netmq.readthedocs.io/en/latest/ )    
- [NetMQ Samples](https://github.com/NetMQ/Samples)  
    
   

## 참고
- [NetMQ: 글 모음](http://somdoron.com/category/netmq/ )
- [NetMQ and IO Completion Ports](http://somdoron.com/2014/11/netmq-iocp/ )
- [Token-Based PubSub](https://somdoron.com/2014/12/token-pubsub/ )
