/// <summary>
/// 数据代理类
/// </summary>
namespace PureMVC
{
    public abstract class Proxy
    {
        protected string proxyName;
        public const string NAME = "Proxy";
        public virtual string ProxyName { get { return proxyName; } }
        public object Data { get; set; }
        public Proxy(string proxyName) : this(proxyName, null) { }
        public Proxy(string proxyName, object data)
        {
            this.proxyName = string.IsNullOrEmpty(proxyName) ? NAME : proxyName;
            Data = data;
        }
        public virtual void OnRegister() { }
        public virtual void OnRemove() { }
    }
}