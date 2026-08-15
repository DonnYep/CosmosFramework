using Cosmos.Operation;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    /// <summary>
    /// 场景句柄。
    /// </summary>
    public class SceneHandle : HandleBase
    {
        readonly TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        /// <summary>
        /// 场景对象
        /// </summary>
        public Scene SceneObject
        {
            get
            {
                if (IsValidWithWarning == false)
                    return default;
                return Provider.SceneObject;
            }
        }
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName
        {
            get
            {
                if (IsValidWithWarning == false)
                    return string.Empty;
                return Provider.SceneName;
            }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<SceneHandle> Completed
        {
            add
            {
                if (Provider != null && Provider.IsDone)
                    value?.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }
        /// <summary>
        /// 异步任务，支持await
        /// </summary>
        public new Task<bool> Task
        {
            get { return taskCompletionSource.Task; }
        }
        /// <summary>
        /// 同步等待加载完成
        /// </summary>
        public Scene WaitForCompletion()
        {
            if (IsValidWithWarning == false)
                return default;
            Provider.WaitForCompletion();
            return SceneObject;
        }
        /// <summary>
        /// 卸载场景
        /// </summary>
        public OperationBase UnloadAsync()
        {
            if (IsValidWithWarning == false)
                return null;
            var operation = new UnloadSceneOperation(SceneName);
            OperationSystem.StartOperation(operation);
            return operation;
        }
        Action<SceneHandle> completed;
        internal SceneHandle(ProviderBase provider) : base(provider)
        {
            provider.AddHandle(this);
        }
        internal override void InvokeCallback()
        {
            if (Provider.Status == OperationStatus.Succeeded)
                taskCompletionSource.TrySetResult(true);
            else
                taskCompletionSource.TrySetException(new InvalidOperationException(Provider.Error ?? "Scene load failed !"));
            completed?.Invoke(this);
        }
    }
}
