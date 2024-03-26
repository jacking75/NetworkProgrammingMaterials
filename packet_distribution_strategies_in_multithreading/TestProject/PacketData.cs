using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject;

//GC 최적화를 한다면 외부에서 큰 버퍼를 만들고, 버퍼의 일부를 여기에 할당해서 
// 큰 메모리 조각을 나누어서 사용하는 것이 더 좋을 수 있다.
// 이 버퍼는 프로그램 종료까지 사용될테니 처음부터 GC 2세대로 보내거나 pin 하는 것이 더 좋다
public class PacketData
{
    public Int32 SessionId { get; private set; }
    public Byte[] Data { get; private set; }

    public void Init(Int32 dataSize)
    {   
        Data = new Byte[dataSize];
    }

    public void SetData(Int32 sessionId, Byte[] data)
    {
        this.SessionId = sessionId;
        Buffer.BlockCopy(data, 0, Data, 0, data.Length);    
    }
}
