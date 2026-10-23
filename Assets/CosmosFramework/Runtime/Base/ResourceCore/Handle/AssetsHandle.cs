using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 批量资产句柄。对应 Addressables 的 AsyncOperationHandle&lt;IList&lt;TObject&gt;&gt;。
    /// <para>一次请求多个资产：同一资产复用 Provider、同一包体复用 BundleLoader，</para>
    /// <para>因此一批请求里每个包体只会发起一次加载；全部子句柄完成后触发一次 Completed。</para>
    /// <code>
    /// var handle = CFResources.LoadAssetsAsync&lt;GameObject&gt;(new[] { "Prefabs/A", "Prefabs/B" });
    /// handle.Completed += h =&gt; { foreach (var asset in h.Assets) ... };
    /// yield return handle;              // 协程等待
    /// var assets = await handle.Task;   // async/await
    /// handle.Release();                 // 释放全部引用
    /// </code>
    /// <para>失败项在 Assets 中为 null，可通过 FailedCount / SucceededCount 判断部分失败。</para>
    /// </summary>
    public class AssetsHandle<T> : IEnumerator
        where T : UnityEngine.Object
    {
        readonly AssetHandle<T>[] handles;
        readonly string[] keys;
        readonly TaskCompletionSource<T[]> taskCompletionSource = new TaskCompletionSource<T[]>();
        T[] cachedAssets;
        int completedCount;
        int failedCount;
        bool released;
        Action<AssetsHandle<T>> completed;

        /// <summary>
        /// 请求的资产数量
        /// </summary>
        public int Count
        {
            get { return handles.Length; }
        }
        /// <summary>
        /// 请求的资产标识（按 key 加载时有值，按标签加载时为 null）
        /// </summary>
        public string[] Keys
        {
            get { return keys; }
        }
        /// <summary>
        /// 是否全部完成
        /// </summary>
        public bool IsDone
        {
            get { return completedCount >= handles.Length; }
        }
        /// <summary>
        /// 整体状态（全部完成后有效；部分失败时仍为 Succeeded，用 FailedCount 区分）
        /// </summary>
        public OperationStatus Status { get; private set; } = OperationStatus.Processing;
        /// <summary>
        /// 加载失败的资产数量
        /// </summary>
        public int FailedCount
        {
            get { return failedCount; }
        }
        /// <summary>
        /// 加载成功的资产数量
        /// </summary>
        public int SucceededCount
        {
            get { return handles.Length - failedCount; }
        }
        /// <summary>
        /// 聚合进度（子句柄进度平均值）
        /// </summary>
        public float Progress
        {
            get
            {
                if (handles.Length == 0)
                    return 1;
                float total = 0;
                for (int i = 0; i < handles.Length; i++)
                {
                    total += handles[i].Progress;
                }
                return total / handles.Length;
            }
        }
        /// <summary>
        /// 加载结果（全部完成后有效）；失败位置为 null
        /// </summary>
        public T[] Assets
        {
            get
            {
                if (!IsDone)
                    return null;
                if (cachedAssets == null)
                {
                    cachedAssets = new T[handles.Length];
                    for (int i = 0; i < handles.Length; i++)
                    {
                        cachedAssets[i] = handles[i].Asset;
                    }
                }
                return cachedAssets;
            }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<AssetsHandle<T>> Completed
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
        /// 异步任务，支持await
        /// </summary>
        public Task<T[]> Task
        {
            get { return taskCompletionSource.Task; }
        }

        internal AssetsHandle(AssetHandle<T>[] handles, string[] keys)
        {
            this.handles = handles ?? new AssetHandle<T>[0];
            this.keys = keys;
            if (this.handles.Length == 0)
            {
                cachedAssets = new T[0];
                Status = OperationStatus.Succeeded;
                taskCompletionSource.TrySetResult(cachedAssets);
                return;
            }
            for (int i = 0; i < this.handles.Length; i++)
            {
                var handle = this.handles[i];
                if (handle == null)
                {
                    // 理论上不会出现，保底计入失败，避免永远不完成
                    OnChildCompleted(null);
                    continue;
                }
                handle.Completed += OnChildCompleted;
            }
        }

        /// <summary>
        /// 同步等待全部加载完成（注意会阻塞主线程）
        /// </summary>
        public T[] WaitForCompletion()
        {
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i]?.WaitForCompletion();
            }
            return Assets;
        }

        /// <summary>
        /// 释放全部子句柄（引用计数减一）；重复调用只生效一次
        /// </summary>
        public void Release()
        {
            if (released)
                return;
            released = true;
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i]?.Release();
            }
        }

        void OnChildCompleted(AssetHandle<T> handle)
        {
            if (handle != null)
            {
                handle.Completed -= OnChildCompleted;
                if (handle.Asset == null)
                    failedCount++;
            }
            completedCount++;
            if (!IsDone)
                return;
            Status = failedCount >= handles.Length ? OperationStatus.Failed : OperationStatus.Succeeded;
            // 完成时固化结果，避免 Release 之后再访问子句柄
            var assets = Assets;
            taskCompletionSource.TrySetResult(assets);
            completed?.Invoke(this);
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
            get { return null; }
        }
        #endregion
    }
}
