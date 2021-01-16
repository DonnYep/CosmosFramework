using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 数据代理类
/// </summary>
namespace Cosmos.Mvvm
{
    public abstract class Proxy
    {
        public string ProxyName { get; protected set; }
        public object Data { get; set; }
        public Proxy() { }
        public Proxy(string proxyName):this(proxyName,null){}
        public Proxy(string proxyName,object data)
        {
            ProxyName = proxyName;
            Data = data;
        }
        public abstract void OnRegister();
        public abstract void OnRemove();

    }
}