using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 批量资产句柄（一次请求多个资源标识）。
    /// <para>同一标识复用同一个操作、多个标识并行加载，全部完成后触发一次 Completed；失败项在 Assets 中为 null。</para>
    /// </summary>
    public sealed class AssetsHandle<T> : IEnumerator
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
        /// 请求的资产标识
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
        /// 整体状态（全部完成后有效）
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
        /// 聚合进度（子句柄平均值）
        /// </summary>
        public float Progress
        {
            get
            {
                if (handles.Length == 0)
                    return 1;
                float total = 0;
                for (int i = 0; i < handles.Length; i++)
                    total += handles[i]?.Progress ?? 1;
                return total / handles.Length;
            }
        }
        /// <summary>
        /// 加载结果；失败位置为 null
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
                        cachedAssets[i] = handles[i]?.Asset;
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
        /// 异步任务，支持 await
        /// </summary>
        public Task<T[]> Task
        {
            get { return taskCompletionSource.Task; }
        }

        /// <summary>
        /// 创建批量句柄。
        /// </summary>
        public AssetsHandle(AssetHandle<T>[] handles, string[] keys)
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
                    OnChildCompleted(null);
                    continue;
                }
                handle.Completed += OnChildCompleted;
            }
        }

        /// <summary>
        /// 同步等待全部完成（会阻塞主线程）。
        /// </summary>
        public T[] WaitForCompletion()
        {
            for (int i = 0; i < handles.Length; i++)
                handles[i]?.WaitForCompletion();
            return Assets;
        }

        /// <summary>
        /// 释放全部子句柄（重复调用只生效一次）。
        /// </summary>
        public void Release()
        {
            if (released)
                return;
            released = true;
            for (int i = 0; i < handles.Length; i++)
                handles[i]?.Release();
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
