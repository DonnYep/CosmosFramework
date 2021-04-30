using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// Json辅助类接口，用于适配各类JSON解决方案
    /// </summary>
    public interface IJsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON字段
        /// </summary>
        /// <param name="obj">需要被序列化的对象</param>
        /// <returns>序列化后的JSON字符串</returns>
        string ToJson(object obj, bool prettyPrint);
        /// <summary>
        /// 将JSON字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="json">需要被反序列化的JSON字符串</param>
        /// <returns>反序列化后的对象</returns>
        T ToObject<T>(string json);
        /// <summary>
        /// 将JSON字符串反序列化为对象
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="json">需要被反序列化的JSON字符串</param>
        /// <returns>反序列化后的对象</returns>
        object ToObject(string json, Type objectType);
    }
}
