package main

import (
	"github.com/nats-io/stan.go"
	"github.com/panjf2000/gnet"
	"sync/atomic"
)

const (
	CONNECTION_STATE_NONE = 0
	CONNECTION_STATE_CONN = 1
	CONNECTION_STATE_PRE_LOGIN = 2
	CONNECTION_STATE_LOGIN = 3
	CONNECTION_STATE_WATCHING = 5
)

type connection struct {
	netConn gnet.Conn
	_networkUniqueID uint64 //네트워크 세션의 유니크 ID

	mqConn stan.Conn

	_userID       string

	_roomNumber int32

	_state int32
	_connectTimeSec int64 // 연결된 시간

	buffer []byte
}

func (c *connection) init(bufSize int32, net gnet.Conn) {
	c.buffer = make([]byte, bufSize)
	c.netConn = net
	c.clear()
}

func (c *connection) clear() {
	c._state = CONNECTION_STATE_NONE
	c._roomNumber = -1
	c.setConnectTimeSec(0, 0)
}

func (c *connection) getNetworkUniqueID() uint64 {
	return atomic.LoadUint64(&c._networkUniqueID)
}

func (c *connection) validNetworkUniqueID(uniqueId uint64) bool {
	return atomic.LoadUint64(&c._networkUniqueID) == uniqueId
}

func (c *connection) getNetworkInfo() uint64 {
	uniqueID := atomic.LoadUint64(&c._networkUniqueID)
	return uniqueID
}

func (c *connection) getUserID() string {
	return c._userID
}

func (c *connection) setConnectTimeSec(timeSec int64, uniqueID uint64) {
	c._state = CONNECTION_STATE_CONN
	atomic.StoreInt64(&c._connectTimeSec, timeSec)
	atomic.StoreUint64(&c._networkUniqueID, uniqueID)
}

func (c *connection) getConnectTimeSec() int64 {
	return atomic.LoadInt64(&c._connectTimeSec)
}

func (c *connection) connectMQ(address string, clusterID string, clientID string) bool {
	conn, err := stan.Connect(clusterID, clientID, stan.NatsURL(address))
	if err != nil {
		return false
	}

	c._state = CONNECTION_STATE_WATCHING
	c.mqConn = conn
	return true
}

func (c *connection) setLogin(userID []byte) {
	c._userID = string(userID)
	c._state = CONNECTION_STATE_LOGIN
	c._roomNumber = -1
}

func (c *connection) isAuth() bool {
	if c._state >= CONNECTION_STATE_LOGIN {
		return true
	}

	return false
}

func (c *connection) isWatching() bool {
	if c._state == CONNECTION_STATE_WATCHING {
		return true
	}

	return false
}

func (c *connection) sendPacket(packetData []byte) bool {
	c.netConn.AsyncWrite(packetData)
	return true
}

