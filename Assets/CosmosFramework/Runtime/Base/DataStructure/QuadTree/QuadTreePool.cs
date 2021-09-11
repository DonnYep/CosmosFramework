using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.QuadTree
{
    public class QuadTreePool<T> : IEnumerable<T>
        where T : class
    {
        public int Count { get { return objects.Count; } }
        readonly Queue<T> objects = new Queue<T>();
        readonly Func<T> objectGenerator;
        readonly Action<T> objectDespawn;
        public QuadTreePool(Func<T> objectGenerator, Action<T> objectDesapwn)
        {
            this.objectGenerator = objectGenerator;
            this.objectDespawn = objectDesapwn;
        }
        public T Spawn()
        {
            if (objects.Count > 0)
            {
                var obj = objects.Dequeue();
                return obj;
            }
            else
            {
                var obj = objectGenerator();
                return obj;
            }
        }
        public void Despawn(T obj)
        {
            objectDespawn?.Invoke(obj);
            objects.Enqueue(obj);
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
