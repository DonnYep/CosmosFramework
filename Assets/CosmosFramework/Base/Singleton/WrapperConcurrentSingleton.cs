using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cosmos
{
    /// <summary>
    /// 多线程--容器形单例；
    /// 被单例的T类型直接作为泛型参数存在与内存中。继承 IBehaviour接口后，自动初始化与释放。
    /// </summary>
    /// <typeparam name="T">须为继承自 IBehaviour的可构造类对象</typeparam>
    public class WrapperConcurrentSingleton<T> : IDisposable
               where T : class, new()
    {
        //读写锁，支持单个写线程和多个读线程。
        static ReaderWriterLockSlim readWritelocker = new ReaderWriterLockSlim();
        protected static T instance;
        static readonly object locker = new object();
        public static T Instance
        {
            get
            {
                readWritelocker.EnterReadLock();
                if (instance == null)
                {
                    instance = new T();
                    if (instance is IBehaviour)
                        (instance as IBehaviour).OnInitialization();
                }
                readWritelocker.ExitReadLock();
                return instance;
            }
        }
        /// <summary>
        ///非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose()
        {
            if (instance is IBehaviour)
                (instance as IBehaviour).OnTermination();
            instance = default(T);
        }
    }
}
