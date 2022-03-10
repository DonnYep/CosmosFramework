using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// 跳跃节点表；
    /// 与SkipList的区别在于，可由T对象中的任意元素K作为comparer；
    /// </summary>
    public class SkipNodeList<T, K> : ICollection<T>
        where K : IComparable<K>
    {
        public class SkipListNode
        {
            private T value;
            private SkipListNode next;
            private SkipListNode previous;
            private SkipListNode above;
            private SkipListNode below;
            public virtual T Value { get { return value; } set { this.value = value; } }
            public SkipListNode Next { get { return next; } set { next = value; } }
            public SkipListNode Previous { get { return previous; } set { previous = value; } }
            public SkipListNode Above { get { return above; } set { above = value; } }
            public SkipListNode Below { get { return below; } set { below = value; } }
            public SkipListNode(T value)
            {
                this.value = value;
            }
            public void Dispose()
            {
                value = default(T);
                next = null;
                previous = null;
                above = null;
                previous = null;
            }
            public virtual bool IsHeader()
            {
                return this.GetType() == typeof(SkipListNodeHeader);
            }
            public virtual bool IsFooter()
            {
                return this.GetType() == typeof(SkipListNodeFooter);
            }
        }
        public class SkipListNodeHeader : SkipListNode
        {
            public SkipListNodeHeader() : base(default(T)) { }
        }
        public class SkipListNodeFooter : SkipListNode
        {
            public SkipListNodeFooter() : base(default(T)) { }
        }
        SkipListNode topLeft;
        SkipListNode bottomLeft;
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
        public SkipListNode Head { get { return bottomLeft; } }
        /// <summary>
        /// 比较器；
        /// </summary>
        Func<T, K> compareHandler;
        public SkipNodeList(Func<T, K> handler)
        {
            this.compareHandler = handler;
            topLeft = AcquireEmptyLevel();
            bottomLeft = topLeft;
            level = 1;
            nodeCount = 0;
            random = new Random();
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
                var newLevel = AcquireEmptyLevel();
                newLevel.Below = topLeft;
                topLeft.Above = newLevel;
                topLeft = newLevel;

                newLevelCount--;
                level++;
            }
            SkipListNode currentNode = topLeft;
            SkipListNode lastNodeAbove = null;
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
                    if (!currentNode.Next.IsFooter() && compareHandler(currentNode.Next.Value).CompareTo(compareHandler(value)) < 0)
                        currentNode = currentNode.Next;
                    else
                        break;// nextNode节点的值大于等于当前插入的值
                }
                var newNode = new SkipListNode(value);
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
            SkipListNode currentNode = this.Head;
            while (currentNode != null)
            {
                SkipListNode nextNode = currentNode.Next;
                if (!currentNode.IsHeader() && !currentNode.IsFooter())
                    this.Remove(currentNode);
                currentNode = nextNode;
            }
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
                {
                    array[i] = enumerator.Current;
                }
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
        public bool Remove(SkipListNode valueNode)
        {
            if (valueNode == null)
                return false;
            else
            {
                if (valueNode.Above != null)
                    valueNode = FindHighest(valueNode);
                var currentNodeDown = valueNode;
                while (currentNodeDown != null)
                {
                    var previousNode = currentNodeDown.Previous;
                    var nextNode = currentNodeDown.Next;

                    previousNode.Next = nextNode;
                    nextNode.Previous = previousNode;

                    var belowNode = currentNodeDown.Below;
                    currentNodeDown.Dispose();
                    currentNodeDown = belowNode;
                }
                nodeCount--;
                ClearEmptyLevels();
                return true;
            }
        }
        public SkipListNode Find(T value)
        {
            var foundNode = this.topLeft;
            while (foundNode != null && foundNode.Next != null)
            {
                if (!foundNode.Next.IsFooter() && compareHandler(foundNode.Next.Value).CompareTo(compareHandler(value)) < 0)
                    foundNode = foundNode.Next;
                else
                {
                    if (!foundNode.Next.IsFooter() && compareHandler(foundNode.Next.Value).CompareTo(compareHandler(value)) == 0)
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
        public SkipListNode FindHighest(SkipListNode valueNode)
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
        public virtual SkipListNode FindHighest(T value)
        {
            SkipListNode valueNode = this.Find(value);
            return this.FindHighest(valueNode);
        }
        public SkipListNode FindLowest(T value)
        {
            SkipListNode valueNode = this.Find(value);
            return this.FindLowest(valueNode);
        }
        public SkipListNode FindLowest(SkipListNode valueNode)
        {
            if (valueNode == null)
                return null;
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
        ///获取节点的高度； 
        /// </summary>
        public int GetHeight(SkipListNode valueNode)
        {
            int height = 0;
            var currentNode = valueNode;
            while (currentNode.Below != null)
            {
                currentNode = currentNode.Below;
            }
            while (currentNode != null)
            {
                height++;
                currentNode = currentNode.Above;
            }
            return height;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new SkipNodeListEnumerator(this);
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
            while (random.Next(0, 1) == 1 && level < this.level)//投掷硬币为1时增加一层
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
        SkipListNode AcquireEmptyLevel()
        {
            var header = new SkipListNodeHeader();
            var footer = new SkipListNodeFooter();
            header.Next = footer;
            footer.Previous = header;
            return header;
        }
        /// <summary>
        /// 跳表迭代对象，遍历最低一层的跳表；
        /// </summary>
        public struct SkipNodeListEnumerator : IEnumerator<T>
        {
            private SkipListNode current;
            private SkipNodeList<T, K> skipList;

            public SkipNodeListEnumerator(SkipNodeList<T, K> skipList)
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