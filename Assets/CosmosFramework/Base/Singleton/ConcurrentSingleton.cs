using System;
namespace Cosmos
{
    /// <summary>
    /// 多线程单例基类，内部包含线程锁
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConcurrentSingleton<T> : IDisposable
              where T : ConcurrentSingleton<T>, new()
    {
        protected static T instance;
        static readonly object locker = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                            instance.OnInitialization();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        ///非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose() { instance.OnTermination(); instance = default(T); }
        /// <summary>
        //空的虚方法，在当前单例对象为空初始化时执行一次
        /// </summary>
        protected virtual void OnInitialization() { }
        /// <summary>
        //空的虚方法，在当前单例对象被销毁时执行一次
        /// </summary>
        protected virtual void OnTermination() { }
    }
}
