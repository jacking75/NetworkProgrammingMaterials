package main

import . "GateWayServer/gohipernetFake"


// 1001 ~ 2000
const (
	MQ_PACKET_ID_DB_LOGIN_REQ = 1001
	MQ_PACKET_ID_DB_LOGIN_RES = 1002

	MQ_PACKET_ID_ROOM_ENTER_REQ = 1021
	MQ_PACKET_ID_ROOM_ENTER_RES = 1022

	MQ_PACKET_ID_ROOM_LEAVE_REQ = 1026
	MQ_PACKET_ID_ROOM_LEAVE_RES = 1027

	MQ_PACKET_ID_RELAY = 1031
)

type MQPacket struct {
	senderInitial byte
	senderIndex uint16
	userNetSessionIndex int32
	userNetSessionUniqueID uint64
	packetID uint16

	data []byte
}

var mqPacketHeaderSize int16 = 17
type MQPacketHeader struct {
	senderInitial int8
	senderIndex uint16
	userNetSessionIndex int32
	userNetSessionUniqueID uint64
	mqPacketID uint16
}

func EncodingMQPacketHeader(writer *RawPacketData, header MQPacketHeader) {
	writer.WriteS8(header.senderInitial)
	writer.WriteU16(header.senderIndex)
	writer.WriteS32(header.userNetSessionIndex)
	writer.WriteU64(header.userNetSessionUniqueID)
	writer.WriteU16(header.mqPacketID)
}

func DecodingMQPacketHeader(reader *RawPacketData, header *MQPacketHeader) {
	header.senderInitial, _ = reader.ReadS8()
	header.senderIndex, _ = reader.ReadU16()
	header.userNetSessionIndex, _ = reader.ReadS32()
	header.userNetSessionUniqueID, _ = reader.ReadU64()
	header.mqPacketID, _ = reader.ReadU16()
}

func EncodingMQPacketNoneBody(header MQPacketHeader) []byte {
	sendBuf := make([]byte, mqPacketHeaderSize)
	writer := MakeWriter(sendBuf, true)

	writer.WriteS8(header.senderInitial)
	writer.WriteU16(header.senderIndex)
	writer.WriteS32(header.userNetSessionIndex)
	writer.WriteU64(header.userNetSessionUniqueID)
	writer.WriteU16(header.mqPacketID)

	return sendBuf
}


type MQLoginReqPacket struct {
	header MQPacketHeader
	userID []byte
	userPW []byte
}

func (loginReq MQLoginReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := mqPacketHeaderSize + MAX_USER_ID_BYTE_LENGTH + MAX_USER_PW_BYTE_LENGTH
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, loginReq.header)
	writer.WriteBytes(loginReq.userID[:])
	writer.WriteBytes(loginReq.userPW[:])
	return sendBuf, totalSize
}

type MQLoginResPacket struct {
	header MQPacketHeader
	result int16
}

func (loginRes *MQLoginResPacket) Decoding(bodyData []byte) bool {
	bodySize := 2
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	loginRes.result, _ = reader.ReadS16()
	return true
}


// 방 입장
type MQEnterRoomReqPacket struct {
	header MQPacketHeader
	userID []byte
	roomNumber int32
}

func (request MQEnterRoomReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := mqPacketHeaderSize + MAX_USER_ID_BYTE_LENGTH + 4
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)
	writer.WriteBytes(request.userID[:])
	writer.WriteS32(request.roomNumber)
	return sendBuf, totalSize
}

type MQEnterRoomResPacket struct {
	header MQPacketHeader
	result int16
	roomNumber int32
}

func (response *MQEnterRoomResPacket) Decoding(bodyData []byte) bool {
	bodySize := 6
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.result, _ = reader.ReadS16()
	response.roomNumber, _ = reader.ReadS32()
	return true
}


// 방 나가기
type MQLeaveRoomReqPacket struct {
	header MQPacketHeader
	isDisConnected int8
}

func (request MQLeaveRoomReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := mqPacketHeaderSize + 1
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)
	writer.WriteS8(request.isDisConnected)
	return sendBuf, totalSize
}

type MQLeaveRoomResPacket struct {
	header MQPacketHeader
	result int16
}

func (response *MQLeaveRoomResPacket) Decoding(bodyData []byte) bool {
	bodySize := 2
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.result, _ = reader.ReadS16()
	return true
}


// 릴레이 패킷
type MQRelayPacket struct {
	header MQPacketHeader
	relayPacket []byte
}

func (request *MQRelayPacket) Encoding() ([]byte, int16) {
	relayDataSize := int16(len(request.relayPacket))
	totalSize := mqPacketHeaderSize + relayDataSize
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)
	writer.WriteBytes(request.relayPacket[:])
	return sendBuf, totalSize
}

func (response *MQRelayPacket) Decoding(bodyData []byte) bool {
	reader := MakeReader(bodyData, true)
	relayDataSize := len(bodyData)
	response.relayPacket = reader.ReadBytes(relayDataSize)
	return true
}