using System;
using System.Collections;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    /// CFramework下的简易单向链表
    /// 复杂链表建议使用.NET的LinkedList
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
                head = new Node(node, head);
            else
            {
                Node preview = head;
                for (int i = 0; i < index - 1; i++)
                    preview = preview.Next;
                preview.Next = new Node(node, preview.Next);
            }
            count++;
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
                if (current.Data.Equals(node))
                    return true;
                current = current.Next;
            }
            return false;
        }
        public T Get(int index)
        {
            if (index < 0 || index > count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            Node current = head;
            for (int i = 0; i < index - 1; i++)
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
            if (index < 0 || index > count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            Node current = head;
            for (int i = 0; i < index; i++)
                current = current.Next;
            current.Data = node;
        }
        public T RemoveAt(int index)
        {
            if (index < 0 || index > count)
                throw new ArgumentOutOfRangeException("TinyLinkedList : 非法索引");
            if (index == 0)
            {
                Node delNode = head;
                head = head.Next;
                count--;
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
            if (head.Equals(node))
            {
                head = head.Next;
                count--;
            }
            else
            {
                Node current = head;
                Node preview = null;
                while (current != null)
                {
                    if (current.Data.Equals(node))
                        break;
                    preview = current;
                    current = current.Next;
                }
                if (current != null)
                {
                    preview.Next = preview.Next.Next;
                    count--;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(head, count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        struct Enumerator : IEnumerator<T>
        {
            T current;
            public T Current { get { return current; } }
            object IEnumerator.Current { get { return Current; } }
            T[] list;
            int count;
            int index;
            public Enumerator(Node head, int count)
            {
                current = default(T);
                list = new T[count];
                this.count = count;
                index = -1;
                int tmpIndex = -1;
                Node currentNode = head;
                while (currentNode != null)
                {
                    list[++tmpIndex] = currentNode.Data;
                    currentNode = currentNode.Next;
                }
            }
            public void Dispose()
            {
                index = -1;
                current = default(T);
                list = null;
            }
            public bool MoveNext()
            {
                if (++index >= count)
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
}