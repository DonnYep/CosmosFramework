using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    /// <summary>
    /// Provider 加载操作基类。<para>一个操作对应一次底层加载流程，可被多个句柄共享；引用计数归零后自动释放底层资源。</para>
    /// <para>适配器只需实现三个钩子：</para>
    /// <para>· <see cref="OnStartLoad"/>：发起三方方案的加载请求；</para>
    /// <para>· <see cref="OnUpdateLoad"/>：轮询/推进进度（回调式方案可留空）；</para>
    /// <para>· <see cref="OnRelease"/>：释放底层句柄或卸载资源。</para>
    /// <para>写回结果使用 SetAsset / SetAssets / SetInstance / SetScene，并调用 Finish 结束流程。</para>
    /// </summary>
    public abstract class ResourceProviderOperation : OperationBase
    {
        readonly List<HandleBase> handles = new List<HandleBase>();
        long loadStartTicks;

        /// <summary>
        /// 所属 Provider
        /// </summary>
        public IResourceProvider Provider { get; private set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public ResourceRequest Request { get; private set; }
        /// <summary>
        /// 资源信息
        /// </summary>
        public AssetInfo AssetInfo
        {
            get { return Request.AssetInfo; }
        }
        /// <summary>
        /// 目标资产类型
        /// </summary>
        public Type AssetType
        {
            get { return Request.AssetType; }
        }
        /// <summary>
        /// 加载方式
        /// </summary>
        public AssetLoadMode LoadMode
        {
            get { return Request.LoadMode; }
        }
        /// <summary>
        /// 当前引用计数
        /// </summary>
        public int RefCount { get; private set; }
        /// <summary>
        /// 操作是否已销毁（引用计数归零或 Provider 销毁）
        /// </summary>
        public bool IsDestroyed { get; private set; }
        /// <summary>
        /// 加载到的资产
        /// </summary>
        public UnityEngine.Object AssetObject { get; protected set; }
        /// <summary>
        /// 加载到的资产集合
        /// </summary>
        public UnityEngine.Object[] AssetObjects { get; protected set; }
        /// <summary>
        /// 实例化出的对象（LoadMode 为 Instance 时）
        /// </summary>
        public GameObject InstanceObject { get; protected set; }
        /// <summary>
        /// 加载到的场景（LoadMode 为 Scene 时）
        /// </summary>
        public Scene SceneObject { get; protected set; }
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName { get; protected set; }
        /// <summary>
        /// 加载耗时（秒），完成时填充
        /// </summary>
        public float LoadDurationSeconds { get; private set; }

        /// <summary>
        /// 操作被销毁事件（资源桥据此清理名称索引）。
        /// </summary>
        public event Action<ResourceProviderOperation> Destroyed;

        protected ResourceProviderOperation(IResourceProvider provider, ResourceRequest request)
        {
            Provider = provider;
            Request = request;
            Priority = request.Priority;
        }

        #region 引用计数
        /// <summary>
        /// 增加一个句柄引用。
        /// </summary>
        public void AddHandle(HandleBase handle)
        {
            if (handle == null || IsDestroyed)
                return;
            if (RefCount == 0)
                loadStartTicks = DateTime.UtcNow.Ticks;
            RefCount++;
            handles.Add(handle);
        }
        /// <summary>
        /// 释放一个句柄引用，归零时销毁操作并释放底层资源。
        /// </summary>
        public void ReleaseHandle(HandleBase handle)
        {
            if (handle == null || RefCount <= 0)
                return;
            handles.Remove(handle);
            RefCount--;
            if (RefCount <= 0)
                DestroyOperation();
        }
        /// <summary>
        /// 释放全部引用。
        /// </summary>
        public void ReleaseAllHandles()
        {
            if (IsDestroyed)
                return;
            var count = handles.Count;
            for (int i = 0; i < count; i++)
                handles[i].SetOperationNull();
            handles.Clear();
            RefCount = 0;
            DestroyOperation();
        }
        void DestroyOperation()
        {
            if (IsDestroyed)
                return;
            IsDestroyed = true;
            var destroyed = Destroyed;
            Destroyed = null;
            destroyed?.Invoke(this);
            if (!IsDone)
            {
                // 主动释放不属于“加载失败”，但要保证等待方（协程/await）立刻结束
                Error = "operation released before completion";
                Status = OperationStatus.Failed;
                OnAbort();
            }
            try
            {
                OnRelease();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] release operation failed : {AssetInfo} : {e}");
            }
            var count = handles.Count;
            for (int i = 0; i < count; i++)
                handles[i].SetOperationNull();
            handles.Clear();
            RefCount = 0;
        }
        #endregion

        #region 结果写回（供适配器调用）
        /// <summary>
        /// 写回单个资产结果。
        /// </summary>
        protected void SetAsset(UnityEngine.Object asset)
        {
            AssetObject = asset;
        }
        /// <summary>
        /// 写回资产集合结果。
        /// </summary>
        protected void SetAssets(UnityEngine.Object[] assets)
        {
            AssetObjects = assets;
            AssetObject = assets != null && assets.Length > 0 ? assets[0] : null;
        }
        /// <summary>
        /// 写回实例化结果。
        /// </summary>
        protected void SetInstance(GameObject instance)
        {
            InstanceObject = instance;
            AssetObject = instance;
        }
        /// <summary>
        /// 写回场景结果。
        /// </summary>
        protected void SetScene(Scene scene, string sceneName)
        {
            SceneObject = scene;
            SceneName = string.IsNullOrEmpty(sceneName) ? scene.name : sceneName;
        }
        /// <summary>
        /// 更新进度。
        /// </summary>
        protected void SetProgress(float value)
        {
            Progress = value;
        }
        /// <summary>
        /// 结束加载流程。
        /// </summary>
        /// <param name="error">错误信息，为空表示成功</param>
        protected void Finish(string error = null)
        {
            if (IsDone)
                return;
            Error = error;
            Status = string.IsNullOrEmpty(error) ? OperationStatus.Succeeded : OperationStatus.Failed;
            Progress = 1f;
        }
        #endregion

        #region 适配器钩子
        /// <summary>
        /// 发起加载请求。
        /// </summary>
        protected abstract void OnStartLoad();
        /// <summary>
        /// 每帧推进（回调式方案可留空，在回调里直接写回结果并 Finish）。
        /// </summary>
        protected virtual void OnUpdateLoad() { }
        /// <summary>
        /// 释放底层资源。
        /// </summary>
        protected abstract void OnRelease();
        /// <summary>
        /// 加载被中断时的回调（可选）。
        /// </summary>
        protected virtual void OnAbortLoad() { }
        /// <summary>
        /// 流程结束后的回调（可选）。
        /// </summary>
        protected virtual void OnLoadFinished() { }
        #endregion

        #region 句柄工厂
        /// <summary>
        /// 创建资产句柄。
        /// </summary>
        public AssetHandle<T> CreateAssetHandle<T>()
            where T : UnityEngine.Object
        {
            return new AssetHandle<T>(this);
        }
        /// <summary>
        /// 创建子资产句柄。
        /// </summary>
        public SubAssetsHandle<T> CreateSubAssetsHandle<T>()
            where T : UnityEngine.Object
        {
            return new SubAssetsHandle<T>(this);
        }
        /// <summary>
        /// 创建全部资产句柄。
        /// </summary>
        public AllAssetsHandle<T> CreateAllAssetsHandle<T>()
            where T : UnityEngine.Object
        {
            return new AllAssetsHandle<T>(this);
        }
        /// <summary>
        /// 创建场景句柄。
        /// </summary>
        public SceneHandle CreateSceneHandle()
        {
            return new SceneHandle(this);
        }
        /// <summary>
        /// 创建实例化句柄。
        /// </summary>
        public GameObjectHandle CreateInstanceHandle()
        {
            return new GameObjectHandle(this);
        }
        #endregion

        #region 生命周期
        protected sealed override void OnStart()
        {
            try
            {
                OnStartLoad();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] start load failed : {AssetInfo} : {e}");
                Finish($"start load failed : {e.Message}");
            }
        }
        protected sealed override void OnUpdate()
        {
            if (IsDone)
                return;
            try
            {
                OnUpdateLoad();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] update load failed : {AssetInfo} : {e}");
                Finish($"update load failed : {e.Message}");
            }
        }
        protected sealed override void OnAbort()
        {
            try
            {
                OnAbortLoad();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] abort load failed : {AssetInfo} : {e}");
            }
        }
        protected sealed override void OnFinish()
        {
            if (loadStartTicks != 0)
                LoadDurationSeconds = (DateTime.UtcNow.Ticks - loadStartTicks) / (float)TimeSpan.TicksPerSecond;
            // 复制一份再回调：用户可能在 Completed 里直接 Release，导致 handles 在遍历中被修改
            var snapshot = new List<HandleBase>(handles);
            for (int i = 0; i < snapshot.Count; i++)
            {
                var handle = snapshot[i];
                if (handle != null && handle.IsValid)
                    handle.InvokeCallback();
            }
            try
            {
                OnLoadFinished();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] finish load failed : {AssetInfo} : {e}");
            }
        }
        #endregion
    }
}
