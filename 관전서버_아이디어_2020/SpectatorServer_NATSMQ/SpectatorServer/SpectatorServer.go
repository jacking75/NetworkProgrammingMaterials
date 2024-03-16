package main

import (
	"github.com/panjf2000/gnet"
	"go.uber.org/zap"
	"main/protocol"
	. "main/util"
	"sync/atomic"
	"fmt"
)

type serverConfig struct {
	port int
	mqAddress string
	mqCID string // 스트르밍MQ 서버 클러스터 ID
}

type pakcetFunc func(*connection, []byte) int16

type spectatorServer struct {
	*gnet.EventServer
	addr       string
	multicore  bool
	async      bool
	codec      gnet.ICodec


	config serverConfig

	seqUniqueID uint64

	funcPackeIDlist []uint16
	funclist        []pakcetFunc

	PacketChan	chan protocol.Packet
}

func (s *spectatorServer) OnInitComplete(srv gnet.Server) (action gnet.Action) {
	LOG_INFO("OnInitComplete", zap.String("addr", srv.Addr.String()), zap.Bool("IsMulticore",srv.Multicore), zap.Int("NumEventLoop", srv.NumEventLoop))
	return
}

func (s *spectatorServer) OnOpened(c gnet.Conn) (out []byte, action gnet.Action) {
	newUID := atomic.AddUint64(&s.seqUniqueID, 1)
	LOG_DEBUG("OnOpened", zap.Uint64("UID", newUID))
	connMgrInst.addSession(c, newUID)
	return
}

func (s *spectatorServer) OnClosed(c gnet.Conn, err error) (action gnet.Action) {
	sessionUID := connMgrInst.getSessionUID(c)
	connMgrInst.removeSession(sessionUID, true)

	LOG_DEBUG("OnClosed", zap.Uint64("UID", sessionUID))
	return
}

func (s *spectatorServer) React(frame []byte, c gnet.Conn) (out []byte, action gnet.Action) {
	session := connMgrInst.getSession(c)
	if session == nil {
		LOG_ERROR("React - unknown client")
		return

	}

	packetData := make([]byte, len(frame))
	copy(packetData, frame)

	s.distributePacket(session, packetData)
	return
}

func startServe(config serverConfig) {
	Init_Log()

	setServerConfig(config)

	serverAddr := fmt.Sprintf("tcp://:%d", config.port)
	cs := &spectatorServer{
		addr: serverAddr,
		multicore: true,
		codec: &protocol.CustomLengthFieldProtocol{},
		config: config,
	}

	connMgrInst.init(2100, 2000)

	protocol.Init_packet()

	//cs.PacketChan = make(chan protocol.Packet, 256)
	//go cs.PacketProcess_goroutine()

	LOG_INFO("string gnet.Serve")

	err := gnet.Serve(cs, serverAddr, gnet.WithMulticore(cs.multicore), gnet.WithTCPKeepAlive(0), gnet.WithCodec(cs.codec))
	if err != nil {
		panic(err)
	}

	LOG_INFO("terminated server")
}



/*
func (server *ChatServer) disConnectClient(sessionIndex int32, sessionUniqueId uint64) {
	// 로그인도 안한 유저라면 그냥 여기서 처리한다.
	// 방 입장을 안한 유저라면 여기서 처리해도 괜찮지만 아래로 넘긴다.
	if connectedSessions.IsLoginUser(sessionIndex) == false {
		NTELIB_LOG_INFO("DisConnectClient - Not Login User", zap.Int32("sessionIndex", sessionIndex))
		connectedSessions.RemoveSession(sessionIndex, false)
		return
	}


	protocol := protocol.Packet {
		sessionIndex,
		sessionUniqueId,
		protocol.PACKET_ID_SESSION_CLOSE_SYS,
		0,
		nil,
	}

	server.PacketChan <- protocol

	NTELIB_LOG_INFO("DisConnectClient - Login User", zap.Int32("sessionIndex", sessionIndex))
}
*/