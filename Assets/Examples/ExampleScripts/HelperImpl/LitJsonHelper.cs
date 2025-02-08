using System;
namespace Cosmos.Test
{
    [Implementer]
    public class LitJsonHelper : Utility.Json.IJsonHelper
    {
        public string ToJson(object obj)
        {
            return LitJson.JsonMapper.ToJson(obj);
        }
        public string ToJson(object obj, bool prettyPrint)
        {
            return LitJson.JsonMapper.ToJson(obj);
        }
        public T ToObject<T>(string json)
        {
            return LitJson.JsonMapper.ToObject< T>(json);
        }
        public object ToObject(string json, Type objectType)
        {
            return LitJson.JsonMapper.ToObject(json, objectType);
        }
    }
}