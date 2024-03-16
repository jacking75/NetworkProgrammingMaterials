using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NPSBDummyLib
{
    public class Utils
    {
        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static public Tuple<int,int> MinMaxThreadCount()
        {
            int minWorkThreads, maxWorkThreads = 0;
            int iocpThreads = 0;

            System.Threading.ThreadPool.GetMaxThreads(out minWorkThreads, out iocpThreads);
            System.Threading.ThreadPool.GetMinThreads(out maxWorkThreads, out iocpThreads);

            return Tuple.Create(minWorkThreads, maxWorkThreads);
        }

        static public Tuple<int, int> 나누기_몫과나머지(int 제수, int 피제수)
        {
            int 몫 = (int)(피제수 / 제수);
            int 나머지 = (int)(피제수 % 제수);
            return Tuple.Create(몫, 나머지);
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public enum Status
        {
            STOP = 0,
            PAUSE = 1,
            RUN = 2,
        }

        public struct PacketData
        {
            public Int16 PacketID;
            public Int16 BodySize;
            public byte[] BodyData;
        }

        public static byte[] MakeRandomStringPacket(int minSize=32, int maxSize=512)
        {
            var length = random.Next(minSize, maxSize);
            var text = Utils.RandomString(length);


            Int16 packetId = 241;
            var textLen = (Int16)Encoding.Unicode.GetBytes(text).Length;
            var bodyLen = (Int16)(textLen + 2);

            var sendData = new byte[4 + 2 + textLen];
            Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bodyLen), 0, sendData, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(textLen), 0, sendData, 4, 2);
            Buffer.BlockCopy(Encoding.Unicode.GetBytes(text), 0, sendData, 6, textLen);

            return sendData;
        }

        public static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        
        public static Int64 CurrentTimeSec() => (Int64)new TimeSpan(DateTime.Now.Ticks).TotalSeconds;


        static public bool IsCompletedRepeatTask(ref UInt64 dummyRepeatCount, TestConfig config, double elapsedTime)
        {
            ++dummyRepeatCount;

            if (config.MaxRepeatCount > 0)
            {
                // 테스트 조건 검사
                if (dummyRepeatCount >= config.MaxRepeatCount)
                {
                    return true;
                }
            }
            else
            {
                if (config.MaxRepeatTimeSec < elapsedTime)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
