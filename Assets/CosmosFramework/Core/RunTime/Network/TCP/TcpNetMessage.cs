using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class TcpNetMessage : INetworkMessage
    {
        public uint Conv { get; set; }

        public void DecodeMessage(byte[] buffer)
        {
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
