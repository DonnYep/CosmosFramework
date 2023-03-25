using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteMVC.Core
{
    /// <summary>
    /// 数据层；
    /// </summary>
    public class Model
    {
        readonly static Dictionary<Type, Proxy> proxyDict = new Dictionary<Type, Proxy>();

        readonly object locker = new object();
        public int ProxyCount { get { return proxyDict.Count; } }

        /// <summary>
        /// 注册proxy；
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        /// <param name="proxy">对象</param>
        public void RegisterProxy<T>(T proxy)
            where T : Proxy
        {
            var type = typeof(T);
            lock (locker)
            {
                if (!proxyDict.ContainsKey(type))
                {
                    proxyDict.Add(type, proxy);
                    proxy.OnRegister();
                }
            }
        }
        /// <summary>
        /// 注销proxy；
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        public void DeregisterProxy<T>()
            where T : Proxy
        {
            var type = typeof(T);
            lock (locker)
            {
                if (proxyDict.ContainsKey(type))
                {
                    var model = proxyDict[type];
                    proxyDict.Remove(type);
                    model.OnDeregister();
                }
            }
        }
        /// <summary>
        /// 是否存在proxy类型
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        /// <returns>存在结果</returns>
        public bool HasProxy<T>()
            where T : Proxy
        {
            var type = typeof(T);
            lock (locker)
            {
                return proxyDict.ContainsKey(type);
            }
        }
        /// <summary>
        /// 获取一个proxy；
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        /// <param name="proxy">对象</param>
        /// <returns>返回结果</returns>
        public bool PeekProxy<T>(out T proxy)
            where T : Proxy
        {
            var type = typeof(T);
            proxy = default(T);
            lock (locker)
            {
                var rst = proxyDict.TryGetValue(type, out var m);
                if (rst)
                    proxy = (T)m;
                return rst;
            }
        }
        /// <summary>
        /// 注销所有proxy；
        /// </summary>
        public void DeregisterAllProxy()
        {
            var models = proxyDict.Values.ToList();
            proxyDict.Clear();
            foreach (var model in models)
            {
                model.OnDeregister();
            }
        }
    }
}
