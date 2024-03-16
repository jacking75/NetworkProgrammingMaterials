package main

import "go.uber.org/zap"
import . "GateWayServer/gohipernetFake"

func mqPacketProcess(packet Packet) {
	sessionIndex := packet.UserSessionIndex
	sessionUniqueId := packet.UserSessionUniqueId

	bodySize := mqPacketHeaderSize - packet.DataSize
	bodyData := packet.Data[mqPacketHeaderSize:]
	switch packet.Id {
	case MQ_PACKET_ID_DB_LOGIN_RES:
		_processMQPacketDBResponseLogin(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case MQ_PACKET_ID_ROOM_ENTER_RES:
		_processMQPacketResponseEnterRoom(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case MQ_PACKET_ID_ROOM_LEAVE_RES:
		_processMQPacketResponseLeaveRoom(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case MQ_PACKET_ID_RELAY:
		sendRelayPacketToClient(sessionIndex, sessionUniqueId, bodyData)
	default:
		NTELIB_LOG_ERROR("Unknown PacketId", zap.Uint16("id", packet.Id))
		break
	}
}


func _processMQPacketDBResponseLogin(sessionIndex int32, sessionUniqueId uint64, dataSize int16, mqDataBody []byte) {

	var mqPacket MQLoginResPacket
	if (&mqPacket).Decoding(mqDataBody) == false {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_PACKET_DECODING_FAIL)
		return
	}

	if mqPacket.result == ERROR_CODE_NONE {
		//성공. 상태를 로그인으로 바꾼다
		connectionMgrInst.setLogin(sessionIndex, sessionUniqueId)
	} else {
		connectionMgrInst.setBackState(sessionIndex, sessionUniqueId)
	}

	sendLoginResult(sessionIndex, sessionUniqueId, mqPacket.result)
}

func _processMQPacketResponseEnterRoom(sessionIndex int32, sessionUniqueId uint64, dataSize int16, mqDataBody []byte) {

	var mqPacket MQEnterRoomResPacket
	if (&mqPacket).Decoding(mqDataBody) == false {
		sendEnterRoomResult(sessionIndex, sessionUniqueId, ERROR_CODE_ENTER_ROOM_MQ_PACKET_DECODING_FAIL, -1)
		return
	}

	if mqPacket.result == ERROR_CODE_NONE {
		//성공. 상태를 방입장으로 바꾸고 방 번호로 저장한다
		connectionMgrInst.setRoom(sessionIndex, sessionUniqueId, mqPacket.roomNumber)
	} else {
		connectionMgrInst.setBackState(sessionIndex, sessionUniqueId)
	}

	sendEnterRoomResult(sessionIndex, sessionUniqueId, mqPacket.result, mqPacket.roomNumber)
}

func _processMQPacketResponseLeaveRoom(sessionIndex int32, sessionUniqueId uint64, dataSize int16, mqDataBody []byte) {

	var mqPacket MQLeaveRoomResPacket
	if (&mqPacket).Decoding(mqDataBody) == false {
		sendLeaveRoomResult(sessionIndex, sessionUniqueId, ERROR_CODE_LEAVE_ROOM_MQ_PACKET_DECODING_FAIL)
		return
	}

	if mqPacket.result == ERROR_CODE_NONE {
		//성공. 상태를 방입장으로 바꾸고 방 번호로 저장한다
		connectionMgrInst.setLogin(sessionIndex, sessionUniqueId)
	}

	sendLeaveRoomResult(sessionIndex, sessionUniqueId, mqPacket.result)
}

