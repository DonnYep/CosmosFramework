using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// Peer对象接口；
    /// </summary>
    public interface IRemotePeer : IReference
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        long Conv { get; }
        /// <summary>
        /// 是否存活；
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// 对当前的peer对象发送消息
        /// </summary>
        /// <param name="netMsg">消息体</param>
        void SendMessage(INetworkMessage netMsg);
        /// <summary>
        /// 终断连接
        /// </summary>
        //void AbortConnection();
    }
}
