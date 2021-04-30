using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public enum UdpHeader:byte
    {
        Connect=0x01,
        Message=0x02,
        Hearbeat=0x03,
        Disconnect=0x04,
    }
}
