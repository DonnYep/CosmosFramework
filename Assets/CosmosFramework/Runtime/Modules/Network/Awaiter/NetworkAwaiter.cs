using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace Cosmos.Network
{
    /// <summary>
    /// 通道等待器：把"事件推送"的通道包装成"可 await 的拉取"语义；
    /// <para>每个通道一个实例（弱表缓存），负责三件事：</para>
    /// <para>1、连接 / 断线等待（同一通道同一时刻只允许一个连接等待者）；</para>
    /// <para>2、收包缓冲：数据可能先于 await 到达，先入队，等待者按 FIFO 取；</para>
    /// <para>3、失败传播：断线时把挂起的连接与收包等待者全部置为失败，避免永久挂起。</para>
    /// <para>线程约定：必须在主线程使用；通道事件（TickRefresh 内触发）也在主线程，
    /// 因此 await 的后续代码同样在主线程继续执行。</para>
    /// </summary>
    internal sealed class NetworkAwaiter
    {
        static readonly ConditionalWeakTable<INetworkClientChannel, NetworkAwaiter> awaiterTable
            = new ConditionalWeakTable<INetworkClientChannel, NetworkAwaiter>();
        static readonly ConditionalWeakTable<INetworkClientChannel, NetworkAwaiter>.CreateValueCallback createCallback
            = channel => new NetworkAwaiter(channel);

        readonly INetworkClientChannel channel;
        /// <summary>
        /// 未被消费的收包缓冲；
        /// </summary>
        readonly Queue<byte[]> messageQueue = new Queue<byte[]>();
        /// <summary>
        /// 等待收包的调用者；唤醒时跳过已取消/已完成的等待者，保证数据不丢；
        /// </summary>
        readonly Queue<TaskCompletionSource<byte[]>> receiveWaiters = new Queue<TaskCompletionSource<byte[]>>();

        TaskCompletionSource<bool> connectWaiter;
        TaskCompletionSource<bool> disconnectWaiter;
        bool connected;
        bool bufferOverflowWarned;

        /// <summary>
        /// 未消费的缓冲数量；
        /// </summary>
        public int BufferedCount { get { return messageQueue.Count; } }
        /// <summary>
        /// 当前是否已连接（跟随通道事件更新）；
        /// </summary>
        public bool IsConnected { get { return connected; } }

        /// <summary>
        /// 获取通道对应的等待器；不存在则创建并开始监听通道事件。
        /// </summary>
        public static NetworkAwaiter Get(INetworkClientChannel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("Network channel is invalid !");
            return awaiterTable.GetValue(channel, createCallback);
        }
        NetworkAwaiter(INetworkClientChannel channel)
        {
            //兜底：若框架尚未初始化主线程（例如无 MonoBehaviour 驱动的场景），以首次使用线程为主线程
            UnityMainThread.EnsureInitialized();
            this.channel = channel;
            connected = channel.IsConnect;
            channel.OnConnected += HandleConnected;
            channel.OnDisconnected += HandleDisconnected;
            channel.OnDataReceived += HandleDataReceived;
        }
        /// <summary>
        /// 发起连接并等待连接成功。
        /// </summary>
        public Task<bool> ConnectAsync(string host, int port)
        {
            CheckMainThread("ConnectAsync");
            if (connected)
                return Task.FromResult(true);
            var waiter = PrepareConnectWaiter();
            try
            {
                channel.Connect(host, port);
            }
            catch (Exception e)
            {
                //同步异常：把等待者一起置为失败，避免残留
                if (ReferenceEquals(connectWaiter, waiter))
                    connectWaiter = null;
                waiter.TrySetException(e);
                throw;
            }
            return waiter.Task;
        }
        /// <summary>
        /// 仅等待连接成功（连接由外部发起）。
        /// </summary>
        public Task<bool> WaitConnectedAsync()
        {
            CheckMainThread("WaitConnectedAsync");
            if (connected)
                return Task.FromResult(true);
            return PrepareConnectWaiter().Task;
        }
        /// <summary>
        /// 等待断线；当前未连接则立即完成。
        /// </summary>
        public Task<bool> WaitDisconnectedAsync()
        {
            CheckMainThread("WaitDisconnectedAsync");
            if (!connected)
                return Task.FromResult(true);
            if (disconnectWaiter != null && !disconnectWaiter.Task.IsCompleted)
                throw new InvalidOperationException($"Network channel : {channel.ChannelName} is already waiting for disconnection.");
            var waiter = new TaskCompletionSource<bool>();
            disconnectWaiter = waiter;
            return waiter.Task;
        }
        /// <summary>
        /// 等待一条数据；已有缓冲则立即返回。
        /// </summary>
        public Task<byte[]> ReceiveAsync()
        {
            CheckMainThread("ReceiveAsync");
            if (messageQueue.Count > 0)
                return Task.FromResult(messageQueue.Dequeue());
            if (!connected)
                return Task.FromException<byte[]>(new NetworkException(channel.ChannelName, "Channel is not connected and no buffered data is available."));
            var waiter = new TaskCompletionSource<byte[]>();
            receiveWaiters.Enqueue(waiter);
            return waiter.Task;
        }
        /// <summary>
        /// 非阻塞取一条缓冲数据。
        /// </summary>
        public bool TryReceive(out byte[] data)
        {
            CheckMainThread("TryReceive");
            if (messageQueue.Count > 0)
            {
                data = messageQueue.Dequeue();
                return true;
            }
            data = null;
            return false;
        }
        /// <summary>
        /// 放弃某个连接等待者（等待被取消或超时）；
        /// <para>不清理缓冲：已到达的数据仍可被后续接收者取走。</para>
        /// </summary>
        public void AbandonConnect(Task task)
        {
            if (connectWaiter == null || !ReferenceEquals(connectWaiter.Task, task))
                return;
            var waiter = connectWaiter;
            connectWaiter = null;
            waiter.TrySetCanceled();
        }
        /// <summary>
        /// 放弃某个断线等待者（等待被取消或超时）。
        /// </summary>
        public void AbandonDisconnect(Task task)
        {
            if (disconnectWaiter == null || !ReferenceEquals(disconnectWaiter.Task, task))
                return;
            var waiter = disconnectWaiter;
            disconnectWaiter = null;
            waiter.TrySetCanceled();
        }
        /// <summary>
        /// 放弃某个收包等待者（等待被取消或超时）；
        /// <para>必须把它从队列摘除，否则下一条数据会被它"吃掉"导致真正等待的人收不到。</para>
        /// </summary>
        public void AbandonReceive(Task<byte[]> task)
        {
            if (task == null || receiveWaiters.Count == 0)
                return;
            var keep = new List<TaskCompletionSource<byte[]>>(receiveWaiters.Count);
            TaskCompletionSource<byte[]> abandoned = null;
            while (receiveWaiters.Count > 0)
            {
                var waiter = receiveWaiters.Dequeue();
                //顺手清掉已完成（已取消/已交付）的stale项
                if (waiter.Task.IsCompleted)
                    continue;
                if (abandoned == null && ReferenceEquals(waiter.Task, task))
                {
                    abandoned = waiter;
                    continue;
                }
                keep.Add(waiter);
            }
            abandoned?.TrySetCanceled();
            for (int i = 0; i < keep.Count; i++)
                receiveWaiters.Enqueue(keep[i]);
        }
        static void CheckMainThread(string operation)
        {
            if (!UnityMainThread.IsMainThread)
                throw new InvalidOperationException($"Network awaiter : {operation} must be called on the main thread.");
        }
        TaskCompletionSource<bool> PrepareConnectWaiter()
        {
            if (connectWaiter != null && !connectWaiter.Task.IsCompleted)
                throw new InvalidOperationException($"Network channel : {channel.ChannelName} is already waiting for connection.");
            var waiter = new TaskCompletionSource<bool>();
            connectWaiter = waiter;
            return waiter;
        }
        void HandleConnected()
        {
            //兜底：自定义通道可能在后台线程抛事件，统一切回主线程再改状态
            if (!UnityMainThread.IsMainThread)
            {
                UnityMainThread.Post(HandleConnected);
                return;
            }
            connected = true;
            var waiter = connectWaiter;
            connectWaiter = null;
            waiter?.TrySetResult(true);
        }
        void HandleDisconnected()
        {
            if (!UnityMainThread.IsMainThread)
            {
                UnityMainThread.Post(HandleDisconnected);
                return;
            }
            connected = false;
            var connect = connectWaiter;
            connectWaiter = null;
            //连接还没建立就断开：视为连接失败，而不是让 await 永远等下去
            connect?.TrySetException(new NetworkException(channel.ChannelName, "Channel disconnected before the connection was established."));

            var disconnect = disconnectWaiter;
            disconnectWaiter = null;
            disconnect?.TrySetResult(true);

            //挂起的收包等待者全部失败（已缓冲的数据不受影响，仍可被立刻取走）
            while (receiveWaiters.Count > 0)
            {
                var waiter = receiveWaiters.Dequeue();
                waiter.TrySetException(new NetworkException(channel.ChannelName, "Channel disconnected while waiting for data."));
            }
        }
        void HandleDataReceived(byte[] data)
        {
            if (data == null)
                return;
            if (!UnityMainThread.IsMainThread)
            {
                UnityMainThread.Post(() => HandleDataReceived(data));
                return;
            }
            while (receiveWaiters.Count > 0)
            {
                var waiter = receiveWaiters.Dequeue();
                //已完成/已取消的等待者跳过，数据继续交给下一个，不能丢
                if (waiter.TrySetResult(data))
                    return;
            }
            var limit = NetworkAwaiterSettings.ReceiveBufferLimit;
            if (limit > 0 && messageQueue.Count >= limit)
            {
                messageQueue.Dequeue();
                if (!bufferOverflowWarned)
                {
                    bufferOverflowWarned = true;
                    Utility.Debug.LogWarning($"[Cosmos] Network channel : {channel.ChannelName} receive buffer overflow (limit {limit}), the oldest message is dropped. Consume the channel or raise NetworkAwaiterSettings.ReceiveBufferLimit.");
                }
            }
            messageQueue.Enqueue(data);
        }
    }
}
