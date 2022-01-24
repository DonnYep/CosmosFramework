using System.Collections.Concurrent;
using System.Linq;
namespace Cosmos.Network
{
    //================================================
    /*
    *1、网络模块的具体连接由通道实现，允许令实例对象作为同时作为服务器
    *与客户端。客户端与服务器通道两条线并行，并且维护各自的逻辑；
    *
    *2、此模块线程安全；
    *
    *3、网络模块只维护通道，具体操作需要根据通道本身作处理；
    */
    //================================================
    [Module]
    internal sealed class NetworkManager : Module, INetworkManager
    {
        /// <summary>
        /// ChannelName===INetworkChannel
        /// </summary>
        ConcurrentDictionary<NetworkChannelKey, INetworkChannel> channelDict;
        public int NetworkChannelCount { get { return channelDict.Count; } }
        public NetworkChannelKey[] NetworkChannelKeys { get { return channelDict.Keys.ToArray(); } }
        public bool AddChannel(INetworkChannel channel)
        {
            var channelKey = channel.NetworkChannelKey;
            return channelDict.TryAdd(channelKey, channel);
        }
        public bool AddChannel(NetworkChannelKey channelKey, INetworkChannel channel)
        {
            return channelDict.TryAdd(channelKey, channel);
        }
        public void AddOrUpdateChannel(INetworkChannel channel)
        {
            var channelKey = channel.NetworkChannelKey;
            if(channelDict.Remove(channelKey,out var oldChannel))
                oldChannel.AbortChannnel();
            channelDict.TryAdd(channelKey, channel);
        }
        public bool RemoveChannel(NetworkChannelKey channelKey, out INetworkChannel channel)
        {
            if (channelDict.TryRemove(channelKey, out channel))
            {
                channel.AbortChannnel();
                return true;
            }
            return false;
        }
        public bool PeekChannel(NetworkChannelKey channelKey, out INetworkChannel channel)
        {
            return channelDict.TryGetValue(channelKey, out channel);
        }
        public bool HasChannel(NetworkChannelKey channelKey)
        {
            return channelDict.ContainsKey(channelKey);
        }
        public INetworkChannel[] GetAllChannels()
        {
            return channelDict.Values.ToArray();
        }
        public NetworkChannelInfo GetChannelInfo(NetworkChannelKey channelKey)
        {
            if (channelDict.TryGetValue(channelKey, out var channel))
            {
                var info = new NetworkChannelInfo();
                info.IPAddress = channel.NetworkChannelKey.ChannelIPAddress;
                info.Name = channel.NetworkChannelKey.ChannelName;
                info.ChannelType = channel.GetType();
                return info;
            }
            return NetworkChannelInfo.None;
        }
        public bool AbortChannel(NetworkChannelKey channelKey)
        {
            if (channelDict.TryRemove(channelKey, out var channel))
            {
                channel.AbortChannnel();
                return true;
            }
            return false;
        }
        protected override void OnInitialization()
        {
            IsPause = false;
            channelDict = new ConcurrentDictionary<NetworkChannelKey, INetworkChannel>();
        }
        protected override void OnTermination()
        {
            foreach (var channel in channelDict)
            {
                channel.Value.AbortChannnel();
            }
            channelDict.Clear();
        }
        [TickRefresh]
        void OnRefresh()
        {
            if (IsPause)
                return;
            //foreach 时不允许操作字典对象，则转换成数组进行操作；
            var channelArr = channelDict.Values.ToArray();
            foreach (var channel in channelArr)
            {
                channel.TickRefresh();
            }
        }


    }
}
