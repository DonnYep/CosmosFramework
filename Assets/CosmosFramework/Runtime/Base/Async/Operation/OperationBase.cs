using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 框架通用异步操作基类（与任何业务模块无关）。
    /// <para>所在程序集：<c>Cosmos.Async</c>（<c>Runtime/Base/Async</c>），不引用任何模块，也不依赖资源桥；
    /// 资源桥、WebRequest、以及任意自定义异步逻辑都继承它。</para>
    /// <para>驱动方式：</para>
    /// <para>1、<see cref="OperationSystem.StartOperation"/> 注册后由 <see cref="OperationDriver"/> 每帧推进；</para>
    /// <para>2、支持协程等待（<c>yield return operation</c>）、完成事件（<see cref="Completed"/>）、
    /// <c>async/await</c>（<see cref="GetAwaiter"/> 与 <see cref="Task"/>）、同步等待；</para>
    /// <para>3、支持优先级排序（数值越大队列越靠前）与手动终止（<see cref="Abort"/>）。</para>
    /// <para>子类只需在 <see cref="OnStart"/>/<see cref="OnUpdate"/>/<see cref="OnFinish"/>/<see cref="OnAbort"/>
    /// 里推进状态；把 <see cref="Status"/> 置为 Succeeded/Failed 即视为完成。</para>
    /// </summary>
    public abstract class OperationBase : IEnumerator, IComparable<OperationBase>
    {
        /// <summary>
        /// 是否已经完成（内部驱动标志，由 OperationSystem 维护）
        /// </summary>
        internal bool IsFinish = false;
        /// <summary>
        /// 状态
        /// </summary>
        public OperationStatus Status { get; protected set; } = OperationStatus.None;
        /// <summary>
        /// 是否已经完成（成功或失败）
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
        /// 是否被用户终止（<see cref="Abort"/>）；与"业务失败"区分开
        /// </summary>
        public bool WasAborted { get; private set; }
        /// <summary>
        /// 操作 Id（由 <see cref="Cosmos.Operation.OperationSystem"/> 登记时分配，0 表示未登记）
        /// <para>用于诊断：<c>OperationSystem.TryGetOperation(id, out var op)</c></para>
        /// </summary>
        public int OperationId { get; internal set; }
        /// <summary>
        /// 描述（调试用；等待类操作会自动填，例如 "wait until"）
        /// </summary>
        public string Description { get; set; }
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
        /// 错误信息（Status 为 Failed 时有效）
        /// </summary>
        public string Error { get; protected set; }
        // 异步操作完成回调事件
        Action<OperationBase> completed;
        /// <summary>
        /// 异步操作完成回调事件
        /// <para>已完成的操作为"立即回调"语义，因此不必担心订阅晚于完成。</para>
        /// </summary>
        public event Action<OperationBase> Completed
        {
            add
            {
                if (value == null)
                    return;
                if (IsDone)
                    value.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }

        #region 等待方式

        /// <summary>
        /// 协程等待：<c>yield return operation;</c>
        /// </summary>
        object IEnumerator.Current { get { return null; } }
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }
        void IEnumerator.Reset()
        {
        }
        /// <summary>
        /// await 支持：<c>OperationBase done = await operation;</c>
        /// <para>只表示"结束"，不区分成功失败：结束后请检查 <see cref="Status"/>/<see cref="Error"/>；
        /// 需要异常语义请改用 <see cref="Task"/>。</para>
        /// </summary>
        public OperationAwaiter GetAwaiter()
        {
            return new OperationAwaiter(this);
        }
        TaskCompletionSource<object> taskCompletionSource;
        /// <summary>
        /// 任务（Task 语义：完成即 SetResult，失败不抛异常，需自行检查 Status）
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
        #endregion

        #region 排序
        /// <inheritdoc/>
        public int CompareTo(OperationBase other)
        {
            if (other == null)
                return -1;
            return other.Priority.CompareTo(this.Priority);
        }
        #endregion

        #region 生命周期（仅由 OperationSystem 调用）
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
            if (IsDone)
                return;
            Status = OperationStatus.Failed;
            Error = "user abort";
            WasAborted = true;
            try
            {
                OnAbort();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            //终止立即完成：Completed/OnFinish/Task 都在 Abort() 当次调用内收尾，不推迟到下一帧
            FinishCore(false);
        }
        /// <summary>
        /// 由 OperationSystem 在 OnUpdate 抛出异常时调用：把操作置为失败，避免一个坏操作卡住整条异步管线
        /// </summary>
        /// <param name="exception">异常</param>
        internal void InvokeFail(Exception exception)
        {
            if (IsDone)
                return;
            Status = OperationStatus.Failed;
            Error = exception == null ? "unknown error" : exception.Message;
            if (exception != null)
                UnityEngine.Debug.LogException(exception);
        }
        internal void InvokeFinish()
        {
            FinishCore(true);
        }
        /// <summary>
        /// 收尾（只执行一次）：完成回调 → OnFinish → Task
        /// </summary>
        /// <param name="setProgress">是否把进度补到1（终止不收尾进度）</param>
        void FinishCore(bool setProgress)
        {
            if (IsFinish)
                return;
            IsFinish = true;
            if (setProgress)
                Progress = 1f;
            var handler = completed;
            completed = null;
            try
            {
                handler?.Invoke(this);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                OnFinish();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
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
        /// <para>仅当操作尚未完成时有效：状态置为 Failed、<see cref="WasAborted"/> 为 true，
        /// 并在本次调用内立即触发 <see cref="Completed"/> 与 <see cref="OnFinish"/>（不推迟到下一帧）。</para>
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
        /// <inheritdoc/>
        public override string ToString()
        {
            var description = string.IsNullOrEmpty(Description) ? GetType().Name : Description;
            return $"Operation[{OperationId}] {description} ({Status})";
        }
    }
    /// <summary>
    /// <see cref="OperationBase"/> 的 awaiter：让 <c>await operation</c> 直接可用。
    /// <para>不抛异常（与 <see cref="OperationBase.Task"/> 一致）；结束后读取 Status/Error 判断结果。</para>
    /// </summary>
    public readonly struct OperationAwaiter : INotifyCompletion
    {
        readonly OperationBase operation;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="operation">异步操作</param>
        public OperationAwaiter(OperationBase operation)
        {
            this.operation = operation;
        }
        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted
        {
            get { return operation == null || operation.IsDone; }
        }
        /// <inheritdoc/>
        public void OnCompleted(Action continuation)
        {
            if (continuation == null)
                return;
            if (operation == null || operation.IsDone)
            {
                continuation();
                return;
            }
            Action<OperationBase> handler = null;
            handler = op =>
            {
                op.Completed -= handler;
                continuation();
            };
            operation.Completed += handler;
        }
        /// <summary>
        /// 取结果（返回操作自身，便于读取 Status/Error/Progress）
        /// </summary>
        /// <returns>异步操作</returns>
        public OperationBase GetResult()
        {
            return operation;
        }
        /// <summary>
        /// 把 awaiter 作为协程指令使用（保持与旧代码 <c>yield return operation</c> 一致）
        /// </summary>
        /// <returns>异步操作</returns>
        public OperationBase GetOperation()
        {
            return operation;
        }
    }
}
