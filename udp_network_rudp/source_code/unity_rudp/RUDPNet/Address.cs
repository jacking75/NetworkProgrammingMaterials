using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDPNet
{
    public class Address
    {
        uint _address;
        ushort _port;


        public Address()
        {
            _address = 0;
            _port = 0;
        }

        public Address(char a, char b, char c, char d, ushort port)
        {
            _address = (uint)(a << 24);
            _address |= (uint)(b << 16);
            _address |= (uint)(c << 8);
            _address |= (uint)(d);
            _port = port;
        }

        public Address(uint address, ushort port)
        {
            _address = address;
            _port = port;
        }

        public uint GetAddress()
        {
            return _address;
        }

        public ushort GetPort()
        {
            return _port;
        }
        
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            Address a = obj as Address;
            if ((System.Object)a == null)
            {
                return false;
            }
            return (_address == a._address) && (_port == a._port);
        }
        
        public override int GetHashCode()
        {
            return (int)(_address ^ _port);
        }

        public static bool operator ==(Address one, Address other)
        {
            return one._address == other._address && one._port == other._port;
        }
        
        public static bool operator !=(Address one, Address other)
        {
            return !(one == other);
        }
        
        public static bool operator <(Address one, Address other)
        {
            if (one._address < other._address)
                return true;
            if (one._address > other._address)
                return false;
            else
                return one._port < other._port;
        }
        
        public static bool operator >(Address one, Address other)
        {
            if (one._address > other._address)
                return true;
            if (one._address < other._address)
                return false;
            else
                return one._port > other._port;
        }
        
        public void Reset()
        {
            _address = 0;
            _port = 0;
        }
        
    }
}
