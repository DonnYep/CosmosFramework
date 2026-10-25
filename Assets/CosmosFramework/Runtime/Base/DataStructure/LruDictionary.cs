using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
namespace Cosmos
{
    /// <summary>
    /// LRU缓存Least Recently Used
    /// </summary>
    public class LruDictionary<Key,Value>:IEnumerable<Value>
    {
        struct LruEnumerator<T> : IEnumerator<T>
        {
            T current;
            int index;
            T[] list;
            public LruEnumerator(ICollection<T> collection)
            {
                current = default(T);
                index = -1;
                // TODO 并不优化的数据结构，LRU
                list = new T[collection.Count];
                int tmpIndex = -1;
                foreach (var v in collection)
                {
                    list[++tmpIndex] = v;
                }
            }
            public T Current { get { return current; } }
            object IEnumerator.Current { get { return Current; } }
            public void Dispose()
            {
                index = -1;
                current = default(T);
                list = null;
            }
            public bool MoveNext()
            {
                if (++index >= list.Length)
                    return false;
                else
                    current = list[index];
                return true;
            }
            public void Reset()
            {
                index = -1;
            }
        }

        // TODO LRUCache实现IDictionary接口

        Action<Value> overflow;
        /// <summary>
        /// 当添加一个元素，整体元素个数超过容量时，移除时触发的事件函数
        /// </summary>
        /// <param name="handler"></param>
        public void AddOverflowAction(Action<Value> handler)
        {
            overflow += handler;
        }
        public void RemoveOverflowAction(Action<Value> handler)
        {
            try
            {
                overflow -= handler;
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Overflow handler not exist !" + handler.ToString());
            }
        }
        const uint DEFAULT_CAPACITY = 255;
        uint capacity;
        ReaderWriterLockSlim locker;
        Dictionary<Key, Value> dictionary;
        LinkedList<Key> linkedList;
        public Value this[Key key]
        {
            get
            {
                //读取即视为使用：与 TryGetValue 一致，命中后提升为最近使用
                TryGetValue(key, out var value);
                return value;
            }
            set
            {
                //索引器赋值与 Set 语义一致：键不存在时插入（必要时淘汰），而不是静默丢弃
                Set(key, value);
            }
        }
        public LruDictionary() : this(DEFAULT_CAPACITY) { }
        public LruDictionary (uint capacity)
        {
            locker = new ReaderWriterLockSlim();
            this.capacity = capacity > 0 ? capacity : DEFAULT_CAPACITY;
            dictionary = new Dictionary<Key, Value>();
            linkedList = new LinkedList<Key>();
        }
        public void Set(Key key, Value value)
        {
            locker.EnterWriteLock();
            try
            {
                dictionary[key] = value;
                linkedList.Remove(key);
                linkedList.AddFirst(key);
                if (linkedList.Count > capacity)
                {
                    dictionary.Remove(linkedList.Last.Value);
                    linkedList.RemoveLast();
                }
            }
            finally { locker.ExitWriteLock(); }
        }
        public void Add(Key key, Value value)
        {
            Value evictedValue = default(Value);
            bool hasEvicted = false;
            locker.EnterWriteLock();
            try
            {
                if (dictionary.ContainsKey(key))
                    return;
                linkedList.AddFirst(key);
                dictionary.Add(key, value);
                if (linkedList.Count > capacity)
                {
                    //Key lastkey = linkedList.Last.Value;
                    evictedValue = dictionary[linkedList.Last.Value];
                    hasEvicted = true;
                    dictionary.Remove(linkedList.Last.Value);
                    linkedList.RemoveLast();
                }
            }
            finally { locker.ExitWriteLock(); }
            //回调在写锁释放后触发，避免回调内部再次访问本字典造成死锁
            if (hasEvicted)
                overflow?.Invoke(evictedValue);
        }
        public Value Remove(Key key)
        {
            locker.EnterWriteLock();
            try
            {
                if (!dictionary.TryGetValue(key, out Value value))
                    return default(Value);
                dictionary.Remove(key);
                linkedList.Remove(key);
                return value;
            }
            finally { locker.ExitWriteLock(); }
        }
        public bool TryGetValue(Key key, out Value value)
        {
            locker.EnterUpgradeableReadLock();
            try
            {
                bool exist = dictionary.TryGetValue(key, out value);
                if (exist)
                {
                    locker.EnterWriteLock();
                    try
                    {
                        linkedList.Remove(key);
                        linkedList.AddFirst(key);
                    }
                    finally { locker.ExitWriteLock(); }
                }
                return exist;
            }
            catch { throw; }
            finally { locker.ExitUpgradeableReadLock(); }
        }
        public bool ContainsKey(Key key)
        {
            locker.EnterReadLock();
            try
            {
                return dictionary.ContainsKey(key);
            }
            finally { locker.ExitReadLock(); }
        }

        public int Count
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    return dictionary.Count;
                }
                finally { locker.ExitReadLock(); }
            }
        }
        public uint Capacity
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    return capacity;
                }
                finally { locker.ExitReadLock(); }
            }
            set
            {
                locker.EnterUpgradeableReadLock();
                try
                {
                    if (value > 0 && capacity != value)
                    {
                        locker.EnterWriteLock();
                        try
                        {
                            capacity = value;
                            //裁剪时两个结构必须同步移除，否则被淘汰的键仍可读到
                            while (linkedList.Count > 0 && dictionary.Count > capacity)
                            {
                                Key lastKey = linkedList.Last.Value;
                                linkedList.RemoveLast();
                                dictionary.Remove(lastKey);
                            }
                        }
                        finally { locker.ExitWriteLock(); }
                    }
                }
                finally { locker.ExitUpgradeableReadLock(); }
            }
        }
        /// <summary>
        /// 键快照；返回副本，避免调用方在锁外枚举活集合。
        /// </summary>
        public ICollection<Key> Keys
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    var keys = new Key[dictionary.Count];
                    dictionary.Keys.CopyTo(keys, 0);
                    return keys;
                }
                finally { locker.ExitReadLock(); }
            }
        }
        /// <summary>
        /// 值快照；返回副本。
        /// </summary>
        public ICollection<Value> Values
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    var values = new Value[dictionary.Count];
                    dictionary.Values.CopyTo(values, 0);
                    return values;
                }
                finally { locker.ExitReadLock(); }
            }
        }

        #region IEnumerable
        public IEnumerator<Value> GetEnumerator()
        {
            //枚举器在构造时快照，读锁只覆盖快照过程，避免释放锁后遍历到正在被修改的字典
            locker.EnterReadLock();
            try
            {
                return new LruEnumerator<Value>(dictionary.Values);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        /// <summary>
        /// 重置数组长度；新长度不大于当前容量时保持原容量。
        /// </summary>
        /// <param name="capacity">新的数组长度</param>
        public void ResetCapacity(uint capacity)
        {
            if (capacity > Capacity)
                Capacity = capacity;
        }
    }
}