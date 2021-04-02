using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 网络模块定义的常量；
    /// </summary>
    public class NetworkConsts
    {
        /// <summary>
        /// 秒级别；
        /// 1代表1秒；
        /// </summary>
        public const byte MaxRecurCount = 3;
        /// <summary>
        /// 最大失效次数
        /// </summary>
        public const uint  HeartbeatInterval = 5;
    }
}
