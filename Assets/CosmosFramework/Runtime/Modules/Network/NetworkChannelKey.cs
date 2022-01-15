using System;
namespace Cosmos.Network
{
    /// <summary>
    /// 网络通道Key；
    /// ChannelName 与 ChannelIPAddress 的组合；
    /// ChannelIPAddress 字段示例为 127.0.0.1:80
    /// </summary>
    public struct NetworkChannelKey : IEquatable<NetworkChannelKey>
    {
        readonly string channelName;
        readonly string channelIPAddress;
        readonly int hashCode;
        public string ChannelName { get { return channelName; } }
        /// <summary>
        /// 示例127.0.0.1:80
        /// </summary>
        public string ChannelIPAddress { get { return channelIPAddress; } }
        public static readonly NetworkChannelKey None = new NetworkChannelKey("<NULL>","<NULL>");
        public NetworkChannelKey(string channelName, string channelIPAddress)
        {
            if (string.IsNullOrEmpty(channelName))
                throw new ArgumentException($"{nameof(channelName)} is invalid !");
            this.channelName = channelName;
            if (string.IsNullOrEmpty(channelIPAddress))
                throw new ArgumentException($"{nameof(channelIPAddress)} is invalid !");
            this.channelIPAddress = channelIPAddress;
            hashCode = channelName.GetHashCode() ^ channelIPAddress.GetHashCode();
        }
        public bool Equals(NetworkChannelKey other)
        {
            return ChannelName == other.ChannelName && ChannelIPAddress == other.ChannelIPAddress;
        }
        public static bool operator ==(NetworkChannelKey a, NetworkChannelKey b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(NetworkChannelKey a, NetworkChannelKey b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is NetworkChannelKey && Equals((NetworkChannelKey)obj);
        }
        public override int GetHashCode()
        {
            return hashCode;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ChannelName))
                throw new ArgumentNullException($"ChannelName is  invalid");
            if (string.IsNullOrEmpty(ChannelIPAddress))
                throw new ArgumentNullException($"ChannelIPAddress is  invalid");
            return $"ChannelName : {ChannelName} ; ChannelIPAddress : {ChannelIPAddress}";
        }
    }
}