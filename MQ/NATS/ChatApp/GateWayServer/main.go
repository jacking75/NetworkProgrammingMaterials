package main

import (
	. "GateWayServer/gohipernetFake"
	"flag"
)

func main() {
	NetLibInitLog()
	netConfig, appConfig := parseAppConfig()
	netConfig.WriteNetworkConfig(true)

	createAnsStartServer(netConfig, appConfig)
}

type arrayChatServerIndexFlag []string
var arrayChatServerIndex arrayChatServerIndexFlag
func (i *arrayChatServerIndexFlag) String() string {
	return "my ChatServerIndex representation"
}
func (i *arrayChatServerIndexFlag) Set(value string) error {
	*i = append(*i, value)
	return nil
}

func parseAppConfig() (NetworkConfig, configAppServer) {
	NTELIB_LOG_INFO("[[Setting NetworkConfig]]")

	appConfig := configAppServer{}
	netConfig := NetworkConfig{}

	flag.BoolVar(&netConfig.IsTcp4Addr, "c_IsTcp4Addr", true, "bool flag")
	flag.StringVar(&netConfig.BindAddress, "c_BindAddress", "192.168.0.11:11021", "string flag")
	flag.IntVar(&netConfig.MaxSessionCount, "c_MaxSessionCount", 0, "int flag")
	flag.IntVar(&netConfig.MaxPacketSize, "c_MaxPacketSize", 0, "int flag")
	flag.StringVar(&appConfig.Name, "c_Name", "matgoServer", "string flag")

	flag.StringVar(&appConfig.MQAddress, "c_MQAddress", "0.0.0.0", "string flag")
	flag.IntVar(&appConfig.MyServerIndex, "c_MyServerIndex", 0, "int flag")
	flag.Var(&arrayChatServerIndex, "c_ChatServerIndexList", "Connected Chat Server Index")

	flag.IntVar(&appConfig.RoomMaxCount, "c_RoomMaxCount", 1000, "int flag")
	flag.IntVar(&appConfig.RoomStartNum, "c_RoomStartNum", 0, "int flag")
	flag.IntVar(&appConfig.MaxUserCount, "c_MaxUserCount", 2, "int flag")

	flag.Parse()

	/*appConfig.ChatServerIndexList = make([]int, 0, 16)
	for _, value := range arrayChatServerIndex {
		index, _ := strconv.Atoi(value)
		appConfig.ChatServerIndexList = append(appConfig.ChatServerIndexList, index)
		NTELIB_LOG_INFO("parseAppConfig", zap.Int("ChatServerIndex", index))
	}*/

	netConfig.ServerIndex = uint16(appConfig.MyServerIndex)
	return netConfig, appConfig
}