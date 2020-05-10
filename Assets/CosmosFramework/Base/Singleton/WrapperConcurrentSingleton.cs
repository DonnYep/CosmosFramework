using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 多线程--容器形单例；
    /// 被单例的T类型直接作为泛型参数存在与内存中。继承 IBehaviour接口后，自动初始化与释放。
    /// </summary>
    /// <typeparam name="T">须为继承自 IBehaviour的可构造类对象</typeparam>
    public class WrapperConcurrentSingleton<T>:IDisposable
               where T : class, IBehaviour, new()
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
    }
}
