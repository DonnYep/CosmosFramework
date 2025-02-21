using System.Collections.Generic;

namespace Cosmos.Resource
{
    internal abstract class ProviderBase : OperationBase
    {
        protected enum ProviderStep
        {
            None = 0,
            /// <summary>
            /// 检测AB包
            /// </summary>
            CheckBundle,
            /// <summary>
            /// 加载中
            /// </summary>
            Loading,
            /// <summary>
            /// 检测加载结果
            /// </summary>
            Checking,
            /// <summary>
            /// 步骤完成
            /// </summary>
            Done,
        }
        /// <summary>
        /// 资源信息
        /// </summary>
        public AssetInfo MainAssetInfo { get; protected set; }
        /// <summary>
        /// 获取的资源对象
        /// </summary>
        public UnityEngine.Object AssetObject { get; protected set; }
        /// <summary>
        /// 获取的资源对象集合
        /// </summary>
        public UnityEngine.Object[] AllAssetObjects { get; protected set; }
        /// <summary>
        /// 获取的场景对象
        /// </summary>
        public UnityEngine.SceneManagement.Scene SceneObject { get; protected set; }
        /// <summary>
        /// 加载的场景名称
        /// </summary>
        public string SceneName { get; protected set; }
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount { get; private set; } = 0;
        /// <summary>
        /// 是否已经销毁
        /// </summary>
        public bool IsDestroyed { get; private set; } = false;
        /// <summary>
        /// 当前步骤
        /// </summary>
        protected ProviderStep currentStep = ProviderStep.None;
        protected bool IsWaitForAsyncComplete { get; private set; } = false;

        private readonly List<HandleBase> handles = new List<HandleBase>();
        public T CreateHandle<T, K>() where T : HandleBase
            where K : UnityEngine.Object
        {
            RefCount++;
            HandleBase handle = default;
            if (typeof(T) == typeof(AssetHandle<>))
                handle = new AssetHandle<K>(this);
            handles.Add(handle);
            return handle as T;
        }
        public void ReleaseHandle(HandleBase handle)
        {
            if (RefCount < 0)
                return;
            handles.Remove(handle);
            // 引用计数减少
            RefCount--;
        }
        internal abstract void WaitForCompletion();
        /// <summary>
        /// 结束流程
        /// </summary>
        protected virtual void InvokeCompletion(string error, OperationStatus status)
        {
            currentStep = ProviderStep.Done;
            Error = error;
            Status = status;

            List<HandleBase> tempHandes = new List<HandleBase>(handles);
            foreach (var hande in tempHandes)
            {
                if (hande.IsValid)
                {
                    hande.InvokeCallback();
                }
            }
        }
    }
}
