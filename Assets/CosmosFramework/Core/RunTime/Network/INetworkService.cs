using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 网络服务接口
    /// </summary>
    public interface INetworkService:IOperable,IRefreshable
    {
        /// <summary>
        /// 网络服务是否可用
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// 发送网络消息;
        /// </summary>
        /// <param name="netMsg">网络消息数据对象</param>
        /// <param name="endPoint">远程对象</param>
        void SendMessage(INetworkMessage netMsg, IPEndPoint endPoint);
        /// <summary>
        /// 发送网络消息;
        /// </summary>
        /// <param name="netMsg">网络消息数据对象</param>
        void SendMessage(INetworkMessage netMsg);
        /// <summary>
        /// 接收网络消息
        /// </summary>
        void OnReceive();
    }
}
