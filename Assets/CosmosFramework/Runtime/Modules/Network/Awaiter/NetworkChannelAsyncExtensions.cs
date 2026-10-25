using System;
using System.Threading;
using System.Threading.Tasks;
namespace Cosmos.Network
{
    /// <summary>
    /// 网络通道的 async/await 扩展；
    /// <para>通道本体是事件推送模型（OnConnected / OnDataReceived / OnDisconnected），
    /// 这里把它包装成可 await 的拉取模型，并补齐失败、超时与取消路径。</para>
    /// <para>线程约定：在主线程调用；通道事件由 TickRefresh 在主线程触发，
    /// 因此 await 之后的代码也在主线程继续执行。</para>
    /// </summary>
    public static class NetworkChannelAsyncExtensions
    {
        /// <summary>
        /// 发起连接并等待连接成功；
        /// <para>连接前就断开 → <see cref="NetworkException"/>；超时 → <see cref="TimeoutException"/>；
        /// 取消 → <see cref="OperationCanceledException"/>。</para>
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="host">地址</param>
        /// <param name="port">端口</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="timeout">超时；为空则使用 <see cref="NetworkAwaiterSettings.ConnectTimeout"/></param>
        public static async Task ConnectAsync(this INetworkClientChannel channel, string host, int port, CancellationToken cancellationToken = default, TimeSpan? timeout = null)
        {
            CheckChannel(channel);
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("Host is invalid !");
            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException("Port is invalid !");
            var awaiter = NetworkAwaiter.Get(channel);
            var task = awaiter.ConnectAsync(host, port);
            await AwaitWithPolicy(task, timeout ?? NetworkAwaiterSettings.ConnectTimeout, cancellationToken, channel.ChannelName, "connect", () => awaiter.AbandonConnect(task)).ConfigureAwait(true);
        }
        /// <summary>
        /// 仅等待连接成功（连接由外部发起，例如 UI 触发 Connect）。
        /// </summary>
        public static async Task WaitConnectedAsync(this INetworkClientChannel channel, CancellationToken cancellationToken = default, TimeSpan? timeout = null)
        {
            CheckChannel(channel);
            var awaiter = NetworkAwaiter.Get(channel);
            var task = awaiter.WaitConnectedAsync();
            await AwaitWithPolicy(task, timeout ?? NetworkAwaiterSettings.ConnectTimeout, cancellationToken, channel.ChannelName, "wait for connection", () => awaiter.AbandonConnect(task)).ConfigureAwait(true);
        }
        /// <summary>
        /// 等待一条数据；数据可能先于本次 await 到达，会从缓冲中直接取出。
        /// <para>断线 → <see cref="NetworkException"/>；超时 → <see cref="TimeoutException"/>。</para>
        /// </summary>
        public static async Task<byte[]> ReceiveDataAsync(this INetworkClientChannel channel, CancellationToken cancellationToken = default, TimeSpan? timeout = null)
        {
            CheckChannel(channel);
            var awaiter = NetworkAwaiter.Get(channel);
            var task = awaiter.ReceiveAsync();
            return await AwaitWithPolicy(task, timeout ?? NetworkAwaiterSettings.ReceiveTimeout, cancellationToken, channel.ChannelName, "receive data", () => awaiter.AbandonReceive(task)).ConfigureAwait(true);
        }
        /// <summary>
        /// 等待断线；当前未连接则立即完成。
        /// </summary>
        public static async Task WaitDisconnectedAsync(this INetworkClientChannel channel, CancellationToken cancellationToken = default, TimeSpan? timeout = null)
        {
            CheckChannel(channel);
            var awaiter = NetworkAwaiter.Get(channel);
            var task = awaiter.WaitDisconnectedAsync();
            await AwaitWithPolicy(task, timeout ?? NetworkAwaiterSettings.DisconnectTimeout, cancellationToken, channel.ChannelName, "wait for disconnection", () => awaiter.AbandonDisconnect(task)).ConfigureAwait(true);
        }
        /// <summary>
        /// 非阻塞取一条已缓冲的数据。
        /// </summary>
        public static bool TryReceiveData(this INetworkClientChannel channel, out byte[] data)
        {
            CheckChannel(channel);
            return NetworkAwaiter.Get(channel).TryReceive(out data);
        }
        /// <summary>
        /// 当前未被消费的缓冲数量。
        /// </summary>
        public static int GetBufferedDataCount(this INetworkClientChannel channel)
        {
            CheckChannel(channel);
            return NetworkAwaiter.Get(channel).BufferedCount;
        }
        static void CheckChannel(INetworkClientChannel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("Network channel is invalid !");
        }
        /// <summary>
        /// 统一的等待策略：失败/结果原样传播，超时与取消补上；
        /// <para>取消/超时后调用 <paramref name="onAbandon"/> 精确摘除本次等待者，避免它吞掉后续数据。</para>
        /// </summary>
        static async Task<T> AwaitWithPolicy<T>(Task<T> task, TimeSpan timeout, CancellationToken cancellationToken, string channelName, string operation, Action onAbandon)
        {
            if (task.IsCompleted)
                return await task.ConfigureAwait(true);
            if (timeout == Timeout.InfiniteTimeSpan && !cancellationToken.CanBeCanceled)
                return await task.ConfigureAwait(true);

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                if (timeout != Timeout.InfiniteTimeSpan)
                    cts.CancelAfter(timeout);
                var cancelSignal = new TaskCompletionSource<bool>();
                using (cts.Token.Register(state => { ((TaskCompletionSource<bool>)state).TrySetResult(true); }, cancelSignal))
                {
                    var completed = await Task.WhenAny(task, cancelSignal.Task).ConfigureAwait(true);
                    if (completed != task)
                    {
                        //竞态：信号先到但任务其实已经完成，优先返回真实结果
                        if (task.IsCompleted)
                            return await task.ConfigureAwait(true);
                        onAbandon?.Invoke();
                        if (cancellationToken.IsCancellationRequested)
                            throw new OperationCanceledException(cancellationToken);
                        throw new TimeoutException($"Network channel : {channelName} {operation} timed out after {timeout.TotalSeconds:0.###}s.");
                    }
                }
            }
            return await task.ConfigureAwait(true);
        }
    }
}
