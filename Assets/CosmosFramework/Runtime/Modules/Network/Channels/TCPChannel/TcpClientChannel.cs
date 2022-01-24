using System;
using Telepathy;
namespace Cosmos.Network
{
    public class TcpClientChannel : INetworkClientChannel
    {
        public const int MaxMessageSize = 1 << 14;//1024*16

        Client client;
        string channelName;
        public bool IsConnect { get { return client.Connected; } }
        Action onAbort;
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
        public NetworkChannelKey NetworkChannelKey { get; private set; }
        public int Port { get; private set; }
        public string IPAddress { get; private set; }
        public TcpClientChannel(string channelName)
        {
            this.channelName = channelName;
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);

        }
        public void Connect(string ip, int port)
        {
            NetworkChannelKey = new NetworkChannelKey(channelName, $"{ip}:{port}");
            this.IPAddress = ip;
            this.Port = port;
            client = new Client(MaxMessageSize);
            client.Connect(IPAddress, Port);
            client.OnData += OnReceiveDataHandler;
        }
        public void TickRefresh()
        {
            client.Tick(100);
        }
        public bool SendMessage(byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return client.Send(segment);
        }
        public void Disconnect()
        {
            client.Disconnect();
            onDataReceived = null;
        }
        public void AbortChannnel()
        {
            Disconnect();
            NetworkChannelKey = NetworkChannelKey.None;
            onAbort?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg)
        {
            onDataReceived?.Invoke(arrSeg.Array);
        }

    }
}
