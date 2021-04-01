using System.Collections;
using Cosmos;
using System.Net;
using System.Net.Sockets;
using System;

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
        public event Action<ArraySegment<byte>> OnReceiveData
        {
            add { onReceiveData += value; }
            remove { onReceiveData -= value; }
        }
        string serverIP;
        int serverPort;
        string clientIP;
        int clientPort;
        INetworkService service;
        IPEndPoint serverEndPoint;
        Action onConnect;
        Action onDisconnect;
        Action<ArraySegment<byte>> onReceiveData;
        IHeartbeat heartbeat;
        public long Conv { get { return service.Conv; } }
        public bool IsConnected { get; private set; }
        public IPEndPoint ServerEndPoint
        {
            get
            {
                if (serverEndPoint == null)
                    serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                return serverEndPoint;
            }
        }
        IPEndPoint clientEndPoint;
        public IPEndPoint ClientEndPoint
        {
            get
            {
                if (clientEndPoint == null)
                    clientEndPoint = new IPEndPoint(IPAddress.Parse(clientIP), clientPort);
                return clientEndPoint;
            }
        }
        public override void OnFixRefresh()
        {
            if (IsPause)
                return;
            service?.OnRefresh();
        }
         public void SendNetworkMessage(INetworkMessage netMsg, IPEndPoint endPoint)
        {
            if (IsConnected)
            {
                service.SendMessageAsync(netMsg, endPoint);
            }
            else
                Utility.Debug.LogError("Can not send net message, no service");
        }
         public void SendNetworkMessage(INetworkMessage netMsg)
        {
            if (IsConnected)
            {
                service.SendMessageAsync(netMsg);
            }
            else
                Utility.Debug.LogError("Can not send net message, no service");
        }
         public void SendNetworkMessage(byte[] buffer)
        {
            if (IsConnected)
            {
                service.SendMessageAsync(buffer);
            }
            else
                Utility.Debug.LogError("Can not send net message, no service");
        }
        /// <summary>
        /// 与远程建立连接；
        /// 当前只有udp
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="protocolType">协议类型</param>
         public void Connect(string ip, int port, NetworkProtocolType protocolType)
        {
            OnUnPause();
            if (IsConnected)
            {
                Utility.Debug.LogError("Network is Connected !");
                return;
            }
            switch (protocolType)
            {
                case NetworkProtocolType.KCP:
                    {
                    }
                    break;
                case NetworkProtocolType.TCP:
                    {
                    }
                    break;
                case NetworkProtocolType.UDP:
                    {
                        if (service == null)
                        {
                            service = new UdpClientService();
                            UdpClientService udp = service as UdpClientService;
                            udp.OnConnect += OnConnectHandler;
                            udp.OnDisconnect += OnDisconnectHandler;
                            udp.OnReceiveData += OnReceiveDataHandler;
                            heartbeat = new Heartbeat();
                            service.SetHeartbeat(heartbeat);
                        }
                        UdpClientService udpSrv = service as UdpClientService;
                        udpSrv.Connect(ip, port);

                        Utility.Debug.LogInfo("Try to connect to the server");
                    }
                    break;
            }
        }
        /// <summary>
        /// 与远程建立连接；
        /// </summary>
        /// <param name="service">自定义实现的服务</param>
         public void Connect(INetworkService service)
        {
            if (service == null)
            {
                Utility.Debug.LogError("No Service ");
                return;
            }
            OnUnPause();
            this.service = service;
            //service.Connect(service.);
            Utility.Debug.LogInfo("Try to connect to the server");
        }
         public void Disconnect(bool notifyRemote = true)
        {
            if (service == null)
            {
                Utility.Debug.LogError("No Service");
                return;
            }
            if (!IsConnected)
            {
                Utility.Debug.LogError("App is not connected to the network! ");
                return;
            }
            service.Disconnect();
        }
        void RunHeartbeat(uint intervalSec, byte maxRecur)
        {
            //var hb = new Heartbeat();
            heartbeat.MaxRecurCount = maxRecur;
            heartbeat.HeartbeatInterval = intervalSec;
            heartbeat.SendHeartbeatHandler = service.SendMessageAsync;
            heartbeat.OnActive();
            //service.SetHeartbeat(hb);
        }
        void OnDisconnectHandler()
        {
            OnPause();
            IsConnected = false;
            Utility.Debug.LogError("Disconnect network, stop service");
            onDisconnect?.Invoke();
        }
        void OnConnectHandler()
        {
            RunHeartbeat(NetworkConsts.HeartbeatInterval, NetworkConsts.MaxRecurCount);
            IsConnected = true;
            Utility.Debug.LogInfo("Network is connected ! ");
            onConnect?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg)
        {
            onReceiveData?.Invoke(arrSeg);
        }
    }
}
