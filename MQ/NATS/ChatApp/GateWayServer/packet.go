package main

import (
	"encoding/binary"
	"reflect"

	. "GateWayServer/gohipernetFake"
)

const (
	PACKET_TYPE_NORMAL   = 0
	PACKET_TYPE_COMPRESS = 1
	PACKET_TYPE_SECURE   = 2
)

const (
	MAX_USER_ID_BYTE_LENGTH      = 16
	MAX_USER_PW_BYTE_LENGTH      = 16
	MAX_CHAT_MESSAGE_BYTE_LENGTH = 126
)

type Header struct {
	TotalSize  uint16
	ID         uint16
	PacketType uint8 // 비트 필드로 데이터 설정. 0 이면 Normal, 1번 비트 On(압축), 2번 비트 On(암호화)
}

type Packet struct {
	UserSessionIndex    int32
	UserSessionUniqueId uint64
	Id                  uint16
	DataSize            int16
	Data                []byte
}

func (packet Packet) GetSessionInfo() (int32, uint64) {
	return packet.UserSessionIndex, packet.UserSessionUniqueId
}

var _clientSessionHeaderSize int16
var _ServerSessionHeaderSize int16

func Init_packet() {
	_clientSessionHeaderSize = protocolInitHeaderSize()
	_ServerSessionHeaderSize = protocolInitHeaderSize()
}

func ClientHeaderSize() int16 {
	return _clientSessionHeaderSize
}
func ServerHeaderSize() int16 {
	return _ServerSessionHeaderSize
}

func protocolInitHeaderSize() int16 {
	var packetHeader Header
	headerSize := Sizeof(reflect.TypeOf(packetHeader))
	return (int16)(headerSize)
}

// Header의 PacketID만 읽는다
func peekPacketID(rawData []byte) uint16 {
	packetID := binary.LittleEndian.Uint16(rawData[2:])
	return packetID
}

// 보디데이터의 참조만 가져간다
func peekPacketBody(rawData []byte) (bodySize int16, refBody []byte) {
	headerSize := ClientHeaderSize()
	totalSize := int16(binary.LittleEndian.Uint16(rawData))
	bodySize = totalSize - headerSize

	if bodySize > 0 {
		refBody = rawData[headerSize:]
	}

	return bodySize, refBody
}

func DecodingPacketHeader(header *Header, data []byte) {
	reader := MakeReader(data, true)
	header.TotalSize, _ = reader.ReadU16()
	header.ID, _ = reader.ReadU16()
	header.PacketType, _ = reader.ReadU8()
}

func EncodingPacketHeader(writer *RawPacketData, totalSize int16, pktId int16, packetType int8) {
	writer.WriteS16(totalSize)
	writer.WriteS16(pktId)
	writer.WriteS8(packetType)
}

///<<< 패킷 인코딩/디코딩

func MakePacket(sessionIndex int32, sessionUniqueId uint64, packetId uint16, dataSize int16, data []byte) Packet {
	packet := Packet{
		sessionIndex,
		sessionUniqueId,
		packetId,
		dataSize,
		data,
	}
	return packet
}

// [[[ 로그인 ]]] PACKET_ID_LOGIN_REQ
type LoginReqPacket struct {
	UserID []byte
	UserPW []byte
}

func (loginReq LoginReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + MAX_USER_ID_BYTE_LENGTH + MAX_USER_PW_BYTE_LENGTH
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_LOGIN_REQ, 0)
	writer.WriteBytes(loginReq.UserID[:])
	writer.WriteBytes(loginReq.UserPW[:])
	return sendBuf, totalSize
}

func (loginReq *LoginReqPacket) Decoding(bodyData []byte) bool {
	bodySize := MAX_USER_ID_BYTE_LENGTH + MAX_USER_PW_BYTE_LENGTH
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	loginReq.UserID = reader.ReadBytes(MAX_USER_ID_BYTE_LENGTH)
	loginReq.UserPW = reader.ReadBytes(MAX_USER_PW_BYTE_LENGTH)
	return true
}

//
type LoginReqPacketInner struct {
	UserID          []byte
	PassWD          []byte
	SessionIndex    int16
	SessionUniqueId uint64
}

type LoginResPacketInner struct {
	UserID          []byte
	SessionIndex    int16
	SessionUniqueId uint64
	WinCount        uint64
	LoseCount       uint64
	Level           int16
	Rating          uint16
	RoomNum         int16
}

type LoginResPacket struct {
	Result int16
}

func (loginRes LoginResPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_LOGIN_RES, 0)
	writer.WriteS16(loginRes.Result)
	return sendBuf, totalSize
}

// [[[  ]]]   PACKET_ID_ERROR_NTF
type ErrorNtfPacket struct {
	ErrorCode int16
}

func (response ErrorNtfPacket) EncodingPacket(errorCode int16) ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ERROR_NTF, 0)
	writer.WriteS16(errorCode)
	return sendBuf, totalSize
}

func (response *ErrorNtfPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != 2 {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.ErrorCode, _ = reader.ReadS16()
	return true
}

/// [ 방 입장 ]
type RoomEnterReqPacket struct {
	RoomNumber int32
}

func (request RoomEnterReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + (4)
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_ENTER_REQ, 0)
	writer.WriteS32(request.RoomNumber)
	return sendBuf, totalSize
}

func (request *RoomEnterReqPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != (4) {
		return false
	}

	reader := MakeReader(bodyData, true)
	request.RoomNumber, _ = reader.ReadS32()
	return true
}

type RoomEnterResPacket struct {
	Result           int16
	RoomNumber       int32
}

func (response RoomEnterResPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2 + 4
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_ENTER_RES, 0)
	writer.WriteS16(response.Result)
	writer.WriteS32(response.RoomNumber)
	return sendBuf, totalSize
}

func (response *RoomEnterResPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != (2 + 4) {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.Result, _ = reader.ReadS16()
	response.RoomNumber, _ = reader.ReadS32()
	return true
}

//<<< 방에서 나가기
type RoomLeaveResPacket struct {
	Result int16
}

func (response RoomLeaveResPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_LEAVE_RES, 0)
	return sendBuf, totalSize
}

func (response *RoomLeaveResPacket) Decoding(bodyData []byte) bool {
	reader := MakeReader(bodyData, true)
	response.Result, _ = reader.ReadS16()
	return true
}

type RoomLeaveUserNtfPacket struct {
	UserUniqueId uint64
}

func (notify RoomLeaveUserNtfPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 8
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_LEAVE_NTF, 0)
	writer.WriteU64(notify.UserUniqueId)
	return sendBuf, totalSize
}

func (notify RoomLeaveUserNtfPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != 8 {
		return false
	}

	reader := MakeReader(bodyData, true)
	notify.UserUniqueId, _ = reader.ReadU64()
	return true
}

/// [ 방 채팅 ]]
func NotifyErrorPacket(sessionIndex int32, sessionUniqueId uint64, errorCode int16) {
	var response ErrorNtfPacket
	sendBuf, _ := response.EncodingPacket(errorCode)
	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendBuf)
}