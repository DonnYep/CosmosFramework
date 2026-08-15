using Cosmos.Operation;
using System.Collections.Generic;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// 网络请求并发调度器。
    /// <para>控制同一时刻最多执行 MaxConcurrent 个请求，其余请求按优先级排队等待；</para>
    /// <para>请求完成或终止时自动补位调度，高优先级任务优先获得并发槽。</para>
    /// </summary>
    internal class WebRequestScheduler
    {
        /// <summary>
        /// 最大并发请求数量
        /// </summary>
        public int MaxConcurrent { get; set; } = 4;
        /// <summary>
        /// 是否暂停调度（暂停后不再派发新请求，进行中的请求继续执行）
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// 正在执行的请求数量
        /// </summary>
        public int ActiveCount
        {
            get { return activeCount; }
        }
        /// <summary>
        /// 等待中的请求数量
        /// </summary>
        public int WaitingCount
        {
            get { return waitingList.Count; }
        }

        int activeCount;
        readonly List<WebRequestHandleBase> waitingList = new List<WebRequestHandleBase>();

        /// <summary>
        /// 入队一个请求句柄（立即尝试获取并发槽）
        /// </summary>
        public void Enqueue(WebRequestHandleBase handle)
        {
            if (handle == null)
                return;
            handle.Scheduler = this;
            OperationSystem.StartOperation(handle);
            TryDispatch();
        }

        /// <summary>
        /// 尝试获取并发槽。
        /// <para>成功返回true；并发已满时进入等待队列并返回false。</para>
        /// </summary>
        public bool TryAcquireSlot(WebRequestHandleBase handle)
        {
            if (activeCount < MaxConcurrent)
            {
                activeCount++;
                return true;
            }
            if (!waitingList.Contains(handle))
            {
                waitingList.Add(handle);
                waitingList.Sort(ComparePriority);
            }
            return false;
        }

        /// <summary>
        /// 释放并发槽并补位调度
        /// </summary>
        public void ReleaseSlot()
        {
            if (activeCount > 0)
                activeCount--;
            TryDispatch();
        }

        /// <summary>
        /// 将句柄移出等待队列（请求被取消时调用）
        /// </summary>
        public void RemoveWaiting(WebRequestHandleBase handle)
        {
            waitingList.Remove(handle);
        }

        /// <summary>
        /// 取消全部等待中的请求
        /// </summary>
        public void CancelWaiting()
        {
            for (int i = waitingList.Count - 1; i >= 0; i--)
            {
                var handle = waitingList[i];
                waitingList.RemoveAt(i);
                handle.Abort();
            }
        }

        void TryDispatch()
        {
            if (Paused)
                return;
            while (activeCount < MaxConcurrent && waitingList.Count > 0)
            {
                var handle = waitingList[0];
                waitingList.RemoveAt(0);
                activeCount++;
                handle.DispatchRequest();
            }
        }

        static int ComparePriority(WebRequestHandleBase a, WebRequestHandleBase b)
        {
            return b.Priority.CompareTo(a.Priority);
        }
    }
}
