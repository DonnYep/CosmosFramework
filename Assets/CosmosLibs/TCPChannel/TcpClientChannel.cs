using System;
using Telepathy;
namespace Cosmos.Network
{
    public class TcpClientChannel : INetworkClientChannel
    {
        Client client;
        Action onAbort;
        ///<inheritdoc/>
        public string ChannelName { get; set; }
        ///<inheritdoc/>
        public bool IsConnect { get { return client.Connected; } }
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action OnConnected
        {
            add { client.OnConnected += value; }
            remove { client.OnConnected -= value; }
        }
        event Action<byte[]> onDataReceived;
        public event Action<byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action OnDisconnected
        {
            add { client.OnDisconnected += value; }
            remove { client.OnDisconnected -= value; }
        }
        ///<inheritdoc/>
        public int Port { get; private set; }
        ///<inheritdoc/>
        public string Host { get; private set; }
        public TcpClientChannel(string channelName)
        {
            this.ChannelName = channelName;
            client = new Client(TcpConstants.MaxMessageSize);
            //接收回调必须早于 Connect 绑定，否则极快建立连接时可能漏掉首批数据
            client.OnData = OnReceiveDataHandler;
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);
        }
        ///<inheritdoc/>
        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            client.Connect(Host, Port);
        }
        ///<inheritdoc/>
        public void TickRefresh()
        {
            client.Tick(100);
        }
        ///<inheritdoc/>
        public bool SendMessage(byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return client.Send(segment);
        }
        ///<inheritdoc/>
        public void Disconnect()
        {
            //只断开连接，不清订阅：通道允许重连，清空订阅会让后续监听静默失效
            client.Disconnect();
        }
        ///<inheritdoc/>
        public void Close()
        {
            Disconnect();
            onAbort?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg)
        {
            var length = arrSeg.Count;
            if (length <= 0)
                return;
            //必须按 Offset/Count 拷贝：arrSeg.Array 是底层整块缓冲，直接传出去长度是错的
            var data = new byte[length];
            Array.Copy(arrSeg.Array, arrSeg.Offset, data, 0, length);
            onDataReceived?.Invoke(data);
        }
    }
}
