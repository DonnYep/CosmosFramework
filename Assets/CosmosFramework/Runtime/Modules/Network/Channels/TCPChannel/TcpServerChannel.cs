using System;
using Cosmos;
using Cosmos.Network;

namespace Telepathy
{
    public class TcpServerChannel
    {
        public const int MaxMessageSize = 1 << 14;//1024*16
        Server server;
        string ip;
        int port;

        /// <summary>
        ///  check if the server is running
        /// </summary>
        public bool Active{ get { return server.Active; } }
        public event Action<int> OnConnected
        {
            add { server.OnConnected += value; }
            remove { server.OnConnected -= value; }
        }
        public event Action<int, ArraySegment<byte>> OnDataReceived
        {
            add { server.OnData += value; }
            remove { server.OnData -= value; }
        }
        public event Action<int> OnDisconnected
        {
            add { server.OnDisconnected += value; }
            remove { server.OnDisconnected -= value; }
        }
        public NetworkChannelKey NetworkChannelKey { get; private set; }
        public TcpServerChannel(string channelName, string ip, int port)
        {
            NetworkChannelKey = new NetworkChannelKey(channelName, $"{ip}:{port}");
            server = new Server(MaxMessageSize);
            Telepathy.Log.Info = (s) => Utility.Debug.LogInfo(s);
            Telepathy.Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Telepathy.Log.Error = (s) => Utility.Debug.LogError(s);
            this.ip = ip;
            this.port = port;
        }
        public bool StartServer()
        {
            return server.Start(port);
        }
        public bool Disconnect(int connectionId)
        {
            return server.Disconnect(connectionId);
        }
        public string GetClientAddress(int connectionId)
        {
            return server.GetClientAddress(connectionId);
        }
        public bool SendMessage(int connectionId, byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return server.Send(connectionId, segment);
        }
        public void TickRefresh()
        {
            server.Tick(100);
        }
        public void StopServer()
        {
            server.Stop();
        }
    }
}
