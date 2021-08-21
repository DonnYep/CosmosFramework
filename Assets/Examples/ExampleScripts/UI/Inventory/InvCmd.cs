using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Test
{
    public enum InvCmd : byte
    {
        Flush = 0x0,
        ShowDescription=0x1
    }
}
