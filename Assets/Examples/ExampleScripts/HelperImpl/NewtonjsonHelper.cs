using System.Collections;
using System.Collections.Generic;
using Cosmos;
using System;
using Newtonsoft.Json;
namespace Cosmos.Test
{
    [Implementer]
    public class NewtonjsonHelper : Utility.Json.IJsonHelper
    {
        public string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public string ToJson(object obj, bool prettyPrint)
        {
            return JsonConvert.SerializeObject(obj,Formatting.Indented);
        }
        public T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public object ToObject(string json, Type objectType)
        {
            return JsonConvert.DeserializeObject(json, objectType);
        }
    }
}