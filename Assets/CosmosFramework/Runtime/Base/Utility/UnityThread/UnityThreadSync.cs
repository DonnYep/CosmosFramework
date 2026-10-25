using System;
namespace Cosmos
{
    /// <summary>
    /// Unity 主线程同步的兼容外观；
    /// <para>实际实现已迁移到 <see cref="UnityMainThread"/>：不再依赖 GameObject，也不再依赖
    /// <c>SynchronizationContext</c>，因此后台线程可以直接使用，不需要先取 <c>Instance</c>。</para>
    /// <para>保留本类型只为兼容旧的 <c>UnityThreadSync.Instance.PostToUnityThread(...)</c> 调用；新代码请直接用
    /// <see cref="UnityMainThread.Post"/> / <see cref="UnityMainThread.Send"/>。</para>
    /// </summary>
    public class UnityThreadSync
    {
        static readonly UnityThreadSync instance = new UnityThreadSync();
        /// <summary>
        /// 共享实例；不再对应任何 GameObject。
        /// </summary>
        public static UnityThreadSync Instance { get { return instance; } }
        /// <summary>
        /// 投递到主线程执行（不阻塞）。
        /// </summary>
        /// <param name="postCallback">回调；参数固定为 null，与旧实现一致</param>
        public void PostToUnityThread(Action<object> postCallback)
        {
            if (postCallback == null)
                return;
            UnityMainThread.Post(() => { postCallback.Invoke(null); });
        }
        /// <summary>
        /// 投递到主线程并等待执行完成（阻塞）。
        /// </summary>
        /// <param name="postCallback">回调；参数固定为 null，与旧实现一致</param>
        public void SendToUnityThread(Action<object> postCallback)
        {
            if (postCallback == null)
                return;
            UnityMainThread.Send(() => { postCallback.Invoke(null); });
        }
    }
}
