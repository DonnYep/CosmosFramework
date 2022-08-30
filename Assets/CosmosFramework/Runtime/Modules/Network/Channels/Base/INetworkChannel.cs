using System;
namespace Cosmos.Network
{
    /// <summary>
    /// 网络通道；
    /// </summary>
    public interface INetworkChannel
    {
        /// <summary>
        /// 网络地址；
        /// </summary>
        string IPAddress { get; }
        /// <summary>
        /// 端口；
        /// </summary>
        int Port { get; }
        /// <summary>
        /// 通道名；
        /// </summary>
        string ChannelName { get; set; }
        /// <summary>
        /// 当通道被终止；
        /// </summary>
        event Action OnAbort;
        /// <summary>
        /// 终结通道；
        /// </summary>
        void AbortChannnel();
        /// <summary>
        /// 刷新通道；
        /// </summary>
        void TickRefresh();
    }
}