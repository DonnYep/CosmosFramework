using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    public class Model:ConcurrentSingleton<Model>
    {
        protected Dictionary<string, Proxy> proxyDict;

        protected readonly object locker= new object();
        public virtual bool HasProxy(string proxyName)
        {
            lock (locker)
            {
                return proxyDict.ContainsKey(proxyName);
            }
        }
        /// <summary>
        /// 获取一个数据代理对象；
        /// </summary>
        /// <param name="proxyName">代理名</param>
        /// <returns>数据的代理对对象</returns>
        public virtual Proxy PeekProxy(string proxyName)
        {
            lock (locker)
            {
                proxyDict.TryGetValue(proxyName, out var proxy);
                return proxy;
            }
        }
        public virtual void RegisterProxy(Proxy proxy)
        {
            lock (locker)
            {
                proxyDict[proxy.ProxyName] = proxy;
            }
            proxy.OnRegister();
        }
        public virtual void RemoveProxy(string proxyName,out Proxy proxy)
        {
            lock (locker)
            {
                proxyDict.Remove(proxyName, out proxy);
            }
            proxy?.OnRemove();
        }
        public Model()
        {
            proxyDict = new Dictionary<string, Proxy>();

            OnInitialization();
        }

        protected virtual void OnInitialization() { }
    }
}