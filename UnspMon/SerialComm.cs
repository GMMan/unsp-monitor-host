using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace UnspMon
{
    public class SerialComm : ISerialComm
    {
        SerialPort serial;
        private bool disposedValue;

        public SerialComm(string portName)
        {
            serial = new SerialPort(portName, 38400, Parity.None, 8, StopBits.One);
            serial.Open();
        }

        void CheckDisposed()
        {
            if (disposedValue) throw new ObjectDisposedException(GetType().FullName);
        }

        byte[] ReadWords(int count)
        {
            byte[] buf = new byte[count * 2];
            int read = 0;
            while (read < buf.Length)
            {
                read += serial.Read(buf, read, buf.Length - read);
            }
            return buf;
        }

        ushort ReadWord()
        {
            byte[] buf = ReadWords(1);
            var word = BitConverter.ToUInt16(buf, 0);
            //Console.WriteLine($"{word:x4}");
            return word;
        }

        void WriteBuffer(byte[] buf)
        {
            serial.Write(buf, 0, buf.Length);
            while (serial.BytesToWrite > 0) Thread.Sleep(10);
        }

        void WriteWord(ushort value)
        {
            byte[] buf = BitConverter.GetBytes(value);
            WriteBuffer(buf);
        }

        void WriteCmd(CommCommand cmd)
        {
            ushort cmdVal = (ushort)cmd;
            WriteWord(cmdVal);
            if (ReadWord() != cmdVal) throw new InvalidDataException("Incorrect response to command");
        }

        void CheckResponse(ushort tag)
        {
            ushort resp = ReadWord();
            if (resp != tag) throw new InvalidDataException("Target did not respond with correct end of operation tag.");
        }

        public bool Ping()
        {
            CheckDisposed();
            WriteCmd(CommCommand.Ping);
            return ReadWord() == 0x4948;
        }

        public byte[] Read(ushort address, ushort count)
        {
            CheckDisposed();
            WriteCmd(CommCommand.Read);
            WriteWord(address);
            WriteWord(count);
            byte[] buf = ReadWords(count);
            CheckResponse(0xaabb);
            return buf;
        }

        public ushort Read(ushort address)
        {
            byte[] buf = Read(address, 1);
            return BitConverter.ToUInt16(buf, 0);
        }

        public void Write(ushort address, byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length % 2 != 0) throw new ArgumentException("Data length must be even.", nameof(data));
            if (data.Length / 2 > ushort.MaxValue) throw new ArgumentException("Data length is too long.", nameof(data));
            CheckDisposed();

            WriteCmd(CommCommand.Write);
            WriteWord(address);
            WriteWord((ushort)(data.Length / 2));
            WriteBuffer(data);
            CheckResponse(0xccdd);
        }

        public void Write(ushort address, ushort value)
        {
            Write(address, BitConverter.GetBytes(value));
        }

        public void Call(ushort address)
        {
            CheckDisposed();
            WriteCmd(CommCommand.Call);
            WriteWord(address);
            CheckResponse(0xeeff);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serial.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
