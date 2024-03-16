using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NPSBDummyLib
{
    public class TestConfig
    {
        public Int32 LimitActionTime;          // 액션에 대한 시간 제한(ms)

        public UInt32 MaxRepeatCount;        // 액션 반복 횟수
        public Int32 MaxRepeatTimeSec;      // 액션 반복 시간 초

        public int DummyStartNumber;
        public int DummyCount;
                
        public string ScenarioName; //TODO 삭제 예정
        
        public int EchoPacketSizeMin;
        public int EchoPacketSizeMax;
                
        // 네트워크 정보
        public string RmoteIP;
        public int RemotePort;
        public int PacketSizeMax;


        
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            var bindingFlags = BindingFlags.Instance |
                               BindingFlags.Public;

            var fieldNames = typeof(TestConfig).GetFields()
                .Select(field => field.Name)
                .ToList();

            var fieldValues = GetType()
                .GetFields(bindingFlags)
                .Select(field => field.GetValue(this))
                .ToList();

            for (var idx = 0; idx < fieldNames.Count(); ++idx)
            {
                result.Append($"{fieldNames[idx]} : {fieldValues[idx]}\n");
            }

            return result.ToString();
        }
    }
}
