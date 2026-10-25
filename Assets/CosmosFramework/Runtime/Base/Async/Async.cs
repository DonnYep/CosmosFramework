using System;
using System.Collections;
using System.Threading.Tasks;
using Cosmos.Operation;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 异步操作统一入口（<c>Cosmos.Async</c> 程序集的唯一门面）。
    /// <para>所有"等一等"的需求都从这里出发，返回的都是 <see cref="OperationBase"/>，
    /// 因此天然具备：优先级、进度、取消（Abort）、完成事件、协程等待、<c>await</c>、Task。</para>
    /// <para>用法：</para>
    /// <code>
    /// await Async.WaitUntil(() => isReady);          // 条件等待（原 FutureTask）
    /// await Async.WaitForSeconds(1.5f);              // 定时（默认受 timeScale 影响）
    /// await Async.NextFrame();                       // 等到下一帧
    /// await Async.WaitForCoroutine(SomeRoutine());   // 等协程结束（等价 yield return 一个协程）
    /// await Async.WaitForAsyncOperation(sceneOp);    // 等 UnityEngine.AsyncOperation
    /// await Async.WaitForTask(task);                 // 后台 Task 完成后回到主线程继续
    /// </code>
    /// </summary>
    public static class Async
    {
        /// <summary>
        /// 条件等待：每帧检查 <paramref name="condition"/>，为 true 时成功。
        /// </summary>
        /// <param name="condition">完成条件</param>
        /// <param name="description">描述（调试用）</param>
        /// <param name="timeoutSeconds">超时时间（秒，&lt;=0 表示不超时）；超时置为失败</param>
        /// <returns>异步操作</returns>
        public static OperationBase WaitUntil(Func<bool> condition, string description = null, float timeoutSeconds = 0f)
        {
            return Start(new WaitUntilOperation(condition, description, timeoutSeconds));
        }
        /// <summary>
        /// 定时等待。
        /// </summary>
        /// <param name="seconds">时长（秒）</param>
        /// <param name="unscaled">是否忽略 timeScale</param>
        /// <returns>异步操作</returns>
        public static OperationBase WaitForSeconds(float seconds, bool unscaled = false)
        {
            return Start(new WaitForSecondsOperation(seconds, unscaled));
        }
        /// <summary>
        /// 等到下一帧。
        /// </summary>
        /// <returns>异步操作</returns>
        public static OperationBase NextFrame()
        {
            return Start(new NextFrameOperation());
        }
        /// <summary>
        /// 等一个协程结束（协程内可继续 yield 任意 Unity 指令）。
        /// </summary>
        /// <param name="routine">协程</param>
        /// <returns>异步操作</returns>
        public static OperationBase WaitForCoroutine(IEnumerator routine)
        {
            return Start(new WaitForCoroutineOperation(routine));
        }
        /// <summary>
        /// 等一个 <see cref="UnityEngine.AsyncOperation"/>（场景加载、资源请求等）。
        /// </summary>
        /// <param name="operation">Unity 异步操作</param>
        /// <returns>异步操作</returns>
        public static OperationBase WaitForAsyncOperation(UnityEngine.AsyncOperation operation)
        {
            return Start(new WaitForAsyncOperation(operation));
        }
        /// <summary>
        /// 等一个 <see cref="Task"/>：轮询在主线程进行，完成后主线程继续。
        /// <para>失败/取消会转成操作失败（Error 带原因），不会把异常抛进主线程。</para>
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns>异步操作</returns>
        public static OperationBase WaitForTask(Task task)
        {
            return Start(new WaitForTaskOperation(task));
        }
        /// <summary>
        /// 启动一个自定义异步操作（返回自身便于链式使用）。
        /// </summary>
        /// <param name="operation">异步操作</param>
        /// <returns>异步操作</returns>
        public static OperationBase Start(OperationBase operation)
        {
            OperationSystem.StartOperation(operation);
            return operation;
        }
    }
    /// <summary>
    /// 把 Unity 的"等待指令"与被等待对象变成可 await 的异步操作。
    /// <para>覆盖：<see cref="IEnumerator"/>、<see cref="YieldInstruction"/>（含 <see cref="Coroutine"/>、
    /// WaitForSeconds、WaitForEndOfFrame、WaitForFixedUpdate）、<see cref="CustomYieldInstruction"/>、
    /// <see cref="UnityEngine.AsyncOperation"/>。</para>
    /// <para><see cref="Task"/> 无法用扩展覆盖（实例方法优先），请用 <see cref="Async.WaitForTask"/>；
    /// 在协程里也可以 <c>yield return Async.WaitForTask(task);</c>（<see cref="OperationBase"/> 本身就是 IEnumerator）。</para>
    /// </summary>
    public static class AwaitableExtensions
    {
        /// <summary>await 一个协程</summary>
        public static OperationAwaiter GetAwaiter(this IEnumerator routine)
        {
            return Async.WaitForCoroutine(routine).GetAwaiter();
        }
        /// <summary>await 一个 Unity 等待指令（WaitForSeconds / Coroutine / WaitForEndOfFrame …）</summary>
        public static OperationAwaiter GetAwaiter(this YieldInstruction instruction)
        {
            return Async.WaitForCoroutine(Wrap(instruction)).GetAwaiter();
        }
        /// <summary>await 一个自定义等待指令（WaitUntil / WaitWhile …）</summary>
        public static OperationAwaiter GetAwaiter(this CustomYieldInstruction instruction)
        {
            return Async.WaitForCoroutine(Wrap(instruction)).GetAwaiter();
        }
        /// <summary>await 一个 Unity 异步操作（场景加载、资源请求 …）</summary>
        public static OperationAwaiter GetAwaiter(this UnityEngine.AsyncOperation operation)
        {
            return Async.WaitForAsyncOperation(operation).GetAwaiter();
        }
        static IEnumerator Wrap(object instruction)
        {
            yield return instruction;
        }
    }
}
