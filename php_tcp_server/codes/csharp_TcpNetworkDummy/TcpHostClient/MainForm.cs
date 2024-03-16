using CSBaseLib;
using NPSBDummyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO 테스트 시작 중인 경우 테스트 진행 률을 표시하자

namespace TcpDummyClient
{
    public partial class MainForm : Form
    {
        NPSBDummyLib.ScenarioRunner DummyScenarioRunner = new NPSBDummyLib.ScenarioRunner();

        NPSBDummyLib.DummyManager DummyMgr = new NPSBDummyLib.DummyManager();

        System.Collections.Concurrent.ConcurrentQueue<string> logMsgQueue;

        System.Windows.Threading.DispatcherTimer dispatcherUITimer;
        System.Windows.Threading.DispatcherTimer dispatcherLogTimer;


        public MainForm()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            DummyScenarioRunner.Init(DummyMgr, AddLog);
            DummyMgr.LogFunc = AddLog;
            logMsgQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();

            dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherUITimer.Tick += new EventHandler(UpdateUI);
            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherUITimer.Start();

            dispatcherLogTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherLogTimer.Tick += new EventHandler(UpdateLogPrint);
            dispatcherLogTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherLogTimer.Start();
        }

        
        void AddLog(string msg)
        {
            logMsgQueue.Enqueue(msg);
        }

        void testResultToUILog()
        {
            var testConfig = DummyManager.Config;

            UInt64 allDummyActionCount = 0;
            int failDummyCount = 0;
            int successDummyCount = 0;

            // 실패한 더미 출력하기
            foreach (var dummy in DummyMgr.DummyList)
            {
                if(dummy.IsSuccessScenarioResult() == false)
                {
                    ++failDummyCount;
                    AddLog($"Fail. Dummy Index: {dummy.Index}, ActionCount: {dummy.CurActionRepeatCount}");
                }
                else
                {
                    ++successDummyCount;
                }

                allDummyActionCount += dummy.CurActionRepeatCount;
            }

            var spantTime = DateTime.Now - DummyMgr.TestStartTime;
            var testResult = $"[{DateTime.Now}][{spantTime.TotalSeconds} s][{testConfig.ScenarioName}] Success:{successDummyCount}, Fail:{failDummyCount}";
            AddLog(testResult);
            AddLog($"All Dummy Action Count: {allDummyActionCount}");
        }

        TestConfig GetTestBaseConfig()
        {
            var config = new NPSBDummyLib.TestConfig
            {
                DummyCount = textBoxDummyCount.Text.ToInt32(),
                DummyStartNumber = textBoxDummyStartNumber.Text.ToInt32(),

                MaxRepeatCount = textBox12.Text.ToUInt32(),
                MaxRepeatTimeSec = textBox2.Text.ToInt32(),
                LimitActionTime = textBox9.Text.ToInt32(),
                
                EchoPacketSizeMin = textBox10.Text.ToInt32(),
                EchoPacketSizeMax = textBox11.Text.ToInt32(),

                RmoteIP = textBoxIP.Text,
                RemotePort = textBoxPort.Text.ToInt32(),
                PacketSizeMax = textBoxMaxPacketSize.Text.ToInt32(),
            };

            if(config.MaxRepeatTimeSec > 0)
            {
                config.MaxRepeatCount = 0;
            }

            return config;
        }
       
        
        void UpdateUI(object sender, EventArgs e)
        {
            try
            {
                if (DummyManager.InProgress)
                {
                    var config = DummyManager.Config;
                    var curAllDummyIterationCount = DummyManager.CurAllDummyIterationCount();

                    textBox1.Text = DummyManager.ConnectedDummyCount().ToString();

                    if (DummyManager.Config.MaxRepeatCount > 0)
                    {
                        var 목표수 = DummyManager.DummyCount * config.MaxRepeatCount;
                        textBoxStatus.Text = $"[CurAllDummyIterationCount: {curAllDummyIterationCount}] [평균완료율: {curAllDummyIterationCount}/{목표수} ]";
                    }
                    else
                    {
                        var dt = DateTime.Now - DummyMgr.TestStartTime;
                        textBoxStatus.Text = $"[CurAllDummyIterationCount: {curAllDummyIterationCount}] [진행율: {dt.TotalSeconds}/{config.MaxRepeatTimeSec}초]";
                    }
                }
                else
                {
                    textBox1.Text = "0";
                    textBoxStatus.Text = "";
                }
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
        }

        void UpdateLogPrint(object sender, EventArgs e)
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (true)
            {
                if (logMsgQueue.TryDequeue(out var logMsg) == false)
                {
                    break;
                }

                ++logWorkCount;

                if (listBoxLog.Items.Count > 512)
                {
                    listBoxLog.Items.RemoveAt(0);
                    //listBoxLog.Items.Clear();
                }

                listBoxLog.Items.Add(logMsg);
                                
                listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;

                if (logWorkCount > 16)
                {
                    break;
                }
            }
        }

           
        
       

        

    }
}
