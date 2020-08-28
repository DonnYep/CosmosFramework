using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// 网络消息接口
    /// </summary>
    public interface INetworkMessage
    {
        uint Conv { get; }
        byte[] EncodeMessage();
        void DecodeMessage(byte[] buffer);
        byte[] GetBuffer();
    }
}