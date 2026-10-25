using System;
using System.Collections;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    /// 单向链表；双向链表建议使用 .NET 的 <see cref="LinkedList{T}"/>。
    /// <para>枚举器直接遍历节点（零分配），并在枚举期间检测结构修改。</para>
    /// </summary>
    public class TinyLinkedList<T> : IEnumerable<T>
    {
        /// <summary>
        /// 链表节点
        /// </summary>
        class Node
        {
            T data;
            public T Data { get { return data; } set { data = value; } }
            Node next;
            public Node Next { get { return next; } set { next = value; } }
            public Node(T nodeData)
            {
                this.data = nodeData;
                this.next = null;
            }
            public Node(T nodeData, Node next)
            {
                this.data = nodeData;
                this.next = next;
            }
            public override string ToString()
            {
                return data.ToString();
            }
        }
        int count;
        int version;
        public int Count { get { return count; } }
        Node head;
        public TinyLinkedList()
        {
            head = null;
            count = 0;
        }
        public bool IsEmpty { get { return count == 0; } }
        public void Add(T node, int index)
        {
            if (index < 0 || index > count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            if (index == 0)
            {
                head = new Node(node, head);
            }
            else
            {
                Node preview = head;
                for (int i = 0; i < index - 1; i++)
                    preview = preview.Next;
                preview.Next = new Node(node, preview.Next);
            }
            count++;
            version++;
        }
        public void AddFirst(T node)
        {
            Add(node, 0);
        }
        public void AddLast(T node)
        {
            Add(node, count);
        }
        public bool Contains(T node)
        {
            Node current = head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Data, node))
                    return true;
                current = current.Next;
            }
            return false;
        }
        public T Get(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            Node current = head;
            for (int i = 0; i < index; i++)
                current = current.Next;
            return current.Data;
        }
        public T GetFirst()
        {
            return Get(0);
        }
        public T GetLast()
        {
            return Get(count - 1);
        }
        public void Set(T node, int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            Node current = head;
            for (int i = 0; i < index; i++)
                current = current.Next;
            current.Data = node;
        }
        public T RemoveAt(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            if (index == 0)
            {
                Node delNode = head;
                head = head.Next;
                count--;
                version++;
                return delNode.Data;
            }
            else
            {
                Node preview = head;
                for (int i = 0; i < index - 1; i++)
                    preview = preview.Next;
                Node delNode = preview.Next;
                preview.Next = delNode.Next;
                count--;
                version++;
                return delNode.Data;
            }

        }
        public T RemoveFirst()
        {
            return RemoveAt(0);
        }
        public T RemoveLast()
        {
            return RemoveAt(count - 1);
        }
        public void Remove(T node)
        {
            if (head == null)
                return;
            if (EqualityComparer<T>.Default.Equals(head.Data, node))
            {
                head = head.Next;
                count--;
                version++;
            }
            else
            {
                Node current = head;
                Node preview = null;
                while (current != null)
                {
                    if (EqualityComparer<T>.Default.Equals(current.Data, node))
                        break;
                    preview = current;
                    current = current.Next;
                }
                if (current != null)
                {
                    preview.Next = preview.Next.Next;
                    count--;
                    version++;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// 节点遍历枚举器：不拷贝整表，枚举期间链表被修改时抛出异常。
        /// </summary>
        struct Enumerator : IEnumerator<T>
        {
            readonly TinyLinkedList<T> list;
            readonly int version;
            Node current;
            T currentValue;
            public Enumerator(TinyLinkedList<T> list)
            {
                this.list = list;
                this.version = list.version;
                current = list.head;
                currentValue = default(T);
            }
            public T Current { get { return currentValue; } }
            object IEnumerator.Current { get { return Current; } }
            public bool MoveNext()
            {
                if (version != list.version)
                    throw new InvalidOperationException("TinyLinkedList : 枚举期间集合被修改。");
                if (current == null)
                    return false;
                currentValue = current.Data;
                current = current.Next;
                return true;
            }
            public void Reset()
            {
                if (version != list.version)
                    throw new InvalidOperationException("TinyLinkedList : 枚举期间集合被修改。");
                current = list.head;
                currentValue = default(T);
            }
            public void Dispose()
            {
                current = null;
                currentValue = default(T);
            }
        }
    }
}
