using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Data
{
    public interface IDataManager :IModuleManager
    {
        bool ContainsDataObject(Type key);
        bool AddDataObject(Type key, object value);
        bool GetDataObject(Type key, out object value);
        bool RemoveDataObject(Type key);
        bool RemoveDataObject(Type key, out object value);
        bool ContainsDataObject<T>() where T : class;
        bool AddDataObject<T>(T value) where T : class;
        bool GetDataObject<T>(out T value) where T : class;
        bool RemoveDataObject<T>() where T : class;
        bool RemoveDataObject<T>(out T value) where T : class;
        bool AddDataString(string key, string value);
        /// <summary>
        /// 通过类名获取json数据；
        /// typeof(Data).Name可作为key；
        /// </summary>
        /// <param name="key">类名</param>
        /// <param name="value">json数据</param>
        /// <returns>是否获取成功</returns>
        bool GetDataString(string key, out string value);
        bool RemoveDataString(string key);
        bool RemoveDataString(string key, out string value);
        bool ContainsDataString(string key);
        void ClearAll();
    }
}
