using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class UdpNetMessageHelper : INetMessageHelper
    {
        public INetworkMessage EncodeMessage(ushort opCode, byte[] dataBuffer)
        {
            return UdpNetMessage.EncodeMessage(opCode, dataBuffer);
        }
    }
}
