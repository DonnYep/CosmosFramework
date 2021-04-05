using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 心跳接口
    /// </summary>
    public interface IHeartbeat : IRefreshable, IRenewable, IOperable, IReference
    {
        long Conv { get; set; }
        /// <summary>
        /// 秒级别；
        /// 1代表1秒；
        /// </summary>
        uint HeartbeatInterval { get; set; }
        /// <summary>
        /// 秒级别；
        /// 上一次心跳时间；
        /// </summary>
        long LatestHeartbeatTime { get; }
        /// <summary>
        /// 最大重传次数
        /// </summary>
        byte MaxRecurCount { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// 失活时触发的委托；
        /// </summary>
        Action UnavailableHandler { get; set; }
        /// <summary>
        /// 发送心跳的委托
        /// </summary>
        Action<INetworkMessage> SendHeartbeatHandler { get; set; }
    }
}
