using System;
using System.Threading;
namespace Cosmos.Network
{
    /// <summary>
    /// 网络通道异步等待的默认策略；
    /// <para>连接默认 10 秒超时（避免永远挂起）；收包与断线默认不限时，按需自行传入。</para>
    /// </summary>
    public static class NetworkAwaiterSettings
    {
        /// <summary>
        /// 连接超时；设为 <see cref="Timeout.InfiniteTimeSpan"/> 表示不限时（不推荐）；
        /// </summary>
        public static TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 等待断线的超时；默认不限时；
        /// </summary>
        public static TimeSpan DisconnectTimeout { get; set; } = Timeout.InfiniteTimeSpan;
        /// <summary>
        /// 收包等待超时；默认不限时（长连接本来就应当一直等）；
        /// </summary>
        public static TimeSpan ReceiveTimeout { get; set; } = Timeout.InfiniteTimeSpan;
        /// <summary>
        /// 未被消费的收包缓冲上限；超过后丢弃最旧的一条并告警。
        /// <para>缓冲是为了解决"数据先到、await 后到"，不是给不读数据的人当仓库用。</para>
        /// </summary>
        public static int ReceiveBufferLimit { get; set; } = 1024;
    }
}
