package gohipernetFake

import (
	"go.uber.org/zap"
	"net"
)

type TcpSession struct {
	Index          int32
	SeqIndex       uint64
	TcpConn        net.Conn
	NetworkFunctor SessionNetworkFunctors
}

func (session *TcpSession) handleTcpRead(networkFunctor SessionNetworkFunctors) {
	session.NetworkFunctor.OnConnect(session.Index, session.SeqIndex)

	var startRecvPos int16
	var result int
	receiveBuff := make([]byte, MAX_RECEIVE_BUFFER_SIZE)

	for {
		recvBytes, err := session.TcpConn.Read(receiveBuff[startRecvPos:])
		if err != nil {
			NTELIB_LOG_ERROR("handleTcpRead error", zap.String("err", err.Error()))
			session.closeProcess()
			return
		}

		if recvBytes < PACKET_HEADER_SIZE {
			NTELIB_LOG_ERROR("handleTcpRead error : recvBytes is small then PACKET_HEADER_SIZE")
			session.closeProcess()
			return
		}

		readAbleByte := int16(startRecvPos) + int16(recvBytes)
		startRecvPos, result = session.makePacket(readAbleByte, receiveBuff)
		if result != NET_ERROR_NONE {
			NTELIB_LOG_ERROR("handleTcpRead makePacket error : NET_ERROR_RECV_MAKE_PACKET_TOO_LARGE_PACKET_SIZE")
			session.closeProcess()
			return
		}

	}
}

func (session *TcpSession) makePacket(readAbleByte int16, receiveBuff []byte) (int16, int) {
	sessionIndex := session.Index
	sessionUnique := session.SeqIndex

	PacketHeaderSize := session.NetworkFunctor.PacketHeaderSize
	PacketTotalSizeFunc := session.NetworkFunctor.PacketTotalSizeFunc
	var startRecvPos int16 = 0
	var readPos int16

	for {
		if readAbleByte < PacketHeaderSize {
			break
		}

		requireDataSize := PacketTotalSizeFunc(receiveBuff[readPos:])

		if requireDataSize > readAbleByte {
			break
		}

		// 패킷 헤더에 문제가 있는 경우
		if requireDataSize < PacketHeaderSize {
			break
		}

		if requireDataSize > MAX_PACKET_SIZE {
			return startRecvPos, NET_ERROR_RECV_MAKE_PACKET_TOO_LARGE_PACKET_SIZE
		}

		ltvPacket := receiveBuff[readPos:(readPos + requireDataSize)]
		readPos += requireDataSize
		readAbleByte -= requireDataSize

		session.NetworkFunctor.OnReceive(sessionIndex, sessionUnique, ltvPacket)
	}

	if readAbleByte > 0 {
		copy(receiveBuff, receiveBuff[readPos:(readPos+readAbleByte)])
	}

	startRecvPos = readAbleByte
	return startRecvPos, NET_ERROR_NONE
}

func (session *TcpSession) closeProcess() {
	Logger.Info("closeProcess", zap.Int32("sessionIndex", session.Index), zap.Uint64("SeqIndex", session.SeqIndex))

	session.TcpConn.Close()
	session.NetworkFunctor.OnClose(session.Index, session.SeqIndex)

	_tcpSessionManager.removeSession(session.Index, session.SeqIndex)
}

// Send bytes to client
func (session *TcpSession) sendPacket(b []byte) error {
	_, err := session.TcpConn.Write(b)
	return err
}

func (session *TcpSession) close() error {
	return session.TcpConn.Close()
}
