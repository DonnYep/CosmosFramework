using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface INetworkMessageHelper
    {
        INetworkMessage EncodeMessage(ushort opCode, byte[] dataBuffer);
    }
}
