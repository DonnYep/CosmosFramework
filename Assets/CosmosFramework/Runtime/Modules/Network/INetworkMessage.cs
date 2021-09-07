using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// 网络消息接口
    /// </summary>
    public interface INetworkMessage
    {
        long Conv { get; }
        byte[] EncodeMessage();
        bool DecodeMessage(byte[] buffer);
        byte[] ServiceData { get; }
        byte[] GetBuffer();
    }
}