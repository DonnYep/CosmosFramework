using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 统一异步方案（Cosmos.Async）示例：原来的 Awaitable + FutureTask 已合并到这里。
/// <para>演示：条件等待、定时、下一帧、协程等待、await 扩展、自定义异步操作、后台 Task、优先级与取消。</para>
/// </summary>
public class AwaitableTest : MonoBehaviour
{
    bool ready;
    int frameStarted;

    async void Start()
    {
        Debug.Log("[Async] 开始：1 秒后条件 ready 变 true");
        // 1) 条件等待（原 FutureTask 的用法）
        var waitOperation = Async.WaitUntil(() => ready, "wait until ready", timeoutSeconds: 5f);
        await waitOperation;
        if (waitOperation.Status != OperationStatus.Succeeded)
            Debug.LogError($"[Async] 条件等待失败：{waitOperation.Error}");
        Debug.Log("[Async] 条件等待结束");

        // 2) 定时（受 timeScale 影响）
        await Async.WaitForSeconds(0.5f);
        Debug.Log("[Async] WaitForSeconds(0.5) 结束");

        // 3) 下一帧
        frameStarted = Time.frameCount;
        await Async.NextFrame();
        Debug.Log($"[Async] NextFrame 结束，跨帧数 {Time.frameCount - frameStarted}");

        // 4) 等协程（协程里可以继续 yield 任意 Unity 指令）
        await Async.WaitForCoroutine(EnumRun());
        Debug.Log("[Async] WaitForCoroutine 结束");

        // 5) await 扩展：WaitForSeconds / 协程
        await new WaitForSeconds(0.2f);
        await EnumRun();
        Debug.Log("[Async] await 扩展结束");

        // 6) 自定义异步操作（GameAsyncOperation）
        await Async.Start(new WaitSecondsOperation(0.3f));
        Debug.Log("[Async] 自定义 GameAsyncOperation 结束");

        // 7) 后台 Task：完成后回到主线程继续，失败会转成操作失败
        var task = Task.Run(async () =>
        {
            await Task.Delay(500);
            Debug.Log($"[Async] task finished on thread {Thread.CurrentThread.ManagedThreadId}");
        });
        var taskOperation = Async.WaitForTask(task);
        await taskOperation;
        Debug.Log($"[Async] WaitForTask 结束，status {taskOperation.Status}");

        // 8) 优先级与取消
        var cancelled = Async.WaitUntil(() => false, "never true", timeoutSeconds: 100f);
        cancelled.Priority = 10;
        await Async.WaitForSeconds(0.1f);
        cancelled.Abort();
        Debug.Log($"[Async] 取消的操作：WasAborted={cancelled.WasAborted} Status={cancelled.Status}");
    }

    IEnumerator EnumRun()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("[Async] 协程体执行完毕");
    }

    void Update()
    {
        // 模拟"1 秒后条件满足"
        if (!ready && Time.timeSinceLevelLoad >= 1f)
            ready = true;
    }
}

/// <summary>自定义异步操作：定时完成（也可以换成任意业务逻辑）</summary>
public class WaitSecondsOperation : GameAsyncOperation
{
    readonly float duration;
    float elapsed;
    public WaitSecondsOperation(float duration)
    {
        this.duration = duration;
        Description = $"WaitSecondsOperation({duration})";
    }
    protected override void OnOpStart() { }
    protected override void OnOpUpdate()
    {
        elapsed += Time.deltaTime;
        Progress = duration <= 0f ? 1f : elapsed / duration;
        if (elapsed >= duration)
            Status = OperationStatus.Succeeded;
    }
    protected override void OnOpFinish() { }
    protected override void OnOpAbort() { }
}
