package main

import (
	"github.com/panjf2000/gnet"
	"sync"
	"sync/atomic"
	"time"
)


const CONN_BUFFER_SIZE = 2048;

// 스레드 세이프 해야 한다.
type connectionManager struct {
	_netConnMap *sync.Map
	_uidMap     *sync.Map

	_maxConnectionCount int32

	_maxUserCount          int32 // ?
	_currentLoginUserCount int32 // ?
}

var connMgrInst connectionManager

func (mgr* connectionManager) init(maxConnectionCount int, maxUserCount int32) bool {
	mgr._netConnMap = new(sync.Map)
	mgr._uidMap = new(sync.Map)
	mgr._maxUserCount = maxUserCount
	mgr._maxConnectionCount = int32(maxConnectionCount)

	return true
}

func (mgr* connectionManager) addSession(net gnet.Conn, sessionUniqueID uint64) *connection {
	conn := new(connection)
	conn.init(CONN_BUFFER_SIZE, net)
	conn.setConnectTimeSec(time.Now().Unix(), sessionUniqueID)

	mgr._netConnMap.Store(net, conn)
	mgr._uidMap.Store(sessionUniqueID, conn)
	return conn
}

func (mgr* connectionManager) removeSession(uid uint64, isLoginedUser bool) bool {
	session := mgr.getSessionByUID(uid)
	if session == nil {
		return false;
	}

	mgr._netConnMap.Delete(session.netConn)
	mgr._uidMap.Delete(uid)

	if isLoginedUser {
		atomic.AddInt32(&mgr._currentLoginUserCount, -1)
		//userID := string(mgr._connectionList[sessionIndex].getUserID())
		//mgr._userIDConnectionMap.Delete(userID)
	}

	return true
}

func (mgr* connectionManager) getSession(net gnet.Conn) *connection {
	conn, ok := mgr._netConnMap.Load(net)
	if ok == false {
		return nil
	}

	session := conn.(*connection)
	return session
}

func (mgr* connectionManager) getSessionByUID(uid uint64) *connection {
	conn, ok := mgr._uidMap.Load(uid)
	if ok == false {
		return nil
	}

	session := conn.(*connection)
	return session
}


func (mgr* connectionManager) getSessionUID(net gnet.Conn) uint64 {
	session := mgr.getSession(net)
	if session == nil {
		return 0
	}

	return session.getNetworkUniqueID()
}

func (mgr* connectionManager) getUserID(uid uint64) (string, bool) {
	session := mgr.getSessionByUID(uid)
	if session == nil {
		return "", false
	}

	return session.getUserID(), true
}

func (mgr* connectionManager) setLogin(uid uint64, userID []byte) bool {
	session := mgr.getSessionByUID(uid)
	if session == nil {
		return false;
	}

	session.setLogin(userID)
	atomic.AddInt32(&mgr._currentLoginUserCount, 1)
	return true
}

func (mgr* connectionManager) sendPacket(uid uint64, packetData []byte) bool {
	session := mgr.getSessionByUID(uid)
	if session == nil {
		return false;
	}

	session.netConn.AsyncWrite(packetData)
	return true
}



