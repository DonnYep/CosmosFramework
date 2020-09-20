//***********************************************************
// 描述：
// 作者：  
// 创建时间：2020-09-12 13:55:58
// 版 本：1.0
//***********************************************************
using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

public interface IMessagePacketSerializeHelper
{
    byte[] Serialize(MessagePacket msgPack);
    MessagePacket Deserialize(byte[] data);
}
