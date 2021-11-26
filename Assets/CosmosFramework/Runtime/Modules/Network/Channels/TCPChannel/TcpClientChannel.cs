using System;
using Cosmos;
using Cosmos.Network;
using Telepathy;
namespace Cosmos
{
    public class TcpClientChannel
    {
        public const int MaxMessageSize = 1 << 14;//1024*16

        Client client;
        string ip;
        int port;
        public bool IsConnect { get { return client.Connected; } }
        public event Action OnConnected
        {
            add { client.OnConnected += value; }
            remove { client.OnConnected -= value; }
        }
        public event Action<ArraySegment<byte>> OnDataReceived
        {
            add { client.OnData += value; }
            remove { client.OnData -= value; }
        }
        public event Action OnDisconnected
        {
            add { client.OnDisconnected += value; }
            remove { client.OnDisconnected -= value; }
        }
        public NetworkChannelKey NetworkChannelKey { get; private set; }
        public TcpClientChannel(string channelName, string ip, int port)
        {
            NetworkChannelKey = new NetworkChannelKey(channelName, $"{ip}:{port}");
            client = new Client(MaxMessageSize);
            Telepathy.Log.Info = (s) => Utility.Debug.LogInfo(s);
            Telepathy.Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Telepathy.Log.Error = (s) => Utility.Debug.LogError(s);
            this.ip = ip;
            this.port = port;
        }
        public void Connect()
        {
            client.Connect(ip, port);
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
        public void DisConnect()
        {
            client.Disconnect();
        }
    }
}
