using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.Network
{
    public struct NetworkChannelInfo
    {
        public string Name;
        public string IPAddress;
        public readonly static NetworkChannelInfo None = new NetworkChannelInfo();
    }
}
