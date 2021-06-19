using System;
using System.Collections.Generic;
using System.Text;

namespace UnspMon
{
    public interface ISerialComm : IDisposable
    {
        bool Ping();
        byte[] Read(ushort address, ushort count);
        ushort Read(ushort address);
        void Write(ushort address, byte[] data);
        void Write(ushort address, ushort value);
        void Call(ushort address);
    }
}
