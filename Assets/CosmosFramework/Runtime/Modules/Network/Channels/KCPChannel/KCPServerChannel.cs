using System;
using kcp2k;

namespace Cosmos.Network
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
        KcpServerEndPoint server;

        Action<int> onConnected;
        Action<int> onDisconnected;
        Action<int, byte[]> onDataReceived;
        Action<int, string> onError;
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
        public event Action<int, string> OnError
        {
            add { onError += value; }
            remove { onError -= value; }
        }
        ///<inheritdoc/>
        public int Port { get; private set; }
        ///<inheritdoc/>
        public bool Active { get { return server.IsActive(); } }
        ///<inheritdoc/>
        public string ChannelName { get; set; }
        ///<inheritdoc/>
        public string IPAddress { get { return server.IPAddress; } }
        public KCPServerChannel(string channelName, ushort port)
        {
            this.ChannelName = channelName;
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);
            this.Port = port;
            server = new KcpServerEndPoint(
                (connectionId) => onConnected?.Invoke(connectionId),
                OnReceiveDataHandler,
                (connectionId) => onDisconnected?.Invoke(connectionId),
                OnErrorHandler,
                false,
                true,
                10
            );
        }
        ///<inheritdoc/>
        public bool StartServer()
        {
            if (Active)
                return false;
            server.Start((ushort)Port);
            return true;
        }
        ///<inheritdoc/>
        public void StopServer()
        {
            server.Stop();
        }
        ///<inheritdoc/>
        public void TickRefresh()
        {
            server.Tick();
        }
        ///<inheritdoc/>
        public bool Disconnect(int connectionId)
        {
            server.Disconnect(connectionId);
            return true;
        }
        ///<inheritdoc/>
        public bool SendMessage(int connectionId, byte[] data)
        {
            return SendMessage(NetworkReliableType.Reliable, connectionId, data);
        }
        public bool SendMessage(NetworkReliableType reliableType, int connectionId, byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            var byteType = (byte)reliableType;
            var channelId = (KcpChannel)byteType;
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    server.Send(connectionId, segment, KcpChannel.Unreliable);
                    break;
                default:
                    server.Send(connectionId, segment, KcpChannel.Reliable);
                    break;
            }
            return true;
        }
        ///<inheritdoc/>
        public string GetConnectionAddress(int connectionId)
        {
            return server.GetClientEndPoint(connectionId).Address.ToString();
        }
        ///<inheritdoc/>
        public void AbortChannnel()
        {
            StopServer();
            onAbort?.Invoke();
            onAbort = null;
        }
        void OnErrorHandler(int connectionId, ErrorCode error, string reason)
        {
            onError?.Invoke(connectionId, $"{error}-{reason}");
        }
        void OnReceiveDataHandler(int conv, ArraySegment<byte> arrSeg, KcpChannel Channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, 1, rcvData, 0, rcvLen);
            onDataReceived?.Invoke(conv, rcvData);
        }
    }
}
