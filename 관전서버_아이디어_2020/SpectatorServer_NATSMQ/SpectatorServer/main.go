package main

import (
	"flag"
)

func main() {
	var config serverConfig

	// --port 11021 --mqIP 127.0.0.1 --mqCID pvp
	flag.IntVar(&config.port, "port", 11021, "server port")
	flag.StringVar(&config.mqAddress, "mqIP", "nats://localhost", "mq server address")
	flag.StringVar(&config.mqCID, "mqCID", "test", "mq server cluster_id")
	flag.Parse()

	startServe(config)
}