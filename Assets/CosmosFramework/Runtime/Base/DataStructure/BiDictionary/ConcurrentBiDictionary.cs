using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
using System.Runtime.Serialization;

namespace Cosmos
{
    [Serializable]
    public class ConcurrentBiDictionary<TFirst, TSecond> : IDictionary<TFirst, TSecond>, IReadOnlyDictionary<TFirst, TSecond>, IDictionary
    {
        private readonly IDictionary<TFirst, TSecond> _firstToSecond = new ConcurrentDictionary<TFirst, TSecond>();
        //反查表参与序列化：反序列化回调触发时正查字段字典尚未填充，无法据此重建反查表
        private IDictionary<TSecond, TFirst> _secondToFirst = new ConcurrentDictionary<TSecond, TFirst>();
        [NonSerialized]
        private ReverseDictionary _reverseDictionary;

        public ConcurrentBiDictionary()
        {
            _reverseDictionary = new ReverseDictionary(this);
        }

        public IDictionary<TSecond, TFirst> Reverse
        {
            get
            {
                if (_reverseDictionary == null)
                    _reverseDictionary = new ReverseDictionary(this);
                return _reverseDictionary;
            }
        }

        public int Count
        {
            get { return _firstToSecond.Count; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_firstToSecond).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_firstToSecond).IsSynchronized; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)_firstToSecond).IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return _firstToSecond.IsReadOnly || _secondToFirst.IsReadOnly; }
        }

        public TSecond this[TFirst key]
        {
            get { return _firstToSecond[key]; }
            set
            {
                //先解除两个方向上与新映射冲突的旧映射，保证一对一关系
                TSecond previousSecond;
                if (_firstToSecond.TryGetValue(key, out previousSecond))
                {
                    _firstToSecond.Remove(key);
                    _secondToFirst.Remove(previousSecond);
                }
                TFirst previousFirst;
                if (_secondToFirst.TryGetValue(value, out previousFirst))
                {
                    _secondToFirst.Remove(value);
                    _firstToSecond.Remove(previousFirst);
                }
                _firstToSecond.Add(key, value);
                _secondToFirst.Add(value, key);
            }
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)_firstToSecond)[key]; }
            set { this[(TFirst)key] = (TSecond)value; }
        }

        public ICollection<TFirst> Keys
        {
            get { return _firstToSecond.Keys; }
        }

        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_firstToSecond).Keys; }
        }

        IEnumerable<TFirst> IReadOnlyDictionary<TFirst, TSecond>.Keys
        {
            get { return ((IReadOnlyDictionary<TFirst, TSecond>)_firstToSecond).Keys; }
        }

        public ICollection<TSecond> Values
        {
            get { return _firstToSecond.Values; }
        }

        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_firstToSecond).Values; }
        }

        IEnumerable<TSecond> IReadOnlyDictionary<TFirst, TSecond>.Values
        {
            get { return ((IReadOnlyDictionary<TFirst, TSecond>)_firstToSecond).Values; }
        }

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            return _firstToSecond.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_firstToSecond).GetEnumerator();
        }

        public void Add(TFirst key, TSecond value)
        {
            if (_firstToSecond.ContainsKey(key))
                throw new ArgumentException("An item with the same key has already been added.", nameof(key));
            if (_secondToFirst.ContainsKey(value))
                throw new ArgumentException("An item with the same value has already been added.", nameof(value));
            _firstToSecond.Add(key, value);
            try
            {
                _secondToFirst.Add(value, key);
            }
            catch
            {
                //反向写入失败时回滚，避免留下半个映射
                _firstToSecond.Remove(key);
                throw;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TFirst)key, (TSecond)value);
        }
        public bool TryAdd(TFirst key, TSecond value)
        {
            if (_firstToSecond.ContainsKey(key) || _secondToFirst.ContainsKey(value))
                return false;
            _firstToSecond.Add(key, value);
            try
            {
                _secondToFirst.Add(value, key);
            }
            catch
            {
                _firstToSecond.Remove(key);
                return false;
            }
            return true;
        }
        public void AddOrUpdate(TFirst key, TSecond value)
        {
            this[key] = value;
        }
        void ICollection<KeyValuePair<TFirst, TSecond>>.Add(KeyValuePair<TFirst, TSecond> item)
        {
            Add(item.Key, item.Value);
        }

        public bool ContainsKey(TFirst key)
        {
            return _firstToSecond.ContainsKey(key);
        }

        bool ICollection<KeyValuePair<TFirst, TSecond>>.Contains(KeyValuePair<TFirst, TSecond> item)
        {
            return _firstToSecond.Contains(item);
        }

        public bool TryGetValue(TFirst key, out TSecond value)
        {
            return _firstToSecond.TryGetValue(key, out value);
        }

        public bool Remove(TFirst key)
        {
            TSecond value;
            if (!_firstToSecond.TryGetValue(key, out value))
                return false;
            //使用键值对的条件删除，防止并发下删掉已被改写的映射
            if (!((ICollection<KeyValuePair<TFirst, TSecond>>)_firstToSecond).Remove(new KeyValuePair<TFirst, TSecond>(key, value)))
                return false;
            _secondToFirst.Remove(value);
            return true;
        }

        void IDictionary.Remove(object key)
        {
            Remove((TFirst)key);
        }

        bool ICollection<KeyValuePair<TFirst, TSecond>>.Remove(KeyValuePair<TFirst, TSecond> item)
        {
            TSecond value;
            if (!_firstToSecond.TryGetValue(item.Key, out value)
                || !EqualityComparer<TSecond>.Default.Equals(value, item.Value))
                return false;
            if (!((ICollection<KeyValuePair<TFirst, TSecond>>)_firstToSecond).Remove(new KeyValuePair<TFirst, TSecond>(item.Key, value)))
                return false;
            _secondToFirst.Remove(value);
            return true;
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_firstToSecond).Contains(key);
        }

        public void Clear()
        {
            _firstToSecond.Clear();
            _secondToFirst.Clear();
        }

        void ICollection<KeyValuePair<TFirst, TSecond>>.CopyTo(KeyValuePair<TFirst, TSecond>[] array, int arrayIndex)
        {
            _firstToSecond.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)_firstToSecond).CopyTo(array, index);
        }

        private class ReverseDictionary : IDictionary<TSecond, TFirst>, IReadOnlyDictionary<TSecond, TFirst>, IDictionary
        {
            private readonly ConcurrentBiDictionary<TFirst, TSecond> _owner;

            public ReverseDictionary(ConcurrentBiDictionary<TFirst, TSecond> owner)
            {
                _owner = owner;
            }

            public int Count
            {
                get { return _owner._secondToFirst.Count; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_owner._secondToFirst).SyncRoot; }
            }

            bool ICollection.IsSynchronized
            {
                get { return ((ICollection)_owner._secondToFirst).IsSynchronized; }
            }

            bool IDictionary.IsFixedSize
            {
                get { return ((IDictionary)_owner._secondToFirst).IsFixedSize; }
            }

            public bool IsReadOnly
            {
                get { return _owner._secondToFirst.IsReadOnly || _owner._firstToSecond.IsReadOnly; }
            }

            public TFirst this[TSecond key]
            {
                get { return _owner._secondToFirst[key]; }
                set
                {
                    TFirst previousFirst;
                    if (_owner._secondToFirst.TryGetValue(key, out previousFirst))
                    {
                        _owner._secondToFirst.Remove(key);
                        _owner._firstToSecond.Remove(previousFirst);
                    }
                    TSecond previousSecond;
                    if (_owner._firstToSecond.TryGetValue(value, out previousSecond))
                    {
                        _owner._firstToSecond.Remove(value);
                        _owner._secondToFirst.Remove(previousSecond);
                    }
                    _owner._secondToFirst.Add(key, value);
                    _owner._firstToSecond.Add(value, key);
                }
            }

            object IDictionary.this[object key]
            {
                get { return ((IDictionary)_owner._secondToFirst)[key]; }
                set { this[(TSecond)key] = (TFirst)value; }
            }
            public ICollection<TSecond> Keys
            {
                get { return _owner._secondToFirst.Keys; }
            }

            ICollection IDictionary.Keys
            {
                get { return ((IDictionary)_owner._secondToFirst).Keys; }
            }

            IEnumerable<TSecond> IReadOnlyDictionary<TSecond, TFirst>.Keys
            {
                get { return ((IReadOnlyDictionary<TSecond, TFirst>)_owner._secondToFirst).Keys; }
            }

            public ICollection<TFirst> Values
            {
                get { return _owner._secondToFirst.Values; }
            }

            ICollection IDictionary.Values
            {
                get { return ((IDictionary)_owner._secondToFirst).Values; }
            }

            IEnumerable<TFirst> IReadOnlyDictionary<TSecond, TFirst>.Values
            {
                get { return ((IReadOnlyDictionary<TSecond, TFirst>)_owner._secondToFirst).Values; }
            }

            public IEnumerator<KeyValuePair<TSecond, TFirst>> GetEnumerator()
            {
                return _owner._secondToFirst.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return ((IDictionary)_owner._secondToFirst).GetEnumerator();
            }

            public void Add(TSecond key, TFirst value)
            {
                if (_owner._secondToFirst.ContainsKey(key))
                    throw new ArgumentException("An item with the same key has already been added.", nameof(key));
                if (_owner._firstToSecond.ContainsKey(value))
                    throw new ArgumentException("An item with the same value has already been added.", nameof(value));
                _owner._secondToFirst.Add(key, value);
                try
                {
                    _owner._firstToSecond.Add(value, key);
                }
                catch
                {
                    _owner._secondToFirst.Remove(key);
                    throw;
                }
            }

            void IDictionary.Add(object key, object value)
            {
                Add((TSecond)key, (TFirst)value);
            }

            void ICollection<KeyValuePair<TSecond, TFirst>>.Add(KeyValuePair<TSecond, TFirst> item)
            {
                Add(item.Key, item.Value);
            }

            public bool ContainsKey(TSecond key)
            {
                return _owner._secondToFirst.ContainsKey(key);
            }

            bool ICollection<KeyValuePair<TSecond, TFirst>>.Contains(KeyValuePair<TSecond, TFirst> item)
            {
                return _owner._secondToFirst.Contains(item);
            }

            public bool TryGetValue(TSecond key, out TFirst value)
            {
                return _owner._secondToFirst.TryGetValue(key, out value);
            }

            public bool Remove(TSecond key)
            {
                TFirst value;
                if (!_owner._secondToFirst.TryGetValue(key, out value))
                    return false;
                if (!((ICollection<KeyValuePair<TSecond, TFirst>>)_owner._secondToFirst).Remove(new KeyValuePair<TSecond, TFirst>(key, value)))
                    return false;
                _owner._firstToSecond.Remove(value);
                return true;
            }

            void IDictionary.Remove(object key)
            {
                Remove((TSecond)key);
            }

            bool ICollection<KeyValuePair<TSecond, TFirst>>.Remove(KeyValuePair<TSecond, TFirst> item)
            {
                TFirst value;
                if (!_owner._secondToFirst.TryGetValue(item.Key, out value)
                    || !EqualityComparer<TFirst>.Default.Equals(value, item.Value))
                    return false;
                if (!((ICollection<KeyValuePair<TSecond, TFirst>>)_owner._secondToFirst).Remove(new KeyValuePair<TSecond, TFirst>(item.Key, value)))
                    return false;
                _owner._firstToSecond.Remove(value);
                return true;
            }

            bool IDictionary.Contains(object key)
            {
                return ((IDictionary)_owner._secondToFirst).Contains(key);
            }

            public void Clear()
            {
                _owner._secondToFirst.Clear();
                _owner._firstToSecond.Clear();
            }

            void ICollection<KeyValuePair<TSecond, TFirst>>.CopyTo(KeyValuePair<TSecond, TFirst>[] array, int arrayIndex)
            {
                _owner._secondToFirst.CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((IDictionary)_owner._secondToFirst).CopyTo(array, index);
            }
        }
    }
}
