using Cosmos.Resource;
using System;
using System.Collections.Generic;

namespace Cosmos.Data
{
    [Module]
    internal sealed class DataManager : Module,IDataManager
    {
        #region Properties
        #endregion
        #region Methods
        /// <summary>
        /// 对象字典；
        /// </summary>
        Dictionary<Type, object> typeObjectDict;
        /// <summary>
        /// json数据字典；
        /// </summary>
        Dictionary<string, string> jsonDict;
        public override void OnActive()
        {
            var objs = Utility.Assembly.GetInstancesByAttribute<ImplementProviderAttribute, IDataProvider>();
            for (int i = 0; i < objs.Length; i++)
            {
                try 
                {
                    objs[i]?.LoadData();
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
        }
        public bool ContainsDataObject(Type key)
        {
            return typeObjectDict.ContainsKey(key);
        }
        public bool AddDataObject(Type key, object value)
        {
            return typeObjectDict. TryAdd(key, value);
        }
        public bool GetDataObject(Type key, out object value)
        {
            return typeObjectDict. TryGetValue(key, out value);
        }
        public bool RemoveDataObject(Type key)
        {
            return typeObjectDict.Remove(key);
        }
        public bool RemoveDataObject(Type key, out object value)
        {
            return typeObjectDict.Remove(key, out value);
        }
        public bool ContainsDataObject<T>()
            where T : class
        {
            return ContainsDataObject(typeof(T));
        }
        public bool AddDataObject<T>(T value)
            where T : class
        {
            return AddDataObject(typeof(T), value);
        }
        public bool GetDataObject<T>(out T value)
            where T : class
        {
            value = default;
            object data;
            var result = GetDataObject(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool RemoveDataObject<T>()
            where T : class
        {
            return RemoveDataObject(typeof(T));
        }
        public bool RemoveDataObject<T>(out T value)
            where T : class
        {
            value = default;
            object data;
            var result = RemoveDataObject(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool AddDataString(string key, string value)
        {
            return jsonDict. TryAdd(key, value);
        }
        /// <summary>
        /// 通过类名获取json数据；
        /// typeof(Data).Name可作为key；
        /// </summary>
        /// <param name="key">类名</param>
        /// <param name="value">json数据</param>
        /// <returns>是否获取成功</returns>
        public bool GetDataString(string key, out string value)
        {
            return jsonDict. TryGetValue(key, out  value);
        }
        public bool RemoveDataString(string key)
        {
            return jsonDict.Remove(key);
        }
        public bool RemoveDataString(string key, out string value)
        {
            return jsonDict.Remove(key, out value);
        }
        public bool ContainsDataString(string key)
        {
            return jsonDict.ContainsKey(key);
        }
        public void ClearAll()
        {
            jsonDict.Clear();
            typeObjectDict.Clear();
        }
        #endregion
    }
}