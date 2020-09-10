using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class TcpNetMessage : INetworkMessage
    {
        public long Conv { get; set; }

        public bool DecodeMessage(byte[] buffer)
        {
            return false;
        }
        public byte[] EncodeMessage()
        {
            return null;
        }

        public byte[] GetBuffer()
        {
            return EncodeMessage();
        }
    }
}
