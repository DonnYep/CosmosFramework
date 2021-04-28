using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
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
            if (objects.Count > 0)
            {
                objects.TryDequeue(out var obj);
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
            if (capacity == 0 || objects.Count < capacity)
            {
                objectDespawn?.Invoke(obj);
                objects.Enqueue(obj);
            }
            else
            {
                objectOverflow?.Invoke(obj);
            }
        }
        public void Clear()
        {
            objects.Clear();
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
