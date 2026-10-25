# Hydrogen（TCP 客户端）

> 一句话结论：**接口设计是好的（header 可插拔 + 回调式），但实现里有 7 个会真出问题的逻辑缺陷**——
> 其中两个直接导致**进程崩溃**与**静默错帧**。已按"保持 API、修内部逻辑"的原则修好，
> 并用真实回环 socket 的 13 个用例 + 变异测试验证。

## 1. 库的定位

| 项 | 内容 |
| --- | --- |
| 位置 | `Runtime/3rdParty/Hydrogen`（独立 asmdef `Hydrogen`，不依赖框架任何模块） |
| 帧格式 | `[header][message]`，header 长度与编码由 `IHeaderEncoder` 决定（默认可 2 字节大端长度） |
| 依赖 | 仅 `UnityEngine.Debug`（通过 `Hydrogen.Log`） |
| 框架内使用方 | 目前没有；`Modules/Network` 走的是 KCP / RUDP / SUDP / Telepathy |

## 2. 复查发现的问题

| # | 问题 | 后果 | 原代码位置 |
| --- | --- | --- | --- |
| 1 | `BeginConnect` 的回调里**没有 `EndConnect`** | 连接被拒/超时**永远不会被发现**；回调里 `GetStream()` 抛 `InvalidOperationException` 且无人捕获 → **进程崩溃**（实测：`Unhandled exception ... at Hydrogen.TcpClient.OnConnectHandler`） | `OnConnectHandler` |
| 2 | `EncodeHeader((ushort)message.Length)` **不校验长度** | >65535 字节的消息被**静默截断长度头**，对端切出垃圾帧、后续整条流错位（实测：发 70000 字节，服务端切出 **3 个垃圾帧**） | `WriteMessage` |
| 3 | `Disconnect()` 里 `reader.Close(); memStream.Close();`，而 `Connect()` 不重建 | **重连必炸**：重连后一旦来数据就抛 `Cannot access a closed Stream`（实测） | `Disconnect` / 构造 |
| 4 | 连接尚未就绪时 `SendMessage` 只打个日志就丢 | 首包静默丢失（`Connect` 后立刻发是常见写法） | `WriteMessage` |
| 5 | 连接失败的 socket 不关闭：`Disconnect()` 只在 `client.Connected` 时 `Close()` | **句柄泄漏**（反复重连失败会累积） | `Connect` 的 catch → `Disconnect` |
| 6 | `Connected` 直接返回 `TcpClient.Connected` | 该属性只表示"最近一次 I/O 的结果"，对端关闭后仍可能为 `true` → 业务误判 | `Connected` |
| 7 | 多线程 `SendMessage` 直接并发 `BeginWrite`；`SendTimeout/ReceiveTimeout` 对异步 I/O 无效 | 帧完整性依赖内核原子性（大包分片时有交错风险）；**看起来有超时保护，其实没有** | `WriteMessage` / 字段 |
| 8 | 帧循环条件写成 `RemainingBytes() > headerLength` | 恰好只剩一个 header 时（例如 0 长度消息）要等下一批数据才解析 | `OnReceive` |
| 9 | 所有回调都在 I/O 线程 | Unity 里在回调中碰 `Transform`/`Instantiate` 等会崩（工程上必须投递主线程） | 全部回调 |
| 10 | 每条消息 `MemoryStream` + `BinaryWriter` + `BinaryReader` + 两次拷贝 | 高频消息 GC 压力大 | `OnReceive` / `OnReceivedMessage` |

## 3. 修复后的实现

公开 API **完全保持**（`Connect` / `Disconnect` / `SendMessage` / `Connected` / `Client` / `MAX_READ` / `NoDelay` /
`SendTimeout` / `ReceiveTimeout` / 三个事件），并**新增**：

| 新增 | 作用 |
| --- | --- |
| `IDisposable` / `IsDisposed` | 忘记 `Disconnect` 也能确定释放 |
| `ConnectTimeoutMs`（默认 5000） | 真正可用的连接超时（时间到了断开并回调） |
| `Dispatcher`（`Action<Action>`） | 把 `OnConnected/OnData/OnDisconnected` 派发到指定线程（Unity 主线程） |
| `MAX_MESSAGE_LENGTH` | 单条上限常量（`ushort.MaxValue`），超长直接报错拒绝 |

内部重写要点：

1. **连接**：`new TcpClient(addressFamily)`（不再替换底层 socket）→ `BeginConnect` → 回调里 `EndConnect`（失败即回调）→ 定时器兜底超时 → 失败路径统一 `CloseClient()`（不再泄漏）。
2. **发送**：单写者队列（`Queue<byte[]>` + `sending` 标志），保证**顺序与调用一致、字节不交错**；未连接时先入队，连上后**补发**；`BeginWrite/EndWrite` 异常 → 断开回调。
3. **接收**：`readBuffer` → `pendingBuffer`（可增长）+ 偏移量；先**解析完再续读**（解析在 I/O 线程单链完成，天然有序、无并发破坏），切出的消息**一次性**派发；每条消息只分配 1 个 `byte[]`（去掉 MemoryStream / BinaryReader / 额外拷贝）。
4. **状态**：`connectedNotified` + 自有状态机，`Connected` 只反映框架认为的状态；`Disconnect` 可重复调用且只回调一次；`Disconnect` 之后可再次 `Connect`（重连）。
5. **解析边界**：`while (pendingLength - offset >= headerLength)`，半包自动回退等待；`CompactPending` 带防御性钳制。

## 4. 用法

```csharp
var client = new Hydrogen.TcpClient(new HeaderEncoder());   // 你自己的 IHeaderEncoder
// Unity 里建议把回调切回主线程
client.Dispatcher = action => Utility.Unity.PostToUnityThread(_ => action());

client.OnConnected    += () => Log.Info("connected");
client.OnData         += payload => { /* 完整的一条消息 */ };
client.OnDisconnected += () => Log.Warn("disconnected");

client.Connect("127.0.0.1", 8888);
client.SendMessage(bytes);      // 连接未就绪时会排队，连上后按序补发
// ...
client.Dispose();               // 或 client.Disconnect() 后复用（重连）
```

- 超过 `MAX_MESSAGE_LENGTH`（65535）的消息会被**明确拒绝并报错**，不会静默错帧；
- `Connect` 会先清理上一条连接（因此重连前不必手动 `Disconnect`）；
- 断开原因分两类写日志：`DisconnectResion.Disconnect`（对端关闭）走 `Log.Info`，`Exception`（连接失败/读写出错/超时）走 `Log.Error`。

## 5. 仍然存在的限制（不是 bug，但要知道）

| 限制 | 说明 |
| --- | --- |
| 单帧 ≤ 64KB | 由 `IHeaderEncoder.EncodeHeader(ushort)` / `DecodeHeader → ushort` 决定。**想支持更大帧必须改接口签名（`int`）**，README 里"header 通常 2 或 4 字节"的说法在 4 字节下也无法表达 >64KB（长度参数本身是 `ushort`） |
| 无心跳 / 自动重连 | 库只管连接与收发，断线检测与重连策略由业务做（`OnDisconnected` 里重连即可） |
| `SendTimeout` / `ReceiveTimeout` | 字段保留但**对 `BeginRead`/`BeginWrite` 无效**（异步 I/O 不吃这两个属性）；要超时用 `ConnectTimeoutMs` 或业务层计时 |
| 无 TLS / 压缩 | 需要时在 `NetworkStream` 外面套 `SslStream`（本库直接 `GetStream()`，要改造） |
| 回调线程 | 默认 I/O 线程；不设 `Dispatcher` 时不要在回调里碰 Unity API |

## 6. 验证

**14 用例 / 51 断言**的临时工程：把库的**真实源码**编进控制台程序，用 `TcpListener` 在 127.0.0.1 跑真实 socket。
位置：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\hydrogen\harness`（仓库外，可删）。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\hydrogen\harness\HydrogenHarness.csproj -- --exclude=T09   # 48 断言
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\hydrogen\harness\HydrogenHarness.csproj -- --only=T09       # 3 断言（独立进程）
```

覆盖：基本收发/顺序、粘包、半包（故意延迟切分）、8192/60000/70000 字节边界、重连、并发发送（小包 200 条 + 8KB 大包 80 条）、
连接被拒（不崩溃 + 报"connect failed"）、对端关闭、未连接时发送、未连接时发送报错、**200 条随机长度随机切片的模糊测试**。

修复前的基线（留存于 `_dshcache\20-projects\cosmos\hydrogen\baseline.txt`）：**33 通过 / 7 失败**，其中
连接被拒那一条直接把进程打崩（`Unhandled exception`），70000 字节那条切出 3 个垃圾帧。

并用变异测试验证用例有效（`mutate.ps1`）：把 4 个缺陷重新注入——
不校验长度、未连接即丢弃、先续读再解析、去掉 `EndConnect`——**4/4 全部检出**。
（"先续读再解析"这一条尤其说明问题：并发解析会让模糊测试只收到 8~9 条就把 200 条流解析乱掉。）

## 7. 给业务侧的建议

1. 用 `Dispatcher` 把回调切主线程，别在 I/O 线程碰引擎对象；
2. 心跳：业务层每 N 秒发一条空/心跳消息，超时未收到就 `Disconnect` + 重连；
3. 需要 >64KB 的大包：业务层自己分片（加序号+总长度），或把 `IHeaderEncoder` 改成 `int` 长度（要改库接口）；
4. 高频小包场景注意 `NoDelay = true` 会让每个包一个 TCP 段，必要时批量合并发送。
