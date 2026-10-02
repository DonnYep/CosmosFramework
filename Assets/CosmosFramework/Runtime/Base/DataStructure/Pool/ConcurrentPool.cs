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
        readonly object capacityLock = new object();
        int retainedCount;
        int pendingCount;
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
            T obj;
            bool hasObject;
            lock (capacityLock)
            {
                hasObject = objects.TryDequeue(out obj);
                if (hasObject && capacity > 0)
                    retainedCount--;
            }

            if (hasObject)
            {
                objectSpawn?.Invoke(obj);
                return obj;
            }
            else
            {
                var obj = objectGenerator();
                objectSpawn?.Invoke(obj);
                return obj;
            }
        }
        public void Despawn(T obj)
        {
            if (capacity == 0)
            {
                objectDespawn?.Invoke(obj);
                lock (capacityLock)
                {
                    objects.Enqueue(obj);
                }
                return;
            }

            bool hasCapacity;
            lock (capacityLock)
            {
                hasCapacity = retainedCount < capacity;
                if (hasCapacity)
                {
                    retainedCount++;
                    pendingCount++;
                }
            }

            if (!hasCapacity)
            {
                objectOverflow?.Invoke(obj);
                return;
            }

            try
            {
                objectDespawn?.Invoke(obj);
                lock (capacityLock)
                {
                    pendingCount--;
                    objects.Enqueue(obj);
                }
            }
            catch
            {
                lock (capacityLock)
                {
                    pendingCount--;
                    retainedCount--;
                }
                throw;
            }
        }
        public void Clear()
        {
            lock (capacityLock)
            {
                objects.Clear();
                retainedCount = pendingCount;
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
