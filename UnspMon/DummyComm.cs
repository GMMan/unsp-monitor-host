using System;
using System.Collections.Generic;
using System.Text;

namespace UnspMon
{
    public class DummyComm : ISerialComm
    {
        bool disposedValue;
        Random random = new Random();

        void CheckDisposed()
        {
            if (disposedValue) throw new ObjectDisposedException(GetType().FullName);
        }

        public void Call(ushort address)
        {
            CheckDisposed();
            Console.Error.WriteLine($"Calling 0x{address:x4}");
        }

        public bool Ping()
        {
            CheckDisposed();
            return true;
        }

        public byte[] Read(ushort address, ushort count)
        {
            CheckDisposed();
            byte[] buf = new byte[count * 2];
            random.NextBytes(buf);
            return buf;
        }

        public ushort Read(ushort address)
        {
            CheckDisposed();
            return unchecked((ushort)random.Next());
        }

        public void Write(ushort address, byte[] data)
        {
            CheckDisposed();
            Console.Error.WriteLine($"Writing to {address:x4}");
            int lines = (data.Length + 15) / 16;
            for (int i = 0; i < lines; ++i)
            {
                for (int j = 0; j < 16; ++j)
                {
                    int currAddr = i * 16 + j;
                    if (currAddr >= data.Length) break;
                    Console.Error.Write($"{data[currAddr]:x2} ");
                }
                Console.Error.WriteLine();
            }
        }

        public void Write(ushort address, ushort value)
        {
            CheckDisposed();
            Console.Error.WriteLine($"Writing 0x{value:x4} to {address:x4}");
        }

        public void Dispose()
        {
            disposedValue = true;
        }
    }
}
