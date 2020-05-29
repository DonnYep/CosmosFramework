using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos {
    public class LRUCacheIEnumerator <T>:  IReference, IEnumerator<T>
    {
        T current=default(T);
        int index=-1;
        T[] list;
        public LRUCacheIEnumerator(ICollection<T> collection)
        {
            // TODO 并不优化的数据结构，LRU
            list = new T[collection.Count];
            int tmpIndex = -1;
            foreach (var v in collection)
            {
                list[++tmpIndex] = v;
            }
        }
        public T Current {get{ return current; } }
        object IEnumerator.Current{get{return Current;}}
        public void Clear()
        {
            index = -1;
            current = default(T);
            list = null;
        }
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
}