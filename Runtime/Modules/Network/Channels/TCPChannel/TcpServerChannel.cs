using System;
using Telepathy;
namespace Cosmos.Network
{
    public class TcpServerChannel : INetworkServerChannel
    {
        Server server;
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public string ChannelName { get; set; }
        ///<inheritdoc/>
        public int Port { get; private set; }
        ///<inheritdoc/>
        public string IPAddress 
        {
            get { return server.listener.LocalEndpoint.ToString(); } 
        }
        public TcpServerChannel(string channelName, int port)
        {
            this.ChannelName = channelName;
            server = new Server(TcpConstants.MaxMessageSize);
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);
            this.Port = port;
        }
        ///<inheritdoc/>
        public bool StartServer()
        {
            if (server.Start(Port))
            {
                server.OnData = OnReceiveDataHandler;
                return true;
            }
            return false;
        }
        ///<inheritdoc/>
        public bool Disconnect(int connectionId)
        {
            if (server.Disconnect(connectionId))
            {
                return true;
            }
            return false;
        }
        ///<inheritdoc/>
        public string GetConnectionAddress(int connectionId)
        {
            return server.GetClientAddress(connectionId);
        }
        ///<inheritdoc/>
        public bool SendMessage(int connectionId, byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return server.Send(connectionId, segment);
        }
        ///<inheritdoc/>
        public void TickRefresh()
        {
            server.Tick(100);
        }
        ///<inheritdoc/>
        public void StopServer()
        {
            server.Stop();
            server.OnData = null;
            onDataReceived = null;
        }
        ///<inheritdoc/>
        public void AbortChannnel()
        {
            StopServer();
            onAbort?.Invoke();
            onAbort = null;
        }
        void OnReceiveDataHandler(int conv, ArraySegment<byte> arrSeg)
        {
            onDataReceived?.Invoke(conv, arrSeg.Array);
        }
    }
}
