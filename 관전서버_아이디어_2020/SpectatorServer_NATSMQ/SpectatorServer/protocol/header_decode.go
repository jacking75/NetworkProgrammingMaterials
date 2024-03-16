package protocol

import "github.com/panjf2000/gnet"

import (
	"encoding/binary"
	"errors"
)



type CustomLengthFieldProtocol struct {
	DataLength uint16
	PacketID	uint16
	Property	byte
	Data       []byte
}

// Encode ...
func (cc *CustomLengthFieldProtocol) Encode(c gnet.Conn, buf []byte) ([]byte, error) {
	// 엔코더는 사용하지 않는다
	return buf, nil
}

// Decode ...
func (cc *CustomLengthFieldProtocol) Decode(c gnet.Conn) ([]byte, error) {
	// parse header
	headerLen := int(DefaultHeadLength)
	if size, header := c.ReadN(headerLen); size == headerLen {
		dataLength := binary.LittleEndian.Uint16(header[TotalSizePosInHeader:])

		//if 만약 프로토콜 읽을 때 에러가 발생하면 {
		//	c.ResetBuffer()
		//	log.Println("not normal protocol:", pbVersion, DefaultProtocolVersion, actionType, dataLength)
		//	return nil, errors.New("not normal protocol")
		//}

		// parse payload
		protocolLen := int(dataLength) //max int32 can contain 210MB payload
		if dataSize, data := c.ReadN(protocolLen); dataSize == protocolLen {
			c.ShiftN(protocolLen)

			// return the payload of the data
			return data, nil
		}
		return nil, errors.New("not enough payload data")
	}
	return nil, errors.New("not enough header data")
}