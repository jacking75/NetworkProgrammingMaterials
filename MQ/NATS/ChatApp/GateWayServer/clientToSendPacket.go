package main

import (
	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

func sendLoginResult(sessionIndex int32, sessionUniqueId uint64, result int16) {
	var response LoginResPacket
	response.Result = result
	sendPacket, _ := response.EncodingPacket()

	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("SendLoginResult", zap.Int32("sessionIndex", sessionIndex), zap.Int16("result", result))
}

func sendEnterRoomResult(sessionIndex int32, sessionUniqueId uint64, result int16, roomNumber int32) {
	var response RoomEnterResPacket
	response.Result = result
	response.RoomNumber = roomNumber
	sendPacket, _ := response.EncodingPacket()

	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("sendEnterRoomResult", zap.Int32("sessionIndex", sessionIndex), zap.Int16("result", result))
}

func sendLeaveRoomResult(sessionIndex int32, sessionUniqueId uint64, result int16) {
	var response RoomLeaveResPacket
	response.Result = result
	sendPacket, _ := response.EncodingPacket()

	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("sendLeaveRoomResult", zap.Int32("sessionIndex", sessionIndex), zap.Int16("result", result))
}

func sendRelayPacketToClient(sessionIndex int32, sessionUniqueId uint64, sendPacket []byte) {
	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("sendRelayPacketToClient", zap.Int32("sessionIndex", sessionIndex))
}