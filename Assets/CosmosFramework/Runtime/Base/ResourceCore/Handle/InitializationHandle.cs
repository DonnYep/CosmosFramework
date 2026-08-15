using Cosmos.Operation;
using System;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 初始化句柄。
    /// </summary>
    public class InitializationHandle
    {
        readonly InitializationOperation operation;
        public InitializationOperation Operation
        {
            get { return operation; }
        }
        public bool IsDone
        {
            get { return operation.IsDone; }
        }
        public float Progress
        {
            get { return operation.Progress; }
        }
        public string Version
        {
            get { return operation.Version; }
        }
        public string PackageName
        {
            get { return operation.PackageName; }
        }
        public string Error
        {
            get { return operation.Error; }
        }
        /// <summary>
        /// 异步任务，支持await
        /// </summary>
        public Task Task
        {
            get { return operation.Task; }
        }
        internal InitializationHandle(InitializationOperation operation)
        {
            this.operation = operation;
        }
        /// <summary>
        /// 同步等待初始化完成
        /// </summary>
        public void WaitForCompletion()
        {
            int frame = 1000;
            while (!operation.IsDone && frame-- > 0)
            {
                OperationSystem.Update();
            }
        }
        /// <summary>
        /// 添加完成回调
        /// </summary>
        public void AddCompletedCallback(Action<InitializationOperation> callback)
        {
            operation.Completed += op => callback?.Invoke(op as InitializationOperation);
        }
    }
}
