using System;
namespace Cosmos.Network
{
    /// <summary>
    /// 网络通道异步等待失败；
    /// <para>超时用 <see cref="TimeoutException"/>，取消用 <see cref="OperationCanceledException"/>，
    /// 连接/收包过程中的通道异常用本类型。</para>
    /// </summary>
    public class NetworkException : Exception
    {
        /// <summary>
        /// 出错的通道名；
        /// </summary>
        public string ChannelName { get; private set; }
        public NetworkException(string channelName, string message) : base(message)
        {
            ChannelName = channelName;
        }
        public NetworkException(string channelName, string message, Exception innerException) : base(message, innerException)
        {
            ChannelName = channelName;
        }
    }
}
