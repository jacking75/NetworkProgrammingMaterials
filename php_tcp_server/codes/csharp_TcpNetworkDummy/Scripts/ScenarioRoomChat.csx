#r "NPSBDummyLib.dll"
#r "nuget: System.Threading.Channels, 4.5.0"
#r "nuget: System.Threading.Tasks.Extensions, 4.5.3"
#r "nuget: MessagePack, 1.7.3"
#r "nuget: NLog, 4.6.7"
#r "nuget: System.ValueTuple, 4.5.0"

using NPSBDummyLib;

var testUniqueIndex = DateTime.Now.Ticks;
var config = new TestConfig
{
	ActionIntervalTime = 100,
	ActionRepeatCount = 3,
	DummyIntervalTime = 0,
	LimitActionTime = 60000,
	RoomNumber = 2,
	ChatMessage = "ScriptTest",
};

DummyManager.SetDummyInfo = new DummyInfo
{
	RmoteIP = "10.10.14.51",
	RemotePort = 32452,
	DummyCount = 300,
	PacketSizeMax = 1400,
	IsRecvDetailProc = false,
};

var DummyManager = new DummyManager();
DummyManager.Init();
DummyManager.Prepare();
var prevTime = DateTime.Now;

Func<Dummy, DateTime, Task<(bool, string)>> func = async (dummy, testStartTime) =>
{
    var connect = ActionBase.MakeActionFactory(TestCase.ACTION_CONNECT, config);
    var login = ActionBase.MakeActionFactory(TestCase.ACTION_LOGIN, config);
    var roomEnter = ActionBase.MakeActionFactory(TestCase.ACTION_ROOM_ENTER, config);
    var roomLeave = ActionBase.MakeActionFactory(TestCase.ACTION_ROOM_LEAVE, config);
    var roomChat = ActionBase.MakeActionFactory(TestCase.ACTION_ROOM_CHAT, config);
    var disConnect = ActionBase.MakeActionFactory(TestCase.ACTION_DISCONNECT, config);

    var repeatCount = 0;
    (bool, string) taskResult;

    while (true)
    {
        taskResult = await connect.Run(dummy);
        if (taskResult.Item1 == false)
        {
            // 실패 통보하면서 더미 실행 중지
            return (false, taskResult.Item2);
        }

        taskResult = await login.Run(dummy);
        if (taskResult.Item1 == false)
        {
            // 실패 통보하면서 더미 실행 중지
            return (false, taskResult.Item2);
        }

        taskResult = await roomEnter.Run(dummy);
        if (taskResult.Item1 == false)
        {
            // 실패 통보하면서 더미 실행 중지
            return (false, taskResult.Item2);
        }

        taskResult = await roomChat.Run(dummy);
        if (taskResult.Item1 == false)
        {
            // 실패 통보하면서 더미 실행 중지
            return (false, taskResult.Item2);
        }

        taskResult = await roomLeave.Run(dummy);
        if (taskResult.Item1 == false)
        {
            // 실패 통보하면서 더미 실행 중지
            return (false, taskResult.Item2);
        }

        taskResult = await disConnect.Run(dummy);
        if (taskResult.Item1 == false)
        {
            // 실패 통보하면서 더미 실행 중지
            return (false, taskResult.Item2);
        }

        ++repeatCount;

        // 테스트 조건 검사
        if (repeatCount > config.ActionRepeatCount)
        {
            break;
        }

        var elapsedTime = DateTime.Now - testStartTime;
        if (elapsedTime.TotalMilliseconds > config.LimitActionTime)
        {
            Utils.MakeResult(dummy.Index, false, "타임 아웃");
        }
    }

    return Utils.MakeResult(dummy.Index, true, "Success");
};

var et = DateTime.Now - prevTime;
Console.WriteLine($"{et.TotalMilliseconds} - Prepare Time");

await DummyManager.RunTestScenario(testUniqueIndex, config, func);
var testResult = DummyManager.GetTestResult(testUniqueIndex, config);

foreach (var report in testResult)
{
	Console.WriteLine(report.Detail);
	Console.WriteLine(report.Message);
}

DummyManager.EndTest();
