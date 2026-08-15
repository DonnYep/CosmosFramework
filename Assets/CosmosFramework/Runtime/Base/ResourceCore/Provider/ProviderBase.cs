using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资产加载方式
    /// </summary>
    public enum AssetLoadMode
    {
        /// <summary>
        /// 单个资产
        /// </summary>
        SingleAsset,
        /// <summary>
        /// 资产及其子资产
        /// </summary>
        SubAssets,
        /// <summary>
        /// 包体内全部资产
        /// </summary>
        AllAssets,
        /// <summary>
        /// 场景
        /// </summary>
        Scene,
    }

    /// <summary>
    /// 资产提供器基类。
    /// <para>一个Provider对应一份资产加载流程，可被多个Handle共享引用，引用计数归零后自动销毁并释放资源。</para>
    /// </summary>
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
        /// 加载的资源类型
        /// </summary>
        public Type AssetType { get; protected set; }
        /// <summary>
        /// 加载方式
        /// </summary>
        public AssetLoadMode LoadMode { get; protected set; }
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
        /// 加载耗时（秒），完成时填充
        /// </summary>
        public float LoadDurationSeconds { get; private set; } = 0;
        long loadStartTicks = 0;
        /// <summary>
        /// 当前步骤
        /// </summary>
        protected ProviderStep currentStep = ProviderStep.None;
        /// <summary>
        /// 是否正在等待异步操作完成
        /// </summary>
        protected bool IsWaitForAsyncComplete { get; private set; } = false;

        readonly List<HandleBase> handles = new List<HandleBase>();

        /// <summary>
        /// 提供器销毁事件
        /// </summary>
        internal event Action OnProviderDestroy;

        public void AddHandle(HandleBase handle)
        {
            if (RefCount == 0)
            {
                loadStartTicks = DateTime.UtcNow.Ticks;
                ResourceDiagnostics.OnProviderStart(this);
            }
            RefCount++;
            handles.Add(handle);
        }
        public void ReleaseHandle(HandleBase handle)
        {
            if (RefCount <= 0)
                return;
            handles.Remove(handle);
            RefCount--;
            if (RefCount <= 0)
                DestroyProvider();
        }
        /// <summary>
        /// 释放全部句柄引用
        /// </summary>
        public void ReleaseAllHandles()
        {
            if (RefCount <= 0)
                return;
            var count = handles.Count;
            for (int i = 0; i < count; i++)
            {
                handles[i].SetProviderNull();
            }
            handles.Clear();
            RefCount = 0;
            DestroyProvider();
        }
        void DestroyProvider()
        {
            if (IsDestroyed)
                return;
            IsDestroyed = true;
            OnProviderDestroy?.Invoke();
            OnProviderDestroy = null;
            if (!IsDone)
                Abort();
            OnDestroy();
            handles.Clear();
            ResourceDiagnostics.OnProviderDestroy(this);
        }
        /// <summary>
        /// 提供器被销毁时释放所占用的资源
        /// </summary>
        protected virtual void OnDestroy() { }
        protected void SetWaitForAsyncComplete()
        {
            IsWaitForAsyncComplete = true;
        }
        protected void ClearWaitForAsyncComplete()
        {
            IsWaitForAsyncComplete = false;
        }
        /// <summary>
        /// 结束流程
        /// </summary>
        protected virtual void InvokeCompletion(string error, OperationStatus status)
        {
            currentStep = ProviderStep.Done;
            Error = error;
            Status = status;
            Progress = 1;
            if (loadStartTicks != 0)
                LoadDurationSeconds = (DateTime.UtcNow.Ticks - loadStartTicks) / (float)TimeSpan.TicksPerSecond;
            ClearWaitForAsyncComplete();
            ResourceDiagnostics.OnProviderFinish(this, status == OperationStatus.Succeeded, error);
            var tempHandles = new List<HandleBase>(handles);
            var count = tempHandles.Count;
            for (int i = 0; i < count; i++)
            {
                var handle = tempHandles[i];
                if (handle.IsValid)
                    handle.InvokeCallback();
            }
        }
        internal abstract void WaitForCompletion();
    }
}
