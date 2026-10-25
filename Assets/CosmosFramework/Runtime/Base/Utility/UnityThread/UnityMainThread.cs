using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
namespace Cosmos
{
    /// <summary>
    /// Unity 主线程调度器；
    /// <para>用于把后台线程产生的事件/回调收敛回主线程执行，避免在非主线程触碰 Unity API 或与主线程状态竞争。</para>
    /// <para>与 <see cref="UnityThreadSync"/> 的区别：本类型不依赖 GameObject、不依赖 SynchronizationContext，
    /// 队列由 <see cref="MonoGameManager"/> 每帧统一 Drain，因此后台线程也可以安全地 Post。</para>
    /// <para>用法约定：</para>
    /// <para>1、主线程启动时调用一次 <see cref="Initialize"/>（框架在 MonoGameManager.Awake 里已调用）；</para>
    /// <para>2、后台线程用 <see cref="Post"/>（不阻塞）或 <see cref="Send"/>（阻塞且带超时）；</para>
    /// <para>3、主线程每帧调用 <see cref="Drain"/> 执行队列（框架在 MonoGameManager.Update 里已调用）。</para>
    /// </summary>
    public static class UnityMainThread
    {
        /// <summary>
        /// 单帧最多执行的回调数，避免后台线程洪泛导致一帧卡死；剩余的回调留到下一帧。
        /// </summary>
        public const int MaxActionsPerDrain = 1024;

        static readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
        static int mainThreadId;

        /// <summary>
        /// 主线程 ID；未初始化时为 0。
        /// </summary>
        public static int MainThreadId { get { return mainThreadId; } }
        /// <summary>
        /// 是否已初始化。
        /// </summary>
        public static bool IsInitialized { get { return mainThreadId != 0; } }
        /// <summary>
        /// 当前代码是否运行在主线程。
        /// </summary>
        public static bool IsMainThread
        {
            get { return mainThreadId != 0 && Environment.CurrentManagedThreadId == mainThreadId; }
        }
        /// <summary>
        /// 在主线程上调用：记录主线程 ID；重复调用会覆盖（域重载/测试可用）。
        /// </summary>
        public static void Initialize()
        {
            mainThreadId = Environment.CurrentManagedThreadId;
        }
        /// <summary>
        /// 尚未初始化时初始化；已初始化则忽略。
        /// <para>供框架内部各入口做惰性兜底，<b>必须在主线程调用</b>。</para>
        /// </summary>
        public static void EnsureInitialized()
        {
            if (mainThreadId == 0)
                Initialize();
        }
        /// <summary>
        /// 投递到主线程执行；当前已在主线程则立即执行，否则入队等待下一次 <see cref="Drain"/>。
        /// </summary>
        /// <param name="action">要执行的回调</param>
        public static void Post(Action action)
        {
            if (action == null)
                return;
            if (IsMainThread)
            {
                action();
                return;
            }
            actionQueue.Enqueue(action);
        }
        /// <summary>
        /// 投递到主线程并等待其执行完成（阻塞调用方）。
        /// <para>超时抛 <see cref="TimeoutException"/>：宁可让后台线程失败，也不要无声卡死。</para>
        /// </summary>
        /// <param name="action">要执行的回调</param>
        /// <param name="timeoutMilliseconds">等待超时（毫秒）</param>
        public static void Send(Action action, int timeoutMilliseconds = 5000)
        {
            if (action == null)
                return;
            if (IsMainThread)
            {
                action();
                return;
            }
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Exception captured = null;
                actionQueue.Enqueue(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        captured = e;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
                if (!waitHandle.Wait(timeoutMilliseconds))
                    throw new TimeoutException($"UnityMainThread.Send timed out after {timeoutMilliseconds} ms. The main thread may be blocked or not draining.");
                if (captured != null)
                    ExceptionDispatchInfo.Capture(captured).Throw();
            }
        }
        /// <summary>
        /// 在主线程上执行已投递的回调；由 <see cref="MonoGameManager"/> 每帧调用。
        /// <para>单个回调抛异常只记录日志，不打断其余回调与主循环。</para>
        /// </summary>
        public static void Drain()
        {
            var processed = 0;
            while (processed < MaxActionsPerDrain && actionQueue.TryDequeue(out var action))
            {
                processed++;
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"[Cosmos] UnityMainThread action failed : {e}");
                }
            }
        }
        /// <summary>
        /// 当前待执行的回调数量。
        /// </summary>
        public static int PendingCount { get { return actionQueue.Count; } }
    }
}
