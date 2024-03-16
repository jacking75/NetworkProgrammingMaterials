package main

import (
	"sync/atomic"
)

const (
	CONNECTION_STATE_CONN = 1
	CONNECTION_STATE_PRE_LOGIN = 2
	CONNECTION_STATE_LOGIN = 3
	CONNECTION_STATE_PRE_ROOM = 4
	CONNECTION_STATE_ROOM = 5
)

type connection struct {
	_index int32

	_networkUniqueID uint64 //네트워크 세션의 유니크 ID

	_userID       [MAX_USER_ID_BYTE_LENGTH]byte
	_userIDLength int8
	_roomNumber int32

	_state int32
	_connectTimeSec int64 // 연결된 시간
}

func (conn *connection) Init(index int32) {
	conn._index = index
	conn.Clear()
}

func (conn *connection) _ClearUserId() {
	conn._userIDLength = 0
}

func (conn *connection) Clear() {
	conn._state = 0
	conn._roomNumber = -1
	conn._ClearUserId()
	conn.SetConnectTimeSec(0, 0)
}

func (conn *connection) GetIndex() int32 {
	return conn._index
}

func (conn *connection) GetNetworkUniqueID() uint64 {
	return atomic.LoadUint64(&conn._networkUniqueID)
}

func (conn *connection) validNetworkUniqueID(uniqueId uint64) bool {
	return atomic.LoadUint64(&conn._networkUniqueID) == uniqueId
}

func (conn *connection) GetNetworkInfo() (int32, uint64) {
	index := conn.GetIndex()
	uniqueID := atomic.LoadUint64(&conn._networkUniqueID)
	return index, uniqueID
}

func (conn *connection) setUserID(userID []byte) {
	conn._userIDLength = int8(len(userID))
	copy(conn._userID[:], userID)
}

func (conn *connection) getUserID() []byte {
	return conn._userID[0:conn._userIDLength]
}

func (conn *connection) getUserIDLength() int8 {
	return conn._userIDLength
}

func (conn *connection) SetConnectTimeSec(timeSec int64, uniqueID uint64) {
	conn._state = CONNECTION_STATE_CONN
	atomic.StoreInt64(&conn._connectTimeSec, timeSec)
	atomic.StoreUint64(&conn._networkUniqueID, uniqueID)
}

func (conn *connection) GetConnectTimeSec() int64 {
	return atomic.LoadInt64(&conn._connectTimeSec)
}

func (conn *connection) SetUser(userID []byte) {
	conn._state = CONNECTION_STATE_PRE_LOGIN
	conn.setUserID(userID)
}

func (conn *connection) SetLogin() {
	conn._state = CONNECTION_STATE_LOGIN
	conn._roomNumber = -1
}

func (conn *connection) SetPreRoom() {
	conn._state = CONNECTION_STATE_PRE_ROOM
}

func (conn *connection) SetRoom(roomNumber int32) {
	conn._state = CONNECTION_STATE_ROOM
	conn._roomNumber = roomNumber
}

func (conn *connection) SetBackState() {
	conn._state -= 1
}

func (conn *connection) IsAuth() bool {
	if conn._userIDLength > 0 {
		return true
	}

	return false
}

func (conn *connection) getRoomNumber() int32 {
	if conn._state != CONNECTION_STATE_ROOM {
		return -1
	}

	return conn._roomNumber
}

