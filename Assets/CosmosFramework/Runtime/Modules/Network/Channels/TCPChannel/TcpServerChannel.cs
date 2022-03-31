using System;
using Telepathy;
namespace Cosmos.Network
{
    public class TcpServerChannel : INetworkServerChannel
    {
        Server server;
        /// <summary>
        ///  check if the server is running
        /// </summary>
        public bool Active { get { return server.Active; } }
        Action onAbort;
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action<int> OnConnected
        {
            add { server.OnConnected += value; }
            remove { server.OnConnected -= value; }
        }
        Action<int, byte[]> onDataReceived;
        public event Action<int, byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action<int> OnDisconnected
        {
            add { server.OnDisconnected += value; }
            remove { server.OnDisconnected -= value; }
        }
        public NetworkChannelKey NetworkChannelKey { get; private set; }
        public int Port { get; private set; }
        public TcpServerChannel(string channelName, int port)
        {
            NetworkChannelKey = new NetworkChannelKey(channelName, $"localhost:{port}");
            server = new Server(TcpConstants.MaxMessageSize);
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);
            this.Port = port;
        }
        public bool StartServer()
        {
            if (server.Start(Port))
            {
                server.OnData += OnReceiveDataHandler;
                return true;
            }
            return false;
        }
        public bool Disconnect(int connectionId)
        {
            if (server.Disconnect(connectionId))
            {
                server.OnData -= OnReceiveDataHandler;
                return true;
            }
            return false;
        }
        public string GetConnectionAddress(int connectionId)
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
        public void AbortChannnel()
        {
            StopServer();
            NetworkChannelKey = NetworkChannelKey.None;
            onAbort?.Invoke();
        }
        void OnReceiveDataHandler(int conv, ArraySegment<byte> arrSeg)
        {
            onDataReceived?.Invoke(conv, arrSeg.Array);
        }
    }
}
