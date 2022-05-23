using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    public class SkipList<T> : ICollection<T>
        where T : IComparable
    {
        SkipListNode<T> topLeft;
        SkipListNode<T> bottomLeft;
        Random random;
        /// <summary>
        /// 当前跳表层数；
        /// </summary>
        int level;
        /// <summary>
        /// 节点数量；
        /// Node count 
        /// </summary>
        int nodeCount;
        int maxLevels = int.MaxValue;
        /// <summary>
        /// 跳表节点缓存；
        /// </summary>
        Queue<SkipListNode<T>> cachedNodes;
        /// <summary>
        /// 当前跳表层数；
        /// </summary>
        public int Level { get { return level; } }
        public int MaxLevels { get { return maxLevels; } set { maxLevels = value; } }
        /// <summary>
        /// 节点数量；
        /// 返回最底层节点数量；
        /// </summary>
        public int Count { get { return nodeCount; } }
        public bool IsReadOnly { get { return false; } }
        /// <summary>
        /// 节点头，即最底层第一个节点；
        /// </summary>
        public SkipListNode<T> Head { get { return bottomLeft; } }
        /// <summary>
        /// 节点缓存数量；
        /// </summary>
        public int CachedNodeCount { get { return cachedNodes.Count; } }
        public SkipList()
        {
            topLeft = AcquireEmptyLevel();
            bottomLeft = topLeft;
            level = 1;
            nodeCount = 0;
            random = new Random();
            cachedNodes = new Queue<SkipListNode<T>>();
        }
        public void Add(T value)
        {
            //添加流程：
            //1. 投掷硬币，获取到新加入节点的层级
            //2. 比较节点值，加入最下层的有序链表
            //3. 分配查询层级节点

            int valueLevel = CoinFlipLevel();
            int newLevelCount = valueLevel - level;
            while (newLevelCount > 0)
            {
                //补全TopLeft节点
                var newLevelLeft = AcquireEmptyLevel();
                newLevelLeft.Below = topLeft;
                topLeft.Above = newLevelLeft;
                topLeft = newLevelLeft;

                newLevelCount--;
                level++;
            }
            SkipListNode<T> currentNode = topLeft;
            SkipListNode<T> lastNodeAbove = null;
            var remainLevel = level - 1;//剩余的层数

            while (currentNode != null && remainLevel >= 0)
            {
                if (remainLevel > valueLevel)
                {
                    //进入到当前节点的高度
                    currentNode = currentNode.Below;
                    remainLevel--;
                    continue;
                }
                while (currentNode.Next != null)
                {
                    if (!currentNode.Next.IsFooter() && currentNode.Next.Value.CompareTo(value) < 0)
                        currentNode = currentNode.Next;
                    else
                        break;// nextNode节点的值大于等于当前插入的值
                }

                var newNode = AcquireNode(value);
                newNode.Next = currentNode.Next;
                newNode.Previous = currentNode;
                newNode.Next.Previous = newNode;
                currentNode.Next = newNode;

                if (lastNodeAbove != null)
                {
                    lastNodeAbove.Below = newNode;
                    newNode.Above = lastNodeAbove;
                }
                lastNodeAbove = newNode;
                currentNode = currentNode.Below;
                remainLevel--;
            }
            nodeCount++;
        }
        public void Clear()
        {
            var currentNode = this.Head;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;
                if (!currentNode.IsHeader() && !currentNode.IsFooter())
                    this.Remove(currentNode);
                currentNode = nextNode;
            }
            cachedNodes.Clear();
        }
        public bool Contains(T value)
        {
            return Find(value) != null;
        }
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            IEnumerator<T> enumerator = this.GetEnumerator();

            for (int i = arrayIndex; i < array.Length; i++)
            {
                if (enumerator.MoveNext())
                    array[i] = enumerator.Current;
                else
                {
                    break;
                }
            }
        }
        public bool Remove(T value)
        {
            var valueNode = FindHighest(value);
            return Remove(valueNode);
        }
        public bool Remove(SkipListNode<T> valueNode)
        {
            if (valueNode == null)
            {
                return false;
            }
            else
            {
                if (valueNode.Above != null)
                {
                    valueNode = FindHighest(valueNode);
                }
                var currentNodeDown = valueNode;
                while (currentNodeDown != null)
                {
                    var previousNode = currentNodeDown.Previous;
                    var nextNode = currentNodeDown.Next;

                    if (previousNode != null)
                        previousNode.Next = nextNode;
                    if (nextNode != null)
                        nextNode.Previous = previousNode;

                    var belowNode = currentNodeDown.Below;
                    ReleaseNode(currentNodeDown);
                    currentNodeDown = belowNode;
                }
                nodeCount--;
                ClearEmptyLevels();
                return true;
            }
        }
        public SkipListNode<T> Find(T value)
        {
            var foundNode = this.topLeft;
            while (foundNode != null && foundNode.Next != null)
            {
                if (!foundNode.Next.IsFooter() && foundNode.Next.Value.CompareTo(value) < 0)
                    foundNode = foundNode.Next;
                else
                {
                    if (!foundNode.Next.IsFooter() && foundNode.Next.Value.Equals(value))
                    {
                        foundNode = foundNode.Next;
                        break;
                    }
                    else
                    {
                        foundNode = foundNode.Below;
                    }
                }
            }
            return foundNode;
        }
        /// <summary>
        /// 获取最高的节点；
        /// 若不存在，则返回空；
        /// </summary>
        /// <param name="valueNode">查询的目标节点</param>
        /// <returns>最高的节点</returns>
        public SkipListNode<T> FindHighest(SkipListNode<T> valueNode)
        {
            if (valueNode == null)
                return null;
            else
            {
                while (valueNode.Above != null)
                {
                    valueNode = valueNode.Above;
                }
                return valueNode;
            }
        }
        public virtual SkipListNode<T> FindHighest(T value)
        {
            SkipListNode<T> valueNode = this.Find(value);
            return this.FindHighest(valueNode);
        }
        public SkipListNode<T> FindLowest(T value)
        {
            SkipListNode<T> valueNode = this.Find(value);
            return this.FindLowest(valueNode);
        }
        public SkipListNode<T> FindLowest(SkipListNode<T> valueNode)
        {
            if (valueNode == null)
            {
                return null;
            }
            else
            {
                while (valueNode.Below != null)
                {
                    valueNode = valueNode.Below;
                }
                return valueNode;
            }
        }
        /// <summary>
        /// 清除跳表节点缓存；
        /// </summary>
        public void ClearCachedNodes()
        {
            cachedNodes.Clear();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new SkipListEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        /// <summary>
        /// 投掷硬币；
        /// Coin flip;
        /// 1 is heads, 0 is tails;
        /// </summary>
        /// <returns>coin value</returns>
        int CoinFlipLevel()
        {
            int level = 0;
            while (random.Next(0, 2) == 1 && level < this.level)//投掷硬币为1时增加一层
            {
                level++;
            }
            return level;
        }
        void ClearEmptyLevels()
        {
            if (level > 1)
            {
                var currentNode = topLeft;
                while (currentNode != bottomLeft)
                {
                    if (currentNode.IsHeader() && currentNode.Next.IsFooter())
                    {
                        var belowNode = currentNode.Below;
                        topLeft = currentNode.Below;

                        currentNode.Next.Dispose();
                        currentNode.Dispose();
                        level--;
                        currentNode = belowNode;
                    }
                    else
                        break;
                }
            }
        }
        SkipListNode<T> AcquireEmptyLevel()
        {
            var header = new SkipListNodeHeader<T>();
            var footer = new SkipListNodeFooter<T>();
            header.Next = footer;
            footer.Previous = header;
            return header;
        }
        SkipListNode<T> AcquireNode(T value)
        {
            SkipListNode<T> node = null;
            if (cachedNodes.Count > 0)
            {
                node = cachedNodes.Dequeue();
                node.Value = value;
            }
            else
            {
                node = new SkipListNode<T>(value);
            }
            return node;
        }
        void ReleaseNode(SkipListNode<T> node)
        {
            node.Dispose();
            cachedNodes.Enqueue(node);
        }
        /// <summary>
        /// 跳表迭代对象，遍历最低一层的跳表；
        /// </summary>
        public struct SkipListEnumerator : IEnumerator<T>
        {
            private SkipListNode<T> current;
            private SkipList<T> skipList;

            public SkipListEnumerator(SkipList<T> skipList)
            {
                this.skipList = skipList;
                current = null;
            }
            public T Current { get { return current.Value; } }
            object IEnumerator.Current { get { return this.Current; } }
            public void Dispose()
            {
                current = null;
            }
            public void Reset()
            {
                current = null;
            }
            public bool MoveNext()
            {
                if (current == null)
                    current = this.skipList.Head.Next;
                else
                    current = current.Next;
                if (current != null && current.IsFooter())
                    current = null;
                return (current != null);
            }
        }
    }
}
