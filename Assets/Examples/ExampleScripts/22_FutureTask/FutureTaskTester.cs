using UnityEngine;
using Cosmos;
using Debug = UnityEngine.Debug;

/// <summary>
/// 原 <c>FutureTask</c> 的示例。
/// <para><b>迁移说明</b>：<c>FutureTask</c> 已合并进 <c>Cosmos.Async</c>——
/// 等价写法是 <see cref="Async.WaitUntil"/>（同样是"轮询条件 + 可 await"，
/// 但共享统一的 OperationSystem：优先级、进度、取消、诊断一致）。</para>
/// <para>类名保持不变，避免场景里的组件引用失效。</para>
/// </summary>
public class FutureTaskTester : MonoBehaviour
{
    bool result = false;
    [SerializeField] float duration = 6;
    float currentTime = 0;
    async void Start()
    {
        Debug.Log("FutureTask await before");
        // 等价于旧写法：FutureTask task = new FutureTask(() => result); await task;
        var operation = Async.WaitUntil(() => result, "future task : wait until result", timeoutSeconds: 0f);
        await operation;
        Debug.Log($"FutureTask await after (status {operation.Status})");
    }
    void OnValidate()
    {
        if (duration < 0)
            duration = 0;
    }
    void Update()
    {
        //这里表示若干秒后result =true
        currentTime += Time.deltaTime;
        if (currentTime >= duration)
        {
            currentTime = duration;
            result = true;
        }
    }
}
