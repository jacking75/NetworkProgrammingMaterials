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
	RoomNumber = 1,
	ChatMessage = "ScriptTest",
};

DummyManager.SetDummyInfo = new DummyInfo
{
	RmoteIP = "10.10.14.51",
	RemotePort = 32452,
	DummyCount = 100,
	PacketSizeMax = 1400,
	IsRecvDetailProc = false,
};

var DummyManager = new DummyManager();
DummyManager.Init();
DummyManager.Prepare();
var prevTime = DateTime.Now;

Func<Dummy, DateTime, Task<(bool, string)>> func = async (dummy, testStartTime) =>
{
	var onlyConnect = ActionBase.MakeActionFactory(TestCase.ACTION_ONLY_CONNECT, config);
	var onlyDisConnect = ActionBase.MakeActionFactory(TestCase.ACTION_ONLY_DISCONNECT, config);	
	(bool, string) taskResult;
	var repeatCount = 0;
	while (true)
	{
		taskResult = await onlyConnect.Run(dummy);
		if (taskResult.Item1 == false)
		{
			return (false, taskResult.Item2);
		}

		taskResult = await onlyDisConnect.Run(dummy);
		if (taskResult.Item1 == false)
		{
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
