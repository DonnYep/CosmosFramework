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
        event Action OnConnect;
        event Action OnDisconnect;
        event Action<byte[]> OnReceiveData;
        //long Conv { get; }
        bool IsConnected { get; }
        void SendNetworkMessage(byte[] buffer);
        /// <summary>
        /// 与远程建立连接；
        /// 当前只有udp
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="protocolType">协议类型</param>
        void Connect(string ip, ushort port, NetworkProtocolType protocolType= NetworkProtocolType.KCP);
        /// <summary>
        /// 断开网络链接；
        /// </summary>
        /// <param name="clearCallbackWhenDisconnected">是否在断开连接后清空回调的监听</param>
        void Disconnect(bool clearCallbackWhenDisconnected = false);
    }
}
