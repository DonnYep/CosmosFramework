using System;
namespace Cosmos
{
    /// <summary>
    /// 通用继承形单例
    /// </summary>
    /// <typeparam name="T">继承自此单例的可构造类型</typeparam>
    public abstract class Singleton<T> : IDisposable
        where T : Singleton<T>, new()
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
