using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public enum NetworkProtocolType:byte
    {
        KCP=0x1,
        UDP=0x2,
        TCP=0x3
    }
}
