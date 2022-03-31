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
        /// <summary>
        /// 通道数量；
        /// </summary>
        public int NetworkChannelCount { get { return channelDict.Count; } }
        /// <summary>
        /// 通道key；
        /// </summary>
        public NetworkChannelKey[] NetworkChannelKeys { get { return channelDict.Keys.ToArray(); } }
        /// <summary>
        /// 添加通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="channel">通道</param>
        /// <returns>是否添加成功</returns>
        public bool AddChannel(INetworkChannel channel)
        {
            var channelKey = channel.NetworkChannelKey;
            return channelDict.TryAdd(channelKey, channel);
        }
        /// <summary>
        /// 添加通道；
        /// </summary>
        /// <param name="channel">通道</param>
        /// <returns>是否添加成功</returns>
        public bool AddChannel(NetworkChannelKey channelKey, INetworkChannel channel)
        {
            return channelDict.TryAdd(channelKey, channel);
        }
        /// <summary>
        /// 添加或更新通道；
        /// </summary>
        /// <param name="channel">通道</param>
        public void AddOrUpdateChannel(INetworkChannel channel)
        {
            var channelKey = channel.NetworkChannelKey;
            if(channelDict.Remove(channelKey,out var oldChannel))
                oldChannel.AbortChannnel();
            channelDict.TryAdd(channelKey, channel);
        }
        /// <summary>
        /// 移除通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="channel">通道</para
        public bool RemoveChannel(NetworkChannelKey channelKey, out INetworkChannel channel)
        {
            if (channelDict.TryRemove(channelKey, out channel))
            {
                channel.AbortChannnel();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取一个通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="channel">通道</param>
        /// <returns>是否获取成功</returns>
        public bool PeekChannel(NetworkChannelKey channelKey, out INetworkChannel channel)
        {
            return channelDict.TryGetValue(channelKey, out channel);
        }
        /// <summary>
        /// 是否存在通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <returns>是否存在通道</returns>
        public bool HasChannel(NetworkChannelKey channelKey)
        {
            return channelDict.ContainsKey(channelKey);
        }
        /// <summary>
        /// 获取所有存在的通道；
        /// </summary>
        /// <returns>通道数组</returns>
        public INetworkChannel[] GetAllChannels()
        {
            return channelDict.Values.ToArray();
        }
        /// <summary>
        /// 获取通道信息；
        /// 若通道存在，则返回具体的信息；若不存在，则返回NetworkChannelInfo.None；
        /// <see cref="NetworkChannelInfo "/>
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <returns>通道的信息</returns>
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
        /// <summary>
        /// 弃用&终结通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <returns>是否存在key</returns>
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
