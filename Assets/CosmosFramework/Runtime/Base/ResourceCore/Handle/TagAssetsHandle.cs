using System;
using System.Collections;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 分类标签资产句柄。
    /// <para>通过分类标签一次加载多个资产（Addressables Label 风格）：</para>
    /// <para>所有子资产加载完成后句柄完成；单个子资产失败不影响其余资产。</para>
    /// <para>支持协程等待、事件回调、async/await；Release 会释放全部子句柄。</para>
    /// </summary>
    public class TagAssetsHandle<T> : IEnumerator
        where T : UnityEngine.Object
    {
        readonly AssetHandle<T>[] handles;
        readonly TaskCompletionSource<T[]> taskCompletionSource = new TaskCompletionSource<T[]>();
        int completedCount;
        bool released;
        Action<TagAssetsHandle<T>> completed;

        /// <summary>
        /// 是否全部加载完成
        /// </summary>
        public bool IsDone
        {
            get { return completedCount >= handles.Length; }
        }
        /// <summary>
        /// 聚合进度（子资产进度平均值）
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
        /// 加载成功的资产集合（全部完成后有效）
        /// </summary>
        public T[] Assets
        {
            get
            {
                if (!IsDone)
                    return null;
                var result = new T[handles.Length];
                for (int i = 0; i < handles.Length; i++)
                {
                    result[i] = handles[i].Asset;
                }
                return result;
            }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<TagAssetsHandle<T>> Completed
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
        /// <summary>
        /// 同步等待全部加载完成
        /// </summary>
        public T[] WaitForCompletion()
        {
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i].WaitForCompletion();
            }
            return Assets;
        }
        /// <summary>
        /// 释放全部子句柄（引用计数减一）
        /// </summary>
        public void Release()
        {
            if (released)
                return;
            released = true;
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i].Release();
            }
        }

        internal TagAssetsHandle(AssetHandle<T>[] handles)
        {
            this.handles = handles;
            if (handles == null || handles.Length == 0)
            {
                completedCount = 1;
                taskCompletionSource.TrySetResult(new T[0]);
                completed?.Invoke(this);
                return;
            }
            for (int i = 0; i < handles.Length; i++)
            {
                var handle = handles[i];
                handle.Completed += OnChildCompleted;
            }
        }

        void OnChildCompleted(AssetHandle<T> handle)
        {
            handle.Completed -= OnChildCompleted;
            completedCount++;
            if (IsDone)
            {
                taskCompletionSource.TrySetResult(Assets);
                completed?.Invoke(this);
            }
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
