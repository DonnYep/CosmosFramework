using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    public interface IMessagePackHelper
    {
        string ToJson<T>(T obj);
        byte[] ToByteArray<T>(T obj);
        T ToObject<T>(byte[] buffer);
        T ToObject<T>(string json);
        object ToObject(byte[] buffer, Type objectType);
        object ToObject(string json, Type objectType);
    }
}
