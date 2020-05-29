using System;
namespace Cosmos
{
    /// <summary>
    /// 容器形单例；
    /// 被单例的T类型直接作为泛型参数存在与内存中。继承 IBehaviour接口后，自动初始化与释放。
    /// </summary>
    /// <typeparam name="T">须为继承自 IBehaviour的可构造类对象</typeparam>
    public class WrapperSingleton<T> : IDisposable
        where T : class,  new()
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    if (instance is IBehaviour)
                        (instance as IBehaviour).OnInitialization();
                }
                return instance;
            }
        }
        /// <summary>
        /// 非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose()
        {
            if (instance is IBehaviour)
                (instance as IBehaviour).OnTermination();
            instance = default(T);
        }
    }
}
