package main

import (
	"sync"
	"sync/atomic"

	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

// 스레드 세이프 해야 한다.
type connectionManager struct {
	_userIDConnectionMap *sync.Map

	_maxConnectionCount int32
	_connectionList     []*connection

	_maxUserCount          int32 // ?
	_currentLoginUserCount int32 // ?
}

var connectionMgrInst connectionManager

func (mgr* connectionManager) init(maxConnectionCount int, maxUserCount int32) bool {
	mgr._userIDConnectionMap = new(sync.Map)
	mgr._maxUserCount = maxUserCount

	mgr._maxConnectionCount = int32(maxConnectionCount)
	mgr._connectionList = make([]*connection, maxConnectionCount)

	for i := 0; i < maxConnectionCount; i++ {
		mgr._connectionList[i] = new(connection)

		index := int32(i)
		mgr._connectionList[i].Init(index)
	}

	return true
}

func (mgr* connectionManager) addSession(sessionIndex int32, sessionUniqueID uint64) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	if mgr._connectionList[sessionIndex].GetConnectTimeSec() > 0 {
		NTELIB_LOG_ERROR("already connected connection", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	// 방어적인 목적으로 한번 더 Clear 한다
	mgr._connectionList[sessionIndex].Clear()
	mgr._connectionList[sessionIndex].SetConnectTimeSec(NetLib_GetCurrnetUnixTime(), sessionUniqueID)
	return true
}

func (mgr* connectionManager) removeSession(sessionIndex int32, isLoginedUser bool) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	if isLoginedUser {
		atomic.AddInt32(&mgr._currentLoginUserCount, -1)

		userID := string(mgr._connectionList[sessionIndex].getUserID())
		mgr._userIDConnectionMap.Delete(userID)
	}

	mgr._connectionList[sessionIndex].Clear()

	return true
}

func (mgr* connectionManager) validSessionIndex(index int32) bool {
	if index < 0 || index >= mgr._maxConnectionCount {
		return false
	}
	return true
}

func (mgr* connectionManager) getNetworkUniqueID(sessionIndex int32) uint64 {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return 0
	}

	return mgr._connectionList[sessionIndex].GetNetworkUniqueID()
}

func (mgr* connectionManager) getUserID(sessionIndex int32) ([]byte, bool) {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return nil, false
	}

	return mgr._connectionList[sessionIndex].getUserID(), true
}

func (mgr* connectionManager) setBackState(sessionIndex int32, sessionUniqueId uint64) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	mgr._connectionList[sessionIndex].SetBackState()
	return true
}

func (mgr* connectionManager) preLogin(sessionIndex int32, sessionUniqueId uint64, userID []byte) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	mgr._connectionList[sessionIndex].SetUser(userID)
	return true
}

func (mgr* connectionManager) setLogin(sessionIndex int32, sessionUniqueId uint64) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	newUserID := string(mgr._connectionList[sessionIndex].getUserID())
	if _, ok := mgr._userIDConnectionMap.Load(newUserID); ok {
		return false
	}

	mgr._connectionList[sessionIndex].SetLogin()
	mgr._userIDConnectionMap.Store(newUserID, mgr._connectionList[sessionIndex])
	atomic.AddInt32(&mgr._currentLoginUserCount, 1)
	return true
}

func (mgr* connectionManager) setPreRoom(sessionIndex int32, sessionUniqueId uint64) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	mgr._connectionList[sessionIndex].SetPreRoom()
	return true
}

func (mgr* connectionManager) setRoom(sessionIndex int32, sessionUniqueId uint64, roomNumber int32) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	mgr._connectionList[sessionIndex].SetRoom(roomNumber)
	return true
}

func (mgr* connectionManager) isLoginUser(sessionIndex int32) bool {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return false
	}

	return mgr._connectionList[sessionIndex].IsAuth()
}

func (mgr* connectionManager) getRoomNumber(sessionIndex int32, sessionUniqueId uint64) int32 {
	if mgr.validSessionIndex(sessionIndex) == false {
		NTELIB_LOG_ERROR("Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return -1
	}

	return mgr._connectionList[sessionIndex].getRoomNumber()
}




