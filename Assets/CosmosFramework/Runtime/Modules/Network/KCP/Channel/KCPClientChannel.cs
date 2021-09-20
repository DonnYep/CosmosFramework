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
    public class KCPClientChannel : INetworkChannel
    {
        string ip;
        ushort port;

        KcpClientService kcpClientService;
        Action<int> onConnected;
        Action<int> onDisconnected;
        Action<int, byte[]> onReceiveData;
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
        public event Action<int, byte[]> OnReceiveData
        {
            add { onReceiveData += value; }
            remove { onReceiveData -= value; }
        }
        public NetworkProtocol NetworkProtocol { get; set; }
        public bool IsConnect { get; private set; }
        public NetworkChannelKey NetworkChannelKey { get; private set; }
        public KCPClientChannel(string channelName, string ip, ushort port)
        {
            NetworkChannelKey = new NetworkChannelKey(channelName, $"{ip}:{port}");
            KCPLog.Info = (s) => Utility.Debug.LogInfo(s);
            KCPLog.Warning = (s) => Utility.Debug.LogInfo(s, MessageColor.YELLOW);
            KCPLog.Error = (s) => Utility.Debug.LogError(s);
            this.ip = ip;
            this.port = port;
        }
        public void Disconnect()
        {
            kcpClientService?.ServiceDisconnect();
        }
        public void TickRefresh()
        {
            kcpClientService?.ServiceTick();
        }
        public void Connect()
        {
            kcpClientService = new KcpClientService();
            kcpClientService.ServiceSetup();
            kcpClientService.OnClientDataReceived += OnReceiveDataHandler;
            kcpClientService.OnClientConnected += OnConnectHandler;
            kcpClientService.OnClientDisconnected += OnDisconnectHandler;
            kcpClientService.ServiceUnpause();
            kcpClientService.Port = port;
            kcpClientService.ServiceConnect(ip);
        }
        /// <summary>
        /// 断开与remote的连接；
        /// </summary>
        /// <param name="connectionId">可以忽略</param>
        public void Disconnect(int connectionId = -1)
        {
            kcpClientService?.ServiceDisconnect();
        }
        public void AbortChannel()
        {
            kcpClientService?.ServiceDisconnect();
            onAbort?.Invoke();
        }
        /// <summary>
        /// 发送数据到remote;
        /// 默认为可靠类型；
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="connectionId">连接Id</param>
        public void SendMessage(byte[] data, int connectionId = -1)
        {
            SendMessage(NetworkReliableType.Reliable, data, connectionId);
        }
        /// <summary>
        ///发送消息到remote;
        /// </summary>
        /// <param name="reliableType">消息可靠类型</param>
        /// <param name="data">数据</param>
        /// <param name="connectionId">可以忽略</param>
        public void SendMessage(NetworkReliableType reliableType, byte[] data, int connectionId = -1)
        {
            var arraySegment = new ArraySegment<byte>(data);
            var byteType = (byte)reliableType;
            kcpClientService?.ServiceSend((KcpChannel)byteType, arraySegment);
        }
        /// <summary>
        /// 获取连接Id的地址；
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        /// <returns></returns>
        public string GetconnectionAddress(int connectionId)
        {
            //客户端通道仅返回本地地址；
            return System.Net.Dns.GetHostName();
        }
        void OnDisconnectHandler()
        {
            IsConnect = false;
            Utility.Debug.LogError($"{NetworkChannelKey} disconnected ! ");
            onDisconnected?.Invoke(-1);
            onConnected = null;
            onDisconnected = null;
            onReceiveData = null;
        }
        void OnConnectHandler()
        {
            IsConnect = true;
            Utility.Debug.LogWarning($"{NetworkChannelKey} connected ! ");
            onConnected?.Invoke(-1);
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg, byte channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, arrSeg.Offset, rcvData, 0, rcvLen);
            onReceiveData?.Invoke(-1, rcvData);
        }
    }
}
