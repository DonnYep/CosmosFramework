using Cosmos.Operation;

namespace Cosmos
{
    /// <summary>
    /// 开放的用于非框架级别的自定义异步操作
    /// </summary>
    public abstract class GameAsyncOperation: OperationBase
    {
        internal override sealed void InternalOnAbort()
        {
            OnOpAbort();
        }
        internal override sealed void InternalOnFinish()
        {
            OnOpFinish();
        }
        internal override sealed void InternalOnStart()
        {
            OnOpStart();
        }
        internal override sealed void InternalOnUpdate()
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
