using System;
using System.Text;
namespace Cosmos.Editor
{
    public static partial class EditorUtil
    {
        public static class Json
        {
            /// <summary>
            /// 将对象序列化为JSON字段
            /// </summary>
            /// <param name="obj">需要被序列化的对象</param>
            /// <returns>序列化后的JSON字符串</returns>xxxx
            public static string ToJson(object obj, bool prettyPrint = false)
            {
                return Cosmos.LitJson.JsonMapper.ToJson(obj);
            }
            /// <summary>
            /// 将对象序列化为JSON流
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <returns>序列化后的JSON流</returns>
            public static byte[] ToJsonData(object obj)
            {
                return Encoding.UTF8.GetBytes(ToJson(obj));
            }
            /// <summary>
            /// 将JSON反序列化为泛型对象
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="json">需要反序列化的JSON字符串</param>
            /// <returns>反序列化后的泛型对象</returns>
            public static T ToObject<T>(string json)
            {
                return Cosmos.LitJson.JsonMapper.ToObject<T>(json);
            }
            /// <summary>
            /// 将JSON字符串反序列化对象
            /// </summary>
            /// <param name="objectType">对象类型</param>
            /// <param name="json">需要反序列化的JSON字符串</param>
            /// <returns>反序列化后的对象</returns>
            public static object ToObject(string json, Type objectType)
            {
                return Cosmos.LitJson.JsonMapper.ToObject(json, objectType);
            }
            /// <summary>
            /// 将JSON流转换为对象
            /// </summary>
            /// <typeparam name="T">目标类型</typeparam>
            /// <param name="jsonData">JSON流</param>
            /// <returns>反序列化后的对象</returns>
            public static T ToObject<T>(byte[] jsonData)
            {
                return ToObject<T>(Encoding.UTF8.GetString(jsonData));
            }
        }
    }
}