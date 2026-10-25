using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
namespace Cosmos
{
    /// <summary>
    ///泛型池对象，线程安全； 
    /// </summary>
    public class ConcurrentPool<T> : IEnumerable
        where T : class
    {
        public int Count { get { return objects.Count; } }
        readonly ConcurrentQueue<T> objects = new ConcurrentQueue<T>();
        readonly Func<T> objectGenerator;
        readonly Action<T> objectDespawn;
        readonly Action<T> objectOverflow;
        readonly Action<T> objectSpawn;
        readonly int capacity = 0;
        readonly object countLock = new object();
        //池内数量 + 在途（已占位、尚未入队）数量
        int totalCount;
        int reservedCount;
        public ConcurrentPool(int capacity, Func<T> objectGenerator, Action<T> objectSpawn, Action<T> objectDesapwn, Action<T> objectOverflow)
        {
            this.objectGenerator = objectGenerator;
            this.objectDespawn = objectDesapwn;
            this.objectOverflow = objectOverflow;
            this.objectSpawn = objectSpawn;
            this.capacity = capacity;
        }
        public ConcurrentPool(Func<T> objectGenerator, Action<T> objectSpawn, Action<T> objectDesapwn) : this(0, objectGenerator, objectSpawn, objectDesapwn, null) { }
        public ConcurrentPool(Func<T> objectGenerator, Action<T> objectDesapwn) : this(0, objectGenerator, null, objectDesapwn, null) { }
        public ConcurrentPool(Func<T> objectGenerator) : this(0, objectGenerator, null, null, null) { }
        public T Spawn()
        {
            //以 TryDequeue 的结果为准，避免并发下 Count 检查通过但队列已空而返回 null
            T obj;
            if (objects.TryDequeue(out obj))
            {
                if (capacity > 0)
                {
                    lock (countLock)
                    {
                        if (totalCount > 0)
                            totalCount--;
                    }
                }
                objectSpawn?.Invoke(obj);
                return obj;
            }
            else
            {
                obj = objectGenerator();
                objectSpawn?.Invoke(obj);
                return obj;
            }
        }
        public void Despawn(T obj)
        {
            if (obj == null)
                return;
            if (capacity <= 0)
            {
                objectDespawn?.Invoke(obj);
                objects.Enqueue(obj);
                return;
            }

            bool reserved = false;
            lock (countLock)
            {
                if (totalCount < capacity)
                {
                    totalCount++;
                    reservedCount++;
                    reserved = true;
                }
            }
            if (!reserved)
            {
                objectOverflow?.Invoke(obj);
                return;
            }

            try
            {
                objectDespawn?.Invoke(obj);
            }
            catch
            {
                ReleaseReservation();
                throw;
            }

            objects.Enqueue(obj);
            lock (countLock)
            {
                //已入队：占位由“在途”转为“池内”
                reservedCount--;
            }
        }
        void ReleaseReservation()
        {
            lock (countLock)
            {
                reservedCount--;
                totalCount--;
            }
        }
        public void Clear()
        {
            Clear(null);
        }
        /// <summary>
        /// 清空池；
        /// </summary>
        /// <param name="release">逐个释放池内对象；传 null 表示只丢弃引用</param>
        public void Clear(Action<T> release)
        {
            T obj;
            while (objects.TryDequeue(out obj))
            {
                release?.Invoke(obj);
            }
            if (capacity > 0)
            {
                lock (countLock)
                {
                    //保留仍在途的占位，池内对象已清空
                    totalCount = reservedCount;
                }
            }
        }
        /// <summary>
        /// 池内对象数 + 在途占位数（容量语义的总量）；
        /// <para><see cref="Count"/> 只表示池内可复用对象数。</para>
        /// </summary>
        public int TotalCount
        {
            get
            {
                if (capacity <= 0)
                    return objects.Count;
                lock (countLock)
                    return totalCount;
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
