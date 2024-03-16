using MessagePack;
using STAN.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csharp_test_client
{
    public partial class mainForm : Form
    {        
        bool IsNetworkThreadRunning = false;
        bool IsBackGroundProcessRunning = false;

        System.Threading.Thread NetworkReadThread = null;
  
        Queue<byte[]> RecvPacketQueue = new Queue<byte[]>();
        
        System.Windows.Threading.DispatcherTimer dispatcherUITimer;

        IStanConnection MQSubClient;
        IStanSubscription MQSub;
        IStanConnection MQPubClient;

        Random random = new Random();


        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            
            IsNetworkThreadRunning = true;
            NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            NetworkReadThread.Start();
            
            IsBackGroundProcessRunning = true;
            dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherUITimer.Tick += new EventHandler(BackGroundProcess);
            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherUITimer.Start();

            btnDisconnect.Enabled = false;

            DevLog.Write("프로그램 시작 !!!", LOG_LEVEL.INFO);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MQSubClient?.Close();
            MQPubClient?.Close();

            IsNetworkThreadRunning = false;
            IsBackGroundProcessRunning = false;
        }

        // mq Sub 접속하기
        private void btnConnect_Click(object sender, EventArgs e)
        {
            var cOpts = StanOptions.GetDefaultOptions();
            cOpts.NatsURL = $"nats://{textBoxIP.Text}:{textBoxPort.Text}";

            MQSubClient = new StanConnectionFactory().CreateConnection(textBoxClusterID.Text, textBoxClientID.Text, cOpts);
            
            labelStatus.Text = string.Format("{0}. 서버에 연결", DateTime.Now);
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;

            DevLog.Write($"MQ 서버 Sub 연결 성공", LOG_LEVEL.INFO);
        }

        // mq Sub 연결 끊기
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectdMQSub();
            MQSubClient.Close();

            DevLog.Write($"MQ 서버 Sub 연결 끊어짐", LOG_LEVEL.INFO);
        }

        // 시청 시작
        private void button1_Click(object sender, EventArgs e)
        {
            var opts = StanSubscriptionOptions.GetDefaultOptions();
            
            switch (listBoxStartType.SelectedIndex)
            {
                case 0: // StartAtSequence: 일련 번호를 지정하여 이 위치부터 시작한다.
                    opts.StartAt(textBox4.Text.ToULong());
                    break;
                case 1: // StartAtTime: 시작 시간으로 지정하여 시작한다
                    opts.StartAt(new DateTime(2016, 07, 28, 5, 35, 04, 570));
                    break;
                case 2: // StartAtTimeDelta: 시작 시간을 기준으로 지정된 시간이 지난 후부터
                    opts.StartAt(new TimeSpan(0, 0, 5));
                    break;
                case 3: // StartWithLastReceived: 마지막 부분부터 시작한다.
                    opts.StartWithLastReceived();
                    break;
                case 4: // 한번에 모두 다 받기
                    opts.DeliverAllAvailable();
                    break;
                default:
                    DevLog.Write($"MQ 서버 Sub 시청 방법을 선택하세요", LOG_LEVEL.ERROR);
                    return;
            }

            MQSub = MQSubClient.Subscribe(textBox1.Text, opts, ReceiveWatchingData);

            DevLog.Write($"MQ 서버 Sub 시청 시작. {listBoxStartType.SelectedItem.ToString()}", LOG_LEVEL.INFO);
        }

        // 시청 종료
        private void button2_Click(object sender, EventArgs e)
        {
            MQSub.Unsubscribe();

            DevLog.Write($"MQ 서버 Sub 시청 종료", LOG_LEVEL.INFO);
        }


        // MQ Pub 접속하기
        private void button5_Click(object sender, EventArgs e)
        {
            var cOpts = StanOptions.GetDefaultOptions();
            cOpts.NatsURL = $"nats://{textBox5.Text}:{textBox2.Text}";

            MQPubClient = new StanConnectionFactory().CreateConnection(textBox7.Text, textBox6.Text, cOpts);

            label11.Text = string.Format("{0}. 서버에 연결", DateTime.Now);
            button5.Enabled = false;
            button6.Enabled = true;

            DevLog.Write($"MQ 서버 Pub 연결 성공", LOG_LEVEL.INFO);
        }

        // MQ Pub 끊기
        private void button6_Click(object sender, EventArgs e)
        {
            SetDisconnectdMQPub();
            MQPubClient.Close();

            DevLog.Write($"MQ 서버 Pub 연결 끊어짐", LOG_LEVEL.INFO);
        }

        // 더미 데이터 추가 1
        private async void button3_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 5; ++i)
            {
                MQPubClient.Publish(textBox1.Text, NewDummyString().ToByteArray());

                DevLog.Write($"MQ 서버 Pub 데이터 추가", LOG_LEVEL.INFO);

                await Task.Delay(1000);
            }
        }

        // 더디 데이터 추가 1 에 새로운 데이터 추가
        private void button4_Click(object sender, EventArgs e)
        {
            MQPubClient.Publish(textBox1.Text, NewDummyString().ToByteArray());
        }


        string NewDummyString()
        {
            return $"[{DateTime.Now}] : {RandomString(16)}";
        }
        string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        void ReceiveWatchingData(object sender, StanMsgHandlerArgs args)
        {
            DevLog.Write($"Received a message: {System.Text.Encoding.UTF8.GetString(args.Message.Data)}, Time:{args.Message.Time}", LOG_LEVEL.INFO);
            //System.Threading.Thread.Sleep(500);
        }

        void NetworkReadProcess()
        {
            while (IsNetworkThreadRunning)
            {
                /*if (Network.IsConnected() == false)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                var recvData = Network.Receive();

                if (recvData != null)
                {
                    PacketBuffer.Write(recvData.Item2, 0, recvData.Item1);

                    while (true)
                    {
                        var data = PacketBuffer.Read();
                        if (data.Count < 1)
                        {
                            break;
                        }
                                                
                        var packetData = new byte[data.Count];
                        Buffer.BlockCopy(data.Array, data.Offset, packetData, 0, data.Count);

                        lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
                        {
                            RecvPacketQueue.Enqueue(packetData);
                        }
                    }*/
                    //DevLog.Write($"받은 데이터: {recvData.Item2}", LOG_LEVEL.INFO);
                /*}
                else
                {
                    //Network.Close();
                    //SetDisconnectd();
                    DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);
                }*/
            }
        }

       
        void BackGroundProcess(object sender, EventArgs e)
        {
            ProcessLog();

            try
            {
                lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
                {
                    if (RecvPacketQueue.Count() > 0)
                    {
                        var packet = RecvPacketQueue.Dequeue();
                        //PacketProcess(packet);
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ReadPacketQueueProcess. error:{0}", ex.Message));
            }
        }

        private void ProcessLog()
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (IsBackGroundProcessRunning)
            {
                System.Threading.Thread.Sleep(1);

                string msg;

                if (DevLog.GetLog(out msg))
                {
                    ++logWorkCount;

                    if (listBoxLog.Items.Count > 512)
                    {
                        listBoxLog.Items.Clear();
                    }

                    listBoxLog.Items.Add(msg);
                    listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
                }
                else
                {
                    break;
                }

                if (logWorkCount > 8)
                {
                    break;
                }
            }
        }


        void SetDisconnectdMQSub()
        {
            if (btnConnect.Enabled == false)
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }

            labelStatus.Text = "서버 접속이 끊어짐";
        }


        void SetDisconnectdMQPub()
        {
            if (button5.Enabled == false)
            {
                button5.Enabled = true;
                button6.Enabled = false;
            }

            labelStatus.Text = "서버 접속이 끊어짐";
        }

    }
}

//TODO 테스트 항목
/*
- Pub 후 Sub 해보기
- Pub 후 Sub 해보기, 시청 종료, Sub 해보기. NATS MQ에 데이터가 아직 남아 있나 
- Pub 후 Sub 해보기, 프로그램 종료, Sub 해보기. NATS MQ에 데이터가 아직 남아 있나
- Pub 후 프로그램 종료, Sub 해보기.  NATS MQ에 데이터가 아직 남아 있나
- Sub를 간격을 조절할 수 있나?
- 종류 별 Sub 하기
*/