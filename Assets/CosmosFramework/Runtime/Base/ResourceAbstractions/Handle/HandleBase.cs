using System.Collections;
using System.Threading.Tasks;
using Cosmos.Operation;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源操作句柄基类。
    /// <para>句柄是运行时唯一的资源交互对象：支持协程等待（yield return handle）、事件回调、async/await 与同步等待。</para>
    /// <para>每个句柄持有一个引用计数，<see cref="Release"/> 归零后由所属操作释放底层资源。</para>
    /// </summary>
    public abstract class HandleBase : IEnumerator
    {
        ResourceProviderOperation operation;
        readonly AssetInfo assetInfo;

        /// <summary>
        /// 所属操作
        /// </summary>
        protected ResourceProviderOperation Operation
        {
            get { return operation; }
        }
        /// <summary>
        /// 资源信息
        /// </summary>
        public AssetInfo AssetInfo
        {
            get { return assetInfo; }
        }
        /// <summary>
        /// 资源标识
        /// </summary>
        public string Key
        {
            get { return assetInfo != null ? assetInfo.Key : null; }
        }
        /// <summary>
        /// 句柄是否有效（未释放且底层操作未销毁）
        /// </summary>
        public bool IsValid
        {
            get { return operation != null && !operation.IsDestroyed; }
        }
        /// <summary>
        /// 是否加载完毕。<para>句柄失效（被释放/操作被销毁）时视为已结束，避免协程永久挂起。</para>
        /// </summary>
        public bool IsDone
        {
            get { return !IsValid || operation.IsDone; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public OperationStatus Status
        {
            get { return IsValid ? operation.Status : OperationStatus.Failed; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error
        {
            get { return IsValid ? operation.Error : "resource handle has been released"; }
        }
        /// <summary>
        /// 加载进度
        /// </summary>
        public float Progress
        {
            get { return IsValid ? operation.Progress : 1f; }
        }
        /// <summary>
        /// 异步任务
        /// </summary>
        public Task Task
        {
            get { return operation != null ? operation.Task : null; }
        }

        protected HandleBase(ResourceProviderOperation operation)
        {
            this.operation = operation;
            assetInfo = operation != null ? operation.Request.AssetInfo : null;
            operation?.AddHandle(this);
        }

        /// <summary>
        /// 释放句柄，减少一个引用计数；归零时底层资源被释放。
        /// </summary>
        public void Release()
        {
            if (operation == null)
                return;
            var temp = operation;
            temp.ReleaseHandle(this);
            OnRelease();
            operation = null;
        }

        internal void SetOperationNull()
        {
            operation = null;
        }
        internal abstract void InvokeCallback();
        protected virtual void OnRelease() { }

        /// <summary>
        /// 同步推进直到完成（会阻塞主线程，仅建议非频繁调用）。
        /// </summary>
        protected void PumpUntilDone()
        {
            int guard = 100000;
            while (!IsDone && guard-- > 0)
                OperationSystem.Update();
        }

        #region 协程
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }
        void IEnumerator.Reset()
        {
        }
        object IEnumerator.Current
        {
            get { return operation; }
        }
        #endregion
    }
}
