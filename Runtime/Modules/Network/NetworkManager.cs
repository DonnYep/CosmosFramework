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
        ConcurrentDictionary<string, INetworkChannel> channelDict;
        ///<inheritdoc/>
        public int NetworkChannelCount { get { return channelDict.Count; } }
        ///<inheritdoc/>
        public bool AddChannel(INetworkChannel channel)
        {
            var channelName = channel.ChannelName;
            return channelDict.TryAdd(channelName, channel);
        }
        ///<inheritdoc/>
        public bool AddChannel(string channelName, INetworkChannel channel)
        {
            return channelDict.TryAdd(channelName, channel);
        }
        ///<inheritdoc/>
        public void AddOrUpdateChannel(INetworkChannel channel)
        {
            var channelName = channel.ChannelName;
            if(channelDict.Remove(channelName,out var oldChannel))
                oldChannel.AbortChannnel();
            channelDict.TryAdd(channelName, channel);
        }
        ///<inheritdoc/>
        public bool RemoveChannel(string channelName, out INetworkChannel channel)
        {
            if (channelDict.TryRemove(channelName, out channel))
            {
                channel.AbortChannnel();
                return true;
            }
            return false;
        }
        ///<inheritdoc/>
        public bool PeekChannel(string channelName, out INetworkChannel channel)
        {
            return channelDict.TryGetValue(channelName, out channel);
        }
        ///<inheritdoc/>
        public bool HasChannel(string channelName)
        {
            return channelDict.ContainsKey(channelName);
        }
        ///<inheritdoc/>
        public INetworkChannel[] GetAllChannels()
        {
            return channelDict.Values.ToArray();
        }
        ///<inheritdoc/>
        public NetworkChannelInfo GetChannelInfo(string channelName)
        {
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                var info = new NetworkChannelInfo();
                info.IPAddress = channel.IPAddress;
                info.Name = channel.ChannelName;
                info.ChannelType = channel.GetType();
                return info;
            }
            return NetworkChannelInfo.None;
        }
        ///<inheritdoc/>
        public bool AbortChannel(string channelName)
        {
            if (channelDict.TryRemove(channelName, out var channel))
            {
                channel.AbortChannnel();
                return true;
            }
            return false;
        }
        protected override void OnInitialization()
        {
            IsPause = false;
            channelDict = new ConcurrentDictionary<string, INetworkChannel>();
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
