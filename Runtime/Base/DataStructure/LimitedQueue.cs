using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 定长队列
    /// </summary>
    /// <typeparam name="T">约束类型</typeparam>
    public class LimitedQueue<T> : Queue<T>
    {
        int limitCount;
        /// <summary>
        /// 限度的长度
        /// </summary>
        public int LimitCount
        {
            get { return limitCount; }
            set
            {
                limitCount = value;
                if (limitCount < 0)
                    limitCount = 0;
            }
        }
        public LimitedQueue(int limit) : base(limit)
        {
            LimitCount = limit;
        }
        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item">入队对象</param>
        public new void Enqueue(T item)
        {
            if (Count >= LimitCount)
            {
                Dequeue();
            }

            base.Enqueue(item);
        }
    }
}
