using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet
{
    internal class HelperFunc
    {
        static public void WriteInteger(ref byte[] data, uint value, int offset)
        {
            data[offset] = (byte)(value >> 24);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }

        static public void ReadInteger(ref byte[] data, ref uint value, int offset)
        {
            value = ((uint)(data[offset] << 24) |
                     (uint)(data[offset + 1] << 16) |
                     (uint)(data[offset + 2] << 8) |
                     (uint)(data[offset + 3]));
        }
    }
}
