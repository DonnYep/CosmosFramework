using System;
using kcp2k;

namespace Cosmos.Network
{
    //================================================
    /*
    * 1、ChlientChannel启动后，维护并保持与远程服务器的连接。
    * 
    *2、主动连接remote超过20000ms未响应时，触发超时事件被，结束连接并
    *触发onDisconnected，返回参数NetworkChannelKey以及 -1；
    *
    *3、连接成功，触发onConnected并返回参数NetworkChannelKey以及-1；
    *
    *4、从remote接收数据，触发onReceiveData，返回byte[] 数组，-1，以及
    *NetworkChannelKey；
    *
    *5、发送消息到remote，需要通过调用SendMessage方法。
    */
    //================================================
    /// <summary>
    /// KCP客户端通道；
    /// </summary>
    public class KCPClientChannel : INetworkClientChannel
    {
        ///<inheritdoc/>
        public string ChannelName { get; set; }

        KcpClient client;

        Action onConnected;
        Action onDisconnected;
        Action<byte[]> onDataReceived;
        Action onAbort;
        Action<string> onError;
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        public event Action OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        public event Action<byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action<string> OnError
        {
            add { onError += value; }
            remove { onError -= value; }
        }
        ///<inheritdoc/>
        public bool IsConnect { get { return client.connected; } }
        ///<inheritdoc/>
        public int Port { get; private set; }
        ///<inheritdoc/>
        public string Host { get; private set; }
        public KCPClientChannel(string channelName)
        {
            this.ChannelName = channelName;
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);
            client = new KcpClient(
                OnConnectHandler,
                OnReceiveDataHandler,
                OnDisconnectHandler,
                OnErrorHandler
            );
        }
        ///<inheritdoc/>
        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            client.Connect(Host, (ushort)port, true, 10);
        }
        ///<inheritdoc/>
        public void TickRefresh()
        {
            client?.Tick();
        }
        ///<inheritdoc/>
        public void Disconnect()
        {
            client.Disconnect();
        }
        ///<inheritdoc/>
        public bool SendMessage(byte[] data)
        {
            return SendMessage(NetworkReliableType.Reliable, data);
        }
        /// <summary>
        ///发送消息到remote;
        /// </summary>
        /// <param name="reliableType">消息可靠类型</param>
        /// <param name="data">数据</param>
        public bool SendMessage(NetworkReliableType reliableType, byte[] data)
        {
            if (!IsConnect)
                return false;
            var arraySegment = new ArraySegment<byte>(data);
            var byteType = (byte)reliableType;
            var channelId = (KcpChannel)byteType;
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    client.Send(arraySegment, KcpChannel.Unreliable);
                    break;
                default:
                    client.Send(arraySegment, KcpChannel.Reliable);
                    break;
            }
            return true;
        }
        ///<inheritdoc/>
        public void AbortChannnel()
        {
            Disconnect();
            onAbort?.Invoke();
        }
        void OnDisconnectHandler()
        {
            onDisconnected?.Invoke();
            onConnected = null;
            onDisconnected = null;
            onDataReceived = null;
        }
        void OnConnectHandler()
        {
            onConnected?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg, KcpChannel channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, arrSeg.Offset, rcvData, 0, rcvLen);
            onDataReceived?.Invoke(rcvData);
        }
        void OnErrorHandler(ErrorCode error, string reason)
        {
            onError?.Invoke($"{error}-{reason}");
        }
    }
}
