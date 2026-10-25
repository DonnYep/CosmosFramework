using System;
using System.Collections;
using System.Collections.Generic;
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
        public Pool(Func<T> objectGenerator, Action<T> objectDespawn) :this(0,objectGenerator,null,objectDespawn,null){ }
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
            if (obj == null)
                return;
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
        /// <summary>
        /// 清空池并对池内对象执行释放委托（池里若是需要销毁的 Unity 对象，请用此重载，否则会泄漏）。
        /// <para>释放委托与 <c>objectDespawn</c> 语义不同：后者在"对象入池"时调用，前者只在清空/销毁时调用一次。</para>
        /// </summary>
        /// <param name="release">逐个释放池内对象</param>
        public void Clear(Action<T> release)
        {
            if (release == null)
            {
                objects.Clear();
                return;
            }
            while (objects.Count > 0)
                release(objects.Dequeue());
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
