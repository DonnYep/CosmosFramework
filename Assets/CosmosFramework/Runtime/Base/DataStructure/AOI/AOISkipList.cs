using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    public class AOISkipList<T, K> : ICollection<T>
        where K : IComparable<K>
    {
        public class AOISkipListNode
        {
            private T value;
            private AOISkipListNode next;
            private AOISkipListNode previous;
            private AOISkipListNode above;
            private AOISkipListNode below;
            /// <summary>
            /// 节点所属的跳表；
            /// 用于 O(1) 判断节点归属，避免按节点删除时沿底层回溯 O(n)。
            /// </summary>
            internal AOISkipList<T, K> owner;
            public virtual T Value { get { return value; } set { this.value = value; } }
            public AOISkipListNode Next { get { return next; } set { next = value; } }
            public AOISkipListNode Previous { get { return previous; } set { previous = value; } }
            public AOISkipListNode Above { get { return above; } set { above = value; } }
            public AOISkipListNode Below { get { return below; } set { below = value; } }
            public AOISkipListNode(T value)
            {
                this.value = value;
            }
            public void Dispose()
            {
                value = default(T);
                next = null;
                previous = null;
                above = null;
                below = null;
                owner = null;
            }
            public virtual bool IsHeader()
            {
                return this.GetType() == typeof(AOISkipListNodeHeader);
            }
            public virtual bool IsFooter()
            {
                return this.GetType() == typeof(AOISkipListNodeFooter);
            }
        }
        public class AOISkipListNodeHeader : AOISkipListNode
        {
            public AOISkipListNodeHeader() : base(default(T)) { }
        }
        public class AOISkipListNodeFooter : AOISkipListNode
        {
            public AOISkipListNodeFooter() : base(default(T)) { }
        }
        AOISkipListNode topLeft;
        AOISkipListNode bottomLeft;
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
        public AOISkipListNode Head { get { return bottomLeft; } }
        /// <summary>
        /// 比较器；
        /// </summary>
        Func<T, K> handler;
        public AOISkipList(Func<T, K> handler)
        {
            this.handler = handler;
            topLeft = AcquireEmptyLevel();
            bottomLeft = topLeft;
            level = 1;
            nodeCount = 0;
            random = new Random();
        }
        /// <summary>
        /// 添加节点；
        /// </summary>
        /// <param name="value">节点值</param>
        /// <returns>插入的最底层节点；该节点可用于 O(1) 的身份化删除，返回空表示插入失败</returns>
        public AOISkipListNode Add(T value)
        {
            //添加流程：
            //1. 投掷硬币，获取到新加入节点的层级
            //2. 比较节点值，加入最下层的有序链表
            //3. 分配查询层级节点

            int valueLevel = CoinFlipLevel();
            //valueLevel 是最高层索引（0 基），因此需要的层数是 valueLevel + 1
            int newLevelCount = valueLevel + 1 - level;
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
            AOISkipListNode currentNode = topLeft;
            AOISkipListNode lastNodeAbove = null;
            //循环最后一次创建的节点即最底层节点，保存它用于身份化删除
            AOISkipListNode lowestNode = null;
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
                    if (!currentNode.Next.IsFooter() && handler(currentNode.Next.Value).CompareTo(handler(value)) < 0)
                        currentNode = currentNode.Next;
                    else
                        break;// nextNode节点的值大于等于当前插入的值
                }
                var newNode = new AOISkipListNode(value);
                newNode.owner = this;
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
                lowestNode = newNode;

                currentNode = currentNode.Below;
                remainLevel--;
            }
            nodeCount++;
            return lowestNode;
        }
        /// <summary>
        /// ICollection&lt;T&gt; 的显式实现；
        /// 泛型接口要求 Add 返回 void，这里转发到返回节点的 Add。
        /// </summary>
        void ICollection<T>.Add(T value)
        {
            Add(value);
        }
        public void Clear()
        {
            AOISkipListNode currentNode = this.Head;
            while (currentNode != null)
            {
                AOISkipListNode nextNode = currentNode.Next;
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
        public bool Remove(AOISkipListNode valueNode)
        {
            //只允许移除属于本跳表的节点，避免误删其它跳表或游离节点
            if (!IsNodeBelongs(valueNode))
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
        /// <summary>
        /// 判断节点是否属于本跳表；
        /// 节点在创建时记录了所属跳表，因此这里是 O(1) 判断
        /// </summary>
        /// <param name="node">待判断的节点</param>
        /// <returns>是否属于本跳表</returns>
        bool IsNodeBelongs(AOISkipListNode node)
        {
            if (node == null || node.IsHeader() || node.IsFooter())
                return false;
            return ReferenceEquals(node.owner, this);
        }
        public AOISkipListNode Find(T value)
        {
            var foundNode = this.topLeft;
            while (foundNode != null && foundNode.Next != null)
            {
                if (!foundNode.Next.IsFooter() && handler(foundNode.Next.Value).CompareTo(handler(value)) < 0)
                    foundNode = foundNode.Next;
                else
                {
                    if (!foundNode.Next.IsFooter() && handler(foundNode.Next.Value).CompareTo(handler(value)) == 0)
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
        public AOISkipListNode FindHighest(AOISkipListNode valueNode)
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
        public virtual AOISkipListNode FindHighest(T value)
        {
            AOISkipListNode valueNode = this.Find(value);
            return this.FindHighest(valueNode);
        }
        public AOISkipListNode FindLowest(T value)
        {
            AOISkipListNode valueNode = this.Find(value);
            return this.FindLowest(valueNode);
        }
        public AOISkipListNode FindLowest(AOISkipListNode valueNode)
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
        public int GetHeight(AOISkipListNode valueNode)
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
            int valueLevel = 0;
            //最高层索引可以比当前层数多一层（用于新增顶层），但不得超过 MaxLevels
            int maxLevelIndex = Math.Min(level, maxLevels - 1);
            while (valueLevel < maxLevelIndex && random.Next(0, 2) == 1)//投掷硬币为1时增加一层
            {
                valueLevel++;
            }
            return valueLevel;
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
        AOISkipListNode AcquireEmptyLevel()
        {
            var header = new AOISkipListNodeHeader();
            var footer = new AOISkipListNodeFooter();
            header.Next = footer;
            footer.Previous = header;
            return header;
        }
        /// <summary>
        /// 跳表迭代对象，遍历最低一层的跳表；
        /// </summary>
        public struct SkipListEnumerator : IEnumerator<T>
        {
            private AOISkipListNode current;
            private AOISkipList<T, K> skipList;

            public SkipListEnumerator(AOISkipList<T, K> skipList)
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