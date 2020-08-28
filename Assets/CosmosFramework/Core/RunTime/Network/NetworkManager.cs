using System.Collections;
using Cosmos;
using System.Net;
using System.Net.Sockets;
namespace Cosmos.Network
{
    //TODO NetworkManager  这里使用了停等ARQ协议，目前未实现包组发送
    /// <summary>
    /// 此模块为客户端网络管理类
    /// </summary>
    internal sealed class NetworkManager : Module<NetworkManager>
    {
        string serverIP;
        int serverPort;
        string clientIP;
        int clientPort;
        INetworkService service;
        IPEndPoint serverEndPoint;
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
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            service?.OnRefresh();
        }
        internal void SendNetworkMessage(INetworkMessage netMsg, IPEndPoint endPoint)
        {
            if (service != null)
            {
                if (service.Available)
                    service.SendMessage(netMsg,endPoint);
                else
                    Utility.Debug.LogError($"当前无网络连接");
            }
            else
                Utility.Debug.LogError($"当前网络未初始化");
        }
        internal void SendNetworkMessage(INetworkMessage netMsg)
        {
            if (service != null)
            {
                if (service.Available)
                    service.SendMessage(netMsg);
                else
                    Utility.Debug.LogError($"当前无网络连接");
            }
            else
                Utility.Debug.LogError($"当前网络未初始化");
        }
        /// <summary>
        /// 与远程建立连接；
        /// 当前只有udp
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="protocolType">协议类型</param>
        internal void Connect(string ip,int port,ProtocolType protocolType)
        {
            OnUnPause();
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
                            UdpClientService udp = service as UdpClientService;
                            udp.IP = ip;
                            udp.Port = port;
                        }
                        service.OnActive();
                        Utility.Debug.LogInfo("建立网络连接");
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
                Utility.Debug.LogError("Service Empty");
                return;
            }
            OnUnPause();
            this.service = service;
            service.OnActive();
            Utility.Debug.LogInfo("建立网络连接");
        }
        internal void Disconnect()
        {
            OnPause();
            if (service == null)
            {
                Utility.Debug.LogError("Service 无服务");
                return;
            }
            service.OnDeactive();
            Utility.Debug.LogError("关闭网络,停用Servce");
        }
    }
}
