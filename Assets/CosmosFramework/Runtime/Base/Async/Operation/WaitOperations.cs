using System;
using Cosmos.Operation;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 条件等待操作：每帧检查一次条件，为 true 时成功；可设置超时。
    /// <para>替代旧的 <c>FutureTask</c>：同样是"轮询条件 + 可 await"，但纳入统一的
    /// <see cref="OperationSystem"/>（优先级/进度/取消/诊断一致），不再需要独立的 Monitor 与 Update 循环。</para>
    /// </summary>
    public sealed class WaitUntilOperation : OperationBase
    {
        readonly Func<bool> condition;
        readonly float timeoutSeconds;
        float elapsedSeconds;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="condition">完成条件</param>
        /// <param name="description">描述（调试用）</param>
        /// <param name="timeoutSeconds">超时时间（秒，&lt;=0 表示不超时）</param>
        public WaitUntilOperation(Func<bool> condition, string description = null, float timeoutSeconds = 0f)
        {
            this.condition = condition;
            this.timeoutSeconds = timeoutSeconds < 0f ? 0f : timeoutSeconds;
            Description = description ?? "wait until";
        }
        /// <summary>
        /// 已等待时长（秒）
        /// </summary>
        public float ElapsedSeconds { get { return elapsedSeconds; } }
        protected override void OnStart()
        {
            if (condition == null)
            {
                Status = OperationStatus.Failed;
                Error = "condition is null";
            }
        }
        protected override void OnUpdate()
        {
            elapsedSeconds += Time.unscaledDeltaTime;
            bool done;
            try
            {
                done = condition();
            }
            catch (Exception e)
            {
                Status = OperationStatus.Failed;
                Error = $"condition threw : {e.Message}";
                return;
            }
            if (done)
            {
                Status = OperationStatus.Succeeded;
                return;
            }
            if (timeoutSeconds > 0f && elapsedSeconds >= timeoutSeconds)
            {
                Status = OperationStatus.Failed;
                Error = $"wait until timeout : {timeoutSeconds}s";
            }
        }
    }
    /// <summary>
    /// 定时等待操作：到点成功。
    /// </summary>
    public sealed class WaitForSecondsOperation : OperationBase
    {
        readonly float duration;
        readonly bool unscaled;
        float elapsedSeconds;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="seconds">时长（秒）</param>
        /// <param name="unscaled">是否忽略 timeScale</param>
        public WaitForSecondsOperation(float seconds, bool unscaled = false)
        {
            duration = seconds < 0f ? 0f : seconds;
            this.unscaled = unscaled;
            Description = unscaled ? $"wait {duration}s (unscaled)" : $"wait {duration}s";
        }
        protected override void OnStart()
        {
            if (duration <= 0f)
            {
                Progress = 1f;
                Status = OperationStatus.Succeeded;
            }
        }
        protected override void OnUpdate()
        {
            elapsedSeconds += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            Progress = duration <= 0f ? 1f : elapsedSeconds / duration;
            if (elapsedSeconds >= duration)
                Status = OperationStatus.Succeeded;
        }
    }
    /// <summary>
    /// 等到下一帧（以启动时的 <see cref="Time.frameCount"/> 为界，保证至少跨越一次帧号变化）。
    /// </summary>
    public sealed class NextFrameOperation : OperationBase
    {
        int startFrame;
        public NextFrameOperation()
        {
            Description = "next frame";
        }
        protected override void OnStart()
        {
            startFrame = Time.frameCount;
        }
        protected override void OnUpdate()
        {
            if (Time.frameCount > startFrame)
                Status = OperationStatus.Succeeded;
        }
    }
    /// <summary>
    /// 等一个协程结束：协程由框架的驱动组件（<see cref="OperationDriver"/>）执行，
    /// 因此不需要业务自己找 MonoBehaviour；操作被 Abort 时会一并 StopCoroutine。
    /// </summary>
    public sealed class WaitForCoroutineOperation : OperationBase
    {
        readonly System.Collections.IEnumerator routine;
        Coroutine coroutine;
        bool finished;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="routine">协程</param>
        public WaitForCoroutineOperation(System.Collections.IEnumerator routine)
        {
            this.routine = routine;
            Description = "coroutine";
        }
        protected override void OnStart()
        {
            if (routine == null)
            {
                Status = OperationStatus.Failed;
                Error = "routine is null";
                return;
            }
            coroutine = OperationSystem.RunCoroutine(WrapRoutine());
            if (coroutine == null)
            {
                Status = OperationStatus.Failed;
                Error = "coroutine runner is unavailable";
            }
        }
        protected override void OnUpdate()
        {
            if (finished)
                Status = OperationStatus.Succeeded;
        }
        protected override void OnAbort()
        {
            StopRunner();
        }
        protected override void OnFinish()
        {
            StopRunner();
        }
        System.Collections.IEnumerator WrapRoutine()
        {
            yield return routine;
            finished = true;
        }
        void StopRunner()
        {
            if (coroutine == null)
                return;
            OperationSystem.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    /// <summary>
    /// 等一个 <see cref="UnityEngine.AsyncOperation"/>（场景加载、资源请求等）。
    /// <para>用轮询 <c>isDone</c> 实现，不占用协程；进度同步到 <see cref="OperationBase.Progress"/>。</para>
    /// </summary>
    public sealed class WaitForAsyncOperation : OperationBase
    {
        readonly UnityEngine.AsyncOperation operation;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="operation">Unity 异步操作</param>
        public WaitForAsyncOperation(UnityEngine.AsyncOperation operation)
        {
            this.operation = operation;
            Description = "unity async operation";
        }
        protected override void OnStart()
        {
            if (operation == null)
            {
                Status = OperationStatus.Failed;
                Error = "async operation is null";
            }
        }
        protected override void OnUpdate()
        {
            Progress = operation.progress;
            if (operation.isDone)
                Status = OperationStatus.Succeeded;
        }
    }
    /// <summary>
    /// 等一个 <see cref="System.Threading.Tasks.Task"/>：在主线程轮询，完成后主线程继续。
    /// <para>失败/取消转成操作失败（Error 带原因），异常不会抛进主线程。</para>
    /// </summary>
    public sealed class WaitForTaskOperation : OperationBase
    {
        readonly System.Threading.Tasks.Task task;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="task">任务</param>
        public WaitForTaskOperation(System.Threading.Tasks.Task task)
        {
            this.task = task;
            Description = "task";
        }
        protected override void OnStart()
        {
            if (task == null)
            {
                Status = OperationStatus.Failed;
                Error = "task is null";
            }
        }
        protected override void OnUpdate()
        {
            if (!task.IsCompleted)
                return;
            if (task.IsFaulted)
            {
                var exception = task.Exception;
                Status = OperationStatus.Failed;
                Error = exception != null ? exception.GetBaseException().Message : "task faulted";
                return;
            }
            if (task.IsCanceled)
            {
                Status = OperationStatus.Failed;
                Error = "task canceled";
                return;
            }
            Status = OperationStatus.Succeeded;
        }
    }
}
