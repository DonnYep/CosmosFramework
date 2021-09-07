using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;

/// <summary>
/// 在有MessagePack引入时，取消注释即可使用；
/// </summary>
namespace Cosmos
{
    //using MessagePack;
    //using MessagePack.Resolvers;
    //public class ImplMessagePackHelper : Utility.MessagePack.IMessagePackHelper
    //{
    //    public ImplMessagePackHelper()
    //    {
    //        StaticCompositeResolver.Instance.Register(
    //                    //GeneratedResolver.Instance,
    //                    StandardResolver.Instance
    //               );
    //        var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
    //        MessagePackSerializer.DefaultOptions = option;
    //    }
    //    /// <summary>
    //    /// 序列化对象到二进制；
    //    /// </summary>
    //    /// <typeparam name="T">mp标记的对象类型</typeparam>
    //    /// <param name="obj">mp对象</param>
    //    /// <returns>序列化后的对象</returns>
    //    public byte[] Serialize<T>(T obj)
    //    {
    //        return MessagePackSerializer.Serialize<T>(obj);
    //    }
    //    /// <summary>
    //    /// 反序列化二进制到对象；
    //    /// </summary>
    //    /// <typeparam name="T">mp标记的对象类型</typeparam>
    //    /// <param name="bytes">需要反序列化的数组</param>
    //    /// <returns>反序列化后的对象</returns>
    //    public T Deserialize<T>(byte[] bytes)
    //    {
    //        return MessagePackSerializer.Deserialize<T>(bytes);
    //    }
    //    /// <summary>
    //    /// 反序列化二进制到对象；
    //    /// </summary>
    //    /// <param name="bytes">需要反序列化的数组</param>
    //    /// <param name="type">mp标记的对象类型</param>
    //    /// <returns>反序列化后的对象</returns>
    //    public object Deserialize(byte[] bytes, Type type)
    //    {
    //        return MessagePackSerializer.Deserialize(type, bytes);
    //    }
    //    /// <summary>
    //    /// byte[]转json字符串；
    //    /// </summary>
    //    /// <param name="jsonBytes">需要被转换成json的byte数组</param>
    //    /// <returns>转换后的json</returns>
    //    public string BytesToJson(byte[] jsonBytes)
    //    {
    //        return MessagePackSerializer.ConvertToJson(jsonBytes);
    //    }
    //    /// <summary>
    //    /// json字符串转byte[]；
    //    /// </summary>
    //    /// <param name="json">需要被转换成bytes的json</param>
    //    /// <returns>转换后的bytes</returns>
    //    public byte[] JsonToBytes(string json)
    //    {
    //        return MessagePackSerializer.ConvertFromJson(json);
    //    }
    //    /// <summary>
    //    /// json字符串反序列化成对象；
    //    /// </summary>
    //    /// <typeparam name="T">mp标记的对象类型</typeparam>
    //    /// <param name="json">需要被转换的json</param>
    //    /// <returns>反序列化后的对象</returns>
    //    public T DeserializeJson<T>(string json)
    //    {
    //        return Deserialize<T>(JsonToBytes(json));
    //    }
    //    /// <summary>
    //    /// json字符串反序列化成对象；
    //    /// </summary>
    //    /// <param name="json">需要被转换的json</param>
    //    /// <param name="type">mp标记的对象类型</param>
    //    /// <returns>反序列化后的对象</returns>
    //    public object DeserializeJson(string json, Type type)
    //    {
    //        return Deserialize(JsonToBytes(json), type);
    //    }
    //    /// <summary>
    //    /// 对象序列化成json字符串；
    //    /// </summary>
    //    /// <typeparam name="T">mp标记的对象类型</typeparam>
    //    /// <param name="obj">mp对象</param>
    //    /// <returns>转换后的json<</returns>
    //    public string SerializeToJson<T>(T obj)
    //    {
    //        return MessagePackSerializer.SerializeToJson<T>(obj);
    //    }
    //    /// <summary>
    //    /// 对象序列化成json的bytes；
    //    /// </summary>
    //    /// <typeparam name="T">mp标记的对象类型</typeparam>
    //    /// <param name="obj">mp对象</param>
    //    /// <returns>转换后的bytes</returns>
    //    public byte[] SerializeToJsonBytes<T>(T obj)
    //    {
    //        return JsonToBytes(SerializeToJson<T>(obj));
    //    }
    //}
}