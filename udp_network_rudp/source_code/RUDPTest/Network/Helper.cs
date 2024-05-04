using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPTest.Network
{
    public class Helper
    {
        static public UInt16 CRC16(byte[] data, int startPos)
        {
            UInt16 crc = 0x0000;
            
            for (int i = startPos; i < data.Length; i++)
            {
                crc ^= (UInt16)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (UInt16)((crc << 1) ^ 0x8005);
                    else
                        crc <<= 1;
                }
            }
            
            return crc;
        }

    }
}
