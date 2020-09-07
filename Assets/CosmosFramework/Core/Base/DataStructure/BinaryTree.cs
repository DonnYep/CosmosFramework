using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    //TODO继承.NET对应的接口
    /// <summary>
    /// 二叉树数据
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
            root = AddNode(root, data);
        }
        Node AddNode(Node node, T data)
        {
            if (node == null)
            {
                count++;
                return new Node(data);
            }
            if (data.CompareTo(node.Data) < 0)
                node.Left = AddNode(node.Left, data);
            else if (data.CompareTo(node.Data) > 0)
                node.Left = AddNode(node.Right, data);
            return node;
        }
        public bool Contains(T data)
        {
            return Contains(root, data);
        }
        bool Contains(Node node, T data)
        {
            if (node == null)
                return false;
            if (data.CompareTo(node.Data) == 0)
                return true;
            else if (data.CompareTo(node.Data) < 0)
                return Contains(node.Left, data);
            else
                return Contains(node.Right, data);
        }
        /// <summary>
        ///层级遍历 
        /// </summary>
        public void LevelTraversal(ref Queue<T> queueData)
        {
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(root);
            while (queue.Count != 0)
            {
                Node current = queue.Dequeue();
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
            Queue<T> queueData = new Queue<T>();
            PreorderTraversal(root, ref queueData);
            return queueData;
        }
        void PreorderTraversal(Node node, ref Queue<T> queueData)
        {
            if (node == null)
                return;
            queueData.Enqueue(node.Data);
            PreorderTraversal(node.Left, ref queueData);
            PreorderTraversal(node.Right, ref queueData);
        }
        /// <summary>
        ///中序遍历 
        /// </summary>
        public Queue<T> InorderTraversal()
        {
            Queue<T> queueData = new Queue<T>();
            InorderTraversal(root, ref queueData);
            return queueData;
        }
        void InorderTraversal(Node node, ref Queue<T> queueData)
        {
            if (node == null)
                return;
            PreorderTraversal(node.Left, ref queueData);
            queueData.Enqueue(node.Data);
            PreorderTraversal(node.Right, ref queueData);
        }
        /// <summary>
        ///后续遍历 
        /// </summary>
        public Queue<T> PostorderTraversal()
        {
            Queue<T> queueData = new Queue<T>();
            PostorderTraversal(root, ref queueData);
            return queueData;
        }
        void PostorderTraversal(Node node, ref Queue<T> queueData)
        {
            if (node == null)
                return;
            PreorderTraversal(node.Left, ref queueData);
            PreorderTraversal(node.Right, ref queueData);
            queueData.Enqueue(node.Data);
        }
        public T MaxData()
        {
            if (IsEmpty)
                throw new ArgumentNullException("Empty tree!");
            return MaxNode(root).Data;
        }
        Node MaxNode(Node node)
        {
            if (node.Right == null)
                return node;
            return MaxNode(node.Right);
        }
        public T MinData()
        {
            if (IsEmpty)
                throw new ArgumentNullException("Empty tree!");
            return MinNode(root).Data;
        }
        Node MinNode(Node node)
        {
            if (node.Left == null)
                return node;
            return MinNode(node.Left);
        }
        public T RemoveMinData()
        {
            T removeSet = MinData();
            root = RemoveMinNode(root);
            return removeSet;
        }
        Node RemoveMinNode(Node node)
        {
            if (node.Left == null)
            {
                count--;
                return node.Right;
            }
            node.Left = RemoveMinNode(node.Left);
            return node;
        }
        public void RemoveData(T data)
        {
            root = RemoveNode(root, data);
        }
        Node RemoveNode(Node node,T data)
        {
            if (node == null) return null;
            if (data.CompareTo(node.Data) < 0)
            {
                node.Left = RemoveNode(node.Left, data);
                return node;
            }else if (data.CompareTo(node.Data) > 0)
            {
                node.Right = RemoveNode(node.Right, data);
                return node;
            }
            else
            {
                if (node.Right == null)
                {
                    count--;
                    return node.Left;
                }
                if (node.Left == null)
                {
                    count--;
                    return node.Right;
                }
                Node n = MinNode(node.Right);
                n.Right = RemoveMinNode(node.Right);
                n.Left = node.Left;
                return n;
            }
        }
        /// <summary>
        ///二叉树的最高高度 
        /// </summary>
        public int MaxHeight()
        {
            return MaxHeight(root);
        }
        int MaxHeight(Node node)
        {
            if (node == null) return 0;
            return Math.Max(MaxHeight(node.Left), MaxHeight(node.Right)) + 1;
        }
    }
}