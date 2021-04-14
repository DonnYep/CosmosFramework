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
    {
        public int Count { get { return objects.Count; } }
        readonly ConcurrentQueue<T> objects = new ConcurrentQueue<T>();
        readonly Func<T> objectGenerator;
        readonly Action<T> objectDispose;
        public ConcurrentPool(Func<T> objectGenerator, Action<T> objectDispose = null)
        {
            this.objectGenerator = objectGenerator;
            this.objectDispose = objectDispose;
        }
        public T Spawn()
        {
            if (objects.Count > 0)
            {
                objects.TryDequeue(out var obj);
                return obj; ;
            }
            else
                return objectGenerator();
        }
        public void Despawn(T obj)
        {
            objectDispose?.Invoke(obj);
            objects.Enqueue(obj);
        }
        public IEnumerator GetEnumerator()
        {
            return objects.GetEnumerator();
        }
        public void Clear()
        {
            objects.Clear();
        }
    }
}
