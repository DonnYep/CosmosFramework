# Hydrogen

**轻量 TCP 客户端**：一个类、三个事件、两个方法，接入成本几乎为零。

```csharp
var client = new Hydrogen.TcpClient(new HeaderEncoder());
client.OnData += bytes => { /* 一条完整消息 */ };
client.Connect("127.0.0.1", 8888);
client.SendMessage(bytes);
```

## 安装

Hydrogen 是一个标准 UPM 包（`com.donnyep.hydrogen`），**不依赖任何其他包**，支持 Unity 2020.3 及以上。

**方式一：Package Manager 填 Git 地址**

`Window → Package Manager → 左上角 + → Add package from git URL`，填入：

```text
https://github.com/DonnYep/Hydrogen.git
```

**方式二：锁定版本**（正式项目推荐，避免上游更新带来意外变更）

```text
https://github.com/DonnYep/Hydrogen.git#v0.1.0
```

**方式三：直接写 `Packages/manifest.json`**

```json
{
  "dependencies": {
    "com.donnyep.hydrogen": "https://github.com/DonnYep/Hydrogen.git#v0.1.0"
  }
}
```

**方式四：本地路径**（改源码调试时用，指向**仓库根**，不是 `Runtime/`）

```json
{
  "dependencies": {
    "com.donnyep.hydrogen": "file:E:/Development/Donn/Hydrogen"
  }
}
```

> - Unity 是通过 **git** 去拉这个包的，本机必须能调用 `git`；否则会报
>   `Cannot perform upm operation: Unable to add package ...`。
> - **包名是 `com.donnyep.hydrogen`，不是文件夹名 `Hydrogen`**，写进 `manifest.json` 时按包名写。
> - `Hydrogen` 这个程序集在同一工程里只能存在一份：如果工程内还留着旧拷贝（例如直接拖进
>   `Assets/` 的那份），请先删掉，否则会出现重复程序集错误。
> - 卸载：Package Manager 中选中 Hydrogen → `Remove`，或从 `manifest.json` 里删掉该行。

## 设计取向

Hydrogen 的目标是**轻便易用**，所以刻意只做一件事：把 TCP 流的收发管好，其余交给业务。

| 选择 | 好处 |
| --- | --- |
| **单类实现**（一个 `TcpClient` + 一个 `IHeaderEncoder` 接口） | 不用学框架、不用装配件，丢进工程就能用 |
| **零依赖** | 只用到 `UnityEngine.Debug` 记日志，替换 `Hydrogen.Log` 后就完全不依赖 Unity |
| **全异步**（`BeginRead` / `BeginWrite`） | 不占协程、不自己开线程、不产生 `Task` 与 GC 压力 |
| **帧头可插拔**（`IHeaderEncoder`） | 2 字节长度、4 字节长度、protobuf varint 都由你定；库只负责"按你的头切帧" |
| **粘包 / 半包自动处理** | 业务回调拿到的永远是**一条完整消息**，不用自己拼缓冲 |
| **失败可见** | 连接被拒、超时、读写异常、对端关闭都会走 `OnDisconnected` 并写明原因，不会静默卡死 |
| **可控的线程边界** | 默认回调在 I/O 线程；设置 `Dispatcher` 即可整体切回 Unity 主线程 |

## 1. 三步接入

### 1.1 定义帧头

帧格式固定为 `[header][message]`：header 里记录消息体长度，**header 多长、怎么编码由你决定**。

```csharp
using Hydrogen;

/// <summary>2 字节大端长度头：单条消息上限 65535 字节</summary>
public class HeaderEncoder : IHeaderEncoder
{
    public int GetHeaderLength() => 2;                       // header 占几个字节（必须固定）

    public byte[] EncodeHeader(ushort length)                // 发送时写入消息体长度
    {
        return new byte[] { (byte)(length >> 8), (byte)length };
    }

    public ushort DecodeHeader(byte[] header)                // 接收时读出消息体长度
    {
        return (ushort)((header[0] << 8) | header[1]);
    }
}
```

> 这里刻意不用 `System.Buffers.Binary.BinaryPrimitives`：它是 .NET Standard 2.1 才有的 API，
> Unity 2020.3 默认的 .NET Standard 2.0 下**不存在**。若你的工程已切到 .NET Standard 2.1，用它会更简洁。

### 1.2 创建并连接

```csharp
var client = new Hydrogen.TcpClient(new HeaderEncoder());

client.OnConnected    += () => Debug.Log("已连接");
client.OnData         += payload => Debug.Log($"收到 {payload.Length} 字节");
client.OnDisconnected += () => Debug.Log("已断开");

client.Connect("127.0.0.1", 8888);
```

### 1.3 收发

```csharp
client.SendMessage(payload);        // 发一条完整消息（库自动加 header）
```

`OnData` 每触发一次就是**一条完整消息**：粘在一起的多个包会拆开分别回调，被拆断的包会等拼齐再回调。

## 2. 完整用法

### 2.1 连接、断开与重连

```csharp
client.Connect(host, port);   // 会先清理上一条连接，所以重连不必手动 Disconnect
client.Disconnect();          // 可重复调用；每个连接最多触发一次 OnDisconnected
// 之后可以再次 Connect：同一实例支持反复重连
```

- **连接超时**：`client.ConnectTimeoutMs = 5000;`（≤0 表示不设超时）。超时会走 `OnDisconnected` 并记 `connect failed : timeout`。
- **连接失败**（端口没人监听、DNS 无效等）同样走 `OnDisconnected`，日志是 `connect failed : <原因>`。
- **不要把 `Connected` 当心跳**：它只表示"框架认为仍连接着"；TCP 半开连接要靠业务心跳发现（见 §5.1）。

### 2.2 发送

```csharp
client.SendMessage(bytes);    // 长度 1 ~ 65535
```

| 情况 | 行为 |
| --- | --- |
| 连接尚未就绪就发送 | **先入队**，连接成功后按调用顺序补发（不丢首包） |
| 同一帧从多个线程发送 | 内部单写者队列，**顺序与调用一致、字节不交错** |
| 空数组 / `null` | 拒绝并 `Log.Error` |
| 超过 `MAX_MESSAGE_LENGTH`（65535） | 拒绝并 `Log.Error`——长度头只有 2 字节，**绝不截断**（截断会让整条流从此刻起错位） |
| 已断开（未 `Connect`） | 拒绝并 `Log.Error`，**不排队**——避免把过期消息发到下一条连接上 |

### 2.3 接收

```csharp
client.OnData += payload =>
{
    // payload 就是发送方 SendMessage 的那一份字节（一条一条，顺序一致）
};
```

- 解析在 I/O 线程持续进行，**主线程再忙也不会阻塞收包**；只有业务回调按 `Dispatcher` 排队。
- 对端发送 0 长度消息时会回调一条空数组（本库自身不会发空消息）。
- 单个连接内消息**严格有序**。

### 2.4 回调切回主线程（Unity）

默认回调都在 I/O 线程。要在回调里安全地碰引擎对象，设置 `Dispatcher`：

```csharp
using System;
using System.Collections.Concurrent;
using UnityEngine;

/// <summary>极简主线程队列，挂到任意常驻物体上即可</summary>
public class UnityMainThread : MonoBehaviour
{
    static readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

    void Update()
    {
        while (queue.TryDequeue(out var action)) action();
    }

    public static void Post(Action action) => queue.Enqueue(action);
}

// 交给 Hydrogen
client.Dispatcher = UnityMainThread.Post;
```

`Dispatcher` 需要**先进先出**（队列式），否则消息顺序可能被打乱；`Post` 就是队列语义，可直接用。
若你已有现成的主线程派发工具（例如 CosmosFramework 的 `Utility.Unity.PostToUnityThread`），直接复用即可。

### 2.5 释放

```csharp
client.Dispose();      // 断开 + 清空回调，之后不可再用
// 或者：client.Disconnect();  之后仍可 Connect 复用
```

`TcpClient` 实现了 `IDisposable`，建议在 `OnDestroy` 里 `Dispose()`。

## 3. 事件与属性

| 成员 | 说明 |
| --- | --- |
| `event Action OnConnected` | 连接成功（每次连接一次） |
| `event Action<byte[]> OnData` | 收到一条完整消息 |
| `event Action OnDisconnected` | 断开：对端关闭、连接失败、超时、读写异常都走这里；每个连接最多一次 |
| `bool Connected` | 框架认为的连接状态（不代替心跳） |
| `System.Net.Sockets.TcpClient Client` | 底层对象，调试用（连接建立前为 `null`） |
| `bool IsDisposed` | 是否已释放 |
| `bool NoDelay = true` | 关闭 Nagle；小包高频场景可按需关掉 |
| `int ConnectTimeoutMs = 5000` | 连接超时（毫秒） |
| `int SendTimeout / ReceiveTimeout` | 同步 API 的收发超时；**对异步收发无效**（保留以兼容） |
| `Action<Action> Dispatcher` | 回调派发器，`null` 表示直接在 I/O 线程回调 |
| `const int MAX_READ` | 读缓冲大小（8192），与单条消息上限无关 |
| `const int MAX_MESSAGE_LENGTH` | 单条消息体上限（65535） |

## 4. 线程模型

```text
        Connect / SendMessage         OnConnected / OnData / OnDisconnected
业务线程 ───────────────────► 库内部 ─────────────────────────────► Dispatcher（可空）→ 业务
                               │
                               ├─ I/O 完成线程：BeginRead / BeginWrite 回调
                               └─ 解析：读缓冲 → pending 缓冲 → 切帧（单链，天然有序）
```

- `SendMessage` 可在任意线程调用（内部加锁排队）；
- `Connect` / `Disconnect` / `Dispose` 建议在业务主线程调用；
- 回调线程 = I/O 线程（未设置 `Dispatcher` 时）。

## 5. 常见场景

### 5.1 心跳 + 断线重连

```csharp
IEnumerator KeepAlive()
{
    var heartbeat = new byte[] { 0x01 };
    while (true)
    {
        if (client.Connected)
            client.SendMessage(heartbeat);
        else
            client.Connect(host, port);       // 失败会走 OnDisconnected，下一轮再试

        yield return new WaitForSecondsRealtime(5f);
    }
}
```

> 半开连接（对端掉线但本机没收到 FIN）只能靠"多久没收到数据就断开"判断：
> 记录最后一次 `OnData` 的时间，超过阈值就 `Disconnect()` 再重连。

### 5.2 超过 64KB 的大包

`IHeaderEncoder` 的长度参数是 `ushort`，单帧上限 65535 字节。更大的数据请在业务层分片：

```text
[总长度 4][序号 4][分片数据 N]      // 发送方切片，接收方按序号拼装
```

或者改造 `IHeaderEncoder` 把长度换成 `int`（属于接口变更，需要同步改库与所有编码器）。

### 5.3 序列化（protobuf / MessagePack / JSON）

库只搬字节，序列化与协议完全由业务决定：

```csharp
client.SendMessage(MessagePackSerializer.Serialize(new LoginRequest { /* ... */ }));

client.OnData += payload =>
{
    var message = MessagePackSerializer.Deserialize<LoginRequest>(payload);
};
```

### 5.4 接管日志

```csharp
Hydrogen.Log.Info    = obj => Debug.Log(obj);
Hydrogen.Log.Warning = obj => Debug.LogWarning(obj);
Hydrogen.Log.Error   = obj => Debug.LogError(obj);     // 默认就是这三个
```

## 6. 限制

| 限制 | 说明 |
| --- | --- |
| 单帧 ≤ 64KB | 由 `ushort` 长度决定，见 §5.2 |
| 无心跳 / 无自动重连 | 库只管连接与收发，策略由业务实现（§5.1） |
| 无 TLS / 压缩 | 需要时自行包 `SslStream` |
| `SendTimeout` / `ReceiveTimeout` 对异步无效 | 异步 I/O 不使用这两个属性 |
| 不保证"对端一定收到" | TCP 只保证有序可靠传输，业务层仍需自己的应答/序号机制 |

## 7. 验证

- **14 个用例 / 51 个断言全部通过**，全部跑在**真实 `127.0.0.1` 回环 socket** 上。覆盖：基本收发、粘包、半包、8192 / 60000 / 70000 字节边界、重连、4 线程并发发送、并发大包、连接被拒、对端关闭、连接未就绪时发送、未连接时发送，以及 200 条"随机长度 + 随机切片"的模糊测试。
- **变异测试 4/4 检出**：把修掉的缺陷重新注入（超长不校验长度、未连接就丢弃、先续读再解析、去掉 `EndConnect`），用例全部报错——说明用例本身有效，不是"碰巧跑绿"。
- 这些验证直接编译**本包 `Runtime/` 的源码**（在 .NET 上用最小的 `UnityEngine` 替身，不启动 Unity），所以结论对应当前发布的内容。
- 首发前修掉的实现缺陷清单见 [`CHANGELOG.md`](CHANGELOG.md) 的 `0.0.1` 节。
- 测试工程**不随包分发**，免得给使用者引入额外的 Test Framework 依赖。要在 CI 里复跑，可在 `Tests/` 下自建带 `UNITY_INCLUDE_TESTS` 约束的测试程序集。

## 8. 版本与包信息

| 项 | 值 |
| --- | --- |
| 包名 | `com.donnyep.hydrogen` |
| 显示名 | Hydrogen |
| 版本 | 0.1.0 |
| 最低 Unity | 2020.3 |
| 依赖 | 无（仅 `UnityEngine.Debug`，可替换） |
| 安装地址 | `https://github.com/DonnYep/Hydrogen.git` |
| 许可证 | MIT |

## 9. 目录说明

```text
Hydrogen/                 ← 仓库根 = 包根
├── package.json          ← UPM 包清单（name / version / unity 等）
├── README.md             ← Package Manager 详情页显示的内容
├── CHANGELOG.md          ← 版本变更记录
├── LICENSE.md            ← MIT
├── Runtime/              ← 运行时代码（成品程序集 Hydrogen）
│   ├── Hydrogen.asmdef
│   ├── TcpClient.cs
│   ├── IHeaderEncoder.cs
│   ├── Log.cs
│   └── DisconnectResion.cs
└── .gitattributes        ← 统一 LF 换行、标记二进制
```

`Runtime/` 与 `Assembly-CSharp` 分离，包内代码只在 `Hydrogen` 程序集里编译，不会污染业务程序集；
`autoReferenced: true` 让业务代码无需手动引用即可直接使用。

<!-- 发布备忘（供维护者，不在 UI 中强调）
  1. 改 package.json 的 version + CHANGELOG 顶部加一节
  2. git add -A && git commit -m "release: vX.Y.Z"
  3. git tag vX.Y.Z && git push origin main --follow-tags
  4. Unity 中按 https://github.com/DonnYep/Hydrogen.git#vX.Y.Z 安装来自验
-->
