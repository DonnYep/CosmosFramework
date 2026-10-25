# 异步底座（Cosmos.Async）

> **框架里所有"要等一等"的东西都跑在这一层之上**：资源加载、WebRequest 请求、下载校验、
> 场景加载、协程、后台 Task、以及业务自定义的异步流程。
> 它不依赖任何模块，任何程序集都可以直接使用。

## 1. 为什么合并（原来的三套会被收成一套）

之前框架里同时存在三套异步设施，各自带一个驱动、各自实现"等待"：

| 旧设施 | 位置 | 问题 |
| --- | --- | --- |
| `OperationBase` / `OperationSystem` | `ResourceAbstractions/Operation` | 本身最完整（状态/进度/优先级/取消/await/Task），但**住在资源程序集里** |
| `FutureTask` / `FutureTaskMonitor` | `Base/FutureTask` | 条件轮询的最小子集：**没有进度、没有优先级、没有取消语义**（`Abort` 只是不再检测，回调仍会触发）、没有异常隔离；诊断只有 id/elapsed |
| `Awaitable` 全家（`CoroutineAwaiter<T>` / `CoroutineAwaiterMonitor` / `WaitForTask` / `WaitForCoroutine`） | `Base/Awaitable` | 只是"把 Unity 等待指令变成可 await"的**适配层**，却自带 Monitor 与协程泵；与 `OperationSystem` 的驱动重复 |

合并原则：**`OperationBase` 是唯一的异步核心**，其余两者只保留"可用性"部分：

- `FutureTask` → `Async.WaitUntil(...)`（就是 `WaitUntilOperation`，条件轮询 + 超时 + 取消 + 诊断）；
- `Awaitable` → `AwaitableExtensions.GetAwaiter(...)` + `WaitForCoroutineOperation`（协程由框架驱动执行）；
- 三个驱动（`OperationDriver` / `FutureTaskMonitor` / `CoroutineAwaiterMonitor`）→ **只剩 `OperationDriver`**。

## 2. 结构

```text
Cosmos.Async（Runtime/Base/Async，只依赖 UnityEngine）
├─ Async                       ← 唯一门面：WaitUntil / WaitForSeconds / NextFrame / WaitForCoroutine /
│                                 WaitForAsyncOperation / WaitForTask / Start
├─ AwaitableExtensions          ← await 扩展：IEnumerator / YieldInstruction / CustomYieldInstruction / AsyncOperation
└─ Operation/
   ├─ OperationBase             ← 状态机：Status/Progress/Priority/Error/OperationId/Description
   │                              + Completed / IEnumerator / GetAwaiter / Task / Abort
   ├─ OperationSystem           ← 登记、优先级排序、每帧推进、异常隔离、按 Id 查、协程执行器
   ├─ OperationDriver           ← 唯一驱动组件（常驻 DontDestroyOnLoad，同时是协程泵）
   ├─ OperationStatus
   ├─ GameAsyncOperation        ← 业务自定义异步操作的薄封装
   └─ WaitOperations            ← WaitUntil / WaitForSeconds / NextFrame / WaitForCoroutine /
                                  WaitForAsyncOperation / WaitForTask
```

依赖方向（仍然是最底层，谁都能引，它不引任何人）：

```text
Cosmos.Async  ←  Cosmos.Resource.Abstractions  ←  Cosmos.Runtime（模块）  ←  Integrations
                                                ↖  Cosmos.Editor / Cosmos.Resource.EditorFallback
```

> ⚠️ **asmdef 必须成对引用**：`Cosmos.Resource.Abstractions` 与 `Cosmos.Runtime` 的公开/受保护 API
> 暴露了 `OperationBase` 等异步类型（例如 `ResourceProviderOperation : OperationBase`），
> 所以**任何引用它们的程序集（含 Editor 侧）都必须同时引用 `Cosmos.Async`**，
> 否则 Unity 会报
> `CS0012: The type 'OperationBase' is defined in an assembly that is not referenced`。
> 编译检查工具（`tools/RoslynCheck`）现在会在编译前校验这条规则，并按 asmdef 分程序集
> （`Cosmos.Resource.EditorFallback` / `Cosmos.Editor`）编译编辑器代码。

## 3. 用法

### 3.1 一行拿到等待操作

```csharp
await Async.WaitUntil(() => isReady);                     // 条件等待（原 FutureTask）
await Async.WaitUntil(() => isReady, "loading", 10f);     // 带描述与超时（超时置失败）
await Async.WaitForSeconds(1.5f);                          // 定时（受 timeScale 影响）
await Async.WaitForSeconds(1.5f, unscaled: true);          // 忽略 timeScale
await Async.NextFrame();                                   // 下一帧
await Async.WaitForCoroutine(SomeRoutine());               // 等协程结束
await Async.WaitForAsyncOperation(sceneOp);                // 等 UnityEngine.AsyncOperation
await Async.WaitForTask(task);                             // 等后台 Task（完成后主线程继续）
```

### 3.2 await 扩展（等价于 `yield return`）

```csharp
await new WaitForSeconds(0.5f);            // YieldInstruction
await WaitForEndOfFrame();                 // 需要 new 一个实例：await new WaitForEndOfFrame();
await SomeCoroutine();                     // Coroutine
await someEnumerator;                      // IEnumerator
await new WaitUntil(() => flag);           // CustomYieldInstruction
await sceneLoadOperation;                  // UnityEngine.AsyncOperation
```

`Task` 不能用扩展（实例方法 `GetAwaiter` 优先），请用 `Async.WaitForTask(task)`；
在协程里也可以 `yield return Async.WaitForTask(task);`（`OperationBase` 本身就是 `IEnumerator`）。

### 3.3 自定义异步操作

```csharp
public class WaitSecondsOperation : GameAsyncOperation
{
    float elapsed, duration;
    protected override void OnOpStart() { }
    protected override void OnOpUpdate()
    {
        elapsed += Time.deltaTime;
        Progress = duration <= 0f ? 1f : elapsed / duration;
        if (elapsed >= duration) Status = OperationStatus.Succeeded;
    }
    protected override void OnOpFinish() { }
    protected override void OnOpAbort() { }
}
await Async.Start(new WaitSecondsOperation(0.3f));
```

### 3.4 优先级、取消与诊断

```csharp
var op = Async.WaitUntil(() => flag, "wait flag");
op.Priority = 10;                 // 数值越大越先被推进
op.OperationId;                   // 登记时分配
OperationSystem.TryGetOperation(op.OperationId, out var found);   // 诊断：它还在不在、是什么
op.Abort();                       // 立即完成：WasAborted=true、Completed/OnFinish 当次触发
```

## 4. 时序

- `MonoGameManager` 执行顺序 `-20000`（模块 tick 先跑）；
- `OperationDriver` 保持默认顺序（0）：所以模块 tick 里启动的操作**当帧就被推进一次**，不白等一帧；
- 协程（`WaitForCoroutineOperation`）由 `OperationDriver` 执行，等价于 Unity 的协程阶段（Update 之后）。

## 5. 迁移表

| 旧写法 | 新写法 |
| --- | --- |
| `new FutureTask(() => cond)` | `Async.WaitUntil(() => cond)` |
| `new FutureTask(cond, polling, completed)` | `Async.WaitUntil(cond)` + `op.Completed += ...`（polling 用 `WaitUntilOperation.ElapsedSeconds`/`Progress` 或自定义操作） |
| `FutureTask.HasFutureTask(id)` | `OperationSystem.TryGetOperation(id, out var op)` |
| `FutureTask.GetFutureTaskInfo(id)` | `op.OperationId` / `op.Description` / `op.Progress` / `op.Status` |
| `FutureTaskMonitor` | `OperationDriver`（唯一驱动，无需关心） |
| `await new WaitForSeconds(x)` | 同写法（扩展已提供）；底层是 `Async.WaitForCoroutine` 包一层 |
| `await someCoroutine` / `await someEnumerator` | 同写法（扩展已提供） |
| `await asyncOperation`（UnityEngine.AsyncOperation） | 同写法（底层是 `WaitForAsyncOperation`，轮询 `isDone`） |
| `new WaitForTask(task)` / `task.AsCoroutine()` | `Async.WaitForTask(task)`；协程里 `yield return Async.WaitForTask(task)` |
| `new WaitForMainThread()` | 已移除：需要回主线程用 `Utility.Unity` 的主线程投递 |
| `new WaitForNextFrame()` | `Async.NextFrame()` |
| `CoroutineAwaiterMonitor` | `OperationDriver` |

> 旧类型（`FutureTask*`、`Cosmos.Awaitable.*`）**已删除**；示例 `11_Awaitable` 与 `22_FutureTask`
> 已改写为新 API（类名保持不变，避免场景引用失效）。

## 6. 约定（给后续模块）

1. **任何模块的异步实现都继承 `OperationBase`/`GameAsyncOperation`**，不要自己再造 `OnUpdate` 轮询或第二个驱动；
2. 启动统一走 `Async.Start(...)` / `OperationSystem.StartOperation(...)`；
3. 需要"优先级/取消/等待/进度"时，返回值直接是 `OperationBase`（`WebRequestHandle<T>` 就是这么做的）；
4. **不要在 `Cosmos.Async` 里引用任何模块**（包括 `Utility.Debug`）：这一层只允许 UnityEngine。

## 7. 验证

**20 用例 / 72 断言**的临时工程：把 `Cosmos.Async` 的**真实源码**编进控制台程序，
用最小 UnityEngine 替身（含可推进时间与可泵协程）手动驱动 `OperationSystem.Update()`。
位置：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\async\harness`（仓库外，可删）。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\async\harness\AsyncHarness.csproj
# 断言：72 通过 / 0 失败  === ALL-PASS ===
```

覆盖：生命周期各回调只执行一次、优先级顺序、异常隔离、`Abort` 立即完成、`Completed` 晚订阅、
`await`/协程/`Task` 三种等待、`GameAsyncOperation`、`DestroyAll`、`WaitUntil`（正常/超时/条件异常）、
`WaitForSeconds`（含 0 秒）、`NextFrame`、`WaitForCoroutine`（正常/取消时停协程）、
`WaitForAsyncOperation`、`WaitForTask`（成功/失败/取消）、await 扩展、`OperationId` 与 `TryGetOperation`。

并用**变异测试**验证用例有效（`mutate.ps1` + `mutate2.ps1`）：
不排序优先级、异常不隔离、终止不立即完成、晚订阅不回调、awaiter 永远已完成、
条件等待忽略超时、等待协程不停止协程 —— 均被检出；
另有 1 个**等价变异**（只删 `OnAbort` 里的停协程）不会被检出，
因为 `OnFinish` 是兜底路径，属于预期的双保险。
