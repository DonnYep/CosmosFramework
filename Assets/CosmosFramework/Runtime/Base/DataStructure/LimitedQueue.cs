using System;
using System.Collections;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    /// 定长队列（先进先出，超出限长时淘汰最旧元素）。
    /// <para>用组合而非继承 <see cref="Queue{T}"/>：避免把实例当作普通 Queue 使用时绕过限长。</para>
    /// </summary>
    /// <typeparam name="T">约束类型</typeparam>
    public class LimitedQueue<T> : IEnumerable<T>
    {
        readonly Queue<T> queue = new Queue<T>();
        int limitCount;
        /// <summary>
        /// 限度的长度；缩小时会同步裁剪已入队的数据
        /// </summary>
        public int LimitCount
        {
            get { return limitCount; }
            set
            {
                limitCount = Math.Max(0, value);
                while (queue.Count > limitCount)
                    queue.Dequeue();
            }
        }
        /// <summary>
        /// 当前元素数量。
        /// </summary>
        public int Count { get { return queue.Count; } }
        public LimitedQueue(int limit)
        {
            limitCount = Math.Max(0, limit);
        }
        /// <summary>
        /// 入队；超出限长时淘汰最旧元素。限长为 0 时直接丢弃。
        /// </summary>
        /// <param name="item">入队对象</param>
        public void Enqueue(T item)
        {
            if (limitCount <= 0)
                return;
            if (queue.Count >= limitCount)
                queue.Dequeue();
            queue.Enqueue(item);
        }
        /// <summary>
        /// 出队。
        /// </summary>
        public T Dequeue()
        {
            return queue.Dequeue();
        }
        /// <summary>
        /// 尝试出队。
        /// </summary>
        public bool TryDequeue(out T item)
        {
            return queue.TryDequeue(out item);
        }
        /// <summary>
        /// 查看队首。
        /// </summary>
        public T Peek()
        {
            return queue.Peek();
        }
        /// <summary>
        /// 尝试查看队首。
        /// </summary>
        public bool TryPeek(out T item)
        {
            return queue.TryPeek(out item);
        }
        /// <summary>
        /// 清空。
        /// </summary>
        public void Clear()
        {
            queue.Clear();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
