using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Network
{
    public interface INetworkClientChannel : INetworkChannel
    {
        /// <summary>
        /// 建立连接回调；
        /// </summary>
        event Action OnConnected;
        /// <summary>
        /// 接收数据回调；
        /// </summary>
        event Action<byte[]> OnDataReceived;
        /// <summary>
        /// 断开连接回调；
        /// </summary>
        event Action OnDisconnected;
        string IPAddress { get; }
        /// <summary>
        /// client是否连接成功；
        /// </summary>
        bool IsConnect { get; }
        void Connect(string ip, int port);
        bool SendMessage(byte[] data);
        void Disconnect();
    }
}
