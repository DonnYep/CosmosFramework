using System;
namespace PureMVC
{
    public class MVCSingleton<TDerived> : IDisposable
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
