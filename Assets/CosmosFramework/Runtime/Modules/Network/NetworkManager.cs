using System.Collections;
using Cosmos;
using System.Net;
using System.Net.Sockets;
using System;
using kcp;

namespace Cosmos.Network
{
    //TODO NetworkManager  这里使用了停等ARQ协议，目前未实现包组发送
    /// <summary>
    /// 此模块为客户端网络管理类
    /// </summary>
    [Module]
    internal sealed class NetworkManager : Module, INetworkManager
    {
        public event Action OnConnect
        {
            add { onConnect += value; }
            remove { onConnect -= value; }
        }
        public event Action OnDisconnect
        {
            add { onDisconnect += value; }
            remove { onDisconnect -= value; }
        }
        public event Action<byte[]> OnReceiveData
        {
            add { onReceiveData += value; }
            remove { onReceiveData -= value; }
        }
        Action onConnect;
        Action onDisconnect;
        Action<byte[]> onReceiveData;
        NetworkProtocolType currentNetworkProtocolType;

        bool clearCallbackWhenDisconnected = false;
        #region UDP
        INetworkService service;
        IHeartbeat heartbeat;
        #endregion

        #region KCP
        KcpClientService kcpClientService;
        #endregion

        public bool IsConnected { get; private set; }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            //if (!IsConnected)
            //    return;
            switch (currentNetworkProtocolType)
            {
                case NetworkProtocolType.TCP:
                    break;
                case NetworkProtocolType.UDP:
                    service?.OnRefresh();
                    break;
                case NetworkProtocolType.KCP:
                    kcpClientService?.ServiceTick();
                    break;
            }
        }
        public void SendNetworkMessage(byte[] data)
        {
            if (IsConnected)
            {
                switch (currentNetworkProtocolType)
                {
                    case NetworkProtocolType.TCP:
                        break;
                    case NetworkProtocolType.UDP:
                        //service?.OnRefresh();
                        break;
                    case NetworkProtocolType.KCP:
                        {
                            var arraySegment = new ArraySegment<byte>(data);
                            kcpClientService?.ServiceSend(KcpChannel.Reliable, arraySegment);
                        }
                        break;
                }
            }
            else
                Utility.Debug.LogError("Can not send net message, no service");
        }
        /// <summary>
        /// 与远程建立连接；
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="protocolType">协议类型</param>
        public void Connect(string ip, ushort port, NetworkProtocolType protocolType)
        {
            OnUnPause();
            if (IsConnected)
            {
                Utility.Debug.LogError("Network is Connected !");
                return;
            }
            currentNetworkProtocolType = protocolType;
            clearCallbackWhenDisconnected = false;
            switch (protocolType)
            {
                case NetworkProtocolType.KCP:
                    {
                        KCPLog.Info = (s) => Utility.Debug.LogInfo(s);
                        KCPLog.Warning = (s) => Utility.Debug.LogInfo(s, MessageColor.YELLOW);
                        KCPLog.Error = (s) => Utility.Debug.LogError(s);
                        var kcpClient = new KcpClientService();
                        kcpClientService = kcpClient;
                        kcpClientService.ServiceSetup();
                        kcpClientService.OnClientDataReceived += OnKCPReceiveDataHandler;
                        kcpClientService.OnClientConnected += OnConnectHandler;
                        kcpClientService.OnClientDisconnected += OnDisconnectHandler;
                        kcpClientService.ServiceUnpause();
                        kcpClientService.Port = (ushort)port;
                        kcpClientService.ServiceConnect(ip);
                    }
                    break;
                case NetworkProtocolType.TCP:
                    {
                    }
                    break;
                case NetworkProtocolType.UDP:
                    {
                        //if (service == null)
                        //{
                        //    service = new UdpClientService();
                        //    UdpClientService udp = service as UdpClientService;
                        //    udp.OnConnect += OnConnectHandler;
                        //    udp.OnDisconnect += OnDisconnectHandler;
                        //    udp.OnReceiveData += OnReceiveDataHandler;
                        //    heartbeat = new UDPHeartbeat();
                        //    service.SetHeartbeat(heartbeat);
                        //}

                        //UdpClientService udpSrv = service as UdpClientService;
                        //udpSrv.Connect(ip, port);

                        //Utility.Debug.LogInfo("Try to connect to the server");
                    }
                    break;
            }
        }
        /// <summary>
        /// 断开网络链接；
        /// </summary>
        /// <param name="clearCallbackWhenDisconnected">是否在断开连接后清空回调的监听</param>
        public void Disconnect(bool clearCallbackWhenDisconnected = false)
        {
            this.clearCallbackWhenDisconnected = clearCallbackWhenDisconnected;
            switch (currentNetworkProtocolType)
            {
                case NetworkProtocolType.TCP:
                    break;
                case NetworkProtocolType.UDP:
                    // service.Disconnect();
                    break;
                case NetworkProtocolType.KCP:
                    kcpClientService?.ServiceDisconnect();
                    break;
            }
        }
        void OnDisconnectHandler()
        {
            OnPause();
            IsConnected = false;
            Utility.Debug.LogInfo("Server Disconnected", MessageColor.RED);
            onDisconnect?.Invoke();
            if (clearCallbackWhenDisconnected)
            {
                onConnect = null;
                onDisconnect = null;
                onReceiveData = null;
            }
        }
        void OnConnectHandler()
        {
            IsConnected = true;
            Utility.Debug.LogInfo("Server Connected ! ");
            onConnect?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg)
        {
            //  onReceiveData?.Invoke(arrSeg);
        }
        void OnKCPReceiveDataHandler(ArraySegment<byte> arrSeg, byte channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, arrSeg.Offset, rcvData, 0, rcvLen);
            onReceiveData?.Invoke(rcvData);
        }
    }
}
