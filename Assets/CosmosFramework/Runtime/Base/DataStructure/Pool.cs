using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class Pool<T> : IEnumerable<T>
        where T:class
    {
        public int Count { get { return objects.Count; } }
        readonly Queue<T> objects = new Queue<T>();
        readonly Func<T> objectGenerator;
        readonly Action<T> objectDespawn;
        readonly Action<T> objectOverflow;
        readonly Action<T> objectSpawn;
        readonly int capacity = 0;
        public Pool(int capacity, Func<T> objectGenerator, Action<T> objectSpawn, Action<T> objectDesapwn, Action<T> objectOverflow)
        {
            this.objectGenerator = objectGenerator;
            this.objectDespawn = objectDesapwn;
            this.objectOverflow = objectOverflow;
            this.objectSpawn = objectSpawn;
            this.capacity = capacity;
        }
        public Pool(Func<T> objectGenerator, Action<T> objectSpawn, Action<T> objectDesapwn):this(0,objectGenerator,objectSpawn,objectDesapwn,null) { }
        public Pool(Func<T> objectGenerator, Action<T> objectDesapwn) :this(0,objectGenerator,null,objectDesapwn,null){ }
        public Pool(Func<T> objectGenerator) : this(0,objectGenerator, null,null,null){}
        public T Spawn()
        {
            if (objects.Count > 0)
            {
                var obj= objects.Dequeue();
                objectSpawn?.Invoke(obj);
                return obj;
            }
            else
            {
                var obj= objectGenerator();
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
