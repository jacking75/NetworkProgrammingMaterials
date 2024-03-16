using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer
{
    public class MqDataProcess
    {
        Dictionary<UInt16, Action<byte[]>> RequestFuncDic = new Dictionary<UInt16, Action<byte[]>>();

        Action<string, byte[]> MQSendFunc;

        UInt16 MyServerIndex = 0;

        byte[] EncodingBuffer = new byte[8012];


        public void Init(UInt16 myServerIndex, Action<string, byte[]> mqSendFunc)
        {
            MyServerIndex = myServerIndex;
            MQSendFunc = mqSendFunc;

            SetHandler();
        }

        void SetHandler()
        {
            RequestFuncDic.Add((UInt16)ServerCommon.MQ_GATEDB_DATA_ID.REQ_LOGIN, LoginReq);

        }

        public void ReqProcess(byte[] mqData)
        {
            var header = new ServerCommon.MQBinaryHeader();
            header.HeaderDecode(mqData);

            if (RequestFuncDic.ContainsKey(header.PacketId))
            {
                RequestFuncDic[header.PacketId](mqData);
            }
            else
            {
                Console.Write("Unknown MQ Req Id: " + header.PacketId);
            }
        }


        void LoginReq(byte[] mqData)
        {
            try
            {
                //Console.WriteLine($"LoginReq");
                var requestMQ = new ServerCommon.MQReqLogin();
                requestMQ.Decode(mqData);

                //TODO 레디스에 확인해 본다

                var responseMq = new ServerCommon.MQResLogin()
                {
                    SenderInitial = Convert.ToSByte('D'),
                    SenderIndex = MyServerIndex,
                    UserNetSessionIndex = requestMQ.UserNetSessionIndex,
                    UserNetSessionUniqueID = requestMQ.UserNetSessionUniqueID,
                    PacketId = ServerCommon.MQ_GATEDB_DATA_ID.RES_LOGIN.ToUInt16(),
                    
                    Result = (Int16)ServerCommon.ERROR_CODE.NONE,
                };

                var sendDataSize = responseMq.Encode(EncodingBuffer);
                var sendData = new byte[sendDataSize];
                Buffer.BlockCopy(EncodingBuffer, 0, sendData, 0, sendDataSize);

                var subject = $"GATE.{requestMQ.SenderIndex}";
                MQSendFunc(subject, sendData);

                //Console.WriteLine($"Response MQResLBLogin. routingKey:{routingKey}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"LoginReq. Exception:{ex.ToString()}");
            }
        }

        void LobbyLoginTemp(byte[] mqData)
        {
            //var requestMq = MessagePackSerializer.Deserialize<ServerCommon.MQReqLBLogin>(mqData);
            //Console.WriteLine($"Sender:{requestMq.Sender}. UserID:{requestMq.UserID}");

            //TODO 레디스에 확인해 본다

            //var responseMq = new ServerCommon.MQResLBLogin()
            //{
            //    ID = ServerCommon.MQ_LBDB_DATA_ID.RES_LOGIN.ToUInt16(),
            //    Sender = MySenderName,
            //    Result = (Int16)ServerCommon.ERROR_CODE.NONE, 
            //    UserNetSessionID = requestMq.UserNetSessionID,
            //    UserID = requestMq.UserID
            //};
            //var sendData = MessagePackSerializer.Serialize(responseMq);

            //var routingKey = $"{MySenderName}_TO_{requestMq.Sender}";
            //MQSendFunc(routingKey, sendData);

            //Console.WriteLine($"Response MQResLBLogin. routingKey:{routingKey}");
        }

    }
}
