package main

import (
	"fmt"
	"github.com/nats-io/nats.go"
	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

// 각 서버들 이니셜
// 게이트웨이 'W', 채팅 서버 'C', DB 서버 'D'

var MQDBReqSubject = "DB"
var MQChatServerSubjectPrefix = "CHAT"
var MQGateServerSubjectPrefix = "GATE"

func mqItit(config configAppServer, inComingMQDataFunc func([]byte)) bool {
	NTELIB_LOG_INFO("MQ Init - ing...")

	_inComingMQDataFunc = inComingMQDataFunc

	if _mqConnect(config.MQAddress) == false {
		return false
	}

	// 나에게 보내는 메시지 받기
	subject := fmt.Sprintf("%s.%d", MQGateServerSubjectPrefix, config.MyServerIndex)
	if _mqReceive(subject) == false {
		return false
	}

	NTELIB_LOG_INFO("MQ Init - Success")
	return true
}

func mqClose() {
	if _isConnected == false {
		return
	}

	_mqConn.Close()
}

func mqSend(subject string, data []byte) bool {
	//Publish는 내부에서 lcok을 걸고, 바로 네트워크로 보낸다
	if err := _mqConn.Publish(subject, data); err != nil {
		NTELIB_LOG_ERROR(err.Error())
		return false
	}

	return true
}

func mqChatServerSubjet(chatServerIndex uint16) string {
	subject := fmt.Sprintf("%s.%d", MQChatServerSubjectPrefix, chatServerIndex)
	return subject
}

func _mqReceive(subJect string) bool {
	_, err := _mqConn.Subscribe(subJect, _receiveCallback)
	if err != nil {
		NTELIB_LOG_ERROR(err.Error(), zap.String("subject", subJect))
		return false
	}
	//log.Printf("Subject: %s", sub.Subject)
	//log.Printf("Queue: %s", sub.Queue)

	NTELIB_LOG_INFO("MQ Subjected", zap.String("subject", subJect))
	return true
}

func _receiveCallback(message *nats.Msg) {
	//message의 Data는 동적할당을 받은 버퍼이다.
	NTELIB_LOG_DEBUG("mq_receiveCallback")
	_inComingMQDataFunc(message.Data)
}

func _mqConnect(address string) bool {
	var err error
	_mqConn, err = nats.Connect(address)
	if err != nil {
		NTELIB_LOG_ERROR(err.Error())
		return false;
	}

	_isConnected = true
	NTELIB_LOG_INFO("Connected NATS !!!")
	return true
}

var _isConnected = false
var _mqConn *nats.Conn
var _inComingMQDataFunc func([]byte)