package main


func sendToMqRequestLogin(serverIndex uint16, sessionIndex int32, sessionUniqueId uint64, userID []byte, userPW []byte) {
	reqMQPkt := MQLoginReqPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = serverIndex
	reqMQPkt.header.userNetSessionIndex = sessionIndex
	reqMQPkt.header.userNetSessionUniqueID = sessionUniqueId
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_DB_LOGIN_REQ
	reqMQPkt.userID = userID
	reqMQPkt.userPW = userPW

	mqData, _ := reqMQPkt.EncodingPacket()
	mqSend(MQDBReqSubject, mqData)
}

func sendToMqRequestEnterRoom(myServerIndex uint16, chatServerIndex uint16, sessionIndex int32, sessionUniqueId uint64, roomNumber int32) {
	userID, _ := connectionMgrInst.getUserID(sessionIndex)

	reqMQPkt := MQEnterRoomReqPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = myServerIndex
	reqMQPkt.header.userNetSessionIndex = sessionIndex
	reqMQPkt.header.userNetSessionUniqueID = sessionUniqueId
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_ROOM_ENTER_REQ
	reqMQPkt.roomNumber = roomNumber
	reqMQPkt.userID = make([]byte, MAX_USER_ID_BYTE_LENGTH)
	copy(reqMQPkt.userID, userID)

	subject := mqChatServerSubjet(chatServerIndex)
	mqData, _ := reqMQPkt.EncodingPacket()
	mqSend(subject, mqData)
}

func sendToMqRequestLeaveRoom(myServerIndex uint16, chatServerIndex uint16, sessionIndex int32, sessionUniqueId uint64, isDisConnected int8) {
	reqMQPkt := MQLeaveRoomReqPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = myServerIndex
	reqMQPkt.header.userNetSessionIndex = sessionIndex
	reqMQPkt.header.userNetSessionUniqueID = sessionUniqueId
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_ROOM_LEAVE_REQ
	reqMQPkt.isDisConnected = isDisConnected

	subject := mqChatServerSubjet(chatServerIndex)
	mqData, _ := reqMQPkt.EncodingPacket()
	mqSend(subject, mqData)
}

func sendToMqRequestRelay(myServerIndex uint16, chatServerIndex uint16, sessionIndex int32, sessionUniqueId uint64, relayData []byte) {
	reqMQPkt := MQRelayPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = myServerIndex
	reqMQPkt.header.userNetSessionIndex = sessionIndex
	reqMQPkt.header.userNetSessionUniqueID = sessionUniqueId
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_RELAY
	reqMQPkt.relayPacket = relayData

	mqData, _ := reqMQPkt.Encoding()
	subject := mqChatServerSubjet(chatServerIndex)
	mqSend(subject, mqData)
}