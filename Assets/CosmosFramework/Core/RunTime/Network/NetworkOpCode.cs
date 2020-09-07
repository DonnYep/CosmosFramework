using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 框架预留操作码；
    /// 0~150预留给框架;
    /// 其中0~50给服务器，51~100客户端，101~150前后端通用
    /// </summary>
    public class NetworkOpCode
    {
        public static readonly ushort _Heartbeat= 101;
    }
}
