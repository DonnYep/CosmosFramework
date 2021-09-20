using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.Network
{
    /// <summary>
    /// 网络消息发送类型；
    /// </summary>
    public enum NetworkReliableType:byte
    {
        Reliable = 0x01,
        Unreliable = 0x02
    }
}
