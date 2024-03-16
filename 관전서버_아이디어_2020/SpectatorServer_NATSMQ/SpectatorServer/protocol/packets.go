package protocol

import (
	"encoding/binary"
)

const (
	DefaultHeadLength    uint16 = 8
	TotalSizePosInHeader uint16 = 3
	PacketIDPosInHeader  uint16 = 5
)

/*const (
	MAX_USER_ID_BYTE_LENGTH      = 32
	MAX_USER_PW_BYTE_LENGTH      = 32
	MAX_CHAT_MESSAGE_BYTE_LENGTH = 126
)*/

type Header struct {
	TotalSize  uint16
	ID         uint16
	PacketType int8 // 비트 필드로 데이터 설정. 0 이면 Normal, 1번 비트 On(압축), 2번 비트 On(암호화)
}

type Packet struct {
	SessionUID uint64
	ID         uint16
	BodyData	[]byte
	TotalData  []byte
}

var _clientSessionHeaderSize int16
var _ServerSessionHeaderSize int16

func Init_packet() {
	_clientSessionHeaderSize = int16(DefaultHeadLength)
	_ServerSessionHeaderSize = int16(DefaultHeadLength)
}

func ClientHeaderSize() int16 {
	return _clientSessionHeaderSize
}
func ServerHeaderSize() int16 {
	return _ServerSessionHeaderSize
}

// Header의 PacketID만 읽는다
func PeekPacketID(rawData []byte) uint16 {
	packetID := binary.LittleEndian.Uint16(rawData[PacketIDPosInHeader:])
	return packetID
}

// 보디데이터의 참조만 가져간다
func PeekPacketBody(rawData []byte) (bodySize int16, refBody []byte) {
	headerSize := ClientHeaderSize()
	totalSize := int16(binary.LittleEndian.Uint16(rawData[TotalSizePosInHeader:]))
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
	header.PacketType, _ = reader.ReadS8()
}

func EncodingPacketHeader(writer *RawPacketData, totalSize uint16, pktId uint16, packetType int8) {
	writer.WriteS8(0)
	writer.WriteS8(0)
	writer.WriteS8(0)
	writer.WriteU16(totalSize)
	writer.WriteU16(pktId)
	writer.WriteS8(packetType)
}



///<<< 패킷 인코딩/디코딩

// [[[ 로그인 ]]] PACKET_ID_LOGIN_REQ
type LoginReqPacket struct {
	UserID []byte
	PassWD []byte
}

func (loginReq *LoginReqPacket) Decoding(bodyData []byte) bool {
	reader := MakeReader(bodyData, true)
	loginReq.UserID, _ = reader.ReadString()
	loginReq.PassWD, _ = reader.ReadString()
	return true
}

type LoginResPacket struct {
	Result int16
}

func (loginRes LoginResPacket) EncodingPacket() ([]byte, uint16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, uint16(totalSize), PACKET_ID_LOGIN_RES, 0)
	writer.WriteS16(loginRes.Result)
	return sendBuf, uint16(totalSize)
}



//
type StartWatchingReqPacket struct {
	Subject []byte
}

func (req *StartWatchingReqPacket) Decoding(bodyData []byte) bool {
	reader := MakeReader(bodyData, true)
	req.Subject, _ = reader.ReadString()
	return true
}


type StartWatchingResPacket struct {
	Result int16
}

func (res StartWatchingResPacket) EncodingPacket() ([]byte, uint16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, uint16(totalSize), PACKET_ID_START_WATCHING_RES, 0)
	writer.WriteS16(res.Result)
	return sendBuf, uint16(totalSize)
}



//
type ErrorNtfPacket struct {
	ErrorCode int16
}

func (response ErrorNtfPacket) EncodingPacket(errorCode int16) ([]byte, uint16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, uint16(totalSize), PACKET_ID_ERROR_NTF, 0)
	writer.WriteS16(errorCode)
	return sendBuf, uint16(totalSize)
}

func (response *ErrorNtfPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != 2 {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.ErrorCode, _ = reader.ReadS16()
	return true
}

