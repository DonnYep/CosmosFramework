using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

public interface IMessagePacketSerializeHelper
{
    byte[] Serialize(MessagePacket msgPack);
    MessagePacket Deserialize(byte[] data);
}
