package main

import (
	"main/protocol"
)

func (s *spectatorServer) settingPacketFunction() {
	maxFuncListCount := 16
	s.funclist = make([]pakcetFunc, 0, maxFuncListCount)
	s.funcPackeIDlist = make([]uint16, 0, maxFuncListCount)

	s._addPacketFunction(protocol.PACKET_ID_LOGIN_REQ, packetProcessLogin)
	s._addPacketFunction(protocol.PACKET_ID_START_WATCHING_REQ, packetProcessStartWatching)
}

func (s *spectatorServer) _addPacketFunction(packetID uint16,
	packetFunc func(*connection, []byte,
) int16) {
	s.funclist = append(s.funclist, packetFunc)
	s.funcPackeIDlist = append(s.funcPackeIDlist, packetID)
}

func (s *spectatorServer) getPacketFunction(packetID uint16	) pakcetFunc {
	for i, value := range s.funcPackeIDlist {
		if value == packetID {
			return s.funclist[i]
		}
	}

	return nil
}



