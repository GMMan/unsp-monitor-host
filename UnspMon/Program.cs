using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace UnspMon
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                Console.Error.WriteLine("Usage: UnspMon <port> <operation> <address> [value]");
                Environment.Exit(1);
            }

            UInt16Converter ushortConverter = new UInt16Converter();
            string port = args[0];
            string op = args[1];
            ushort address = 0;
            try
            {
                address = (ushort)ushortConverter.ConvertFromString(args[2]);
            }
            catch
            {
                Console.Error.WriteLine("Cannot parse address.");
                Environment.Exit(2);
            }

            bool isValue = args.Length == 4;
            ushort value = 0;
            if (isValue)
            {
                try
                {
                    value = (ushort)ushortConverter.ConvertFromString(args[3]);
                }
                catch
                {
                    Console.Error.WriteLine("Cannot parse value.");
                    Environment.Exit(2);
                }
            }

            try
            {
                using (ISerialComm comm = new SerialComm(port))
                {
                    if (!comm.Ping())
                    {
                        Console.Error.WriteLine("Failed to ping");
                        Environment.Exit(4);
                    }

                    switch (op.ToUpperInvariant())
                    {
                        case "READ":
                            if (isValue)
                                CommandRead(comm, address, value);
                            else
                                CommandRead(comm, address);
                            break;
                        case "WRITE":
                            if (isValue)
                                CommandWrite(comm, address, value);
                            else
                                CommandWrite(comm, address);
                            break;
                        case "CALL":
                            if (isValue)
                            {
                                Console.Error.WriteLine("Specifying value is invalid for command.");
                                Environment.Exit(2);
                            }
                            CommandCall(comm, address);
                            break;
                        case "EXEC":
                            if (isValue)
                            {
                                Console.Error.WriteLine("Specifying value is invalid for command.");
                                Environment.Exit(2);
                            }
                            CommandExec(comm, address);
                            break;
                        default:
                            Console.Error.WriteLine("Unknown command.");
                            Environment.Exit(5);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Something went wrong: {ex}");
                Environment.Exit(3);
            }
        }

        static void CommandRead(ISerialComm comm, ushort addr, ushort count)
        {
            var data = comm.Read(addr, count);
            var stdout = Console.OpenStandardOutput();
            stdout.Write(data, 0, data.Length);
            stdout.Flush();
        }

        static void CommandRead(ISerialComm comm, ushort addr)
        {
            var value = comm.Read(addr);
            Console.WriteLine($"{value:x4}");
        }

        static void CommandWrite(ISerialComm comm, ushort addr)
        {
            var stdin = Console.OpenStandardInput();
            using (MemoryStream ms = new MemoryStream())
            {
                stdin.CopyTo(ms);
                byte[] data = ms.ToArray();
                if (data.Length > ushort.MaxValue)
                    throw new InvalidDataException("Input data is too long.");
                comm.Write(addr, data);
            }
        }

        static void CommandWrite(ISerialComm comm, ushort addr, ushort value)
        {
            comm.Write(addr, value);
        }

        static void CommandCall(ISerialComm comm, ushort addr)
        {
            comm.Call(addr);
        }

        static void CommandExec(ISerialComm comm, ushort addr)
        {
            CommandWrite(comm, addr);
            CommandCall(comm, addr);
        }
    }
}
