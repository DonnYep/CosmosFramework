using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
namespace Cosmos
{
    /// <summary>
    /// LRU缓存Least Recently Used
    /// </summary>
    public class LRU<Key,Value>:IEnumerable<Value>
    {
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
                bool exist = dictionary.ContainsKey(key);
                Value value = default(Value);
                if (exist)
                {
                    locker.EnterReadLock();
                    try
                    {
                        value= dictionary[key];
                    }
                    finally {locker.ExitReadLock();}
                }
                return value;
            }
            set
            {
                bool exist = dictionary.ContainsKey(key);
                if (exist)
                {
                    locker.EnterWriteLock();
                    try
                    {
                        dictionary[key] = value;
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
        }
        public LRU() : this(DEFAULT_CAPACITY) { }
        public LRU (uint capacity)
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
            locker.EnterWriteLock();
            bool exist = dictionary.ContainsKey(key);
            if (!exist)
            {
                try
                {
                    linkedList.AddFirst(key);
                    dictionary.Add(key, value);
                    if (linkedList.Count > capacity)
                    {
                        //Key lastkey = linkedList.Last.Value;
                        Value lastValue = dictionary[linkedList.Last.Value];
                        dictionary.Remove(linkedList.Last.Value);
                        linkedList.RemoveLast();
                        overflow?.Invoke(lastValue);
                    }
                }
                finally { locker.ExitWriteLock(); }
            }
        }
        public Value Remove(Key key)
        {
            locker.EnterWriteLock();
            bool exist = dictionary.ContainsKey(key);
            Value value = default(Value);
            if (exist)
            {
                try
                {
                    value = dictionary[key];
                    dictionary.Remove(key);
                    linkedList.Remove(key);
                }
                finally { locker.ExitWriteLock(); }
            }
            return value;
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
                            while (linkedList.Count > capacity)
                            {
                                linkedList.RemoveLast();
                            }
                        }
                        finally { locker.ExitWriteLock(); }
                    }
                }
                finally { locker.ExitUpgradeableReadLock(); }
            }
        }
        public ICollection<Key> Keys
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    return dictionary.Keys;
                }
                finally { locker.ExitReadLock(); }
            }
        }
        public ICollection<Value> Values
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    return dictionary.Values;
                }
                finally { locker.ExitReadLock(); }
            }
        }

        #region IEnumerable
        public IEnumerator<Value> GetEnumerator()
        {
            return new LRUEnumerator<Value>(dictionary.Values);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        //TODO 重新设置数组长度，多的部分需要清理，未实现
        /// <summary>
        /// 重置数组长度；当前重置长度，除非数值比原先大，否则使用原有容量
        /// </summary>
        /// <param name="capacity">新的数组长度</param>
        public void ResetCapacity(uint capacity )
        {
            this.capacity = capacity >this.capacity  ? capacity : this.capacity;
        }
    }
}