using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;


using ServerCommon;



namespace ChatServer
{
    public class ChatServer
    {
        public static ServerOption ServerOption;
        
        PacketProcessor MainPacketProcessor = new PacketProcessor();
        RoomManager RoomMgr = new RoomManager();

        MqManager MQMgr = new MqManager();


        public ChatServer()
        {            
        }

        public void InitConfig(ServerOption option)
        {
            ServerOption = option;                        
        }

        public void CreateStartServer()
        {
            try
            {
                CreateComponent();

                Console.WriteLine("서버 생성 성공");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 서버 생성 실패: {ex.ToString()}");
            }          
        }

        
        public void StopServer()
        {            
            MQMgr.Destory();

            MainPacketProcessor.Destory();
        }

        public ERROR_CODE CreateComponent()
        {
            Room.NetSendFunc = this.SendData;
            RoomMgr.CreateRooms();

            MainPacketProcessor = new PacketProcessor();
            MainPacketProcessor.CreateAndStart(RoomMgr.GetRoomList());
            MainPacketProcessor.SetMQ(MQMgr.SendMQ);

            var mqSubject = $"{ServerOption.MQSubsubject}.{ServerOption.Index}";
            MQMgr.Init(ServerOption.MQServerAddress,
                mqSubject, "", 
                MainPacketProcessor.InsertMQMessage);

            Console.WriteLine("CreateComponent - Success");
            return ERROR_CODE.NONE;
        }
                       

        public bool SendData(string subject, byte[] sendData, int offset, int count)
        {
            try
            {
                MQMgr.SendMQ(subject, sendData, offset, count);
            }
            catch(Exception ex)
            {
                // TimeoutException 예외가 발생할 수 있다
                Console.WriteLine($"{ex.ToString()},  {ex.StackTrace}");
            }
            return true;
        }

        //public void Distribute(ServerPacketData requestPacket)
        //{
        //    MainPacketProcessor.InsertPacket(requestPacket);
        //}
         
    }

    
}
