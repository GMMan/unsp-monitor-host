using System;
using System.Collections.Generic;
using System.Text;

namespace UnspMon
{
    public enum CommCommand : ushort
    {
        Ping,
        Read,
        Write,
        Call
    }
}
