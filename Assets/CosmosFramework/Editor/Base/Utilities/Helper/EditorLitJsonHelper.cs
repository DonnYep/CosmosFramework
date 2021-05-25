using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.Text;
namespace Cosmos.CosmosEditor
{
    [ImplementProvider]
    public class EditorLitJsonHelper : IEditorJsonHelper
    {
        StringBuilder stringBuilder = new StringBuilder();
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
            return LitJson.JsonMapper.ToObject<T>(json);
        }
        public object ToObject(string json, Type objectType)
        {
            return LitJson.JsonMapper.ToObject(json, objectType);
        }
    }
}