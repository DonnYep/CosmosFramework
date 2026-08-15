using Cosmos.Operation;

namespace Cosmos
{
    /// <summary>
    /// 开放的用于非框架级别的自定义异步操作
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
        protected void StartOperation(GameAsyncOperation asyncOp)
        {
            OperationSystem.StartOperation(asyncOp);
        }
    }
}
