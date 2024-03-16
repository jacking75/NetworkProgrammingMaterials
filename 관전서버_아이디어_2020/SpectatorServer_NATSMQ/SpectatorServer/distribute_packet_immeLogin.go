package main

import (
	"go.uber.org/zap"
	. "main/protocol"
	. "main/util"
)

func packetProcessLogin(connUser *connection, bodyData []byte) int16 {
	sessionUID := connUser.getNetworkUniqueID()

	//DB와 연동하지 않으므로 동일 세션이 중복 로그인만 아니면 다 성공으로 한다
	var request LoginReqPacket
	if (&request).Decoding(bodyData) == false {
		return _sendLoginResult(connUser, ERROR_CODE_PACKET_DECODING_FAIL)
	}

	if connMgrInst.setLogin(sessionUID, request.UserID) == false {
		return _sendLoginResult(connUser, ERROR_CODE_LOGIN_USER_DUPLICATION)
	}

	_sendLoginResult(connUser, ERROR_CODE_NONE)
	return ERROR_CODE_NONE
}

func _sendLoginResult(c *connection, result int16) int16 {
	sessionUID := c.getNetworkUniqueID()

	var response LoginResPacket
	response.Result = result
	sendPacket, _ := response.EncodingPacket()

	c.sendPacket(sendPacket)
	LOG_DEBUG("SendLoginResult", zap.Uint64("sessionUID", sessionUID), zap.Int16("result", result))
	return result
}



