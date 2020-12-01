using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    public class JsonUtilityHelper : IJsonHelper
    {
        public string ToJson(object obj, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public object ToObject(string json, Type objectType)
        {
            return JsonUtility.FromJson(json, objectType);
        }
    }
}
