using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NPSBDummyLib;

namespace TcpDummyClient
{
    public partial class MainForm : Form
    {
        #region 접속만하기
        // 접속만....연결하기
        private void button1_Click(object sender, EventArgs e)
        {
            AddLog($"[{DateTime.Now}][테스트 시작] 접속만 하기");
            var config = GetTestBaseConfig();

            DummyScenarioRunner.Start(ScenarioCase.OnlyConnect, config);
        }

        // 접속만.... - 접속 끊기
        private void button2_Click(object sender, EventArgs e)
        {
            DummyMgr.EndTest();

        }
        #endregion



        #region 접속/끊기 반복
        // 테스트 시작
        private void button4_Click(object sender, EventArgs e)
        {
            AddLog($"[{DateTime.Now}][테스트 시작] 접속/끊기 반복");
            var config = GetTestBaseConfig();

            DummyScenarioRunner.Start(ScenarioCase.RepeatConnectDisconnect, config);
        }

        // 테스트 중단
        private void button3_Click(object sender, EventArgs e)
        {
            DummyMgr.EndTest();
        }
        #endregion



        #region 에코 테스트
        private void button9_Click(object sender, EventArgs e)
        {
            AddLog($"[{DateTime.Now}][테스트 시작] Echo");
            var config = GetTestBaseConfig();

            DummyScenarioRunner.Start(ScenarioCase.RepeatEcho, config);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DummyMgr.EndTest();
        }
        #endregion



        // 로그인/로그아웃
        private void button16_Click(object sender, EventArgs e)
        {
            AddLog($"[{DateTime.Now}][테스트 시작] [반복] 접속-로그인-끊기");
            var config = GetTestBaseConfig();

            DummyScenarioRunner.Start(ScenarioCase.RepeatLogInOut, config);
        }
        
        //반복해서 방 입장/나가기
        private void button5_Click_1(object sender, EventArgs e)
        {
            AddLog($"[{DateTime.Now}][테스트 시작] [반복] 방 입장-나가기");
            var config = GetTestBaseConfig();

            DummyScenarioRunner.Start(ScenarioCase.RepeatRoomInOut, config);
        }
        
        // 방 채팅
        private void button18_Click(object sender, EventArgs e)
        {
            AddLog($"[{DateTime.Now}][테스트 시작] [반복] 방 채팅");
            var config = GetTestBaseConfig();

            DummyScenarioRunner.Start(ScenarioCase.RepeatRoomChat, config);
        }

        // 시나리오 실행 중단
        private void button6_Click(object sender, EventArgs e)
        {
            AddLog($"End - [{DummyManager.Config.ScenarioName}] 수동으로 중단");
            DummyMgr.EndTest();
        }


    }
}
