using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 框架内部操作码；
    /// </summary>
    internal class InnerOpCode
    {
        public static readonly ushort _Heartbeat = 65534;
        public static readonly byte _PeerConnect= 1;
        public static readonly  byte _PeerDisconnect= 2;
    }
}
