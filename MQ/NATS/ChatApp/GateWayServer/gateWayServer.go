package main

import (
	"go.uber.org/zap"
	"strconv"
	"strings"
	"time"

	. "GateWayServer/gohipernetFake"
)

type configAppServer struct {
	Name string

	MaxPacketChanCapycity int32

	MQAddress              string

	MyServerIndex int
	//ChatServerIndexList []int

	MaxUserCount int
	RoomStartNum int
	RoomMaxCount int
}

type GateWayServer struct {
	ServerIndex int
	IP          string
	Port        int

	PacketChan      chan Packet
}

func createAnsStartServer(netConfig NetworkConfig, appConfig configAppServer) {
	NTELIB_LOG_INFO("CreateServer !!!")

	var server GateWayServer
	server.ServerIndex = appConfig.MyServerIndex

	if server.setIPAddress(netConfig.BindAddress) == false {
		NTELIB_LOG_ERROR("fail. server address")
		return
	}

	Init_packet()

	distributePacketInit(appConfig)

	connectionMgrInst.init(netConfig.MaxSessionCount, int32(appConfig.MaxUserCount))

	server.PacketChan = make(chan Packet, appConfig.MaxPacketChanCapycity)

	inComingMqDataFunc := func(data []byte) {
		server.inComingMQData(data)
	}

	mqItit(appConfig, inComingMqDataFunc)

	//TODO 아래 함수는 테스트 용이니 제거해야 한다
	//sendToMqRequestLogin(uint16(server.ServerIndex), 1, 1, []byte("dds"), []byte("AAA"))

	go server.packetProcess_goroutine()


	stopFunc := func() {
		server.Stop()
	}
	go signalsHandling_goroutine(stopFunc)

	networkFunctor := SessionNetworkFunctors{}
	networkFunctor.OnConnect = server.OnConnect
	networkFunctor.OnReceive = server.OnReceive
	networkFunctor.OnReceiveBufferedData = nil
	networkFunctor.OnClose = server.OnClose
	networkFunctor.PacketTotalSizeFunc = PacketTotalSize
	networkFunctor.PacketHeaderSize = PACKET_HEADER_SIZE
	networkFunctor.IsClientSession = true

	NetLibInitNetwork(PACKET_HEADER_SIZE, PACKET_HEADER_SIZE)
	NetLibStartNetwork(&netConfig, networkFunctor)

	waitingServerStop()
	time.Sleep(1 * time.Second)
	NTELIB_LOG_INFO("END !!!!!!!")
}

func (server *GateWayServer) setIPAddress(ipAddress string) bool {
	results := strings.Split(ipAddress, ":")
	if len(results) != 2 {
		return false
	}

	server.IP = results[0]
	server.Port, _ = strconv.Atoi(results[1])

	NTELIB_LOG_INFO("Server Address", zap.String("IP", server.IP), zap.Int("Port", server.Port))
	return true
}

func (server *GateWayServer) Stop() {
	NTELIB_LOG_INFO("Stop GateWayServer Start !!!")

	mqClose()

	NetLibStop() // 이 함수가 꼭 제일 먼저 호출 되어야 한다.

	NTELIB_LOG_INFO("Stop GateWayServer End !!!")
}

func (server *GateWayServer) OnConnect(sessionIndex int32, sessionUniqueID uint64) {
	NTELIB_LOG_INFO("client OnConnect", zap.Int32("sessionIndex", sessionIndex), zap.Uint64("sessionUniqueId", sessionUniqueID))

	connectionMgrInst.addSession(sessionIndex, sessionUniqueID)
}

func (server *GateWayServer) OnReceive(sessionIndex int32, sessionUniqueID uint64, data []byte) bool {
	NTELIB_LOG_DEBUG("OnReceive", zap.Int32("sessionIndex", sessionIndex),
		zap.Uint64("sessionUniqueID", sessionUniqueID),
		zap.Int("packetSize", len(data)))

	distributePacket(sessionIndex, sessionUniqueID, data)
	return true
}

func (server *GateWayServer) OnClose(sessionIndex int32, sessionUniqueID uint64) {
	NTELIB_LOG_INFO("client OnCloseClientSession", zap.Int32("sessionIndex", sessionIndex), zap.Uint64("sessionUniqueId", sessionUniqueID))

	server.disConnectClient(sessionIndex, sessionUniqueID)
}

func (server *GateWayServer) disConnectClient(sessionIndex int32, sessionUniqueId uint64) {
	roomNumber := connectionMgrInst.getRoomNumber(sessionIndex, sessionUniqueId)
	if roomNumber != -1 {
		chatServerIndex := uint16(21)
		sendToMqRequestLeaveRoom(uint16(_appConfig.MyServerIndex), chatServerIndex, sessionIndex, sessionUniqueId, 1)
	}

	connectionMgrInst.removeSession(sessionIndex, false)
	NTELIB_LOG_INFO("DisConnectClient - Login User", zap.Int32("sessionIndex", sessionIndex))
}