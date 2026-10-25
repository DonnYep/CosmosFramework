using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Hydrogen
{
    /// <summary>
    /// TCP 客户端：异步收发，帧格式为 [header][message]，header 长度与编码由 <see cref="IHeaderEncoder"/> 决定。
    /// <para>线程模型：所有回调默认在 I/O 线程触发；需要回到主线程时设置 <see cref="Dispatcher"/>。</para>
    /// <para>状态：同一实例支持 Disconnect 后再次 Connect（重连）；Dispose 之后不可再用。</para>
    /// </summary>
    public class TcpClient : IDisposable
    {
        /// <summary>
        /// 读缓冲大小
        /// </summary>
        public const int MAX_READ = 1 << 13;
        /// <summary>
        /// 单条消息体上限（由 2 字节长度头决定）
        /// </summary>
        public const int MAX_MESSAGE_LENGTH = ushort.MaxValue;

        System.Net.Sockets.TcpClient client;
        NetworkStream outStream;
        IHeaderEncoder headerEncoder;

        //读缓冲：BeginRead 复用它，收到后立即拷出再续读
        readonly byte[] readBuffer = new byte[MAX_READ];
        //粘包/半包缓存：pendingBuffer[0..pendingLength) 是尚未成帧的数据
        byte[] pendingBuffer = new byte[MAX_READ];
        int pendingLength;
        byte[] headerBuffer;

        //发送：单写者队列，保证帧不交错、顺序与调用一致；未连接时先入队，连上后补发
        readonly object sendLocker = new object();
        readonly Queue<byte[]> sendQueue = new Queue<byte[]>();
        bool sending;
        bool disposed;

        Timer connectTimer;
        volatile bool connectedNotified;

        Action onConnected;
        Action<byte[]> onData;
        Action onDisconnected;

        /// <summary>
        /// 连接成功回调（I/O 线程，或 <see cref="Dispatcher"/> 指定线程）
        /// </summary>
        public event Action OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        /// <summary>
        /// 收到完整消息回调（I/O 线程，或 <see cref="Dispatcher"/> 指定线程）
        /// </summary>
        public event Action<byte[]> OnData
        {
            add { onData += value; }
            remove { onData -= value; }
        }
        /// <summary>
        /// 断开回调（正常断开、对端关闭、异常、连接失败都会触发；每个连接最多一次）
        /// </summary>
        public event Action OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        /// <summary>
        /// 是否处于已连接状态
        /// <para>本属性只反映"框架认为仍连接着"，不再直接读 <c>TcpClient.Connected</c>
        /// （后者只表示最近一次 I/O 的结果，对端关闭后仍可能为 true）。</para>
        /// </summary>
        public bool Connected
        {
            get
            {
                var tcp = client;
                return tcp != null && connectedNotified && tcp.Connected;
            }
        }
        /// <summary>
        /// 底层 TcpClient（调试用；连接建立前为 null）
        /// </summary>
        public System.Net.Sockets.TcpClient Client { get { return client; } }
        /// <summary>
        /// 是否已 Dispose
        /// </summary>
        public bool IsDisposed { get { return disposed; } }
        /// <summary>
        /// 关闭 Nagle 算法
        /// </summary>
        public bool NoDelay = true;
        /// <summary>
        /// 同步发送超时（毫秒）；对 BeginWrite 异步发送无效
        /// </summary>
        public int SendTimeout = 5000;
        /// <summary>
        /// 同步接收超时（毫秒）；对 BeginRead 异步接收无效
        /// </summary>
        public int ReceiveTimeout = 1000;
        /// <summary>
        /// 连接超时（毫秒，&lt;=0 表示不设超时）
        /// </summary>
        public int ConnectTimeoutMs = 5000;
        /// <summary>
        /// 回调派发器：为 null 时回调直接在 I/O 线程执行。
        /// <para>Unity 里通常设置为 <c>action =&gt; Utility.Unity.PostToUnityThread(_ =&gt; action())</c>。</para>
        /// </summary>
        public Action<Action> Dispatcher { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="encoder">消息头编解码器</param>
        public TcpClient(IHeaderEncoder encoder)
        {
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));
            headerEncoder = encoder;
            headerBuffer = new byte[encoder.GetHeaderLength()];
        }

        #region 连接 / 断开

        /// <summary>
        /// 连接服务器；同一实例可重复调用（会先清理上一次连接）。
        /// </summary>
        /// <param name="host">主机名或 IP</param>
        /// <param name="port">端口</param>
        public void Connect(string host, int port)
        {
            if (disposed)
            {
                Log.Error("[Hydrogen] connect failed : client disposed");
                return;
            }
            Disconnect();
            try
            {
                var addresses = Dns.GetHostAddresses(host);
                if (addresses == null || addresses.Length == 0)
                {
                    OnDisconnectedHandler(DisconnectResion.Exception, "[Hydrogen] connect failed : host invalid");
                    return;
                }
                var address = addresses[0];
                var tcp = new System.Net.Sockets.TcpClient(address.AddressFamily);
                tcp.SendTimeout = SendTimeout;
                tcp.ReceiveTimeout = ReceiveTimeout;
                tcp.NoDelay = NoDelay;
                client = tcp;
                connectedNotified = false;
                StartConnectTimer();
                tcp.BeginConnect(address, port, OnConnectHandler, null);
            }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] connect failed : {e.Message}");
            }
        }
        /// <summary>
        /// 断开连接（可重复调用；触发一次 <see cref="OnDisconnected"/>）。
        /// <para>不会销毁本实例，之后可以再次 <see cref="Connect"/>。</para>
        /// </summary>
        public void Disconnect()
        {
            var wasConnected = client != null;
            StopConnectTimer();
            CloseClient();
            if (wasConnected)
                Dispatch(onDisconnected);
        }
        /// <summary>
        /// 释放实例：断开连接并清空回调，之后不可再用。
        /// </summary>
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            Disconnect();
            onConnected = null;
            onData = null;
            onDisconnected = null;
            if (connectTimer != null)
            {
                connectTimer.Dispose();
                connectTimer = null;
            }
        }

        void CloseClient()
        {
            var tcp = client;
            client = null;
            connectedNotified = false;
            lock (sendLocker)
            {
                outStream = null;
                sendQueue.Clear();
                sending = false;
            }
            pendingLength = 0;
            if (tcp == null)
                return;
            try { tcp.Close(); }
            catch (Exception e) { Log.Warning($"[Hydrogen] close failed : {e.Message}"); }
        }

        void StartConnectTimer()
        {
            StopConnectTimer();
            if (ConnectTimeoutMs <= 0)
                return;
            connectTimer = new Timer(OnConnectTimeout, null, ConnectTimeoutMs, Timeout.Infinite);
        }
        void StopConnectTimer()
        {
            if (connectTimer == null)
                return;
            connectTimer.Dispose();
            connectTimer = null;
        }
        void OnConnectTimeout(object state)
        {
            if (disposed || connectedNotified)
                return;
            OnDisconnectedHandler(DisconnectResion.Exception, "[Hydrogen] connect failed : timeout");
        }

        void OnConnectHandler(IAsyncResult result)
        {
            var tcp = client;
            if (tcp == null || disposed)
                return;
            try
            {
                //关键：必须 EndConnect，否则"连接被拒/失败"永远不会被发现
                tcp.EndConnect(result);
            }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] connect failed : {e.Message}");
                return;
            }
            StopConnectTimer();
            NetworkStream stream;
            try { stream = tcp.GetStream(); }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] get stream failed : {e.Message}");
                return;
            }
            lock (sendLocker)
            {
                outStream = stream;
            }
            connectedNotified = true;
            Dispatch(onConnected);
            //连接前挂起的消息补发
            lock (sendLocker)
            {
                if (!sending && sendQueue.Count > 0)
                {
                    sending = true;
                    StartNextSend();
                }
            }
            StartRead(stream);
        }

        #endregion

        #region 发送

        /// <summary>
        /// 发送一条消息（加 header 后写入）。
        /// <para>未连接时先入队，连接成功后按序补发；超过 <see cref="MAX_MESSAGE_LENGTH"/> 会拒绝并报错。</para>
        /// </summary>
        /// <param name="message">消息体</param>
        public void SendMessage(byte[] message)
        {
            if (disposed)
            {
                Log.Error("[Hydrogen] send failed : client disposed");
                return;
            }
            if (message == null || message.Length == 0)
            {
                Log.Error("[Hydrogen] send failed : empty message");
                return;
            }
            if (message.Length > MAX_MESSAGE_LENGTH)
            {
                //长度头只有 2 字节，超长必须明确拒绝：截断会静默产生错帧
                Log.Error($"[Hydrogen] send failed : message length {message.Length} exceeds {MAX_MESSAGE_LENGTH}");
                return;
            }
            if (client == null)
            {
                //没有连接（也没在连接中）：直接报错，不排队——避免把过期消息发到下一条连接上
                Log.Error("[Hydrogen] send failed : not connected");
                return;
            }
            var header = headerEncoder.EncodeHeader((ushort)message.Length);
            if (header == null || header.Length != headerBuffer.Length)
            {
                Log.Error("[Hydrogen] send failed : invalid header from encoder");
                return;
            }
            var payload = new byte[header.Length + message.Length];
            Buffer.BlockCopy(header, 0, payload, 0, header.Length);
            Buffer.BlockCopy(message, 0, payload, header.Length, message.Length);
            lock (sendLocker)
            {
                sendQueue.Enqueue(payload);
                if (!sending)
                {
                    sending = outStream != null;
                    if (sending)
                        StartNextSend();
                }
            }
        }
        /// <summary>
        /// 发送下一条（调用方必须持有 sendLocker）
        /// </summary>
        void StartNextSend()
        {
            if (sendQueue.Count == 0)
            {
                sending = false;
                return;
            }
            var stream = outStream;
            if (stream == null)
            {
                //还没连上：保留队列，等 OnConnectHandler 里补发
                sending = false;
                return;
            }
            var payload = sendQueue.Peek();
            try
            {
                stream.BeginWrite(payload, 0, payload.Length, OnWrite, null);
            }
            catch (Exception e)
            {
                sending = false;
                Log.Error($"[Hydrogen] send failed : {e.Message}");
            }
        }
        void OnWrite(IAsyncResult result)
        {
            var stream = outStream;
            string error = null;
            try
            {
                stream?.EndWrite(result);
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            if (error == null)
            {
                lock (sendLocker)
                {
                    if (sendQueue.Count > 0)
                        sendQueue.Dequeue();
                    StartNextSend();
                }
                return;
            }
            OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] write failed : {error}");
        }

        #endregion

        #region 接收

        void OnReadHandler(IAsyncResult result)
        {
            var stream = outStream;
            if (stream == null)
                return;
            int read;
            try
            {
                read = stream.EndRead(result);
            }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] read failed : {e.Message}");
                return;
            }
            if (read <= 0)
            {
                OnDisconnectedHandler(DisconnectResion.Disconnect, "[Hydrogen] disconnected by remote");
                return;
            }
            //解析必须在 I/O 线程按读取顺序单链完成（否则并发解析会破坏 pending 缓冲、甚至颠倒消息顺序），
            //解析完再续读，业务回调通过 Dispatch 派发。
            try { OnReceive(readBuffer, read); }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] parse failed : {e.Message}");
                return;
            }
            StartRead(stream);
        }
        void StartRead(NetworkStream stream)
        {
            try { stream.BeginRead(readBuffer, 0, readBuffer.Length, OnReadHandler, null); }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, $"[Hydrogen] begin read failed : {e.Message}");
            }
        }

        /// <summary>
        /// 粘包/半包解析：把 chunk 追加到 pendingBuffer，尽量切出完整消息
        /// </summary>
        /// <summary>
        /// 粘包/半包解析：把 buffer[0..length) 追加进 pending 缓冲，切出所有完整消息
        /// </summary>
        void OnReceive(byte[] buffer, int length)
        {
            AppendPending(buffer, length);
            var headerLength = headerBuffer.Length;
            var offset = 0;
            List<byte[]> messages = null;
            while (pendingLength - offset >= headerLength)
            {
                Buffer.BlockCopy(pendingBuffer, offset, headerBuffer, 0, headerLength);
                var messageLength = headerEncoder.DecodeHeader(headerBuffer);
                if (pendingLength - offset - headerLength < messageLength)
                    break;      //半包：等更多数据
                var message = new byte[messageLength];
                if (messageLength > 0)
                    Buffer.BlockCopy(pendingBuffer, offset + headerLength, message, 0, messageLength);
                offset += headerLength + messageLength;
                if (messages == null)
                    messages = new List<byte[]>();
                messages.Add(message);
            }
            //先把内部状态收尾，再回调业务：业务在回调里断开/重连也不会破坏解析状态
            CompactPending(offset);
            if (messages == null || onData == null)
                return;
            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                Dispatch(() => onData?.Invoke(message));
            }
        }
        void AppendPending(byte[] buffer, int length)
        {
            if (pendingLength + length > pendingBuffer.Length)
            {
                var capacity = pendingBuffer.Length;
                while (capacity < pendingLength + length)
                    capacity <<= 1;
                var grown = new byte[capacity];
                Buffer.BlockCopy(pendingBuffer, 0, grown, 0, pendingLength);
                pendingBuffer = grown;
            }
            Buffer.BlockCopy(buffer, 0, pendingBuffer, pendingLength, length);
            pendingLength += length;
        }
        void CompactPending(int consumed)
        {
            if (consumed <= 0)
                return;
            if (consumed > pendingLength)
                consumed = pendingLength;
            var remain = pendingLength - consumed;
            if (remain > 0)
                Buffer.BlockCopy(pendingBuffer, consumed, pendingBuffer, 0, remain);
            pendingLength = remain;
        }

        #endregion

        void Dispatch(Action action)
        {
            if (action == null)
                return;
            var dispatcher = Dispatcher;
            if (dispatcher == null)
            {
                action();
                return;
            }
            dispatcher(action);
        }
        void OnDisconnectedHandler(DisconnectResion reason, string message)
        {
            if (reason == DisconnectResion.Exception)
                Log.Error(message);
            else
                Log.Info(message);
            Disconnect();
        }
    }
}
