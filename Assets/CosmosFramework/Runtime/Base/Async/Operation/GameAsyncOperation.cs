using Cosmos.Operation;

namespace Cosmos
{
    /// <summary>
    /// 开放的用于非框架级别的自定义异步操作。
    /// <para>属于通用异步底座（与资源无关）：继承后实现 OnOpStart / OnOpUpdate / OnOpFinish / OnOpAbort 即可，
    /// 由 <see cref="OperationSystem"/> 每帧驱动，支持 Completed 事件、协程等待、async/await 与优先级。</para>
    /// <code>
    /// public class WaitOneSecondOp : GameAsyncOperation
    /// {
    ///     float elapsed;
    ///     protected override void OnOpStart() { }
    ///     protected override void OnOpUpdate()
    ///     {
    ///         elapsed += UnityEngine.Time.deltaTime;
    ///         Progress = elapsed;
    ///         if (elapsed >= 1f)
    ///             Status = OperationStatus.Succeeded;
    ///     }
    ///     protected override void OnOpFinish() { }
    ///     protected override void OnOpAbort() { }
    /// }
    /// </code>
    /// </summary>
    public abstract class GameAsyncOperation : OperationBase
    {
        protected override sealed void OnAbort()
        {
            OnOpAbort();
        }
        protected override sealed void OnFinish()
        {
            OnOpFinish();
        }
        protected override sealed void OnStart()
        {
            OnOpStart();
        }
        protected override sealed void OnUpdate()
        {
            OnOpUpdate();
        }
        protected abstract void OnOpAbort();
        protected abstract void OnOpFinish();
        protected abstract void OnOpStart();
        protected abstract void OnOpUpdate();
        /// <summary>
        /// 开始驱动该操作。
        /// </summary>
        protected void StartOperation(GameAsyncOperation asyncOp)
        {
            OperationSystem.StartOperation(asyncOp);
        }
    }
}
