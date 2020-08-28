using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    ///客户端；
    ///网络并发事件Core，Key为ushort的码，值为INetworkMessage
    /// </summary>
    public class NetworkEventCore:EventCore<ushort,INetworkMessage,NetworkEventCore>
    {

    }
}
