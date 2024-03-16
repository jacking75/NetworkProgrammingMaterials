# ZeroMq Samples
소켓 옵션에서 Id를 부여하면 자동으로 메시지 보낼 때 id-공백-메시지가 되나?   - req-res만    
  


## REQ-RES
- REQ는 Send 했을 때만 Receive 할 수 있다.
    - Send를 하지 않았는데 Receive를 하면 안 된다.
    - Send를 한 후 Receive를 하지 않았는데 또 Send 요청을 하면 안 된다.
- 서버에 여러 클라이언트가 접속된 상태라도 서버는 요청한 클라이언트에게만 답변을 준다.  
- 서버도 Receive 하면 바로 해당 클라이언트에게 Send를 해야 한다
- 비동기적인 동작을 할 수 없다.
      
C#: ReqReqServer, ReqReqClient    
  

## Request-Router
- Route는 받은 순서와 다르게 메시지를 클라이언트에 보낼 수 있다. 즉 메시지를 받은 후 비동기로 요청을 처리한 후 답변을 클라이언트에 보낼 수 있다.
- 클라이언트의 Id를 알고 있어야 하며, 클라이언트에 보낼 메시지에 Id를 지정해야 한다.
- Id를 지정한 메시지는 Id-빈공백-데이터 포맷이 된다.
  
C#: ReqRouterServer, ReqRouterClient    
  

## RequestReplyBroker
- Request -> [front](RouterSocket)[backend](DealerSocket) <- [Worker](ResponseSocket)
- worker가 복수인 경우에도 라운드로빈으로 클라이언트의 요청을 배분한다
  
C#: RequestReplyBroker  
  
  
## ROUTERbrokerDEALERworkers
- Router <- Dealer
- Deler-Router 패턴
- Router에서 복수의 Dealer에게 작업을 전달
    
```
internal static class Program
    {
        private const int PortNumber = 5555;

        // We have two workers, here we copy the code, normally these would run on different boxes…
        public static void Main()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var workers = new List<Thread>(new[] { new Thread(WorkerTaskA), new Thread(WorkerTaskB) });

            using (var client = new RouterSocket())
            {
                client.Bind($"tcp://localhost:{PortNumber}");

                foreach (var thread in workers)
                {
                    thread.Start(PortNumber);
                }

                // Wait for threads to connect, since otherwise the messages we send won't be routable.
                Thread.Sleep(1000);

                for (int taskNumber = 0; taskNumber < 1000; taskNumber++)
                {
                    // Send two message parts, first the address…
                    client.SendMoreFrame(random.Next(3) > 0 ? Encoding.Unicode.GetBytes("A") : Encoding.Unicode.GetBytes("B"));

                    // And then the workload
                    client.SendFrame("This is the workload");
                }

                client.SendMoreFrame(Encoding.Unicode.GetBytes("A"));
                client.SendFrame("END");

                client.SendMoreFrame(Encoding.Unicode.GetBytes("B"));
                client.SendFrame("END");
            }

            Console.ReadKey();
        }

        private static void WorkerTaskA(object portNumber)
        {
            using (var worker = new DealerSocket())
            {
                worker.Options.Identity = Encoding.Unicode.GetBytes("A");
                worker.Connect($"tcp://localhost:{portNumber}");

                int total = 0;

                bool end = false;

                while (!end)
                {
                    string request = worker.ReceiveFrameString();

                    if (request == "END")
                    {
                        end = true;
                    }
                    else
                    {
                        total++;
                    }
                }

                Console.WriteLine("A Received: {0}", total);
            }
        }

        private static void WorkerTaskB(object portNumber)
        {
            using (var worker = new DealerSocket())
            {
                worker.Options.Identity = Encoding.Unicode.GetBytes("B");
                worker.Connect($"tcp://localhost:{portNumber}");

                int total = 0;

                bool end = false;

                while (!end)
                {
                    string request = worker.ReceiveFrameString();

                    if (request == "END")
                    {
                        end = true;
                    }
                    else
                    {
                        total++;
                    }
                }

                Console.WriteLine("B Received: {0}", total);
            }
        }
    }
```
  

## Queue 디바이스를 이용한 멀티스레드
- 위의 샘플 중 `ROUTERbrokerDEALERworkers`와 비슷한 것이다.  
- Request -> Queue <- Respone
- 클라이언트가 요청하면 Queue를 통해서 Worker에 라운드로빈으로 전달한다. Worker는 답변을 보내면 Queue를 통해서 클라이언트에 통보된다. 
     
```
internal static class Program
{
    private static CancellationToken s_token;

    private static void Main()
    {
        Console.Title = "NetMQ Multi-threaded Service";

        var queue = new QueueDevice("tcp://localhost:5555", "tcp://localhost:5556", DeviceMode.Threaded);

        var source = new CancellationTokenSource();
        s_token = source.Token;

        for (int threadId = 0; threadId < 10; threadId++)
            Task.Factory.StartNew(WorkerRoutine, s_token);

        queue.Start();

        var clientThreads = new List<Task>();
        for (int threadId = 0; threadId < 1000; threadId++)
        {
            int id = threadId;
            clientThreads.Add(Task.Factory.StartNew(() => ClientRoutine(id)));
        }

        Task.WaitAll(clientThreads.ToArray());

        source.Cancel();

        queue.Stop();

        Console.WriteLine("Press ENTER to exit...");
        Console.ReadLine();
    }

    private static void ClientRoutine(object clientId)
    {
        try
        {
            using (var req = new RequestSocket())
            {
                req.Connect("tcp://localhost:5555");

                byte[] message = Encoding.Unicode.GetBytes($"{clientId} Hello");

                Console.WriteLine("Client {0} sent \"{0} Hello\"", clientId);
                req.SendFrame(message, message.Length);

                var response = req.ReceiveFrameString(Encoding.Unicode);
                Console.WriteLine("Client {0} received \"{1}\"", clientId, response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception on ClientRoutine: {0}", ex.Message);
        }
    }

    private static void WorkerRoutine()
    {
        try
        {
            using (ResponseSocket rep = new ResponseSocket())
            {
                rep.Options.Identity = Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());
                rep.Connect("tcp://localhost:5556");
                //rep.Connect("inproc://workers");
                rep.ReceiveReady += RepOnReceiveReady;
                while (!s_token.IsCancellationRequested)
                {
                    rep.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception on WorkerRoutine: {0}", ex.Message);
            throw;
        }
    }

    private static void RepOnReceiveReady(object sender, NetMQSocketEventArgs args)
    {
        try
        {
            NetMQSocket rep = args.Socket;

            byte[] message = rep.ReceiveFrameBytes();

            //Thread.Sleep(1000); //  Simulate 'work'

            byte[] response =
                Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(message) + " World from worker " + Encoding.Unicode.GetString(rep.Options.Identity));

            rep.TrySendFrame(response, response.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception on RepOnReceiveReady: {0}", ex.Message);
            throw;
        }
    }
}
```
  
      
## 채팅 서버


## 대전 게임 관전
  

## 클라이언트도 ZeroMq 사용하기  