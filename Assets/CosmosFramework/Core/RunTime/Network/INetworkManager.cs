using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface INetworkManager: IModuleManager
    {
        event Action NetworkOnConnect;
        event Action NetworkOnDisconnect;
        long Conv { get; }
        bool IsConnected { get; }
        IPEndPoint ServerEndPoint { get; }
        IPEndPoint ClientEndPoint { get; }
        void SendNetworkMessage(INetworkMessage netMsg, IPEndPoint endPoint);
        void SendNetworkMessage(INetworkMessage netMsg);
        void SendNetworkMessage(byte[] buffer);
        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="message">序列化后的数据</param>
        void SendNetworkMessage(ushort opCode, byte[] message);
        /// <summary>
        /// 与远程建立连接；
        /// 当前只有udp
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="protocolType">协议类型</param>
        void Connect(string ip, int port, ProtocolType protocolType);
        /// <summary>
        /// 与远程建立连接；
        /// </summary>
        /// <param name="service">自定义实现的服务</param>
        void Connect(INetworkService service);
        void Disconnect(bool notifyRemote = true);
        void RunHeartbeat(uint intervalSec, byte maxRecur);
    }
}
