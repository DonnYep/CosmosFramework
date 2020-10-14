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
    internal sealed class NetworkManager : Module<NetworkManager>
    {
        public event Action NetworkOnConnect
        {
            add { networkOnConnect += value; }
            remove
            {
                try
                {
                    networkOnConnect -= value;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
        }
        public event Action NetworkOnDisconnect
        {
            add { networkOnDisconnect += value; }
            remove
            {
                try
                {
                    networkOnDisconnect -= value;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
        }
        string serverIP;
        int serverPort;
        string clientIP;
        int clientPort;
        INetworkService service;
        IPEndPoint serverEndPoint;
        INetworkMessageHelper netMessageHelper;
        Action networkOnConnect;
        Action networkOnDisconnect;
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
        internal void SendNetworkMessage(INetworkMessage netMsg, IPEndPoint endPoint)
        {
            if (IsConnected)
            {
                service.SendMessageAsync(netMsg, endPoint);
            }
            else
                Utility.Debug.LogError("Can not send net message, no service");
        }
        internal void SendNetworkMessage(INetworkMessage netMsg)
        {
            if (IsConnected)
            {
                service.SendMessageAsync(netMsg);
            }
            else
                Utility.Debug.LogError("Can not send net message, no service");
        }
        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="message">序列化后的数据</param>
        internal void SendNetworkMessage(ushort opCode, byte[] message)
        {
            if (IsConnected)
            {
                var netMsg = netMessageHelper.EncodeMessage(opCode, message);
                service.SendMessageAsync(netMsg);
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
        internal void Connect(string ip, int port, ProtocolType protocolType)
        {
            OnUnPause();
            if (IsConnected)
            {
                Utility.Debug.LogError("Network is Connected !");
                return;
            }
            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    {
                    }
                    break;
                case ProtocolType.Udp:
                    {
                        if (service == null)
                        {
                            service = new UdpClientService();
                            netMessageHelper = new UdpNetMessageHelper();
                            UdpClientService udp = service as UdpClientService;
                            udp.OnConnect += OnConnectHandler;
                            udp.OnDisconnect += OnDisconnectHandler;
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
        internal void Connect(INetworkService service)
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
        internal void Disconnect(bool notifyRemote = true)
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
        internal void RunHeartbeat(uint intervalSec, byte maxRecur)
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
            networkOnDisconnect?.Invoke();
        }
        void OnConnectHandler()
        {
            IsConnected = true;
            Utility.Debug.LogInfo("Network is connected ! ");
            networkOnConnect?.Invoke();
        }
    }
}
