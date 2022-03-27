using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Lockstep
{
    public enum MultiplayParameterCode : byte
    {
        AuthorityConv=1,
        RemoteConvs=2,
        ServerSyncInterval=3
    }
}
