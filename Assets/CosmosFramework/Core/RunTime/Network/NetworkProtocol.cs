using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public enum NetworkProtocolType:byte
    {
        TCP=0x0,
        UDP=0x1,
        KCP=0x2
    }
}
