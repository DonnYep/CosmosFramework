# 网络通道的异步等待（async/await）

> 适用范围：`Assets/CosmosFramework/Runtime/Modules/Network/Awaiter`
> 相关文档：[WebRequest 与 Download 的分工](NETWORKING.md)、[使用手册 §3.12](MANUAL.md)

## 1. 定位

通道本体是**事件推送**模型（`OnConnected` / `OnDataReceived` / `OnDisconnected`），
等待层把它包装成**可 await 的拉取**模型，并负责失败、超时、取消、缓冲与线程切换。

## 2. 模型

```
通道事件（TickRefresh 内触发）                 await 侧
OnConnected      ─┐
OnDisconnected   ─┼→ NetworkAwaiter（每通道一个）
OnDataReceived   ─┘     ├─ 连接等待者（同一时刻 1 个）
                        ├─ 断线等待者（同一时刻 1 个）
                        └─ 收包：缓冲队列 + 等待者队列（FIFO，多个等待者）
```

要点：

- 等待器按通道缓存（弱表），跟随通道生命周期，无需手动创建 / 销毁；
- **收包先入缓冲**：数据早于 `await` 到达也不会丢，下一次 `ReceiveDataAsync` 立刻取走；
- **断线传播失败**：挂起的连接 / 收包等待者立即以 `NetworkException` 结束；
- **取消 / 超时的等待者会被精确摘除**，不会吞掉后续数据。

## 3. 必须先驱动 TickRefresh

通道不会自己跑：`OnConnected / OnDataReceived / OnDisconnected` 都在 `TickRefresh()` 里产生。
**不驱动就没有事件，`await` 只能等到超时。** 两种驱动方式：

```csharp
// 方式一（推荐）：交给网络模块，模块在 OnUpdate 里统一 TickRefresh
CosmosEntry.NetworkManager.AddChannel(channel);

// 方式二：自己在 Update 里驱动
void Update() { channel.TickRefresh(); }
```

## 4. API

| 方法 | 说明 |
| --- | --- |
| `ConnectAsync(host, port, ct?, timeout?)` | 发起连接并等待成功 |
| `WaitConnectedAsync(ct?, timeout?)` | 只等待连接成功（连接由别处发起） |
| `ReceiveDataAsync(ct?, timeout?)` | 等待一条数据（优先取缓冲） |
| `WaitDisconnectedAsync(ct?, timeout?)` | 等待断线；未连接时立即完成 |
| `TryReceiveData(out byte[])` | 非阻塞取缓冲 |
| `GetBufferedDataCount()` | 当前缓冲数量 |

结果语义：

| 场景 | 结果 |
| --- | --- |
| 连接成功 | 完成 |
| 连接建立前断开 | `NetworkException` |
| 连接超时 | `TimeoutException` |
| 取消 | `OperationCanceledException` |
| 已连接时收包 | 完成，返回报文 `byte[]` |
| 收包期间断线 | `NetworkException` |
| 未连接且无缓冲时收包 | 立即 `NetworkException` |
| 已连接时等待断线 | 断线后完成 |

默认策略见 `NetworkAwaiterSettings`：连接 10 秒超时，收包与断线默认不限时，
收包缓冲上限 1024 条（超出丢最旧的一条并告警一次）。

## 5. 用法

```csharp
var channel = new TcpClientChannel("MAIN");
CosmosEntry.NetworkManager.AddChannel(channel);   // 由模块驱动 TickRefresh

try
{
    await channel.ConnectAsync("127.0.0.1", 7777, cancellationToken);
    while (!cancellationToken.IsCancellationRequested)
    {
        var data = await channel.ReceiveDataAsync(cancellationToken);
        // data 的长度就是实际报文长度
    }
}
catch (OperationCanceledException) { /* 正常退出 */ }
catch (TimeoutException) { /* 连接超时 */ }
catch (NetworkException e) { /* 连接失败 / 断线，e.ChannelName 指出通道 */ }
```

取消令牌贯穿生命周期：

```csharp
CancellationTokenSource cts;
async void Start()
{
    cts = new CancellationTokenSource();
    await RunAsync(cts.Token);
}
void OnDestroy()
{
    cts.Cancel();      // 挂起的 await 立刻以 OperationCanceledException 结束
    cts.Dispose();
    channel.Close();
}
```

## 6. 语义边界

- **线程**：等待器 API 必须在主线程调用（否则抛 `InvalidOperationException`）；
  `TickRefresh` 在主线程时，`await` 之后的代码也在主线程继续执行。
- **后台线程兜底**：通道在后台线程抛事件时，等待器会把事件投递到主线程队列，
  由 `MonoGameManager.Update` 每帧 `UnityMainThread.Drain()` 统一执行；
  队列积压可用 `UnityMainThread.PendingCount` 观察。
- **事件与 await 可以共存**：等待器是通道事件的又一个订阅者，事件订阅者仍收到全部数据；
  只有多个 `await` 者之间是 FIFO 分配（一条数据只交给一个等待者）。
- **同一通道同一时刻只允许一个连接等待者**，重复发起抛 `InvalidOperationException`；
  收包等待者可以多个，按 FIFO 交付。
- **重连**：断线后可直接再次 `ConnectAsync`，等待器复用同一通道实例。
- **不读数据的代价**：没人消费时数据会缓冲到上限，超出后丢弃最旧的一条并告警；
  长连接应保持一个接收循环。

## 7. 主线程调度器（线程兜底）

`UnityMainThread`（`Runtime/Base/Utility/UnityThread`）不依赖 GameObject、不依赖 `SynchronizationContext`：

| 成员 | 说明 |
| --- | --- |
| `IsMainThread` / `MainThreadId` | 当前是否主线程 |
| `Post(Action)` | 已在主线程则立即执行，否则入队 |
| `Send(Action, timeoutMs)` | 投递并阻塞等待，超时抛 `TimeoutException` |
| `Drain()` | 主线程每帧执行队列；单个回调异常只记日志 |
| `PendingCount` | 待执行数量 |

驱动：`MonoGameManager.Awake` 调 `Initialize()`，`Update` 在模块刷新后调 `Drain()`。
无 `MonoGameManager` 的纯逻辑宿主需自行在主线程循环里 `Drain()`。

`UnityThreadSync` 保留为兼容外观（`Instance.PostToUnityThread` / `SendToUnityThread`）。

## 8. 通道实现要求

等待层依赖通道满足以下约定，自定义通道请一并遵守：

1. `OnDataReceived` 交付的 `byte[]` 长度必须**等于实际报文长度**（按 `ArraySegment.Offset/Count` 拷贝）；
2. 接收回调要在 `Connect` 之前绑定，避免极快建连时漏掉首批数据；
3. `Disconnect` / `Close` 只断开连接，**不要清空订阅委托**，否则重连后的监听会静默失效；
4. 事件在主线程触发（`TickRefresh` 内）。

## 9. 破坏性变更

- `WaitConnectedAwaiter`、`WaitReceiveDataAwaiter`、`WaitForChannelExts` 已删除；
- `ConnectAsync` / `ReceiveDataAsync` 保留原名字与返回类型，新增可选 `CancellationToken` 与 `timeout` 参数；
- 新增 `WaitConnectedAsync` / `WaitDisconnectedAsync` / `TryReceiveData` / `GetBufferedDataCount`；
- 完整清单见 [README 的破坏性更新](../README.md#破坏性更新)。
