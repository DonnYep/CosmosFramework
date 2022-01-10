using System;

namespace Cosmos.Network
{
    public struct NetworkChannelInfo
    {
        public string Name;
        public string IPAddress;
        public Type ChannelType;
        public readonly static NetworkChannelInfo None = new NetworkChannelInfo();
    }
}
