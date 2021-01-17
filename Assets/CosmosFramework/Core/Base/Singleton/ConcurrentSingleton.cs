using System;
namespace Cosmos
{
    /// <summary>
    /// 多线程单例基类，内部包含线程锁;
    /// 可选实现IConstruction接口;
    /// <typeparam name="TDerived">继承自此单例的可构造类型</typeparam>
    public abstract class ConcurrentSingleton<TDerived> : IDisposable
              where TDerived : class, new()
    {
        protected static TDerived instance;
        static readonly object locker = new object();
        public static TDerived Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new TDerived();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        ///非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose()
        {
            instance = default(TDerived);
        }
    }
}
