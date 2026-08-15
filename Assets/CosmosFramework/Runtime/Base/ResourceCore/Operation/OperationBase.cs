using System;
using System.Collections;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 框架内部一步操作基类。
    /// <para>异步操作核心基类，驱动方式：</para>
    /// <para>1、通过 OperationSystem.StartOperation 注册后由 OperationDriver 每帧驱动；</para>
    /// <para>2、支持协程等待（yield return）、事件回调（Completed）、async/await（Task）、同步等待；</para>
    /// <para>3、支持优先级排序与手动终止（Abort）。</para>
    /// </summary>
    public abstract class OperationBase : IEnumerator, IComparable<OperationBase>
    {
        /// <summary>
        /// 是否已经完成（内部驱动标志）
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
        /// 优先级，数值越大越先执行
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

        #region 生命周期（由 OperationSystem 同程序集内调用）
        internal void InvokeStart()
        {
            Status = OperationStatus.Processing;
            OnStart();
        }
        internal void InvokeUpdate()
        {
            if (IsDone == false)
                OnUpdate();
        }
        internal void InvokeAbort()
        {
            if (!IsDone)
            {
                Status = OperationStatus.Failed;
                Error = "user abort";
                OnAbort();
            }
        }
        internal void InvokeFinish()
        {
            IsFinish = true;
            Progress = 1f;
            completed?.Invoke(this);
            OnFinish();
            if (taskCompletionSource != null)
                taskCompletionSource.TrySetResult(null);
        }
        #endregion

        #region 子类钩子（跨程序集可继承）
        /// <summary>
        /// 操作开始时调用
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// 操作进行中每帧调用
        /// </summary>
        protected virtual void OnUpdate() { }
        /// <summary>
        /// 操作被终止时调用
        /// </summary>
        protected virtual void OnAbort() { }
        /// <summary>
        /// 操作完成时调用
        /// </summary>
        protected virtual void OnFinish() { }
        #endregion

        /// <summary>
        /// 手动终止操作。
        /// <para>仅当操作尚未完成时有效，终止后状态置为 Failed。</para>
        /// </summary>
        public void Abort()
        {
            InvokeAbort();
        }
        /// <summary>
        /// 清除完成回调
        /// </summary>
        protected void ClearCompleteCallback()
        {
            completed = null;
        }
    }
}
