using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 框架预留操作码；
    /// 101~150预留给框架，剩下的码皆可自定义
    /// </summary>
    public class NetworkOpCode
    {
        public static readonly ushort _Heartbeat= 101;
    }
}
