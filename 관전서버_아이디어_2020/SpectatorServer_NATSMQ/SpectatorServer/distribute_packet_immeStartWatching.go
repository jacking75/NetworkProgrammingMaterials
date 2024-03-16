package main

import (
	"go.uber.org/zap"
	. "main/protocol"
	. "main/util"
)

func packetProcessStartWatching(connUser *connection, bodyData []byte) int16 {
	if connUser.isAuth() == false {
		return _sendStartWatchingResult(connUser, ERROR_CODE_START_WATCHING_INVALID_AUTH)
	}

	// 이미 관전 중이면 안됨
	if connUser.isWatching()  {
		return _sendStartWatchingResult(connUser, ERROR_CODE_START_WATCHING_ALREADY)
	}

	// 스트리밍 서버에 연결한다
	srvConf := getServerConfig()

	if connUser.connectMQ(srvConf.mqAddress, srvConf.mqCID, connUser.getUserID()) == false {
		return _sendStartWatchingResult(connUser, ERROR_CODE_START_WATCHING_FAIL_CONNECT)
	}

	_sendStartWatchingResult(connUser, ERROR_CODE_NONE)
	return ERROR_CODE_NONE
}

func _sendStartWatchingResult(c *connection, result int16) int16 {
	sessionUID := c.getNetworkUniqueID()

	var response StartWatchingResPacket
	response.Result = result
	sendPacket, _ := response.EncodingPacket()

	c.sendPacket(sendPacket)
	LOG_DEBUG("_sendStartWatchingResult", zap.Uint64("sessionUID", sessionUID), zap.Int16("result", result))
	return result
}
