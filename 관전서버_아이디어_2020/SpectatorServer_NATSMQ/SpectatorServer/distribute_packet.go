package main

import (
	"go.uber.org/zap"
	"main/protocol"
	. "main/util"
)

func (s *spectatorServer) distributePacket(connUser *connection, packetData []byte) {
	uid := connUser.getNetworkUniqueID()

	packetID := protocol.PeekPacketID(packetData)
	bodySize, bodyData := protocol.PeekPacketBody(packetData)
	LOG_DEBUG("distributePacket", zap.Uint64("sessionUniqueID", uid), zap.Uint16("PacketID", packetID), zap.Int16("bodySize", bodySize))

	pfunc := s.getPacketFunction(packetID)

	if(pfunc != nil) {
		pfunc(connUser, bodyData)
	} else {
		/*packet := protocol.Packet{ID: packetID}
		packet.SessionUID = uid
		packet.ID = packetID
		packet.BodyData = bodyData
		packet.TotalData = packetData

		s.PacketChan <- packet*/
	}
}



var _config serverConfig

func setServerConfig(config serverConfig) {
	_config = config
}

func getServerConfig() *serverConfig {
	return &_config
}


/*func (s *spectatorServer) distributePacket_system(uid uint64, packetID uint16) {
	packet := protocol.Packet{ID: packetID}
	packet.SessionUID = uid
	packet.BodyData = nil
	packet.TotalData = nil

	s.PacketChan <- packet

	LOG_DEBUG("distributePacket_system", zap.Uint64("sessionUID", uid), zap.Uint16("PacketID", packetID))
}*/

/*func (s *spectatorServer) PacketProcess_goroutine() {
	LOG_INFO("start packetProcess goroutine")

	for {
		if s.PacketProcess_goroutine_Impl() {
			LOG_INFO("Wanted Stop packetProcess goroutine")
			break
		}
	}

	LOG_INFO("Stop rooms packetProcess goroutine")
}*/

/*func (s *spectatorServer) PacketProcess_goroutine_Impl() bool {
	IsWantedTermination := false  // 여기에서는 의미 없음. 서버 종료를 명시적으로 하는 경우만 유용
	defer PrintPanicStack()

	//TODO 수정해야 한다
	for {
		packet := <-s.PacketChan
		//uid := packet.SessionUID
		//bodyData := packet.BodyData

		if packet.ID == protocol.PACKET_ID_LOGIN_REQ {
			//ProcessPacketLogin(uid, bodyData)
		} else if packet.ID == protocol.PACKET_ID_SESSION_CLOSE_SYS {
			//processPacketSessionClosed(s, uid)
		} else {
			//roomNumber := connMgrInst.getRoomNumber(uid)
			//s.RoomMgr.packetProcess(roomNumber, packet)
		}
	}

	return IsWantedTermination
}*/




