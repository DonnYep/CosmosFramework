using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 实例化句柄。
    /// <para>仅支持 <see cref="ResourceCapabilities.Instance"/> 的 Provider 可用（如 Addressables 的 InstantiateAsync）；</para>
    /// <para>释放句柄即销毁实例（底层若带对象池则归还池中）。</para>
    /// </summary>
    public sealed class GameObjectHandle : HandleBase
    {
        readonly TaskCompletionSource<GameObject> taskCompletionSource = new TaskCompletionSource<GameObject>();
        Action<GameObjectHandle> completed;

        /// <summary>
        /// 实例化出的对象
        /// </summary>
        public GameObject Instance
        {
            get { return IsValid ? Operation.InstanceObject : null; }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<GameObjectHandle> Completed
        {
            add
            {
                if (IsDone)
                    value?.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }
        /// <summary>
        /// 异步任务，支持 await
        /// </summary>
        public new Task<GameObject> Task
        {
            get { return taskCompletionSource.Task; }
        }

        /// <summary>
        /// 创建实例化句柄（供 Provider 与资源桥使用）。
        /// </summary>
        public GameObjectHandle(ResourceProviderOperation operation) : base(operation)
        {
        }

        /// <summary>
        /// 同步等待完成。
        /// </summary>
        public GameObject WaitForCompletion()
        {
            PumpUntilDone();
            return Instance;
        }

        internal override void InvokeCallback()
        {
            if (IsValid && Operation.Status == OperationStatus.Succeeded)
                taskCompletionSource.TrySetResult(Instance);
            else
                taskCompletionSource.TrySetException(new InvalidOperationException((IsValid ? Operation.Error : null) ?? "instance load failed"));
            completed?.Invoke(this);
        }
    }
}
