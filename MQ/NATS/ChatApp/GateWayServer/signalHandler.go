package main

import (
	. "GateWayServer/gohipernetFake"
	"os"
	"os/signal"
	"syscall"
)

var (
	// 호스트측에 종료를 통보
	_onDoneProcessExit = make(chan struct{})
)

func waitingServerStop() {
	NTELIB_LOG_INFO("Server Stop wating...")

	<- _onDoneProcessExit
}
// handle signals
func signalsHandling_goroutine(serverStopFunc func()) {
	NTELIB_LOG_INFO("start SignalsHandling goroutine ")

	ch := make(chan os.Signal, 1)
	signal.Notify(ch, syscall.SIGINT, syscall.SIGTERM)

	for {
		msg := <-ch

		switch msg {
		case syscall.SIGTERM: // os 명령어 kill로 종료 시켰음
			NTELIB_LOG_INFO("sigterm received: syscall.SIGTERM")

			_processExit(serverStopFunc)
		case syscall.SIGINT: // ctrl + c 로 종료 시켰음
			NTELIB_LOG_INFO("sigterm received: syscall.SIGINT")

			_processExit(serverStopFunc)
		}

		close(ch)
		return
	}

}

func _processExit(serverStopFunc func()) {
	serverStopFunc()
	close(_onDoneProcessExit)
}