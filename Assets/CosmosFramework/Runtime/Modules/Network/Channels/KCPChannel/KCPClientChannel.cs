using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Network;
using kcp;

namespace Cosmos
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
        string channelName;
        KcpClientService kcpClientService;
        Action onConnected;
        Action onDisconnected;
        Action<byte[]> onDataReceived;
        Action onAbort;
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
        public NetworkProtocol NetworkProtocol { get; set; }
        public bool IsConnect { get; private set; }
        public NetworkChannelKey NetworkChannelKey { get; private set; }

        public int Port { get; private set; }
        public string IPAddress { get; private set; }

        public KCPClientChannel(string channelName)
        {
            this.channelName = channelName;
            KCPLog.Info = (s) => Utility.Debug.LogInfo(s);
            KCPLog.Warning = (s) => Utility.Debug.LogWarning(s);
            KCPLog.Error = (s) => Utility.Debug.LogError(s);
        }
        public void Connect(string ip, int port)
        {
            this.IPAddress = ip;
            this.Port = port;
            NetworkChannelKey = new NetworkChannelKey(channelName, $"{ip}:{port}");
            kcpClientService = new KcpClientService();
            kcpClientService.ServiceSetup();
            kcpClientService.OnClientDataReceived += OnReceiveDataHandler;
            kcpClientService.OnClientConnected += OnConnectHandler;
            kcpClientService.OnClientDisconnected += OnDisconnectHandler;
            kcpClientService.ServiceUnpause();
            kcpClientService.Port = (ushort)Port;
            kcpClientService.ServiceConnect(IPAddress);
        }
        public void TickRefresh()
        {
            kcpClientService?.ServiceTick();
        }
        public void Disconnect()
        {
            kcpClientService?.ServiceDisconnect();
        }
        /// <summary>
        /// 发送数据到remote;
        /// 默认为可靠类型；
        /// </summary>
        /// <param name="data">数据</param>
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
            kcpClientService?.ServiceSend((KcpChannel)byteType, arraySegment);
            return true;
        }
        public void AbortChannne()
        {
            Disconnect();
            NetworkChannelKey = NetworkChannelKey.None;
            onAbort?.Invoke();
        }
        void OnDisconnectHandler()
        {
            IsConnect = false;
            Utility.Debug.LogError($"{NetworkChannelKey} disconnected ! ");
            onDisconnected?.Invoke();
            onConnected = null;
            onDisconnected = null;
            onDataReceived = null;
        }
        void OnConnectHandler()
        {
            IsConnect = true;
            Utility.Debug.LogWarning($"{NetworkChannelKey} connected ! ");
            onConnected?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg, byte channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, arrSeg.Offset, rcvData, 0, rcvLen);
            onDataReceived?.Invoke(rcvData);
        }
    }
}
