using System.Collections;
using System;
namespace Cosmos
{
    public sealed partial class Utility
    {
        public static class Json
        {
            static IJsonWrapper jsonWarpper = null;
            public static void SetJsonWarpper(IJsonWrapper warpper)
            {
                jsonWarpper = warpper;
            }
            /// <summary>
            /// 将对象序列化为JSON字段
            /// </summary>
            /// <param name="obj">需要被序列化的对象</param>
            /// <returns>序列化后的JSON字符串</returns>xxxx
            public static string ToJson(object obj)
            {
                if (jsonWarpper == null)
                {
                    throw new ArgumentNullException("Utility.Json :JSON  warpper is invalid");
                }
                try
                {
                    return jsonWarpper.ToJson(obj);
                }
                catch
                {
                    throw new InvalidOperationException(Utility.Text.Format("Utility.Json : Can not convert to JSON with exception"));
                }
            }
            /// <summary>
            /// 将对象序列化为JSON流
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <returns>序列化后的JSON流</returns>
            public static byte[] ToJsonData(object obj)
            {
                return Utility.Converter.GetBytes(ToJson(obj));
            }
            /// <summary>
            /// 将JSON反序列化为泛型对象
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="json">需要反序列化的JSON字符串</param>
            /// <returns>反序列化后的泛型对象</returns>
            public static T ToObject<T>(string json)
            {
                if (jsonWarpper == null)
                {
                    throw new ArgumentNullException(" Utility.Json : JSON warpper is invalid");
                }
                try
                {
                    return jsonWarpper.ToObject<T>(json);
                }
                catch 
                {
                    throw new InvalidOperationException(Utility.Text.Format(" Utility.Json : Can not convert to JSON "));
                }
            }
            /// <summary>
            /// 将JSON字符串反序列化对象
            /// </summary>
            /// <param name="objectType">对象类型</param>
            /// <param name="json">需要反序列化的JSON字符串</param>
            /// <returns>反序列化后的对象</returns>
            public static object ToObject( string json,Type objectType)
            {
                if (jsonWarpper == null)
                {
                    throw new ArgumentNullException("Utility.Json : JSON warpper is invalid");
                }
                if (objectType == null)
                {
                    throw new ArgumentNullException("Utility.Json : Object type is invalid");
                }
                try
                {
                    return jsonWarpper.ToObject(json, objectType);
                }
                catch
                {
                    throw new InvalidOperationException(Utility.Text.Format("Utility.Json : Can not convert to JSON"));
                }
            }
            /// <summary>
            /// 将JSON流转换为对象
            /// </summary>
            /// <typeparam name="T">目标类型</typeparam>
            /// <param name="jsonData">JSON流</param>
            /// <returns>反序列化后的对象</returns>
            public static T ToObject<T>(byte[] jsonData)
            {
                return ToObject<T>(Utility.Converter.GetString(jsonData));
            }
        }
    }
}