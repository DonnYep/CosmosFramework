using System;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    /// 二叉搜索树（不自动平衡）。
    /// <para>全部操作使用迭代实现，避免已排序数据插入后退化为链表时递归过深导致栈溢出。</para>
    /// <para>注意：顺序插入会退化为 O(n)，适合查找为主的中小规模数据；需要有序动态集合请用跳表或平衡树。</para>
    /// </summary>
    public class BinaryTree<T>
         where T : IComparable<T>
    {
        /// <summary>
        /// 二叉树节点类
        /// </summary>
        class Node
        {
            T data;
            public T Data { get { return data; } set { data = value; } }
            Node left;
            public Node Left { get { return left; } set { left = value; } }
            Node right;
            public Node Right { get { return right; } set { right = value; } }
            public Node(T data)
            {
                this.data = data;
                this.left = null;
                this.right = null;
            }
        }
        Node root;
        int count;
        public int Count { get { return count; } }
        public BinaryTree()
        {
            root = null;
            count = 0;
        }
        public bool IsEmpty { get { return count == 0; } }
        public void AddNode(T data)
        {
            if (root == null)
            {
                root = new Node(data);
                count++;
                return;
            }
            var current = root;
            while (true)
            {
                var compare = data.CompareTo(current.Data);
                if (compare == 0)
                    return;//重复值忽略
                if (compare < 0)
                {
                    if (current.Left == null)
                    {
                        current.Left = new Node(data);
                        count++;
                        return;
                    }
                    current = current.Left;
                }
                else
                {
                    if (current.Right == null)
                    {
                        current.Right = new Node(data);
                        count++;
                        return;
                    }
                    current = current.Right;
                }
            }
        }
        public bool Contains(T data)
        {
            var current = root;
            while (current != null)
            {
                var compare = data.CompareTo(current.Data);
                if (compare == 0)
                    return true;
                current = compare < 0 ? current.Left : current.Right;
            }
            return false;
        }
        /// <summary>
        /// 层级遍历
        /// </summary>
        public void LevelTraversal(ref Queue<T> queueData)
        {
            if (root == null)
                return;
            if (queueData == null)
                queueData = new Queue<T>();
            var queue = new Queue<Node>();
            queue.Enqueue(root);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                queueData.Enqueue(current.Data);
                if (current.Left != null)
                    queue.Enqueue(current.Left);
                if (current.Right != null)
                    queue.Enqueue(current.Right);
            }
        }
        /// <summary>
        /// 前序遍历
        /// </summary>
        public Queue<T> PreorderTraversal()
        {
            var queueData = new Queue<T>();
            if (root == null)
                return queueData;
            var stack = new Stack<Node>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                queueData.Enqueue(current.Data);
                if (current.Right != null)
                    stack.Push(current.Right);
                if (current.Left != null)
                    stack.Push(current.Left);
            }
            return queueData;
        }
        /// <summary>
        /// 中序遍历
        /// </summary>
        public Queue<T> InorderTraversal()
        {
            var queueData = new Queue<T>();
            var stack = new Stack<Node>();
            var current = root;
            while (current != null || stack.Count > 0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                current = stack.Pop();
                queueData.Enqueue(current.Data);
                current = current.Right;
            }
            return queueData;
        }
        /// <summary>
        /// 后续遍历
        /// </summary>
        public Queue<T> PostorderTraversal()
        {
            var queueData = new Queue<T>();
            if (root == null)
                return queueData;
            var stack = new Stack<Node>();
            var reversed = new Stack<T>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                reversed.Push(current.Data);
                //先左后右入栈，出栈顺序为根-右-左，反转后即后序
                if (current.Left != null)
                    stack.Push(current.Left);
                if (current.Right != null)
                    stack.Push(current.Right);
            }
            while (reversed.Count > 0)
                queueData.Enqueue(reversed.Pop());
            return queueData;
        }
        public T MaxData()
        {
            if (IsEmpty)
                throw new ArgumentNullException("Empty tree!");
            var current = root;
            while (current.Right != null)
                current = current.Right;
            return current.Data;
        }
        public T MinData()
        {
            if (IsEmpty)
                throw new ArgumentNullException("Empty tree!");
            var current = root;
            while (current.Left != null)
                current = current.Left;
            return current.Data;
        }
        public T RemoveMinData()
        {
            if (IsEmpty)
                throw new ArgumentNullException("Empty tree!");
            Node parent = null;
            var current = root;
            while (current.Left != null)
            {
                parent = current;
                current = current.Left;
            }
            var removed = current.Data;
            if (parent == null)
                root = current.Right;
            else
                parent.Left = current.Right;
            count--;
            return removed;
        }
        public void RemoveData(T data)
        {
            Node parent = null;
            var current = root;
            while (current != null)
            {
                var compare = data.CompareTo(current.Data);
                if (compare == 0)
                    break;
                parent = current;
                current = compare < 0 ? current.Left : current.Right;
            }
            if (current == null)
                return;

            if (current.Left != null && current.Right != null)
            {
                //用右子树最小节点替换当前值，再摘除该最小节点（其无左孩子）
                Node successorParent = current;
                var successor = current.Right;
                while (successor.Left != null)
                {
                    successorParent = successor;
                    successor = successor.Left;
                }
                current.Data = successor.Data;
                if (successorParent == current)
                    successorParent.Right = successor.Right;
                else
                    successorParent.Left = successor.Right;
                count--;
                return;
            }

            var child = current.Left ?? current.Right;
            if (parent == null)
                root = child;
            else if (parent.Left == current)
                parent.Left = child;
            else
                parent.Right = child;
            count--;
        }
        /// <summary>
        /// 二叉树的最高高度
        /// </summary>
        public int MaxHeight()
        {
            if (root == null)
                return 0;
            var queue = new Queue<Node>();
            queue.Enqueue(root);
            var height = 0;
            while (queue.Count > 0)
            {
                var levelCount = queue.Count;
                for (int i = 0; i < levelCount; i++)
                {
                    var current = queue.Dequeue();
                    if (current.Left != null)
                        queue.Enqueue(current.Left);
                    if (current.Right != null)
                        queue.Enqueue(current.Right);
                }
                height++;
            }
            return height;
        }
    }
}
