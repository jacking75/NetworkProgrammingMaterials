package main

import (
	"bytes"
	"time"

	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

func distributePacketInit(appConfig configAppServer) {
	_appConfig = appConfig
}

func distributePacket(sessionIndex int32, sessionUniqueId uint64, packetData []byte) {
	packetID := peekPacketID(packetData)
	bodySize, bodyData := peekPacketBody(packetData)
	NTELIB_LOG_DEBUG("distributePacket", zap.Int32("sessionIndex", sessionIndex), zap.Uint64("sessionUniqueId", sessionUniqueId), zap.Uint16("PacketID", packetID))

	if _enableClientRequestPacketIDRange(packetID, sessionIndex) == false{
		return
	}

	switch packetID {
	case PACKET_ID_LOGIN_REQ:
		_processPacketLogin(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case PACKET_ID_ROOM_ENTER_REQ:
		_processPacketEnterRoom(sessionIndex, sessionUniqueId, bodySize, bodyData)
	case PACKET_ID_ROOM_LEAVE_REQ:
		_processPacketLeaveRoom(sessionIndex, sessionUniqueId, bodySize, bodyData)
	default:
		if _enableClientRequestRelayPacketIDRange(packetID) {
			_processPacketRelay(sessionIndex, sessionUniqueId, packetData)
		} else {
			NTELIB_LOG_ERROR("Unknown PacketId", zap.Uint16("id", packetID))
		}
	}

	/*packet := Packet{Id: packetID}
	packet.UserSessionIndex = sessionIndex
	packet.UserSessionUniqueId = sessionUniqueId
	packet.Id = packetID
	packet.DataSize = bodySize
	packet.Data = make([]byte, packet.DataSize)
	copy(packet.Data, bodyData)

	server.PacketChan <- packet*/

	NTELIB_LOG_DEBUG("_distributePacket", zap.Int32("sessionIndex", sessionIndex), zap.Uint16("PacketId", packetID))
}

func (server *GateWayServer) sendInnerPacket(packet Packet) {
	server.PacketChan <- packet
}

func (server *GateWayServer) inComingMQData(data []byte) {
	NTELIB_LOG_DEBUG("inComingMQData", zap.Int("mqDataSize", len(data)))

	var header MQPacketHeader
	reader := MakeReader(data, true)
	DecodingMQPacketHeader(&reader, &header)

	packet := Packet{Id: header.mqPacketID}
	packet.UserSessionIndex = header.userNetSessionIndex
	packet.UserSessionUniqueId = header.userNetSessionUniqueID
	packet.DataSize = int16(len(data))
	packet.Data = data

	server.PacketChan <- packet
}

func (server *GateWayServer) packetProcess_goroutine() {
	NTELIB_LOG_INFO("start PacketProcess goroutine")

	for {
		if server.packetProcess_goroutine_Impl() {
			NTELIB_LOG_INFO("Wanted Stop PacketProcess goroutine")
			break
		}
	}

	NTELIB_LOG_INFO("Stop rooms PacketProcess goroutine")
}

func (server *GateWayServer) packetProcess_goroutine_Impl() bool {
	defer PrintPanicStack()

	secondTimeTicker := time.NewTicker(time.Second)
	defer secondTimeTicker.Stop()

	for {
		select {
		case packet := <-server.PacketChan:
			{
				if packet.Id > PACKET_ID_C2S_END { //MQ 패킷
					mqPacketProcess(packet)
				}
			}
		/*case curTime := <-secondTimeTicker.C:
			{
				server.RoomMgr.CheckRoomState(curTime.Unix())
			}*/
		}
	}
}


func _processPacketLogin(sessionIndex int32, sessionUniqueId uint64,
	bodySize int16, bodyData []byte) {
	//TODO connections 객체를 조사해서 로그인 요청 전인지 조사한다

	var request LoginReqPacket
	if (&request).Decoding(bodyData) == false {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_PACKET_DECODING_FAIL)
		return
	}

	userId := bytes.Trim(request.UserID[:], "\x00")
	if len(userId) <= 0 {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_LOGIN_USER_INVALID_ID)
		return
	}

	userPw := bytes.Trim(request.UserPW[:], "\x00")
	if len(userPw) <= 0 {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_LOGIN_USER_INVALID_PW)
		return
	}

	sendToMqRequestLogin(uint16(_appConfig.MyServerIndex), sessionIndex, sessionUniqueId, request.UserID, request.UserPW)

	connectionMgrInst.preLogin(sessionIndex, sessionUniqueId, request.UserID)
}

func _processPacketEnterRoom(sessionIndex int32, sessionUniqueId uint64, bodySize int16, bodyData []byte) {
	//TODO connections 객체를 조사해서 로그인이 완료된 상태인지 조사한다.
	// 유저 상태는 접속, 로그인 중, 로그인, 방입장중, 방입장

	var request RoomEnterReqPacket
	if (&request).Decoding(bodyData) == false {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_ENTER_ROOM_PACKET_DECODING_FAIL)
		return
	}

	//TODO 방을 관리하는 로비 서버로 보낸다.

	chatServerIndex := uint16(21)
	sendToMqRequestEnterRoom(uint16(_appConfig.MyServerIndex), chatServerIndex, sessionIndex, sessionUniqueId, request.RoomNumber)

	//TODO 방 입장 중으로 바꾼다
	connectionMgrInst.setPreRoom(sessionIndex, sessionUniqueId)
}

func _processPacketLeaveRoom(sessionIndex int32, sessionUniqueId uint64, bodySize int16, bodyData []byte) {
	//TODO connections 객체를 조사해서 방입장 상태만 가능


	//TODO 방을 관리하는 로비 서버로 보낸다.
	chatServerIndex := uint16(21)
	sendToMqRequestLeaveRoom(uint16(_appConfig.MyServerIndex), chatServerIndex, sessionIndex, sessionUniqueId, 0)
}

func _processPacketRelay(sessionIndex int32, sessionUniqueId uint64, packetData []byte) {
	//TODO connections 객체를 조사해서 방입장을 한 상태만 릴레이 가능


	//TODO 방을 관리하는 로비 서버로 보낸다.
	chatServerIndex := uint16(21)
	sendToMqRequestRelay(uint16(_appConfig.MyServerIndex), chatServerIndex, sessionIndex, sessionUniqueId, packetData)
}

func _enableClientRequestPacketIDRange(packetID uint16, sessionIndex int32) bool {
	if packetID <= PACKET_ID_C2S_START || packetID >= PACKET_ID_C2S_END {
		NTELIB_LOG_DEBUG("_distributePacket. Invalid Packet", zap.Int32("sessionIndex", sessionIndex), zap.Uint16("PacketId", packetID))
		return false
	}

	return true
}

func _enableClientRequestRelayPacketIDRange(packetID uint16) bool {
	//TODO 릴레이 할 것이 맞아지면 딕셔너릴 등으로 조사한다
	if packetID == PACKET_ID_ROOM_CHAT_REQ {
		return true
	}

	return false
}

var _appConfig configAppServer




/*
func _processPacketSessionClosed(server *GateWayServer, sessionIndex int32, sessionUniqueId uint64) {
	roomNumber, _ := connectedSessions.GetRoomNumber(sessionIndex)

	if roomNumber > -1 {
		packet := protocol.MakePacket(sessionIndex, sessionUniqueId, protocol.PACKET_ID_ROOM_LEAVE_REQ, 0, nil)

		server.RoomMgr.PacketProcess(roomNumber, packet)
	}

	connectedSessions.RemoveSession(sessionIndex, true)
}*/