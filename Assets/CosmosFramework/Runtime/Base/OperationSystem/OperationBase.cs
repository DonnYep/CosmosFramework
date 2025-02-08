using System;
using System.Collections;
using System.Threading.Tasks;
namespace Cosmos
{
    /// <summary>
    /// 框架内部一步操作基类
    /// </summary>
    public abstract class OperationBase : IEnumerator, IComparable<OperationBase>
    {
        /// <summary>
        /// 是否已经完成
        /// </summary>
        internal bool IsFinish = false;
        /// <summary>
        /// 状态
        /// </summary>
        public OperationStatus Status { get; protected set; } = OperationStatus.None;
        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool IsDone
        {
            get
            {
                return Status == OperationStatus.Failed
                    || Status == OperationStatus.Succeeded;
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public uint Priority { set; get; } = 0;
        // 进度，值范围[0,1]。
        float progress;
        /// <summary>
        /// 进度，值范围[0,1]。
        /// </summary>
        public float Progress
        {
            get { return progress; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1f)
                    value = 1;
                progress = value;
            }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; protected set; }
        // 异步操作完成回调事件
        Action<OperationBase> completed;
        /// <summary>
        /// 异步操作完成回调事件
        /// </summary>
        public event Action<OperationBase> Completed
        {
            add
            {
                if (IsDone)
                    value.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }

        #region Async
        object IEnumerator.Current => null;
        private TaskCompletionSource<object> taskCompletionSource;
        /// <summary>
        /// 异步操作任务
        /// </summary>
        public Task Task
        {
            get
            {
                if (taskCompletionSource == null)
                {
                    taskCompletionSource = new TaskCompletionSource<object>();
                    if (IsDone)
                        taskCompletionSource.SetResult(null);
                }
                return taskCompletionSource.Task;
            }
        }
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }
        void IEnumerator.Reset()
        {
        }
        #endregion
        #region Sort
        public int CompareTo(OperationBase other)
        {
            return other.Priority.CompareTo(this.Priority);
        }
        #endregion
        internal abstract void InternalOnStart();
        internal abstract void InternalOnUpdate();
        internal virtual void InternalOnAbort() { }
        internal virtual void InternalOnFinish() { }
        internal void SetStart()
        {
            Status = OperationStatus.Processing;
            InternalOnStart();
        }
        internal void SetFinish()
        {
            IsFinish = true;
            Progress = 1f;
            completed?.Invoke(this);
            InternalOnFinish();
            if (taskCompletionSource != null)
                taskCompletionSource.TrySetResult(null);
        }
        internal void SetAbort()
        {
            if (!IsDone)
            {
                Status = OperationStatus.Failed;
                Error = "user abort";
                InternalOnAbort();
            }
        }
        protected void ClearCompleteCallback()
        {
            completed = null;
        }
    }
}