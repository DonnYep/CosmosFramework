using Cosmos.Network;
using kcp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cosmos
{
    //================================================
    /*
    *1、网络模块的具体连接由通道实现，允许令实例对象作为同时作为服务器
    *与客户端。客户端与服务器通道两条线并行，并且维护各自的逻辑；
    *
    *2、此模块线程安全；
    */
    //================================================
    public interface INetworkManager : IModuleManager
    {
        /// <summary>
        /// 通道数量；
        /// </summary>
        int NetworkChannelCount { get; }
        /// <summary>
        /// 通道key；
        /// </summary>
        NetworkChannelKey[] NetworkChannelKeys { get; }
        /// <summary>
        /// 添加通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="channel">通道</param>
        /// <returns>是否添加成功</returns>
        bool AddChannel(NetworkChannelKey channelKey, INetworkChannel channel);
        /// <summary>
        /// 添加通道；
        /// </summary>
        /// <param name="channel">通道</param>
        /// <returns>是否添加成功</returns>
        bool AddChannel(INetworkChannel channel);
        /// <summary>
        /// 移除通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="channel">通道</param>
        /// <returns>是否移除成功</returns>
        bool RemoveChannel(NetworkChannelKey channelKey, out INetworkChannel channel);
        /// <summary>
        /// 获取一个通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="channel">通道</param>
        /// <returns>是否获取成功</returns>
        bool PeekChannel(NetworkChannelKey channelKey, out INetworkChannel channel);
        /// <summary>
        /// 是否存在通道；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <returns></returns>
        bool HasChannel(NetworkChannelKey channelKey);
        /// <summary>
        /// 获取指定通道的一个会话remote地址；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="conv">会话Id</param>
        /// <param name="address">地址信息</param>
        /// <returns>是否获取成功</returns>
        bool GetConnectionAddress(NetworkChannelKey channelKey, int conv, out string address);
        /// <summary>
        /// 获取所有存在的通道；
        /// </summary>
        /// <returns>通道数组</returns>
        INetworkChannel[] PeekAllChannels();
        /// <summary>
        /// 获取通道信息；
        /// 若通道存在，则返回具体的信息；若不存在，则返回NetworkChannelInfo.None
        /// <see cref="NetworkChannelInfo "/>
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <returns>通道的信息；</returns>
        NetworkChannelInfo GetChannelInfo(NetworkChannelKey channelKey);
        /// <summary>
        /// 发送消息；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="reliableType">消息可靠类型</param>
        /// <param name="data">数据</param>
        /// <param name="connectionId">连接的id</param>
        void SendNetworkMessage(NetworkChannelKey channelKey, NetworkReliableType reliableType, byte[] data, int connectionId);
        /// <summary>
        /// 建立连接；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        void Connect(NetworkChannelKey channelKey);
        /// <summary>
        /// 断开连接；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        /// <param name="connectionId">连接的id</param>
        void Disconnect(NetworkChannelKey channelKey, int connectionId);
        /// <summary>
        /// 弃用&终结端口；
        /// </summary>
        /// <param name="channelKey">通道key</param>
        void AbortChannel(NetworkChannelKey channelKey);
    }
}
