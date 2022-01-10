using System;
using Cosmos.Network;
using kcp;

namespace Cosmos
{
    //================================================
    /*
    *1、ServerChannel启动后，接收并维护remote进入的连接;
    *
    *2、当有请求进入并成功建立连接时，触发OnConnected，分发参数分别为
    *NetworkChannelKey以及建立连接的conv;
    *
    *3、当请求断开连接，触发OnDisconnected，分发NetworkChannelKey以及
    *断开连接的conv;
    *
    *4、已连接对象发来数据时，触发OnDataReceived，分发NetworkChannelKey
    *以及发送来数据的conv;
    */
    //================================================
    /// <summary>
    /// / KCP服务端通道；
    /// </summary>
    public class KCPServerChannel : INetworkServerChannel
    {
        KcpServerService server;
        Action<int> onConnected;
        Action<int> onDisconnected;
        Action<int, byte[]> onDataReceived;
        Action onAbort;
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action<int> OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        public event Action<int> OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        public event Action<int, byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public int Port { get; private set; }

        public bool Active
        {
            get
            {
                if (server.Server == null)
                    return false;
                return server.Server.IsActive();
            }
        }
        public NetworkChannelKey NetworkChannelKey { get; private set; }
        public KCPServerChannel(string channelName, ushort port)
        {
            NetworkChannelKey = new NetworkChannelKey(channelName, $"localhost:{port}");
            KCPLog.Info = (s) => Utility.Debug.LogInfo(s);
            KCPLog.Warning = (s) => Utility.Debug.LogWarning(s);
            KCPLog.Error = (s) => Utility.Debug.LogError(s);
            this.Port = port;
            server = new KcpServerService();
            server.Port = port;
        }
        /// <summary>
        /// 服务端启动服务器；
        /// </summary>
        public bool StartServer()
        {
            if (Active)
                return false;
            server.ServiceSetup();
            server.ServiceUnpause();
            server.OnServerDataReceived += OnReceiveDataHandler;
            server.OnServerDisconnected += OnDisconnectedHandler;
            server.OnServerConnected += OnConnectedHandler;
            server.ServiceConnect();
            return true;
        }
        public void StopServer()
        {
            server?.ServicePause();
            server.OnServerDataReceived -= OnReceiveDataHandler;
            server.OnServerDisconnected -= OnDisconnectedHandler;
            server.OnServerConnected -= OnConnectedHandler;
            server?.ServerServiceStop();
        }
        public void TickRefresh()
        {
            server?.ServiceTick();
        }
        /// <summary>
        /// 与已经连接的connectionId断开连接；
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        public void Disconnect(int connectionId)
        {
            server?.ServiceDisconnect(connectionId);
        }
        /// <summary>
        /// 发送数据到remote;
        /// 默认为可靠类型；
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="connectionId">连接Id</param>
        public bool SendMessage(int connectionId, byte[] data)
        {
            return SendMessage(NetworkReliableType.Reliable, connectionId, data);
        }
        public bool SendMessage(NetworkReliableType reliableType, int connectionId, byte[] data)
        {
            if (!Active)
                return false;
            var segment = new ArraySegment<byte>(data);
            var byteType = (byte)reliableType;
            server?.ServiceSend((KcpChannel)byteType, segment, connectionId);
            return true;
        }
        /// <summary>
        /// 获取连接Id的地址；
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        /// <returns></returns>
        public string GetConnectionAddress(int connectionId)
        {
            return server.Server.GetClientAddress(connectionId);
        }
        public void AbortChannne()
        {
            StopServer();
            NetworkChannelKey = NetworkChannelKey.None;
            onAbort?.Invoke();
        }
        void OnDisconnectedHandler(int conv)
        {
            onDisconnected?.Invoke(conv);
        }
        void OnConnectedHandler(int conv)
        {
            onConnected?.Invoke(conv);
        }
        void OnReceiveDataHandler(int conv, ArraySegment<byte> arrSeg, int Channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, 1, rcvData, 0, rcvLen);
            onDataReceived?.Invoke(conv, rcvData);
        }


    }
}
