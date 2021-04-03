using Cosmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MessagePacketJsonHelper : IMessagePacketSerializeHelper
{
    public MessagePacket Deserialize(byte[] data)
    {
        MessagePacket mp = new MessagePacket();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {

                mp.OperationCode = reader.ReadByte();
                mp.ReturnCode = reader.ReadInt16();
                try
                {
                    string json = reader.ReadString();
                    mp.Messages = Utility.Json.ToObject<Dictionary<byte, object>>(json);
                }
                catch (Exception e)
                {
                    //仅仅打印Error消息
                    Utility.Debug.LogError ($"MessagePacket 's  Parameters is empty : {e}");
                }
                return mp;
            }
        }
    }
    public byte[] Serialize(MessagePacket msgPack)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(msgPack.OperationCode);
                writer.Write(msgPack.ReturnCode);
                if (msgPack.Messages != null)
                {
                    string json = Utility.Json.ToJson(msgPack.Messages);
                    writer.Write(json);
                }
            }
            return ms.ToArray();
        }
    }
}
