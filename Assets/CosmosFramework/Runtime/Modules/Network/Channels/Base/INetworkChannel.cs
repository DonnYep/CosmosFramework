using System;
namespace Cosmos.Network
{
    /// <summary>
    /// 网络通道；
    /// </summary>
    public interface INetworkChannel
    {
        /// <summary>
        /// 端口
        /// </summary>
        int Port { get; }
        /// <summary>
        /// 通道的唯一识别key；
        /// </summary>
        NetworkChannelKey NetworkChannelKey { get; }
        /// <summary>
        /// 当通道被终止；
        /// </summary>
        event Action OnAbort;
        /// <summary>
        /// 终结通道；
        /// </summary>
        void AbortChannne();
        void TickRefresh();
    }
}