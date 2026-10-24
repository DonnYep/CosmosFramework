using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    /// <summary>
    /// 场景句柄。
    /// <para>释放句柄时由 Provider 决定卸载语义（Addressables 走 UnloadSceneAsync，YooAsset 走 UnloadAsync）。</para>
    /// </summary>
    public sealed class SceneHandle : HandleBase
    {
        readonly TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        Action<SceneHandle> completed;

        /// <summary>
        /// 场景对象
        /// </summary>
        public Scene SceneObject
        {
            get { return IsValid ? Operation.SceneObject : default; }
        }
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName
        {
            get { return IsValid ? Operation.SceneName : string.Empty; }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<SceneHandle> Completed
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
        public new Task<bool> Task
        {
            get { return taskCompletionSource.Task; }
        }

        /// <summary>
        /// 创建场景句柄（供 Provider 与资源桥使用）。
        /// </summary>
        public SceneHandle(ResourceProviderOperation operation) : base(operation)
        {
        }

        /// <summary>
        /// 同步等待加载完成。
        /// </summary>
        public Scene WaitForCompletion()
        {
            PumpUntilDone();
            return SceneObject;
        }

        /// <summary>
        /// 卸载场景（等同于 Release）。
        /// </summary>
        public void Unload()
        {
            Release();
        }

        internal override void InvokeCallback()
        {
            if (IsValid && Operation.Status == OperationStatus.Succeeded)
                taskCompletionSource.TrySetResult(true);
            else
                taskCompletionSource.TrySetException(new InvalidOperationException((IsValid ? Operation.Error : null) ?? "scene load failed"));
            completed?.Invoke(this);
        }
    }
}
