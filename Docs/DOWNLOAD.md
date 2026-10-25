# 下载模块（队列 + 断点续传 + 校验）

> 结论：**下载模块保留在 `Runtime/Modules/Download`，与其它模块同一程序集。**
> 它需要 `Utility.*` / `CosmosEntry` / `ReferencePool` 这些框架设施，拆成独立程序集只会换来
> "跨程序集不能 partial CosmosEntry、访问器要另起一个类"的成本，收益很小。
> 真要瘦身，删除目录 + `BuiltinModuleRegistry` 一行即可（其它核心模块不依赖它）。

## 1. 旧实现的问题（逐条对应"不完整"）

| 问题 | 后果 |
| --- | --- |
| 只设置 `Range` 头就当续传成功 | 服务端忽略 Range（返回 200）时，`DownloadHandlerFile(append:true)` 会把**完整文件追加到半截文件后面** → 文件损坏且大小翻倍 |
| 没有长度校验 | 断流/被代理截断的下载被当作成功提交，得到一个"能读但内容不全"的文件 |
| `DownloadTask` 是 struct 且只有 4 个字段 | 没有状态、没有预期长度、没有重试、没有错误信息，外部无法查询"这个任务到底怎么了" |
| 失败不可重试 | 一次瞬时网络错误即永久失败 |
| 回收/清理耦合在播放式的流程里 | `OnCancelDownload` 里留着 `//todo 这里需要将pending列表中的任务变更为下载失败`，取消后失败信息直接丢失 |
| `static long downloadId` | 多个下载器实例共享序列号 |
| `DownloadDataProxy` 静态全局 | 超时/重定向是全局静态，无法按模块配置 |
| 没有并发上限、没有优先级、没有暂停 | "支持暂停/恢复"只写在注释里 |
| 进度用 `operation.progress` | 续传时进度不含已下载偏移，永远是错的 |

## 2. 新结构

```text
DownloadManager（Module，只做门面）
   │  OnUpdate：delta = unscaledTime 差值（timeScale=0 时下载不中断）
   ▼
DownloadScheduler（纯 C#，可脱离 Unity 单测）
   │  任务状态机 / 并发 / 优先级 / 重试退避 / 断点校验
   ▼
IDownloadTransport ──► UnityWebRequestDownloadTransport（默认实现）
   │
   ▼
DownloadStorage（临时文件 / 长度 / 提交）
```

分层的意义：**策略与传输解耦**。调度器的所有状态迁移都发生在 `Tick` 里，传输层只回答
"这一帧写了多少字节、HTTP 码是多少、总长多少"，因此断点续传逻辑可以被完整单测（见 §7）。

## 3. 任务状态机

```text
Queued ──► Downloading ──► Verifying ──► Completed
  ▲            │  │
  │            │  └──► WaitingRetry ──(退避到点)──► Queued
  │            └──► Paused ──(Resume)──► Queued
  └──────────────► Failed（重试耗尽）
                 Canceled（移除/取消）
```

| 状态 | 含义 |
| --- | --- |
| `Queued` | 已入队，等并发槽 |
| `Downloading` | 传输中 |
| `Verifying` | 传输已结束，正在校验长度 |
| `Completed` | 临时文件已提交到目标路径 |
| `WaitingRetry` | 失败后退避中 |
| `Paused` | 已暂停（临时文件保留） |
| `Failed` / `Canceled` | 终止状态 |

## 4. 断点续传协议

```text
1) 切分：下载中的数据只写   <目标路径>.download   （DownloadSettings.TempFileExtension）
2) 偏移：临时文件的长度 = 断点偏移（跨进程有效：重装/重启后再次 Add 同一目标即可续传）
3) 请求：偏移 > 0 时带 Range: bytes=<offset>-，且 removeFileOnAbort = false（否则中断会删掉断点）
4) 校验：响应码必须是 206
          · 返回 200（服务端忽略 Range）→ 丢弃临时文件，从 0 重下（绝不追加脏数据）
5) 校验：文件长度必须等于「响应长度 + 偏移」（无 Content-Length 时跳过）
          · 偏长 → 删除重下；偏短 → 保留，下一次续传补齐
6) 提交：长度校验通过后才 File.Move 到目标路径（半截文件永远不会被当成可用资源）
```

> 第 4 步与第 5 步是**双保险**：服务端返回 200 时通常长度校验也会发现异常，但如果响应里没有
> `Content-Length`（分块传输、代理剥头），长度校验会失效，此时 Range 校验是唯一防线。

## 5. 重试与节流

- 重试：`AttemptCount <= RetryLimit` 时进入 `WaitingRetry`，等待 `RetryDelaySeconds * 2^(n-1)`（指数退避）。
- 失败后若临时文件比预期长（写脏了）先删除，否则保留以续传。
- 进度事件默认每 `0.1s` 一次（`DownloadSettings.ProgressIntervalSeconds`），避免每帧刷 UI。
- 并发默认 3（`MaxConcurrent`），队列按 `Priority` 取最大者，同优先级按入队顺序。

## 6. API 速查

```csharp
var dl = CosmosEntry.DownloadManager;          // 内置模块，未启动时为 null

// 入队（默认断点续传）+ 启动
long id = dl.AddDownload(url, savePath);                        // ResumeIfPossible
dl.AddDownload(url2, savePath2, DownloadMode.Overwrite);
var options = DownloadOptions.Default;                          // 可指定优先级/重试/超时/追加偏移
options.Priority = 10;
dl.AddDownload(url3, savePath3, options);
dl.LaunchDownload();

// 运行控制
dl.PauseDownload();        // 中止进行中的传输，保留断点
dl.ResumeDownload();       // 重新排队，从断点继续
dl.RemoveDownload(id);     // 移除（DeleteFileOnAbort=true 时连同临时文件）
dl.CancelDownload();       // 等价于 RemoveAllDownload

// 查询
dl.GetDownloadInfo(id, out var info);   // 状态/已下载/预期/进度/速度/尝试次数
dl.GetDownloadTaskCount();
dl.Settings.MaxConcurrent = 4;          // 调度参数
dl.DownloadTimeout = 60;
dl.DeleteFileOnAbort = false;           // 默认 false：保留断点
```

事件：`OnDownloadStart` / `OnDownloadSuccess` / `OnDownloadFailure` / `OnDownloadOverallProgress` /
`OnAllDownloadTaskCompleted`。进度事件里新增了 **`OverallProgress` / `TotalDownloadedBytes` /
`TotalExpectedBytes`**（按字节加权，比旧的"序号/总数"准确）。

## 7. 行为验证

仓库没有该模块的单测工程。本次重写的验证是一个 **16 用例 / 53 断言的临时工程**：
把 `DownloadScheduler` / `DownloadTask` / `DownloadStorage` 的**真实源码**编进控制台程序，
用可编程的假传输（可模拟"忽略 Range / 断流截断 / 网络失败 / 限速"）逐帧驱动。
位置：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\download\harness`（仓库外，可删）。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\download\harness\DownloadHarness.csproj
# 断言：53 通过 / 0 失败  === ALL-PASS ===
```

并用**变异测试**验证用例有效：重新注入 6 个缺陷（不校验 Range、不校验长度、忽略断点、
不重试、并发上限失效、优先级失效）**全部被检出**。脚本：`E:\DeekSeekSetting\_dshcache\20-projects\cosmos\download\mutate.ps1`。

## 8. 与 WebRequest 模块的分工

| | WebRequest 模块 | Download 模块 |
| --- | --- | --- |
| 定位 | 通用网络请求（文本/字节/纹理/音频/AB/上传） | **文件批量落地** |
| 断点续传 | 不支持 | 支持（临时文件 + Range 校验） |
| 落盘校验 | 无 | 长度校验 → 原子提交 |
| 队列 | 有自己的句柄+调度器 | 自己的队列（面向"一批文件 + 总进度"） |

两者都基于 `UnityWebRequest`；需要"下载一个文件并落盘、可断点续传"时用 Download，
只需要"拿一份数据"时用 WebRequest。没有互相依赖，避免了一个模块两套调度器互相等待。

## 9. 相对旧版的 API 变化

| 旧 | 新 |
| --- | --- |
| `IDownloader` + `SetOrUpdateDownloadHelper` | `IDownloadTransport` + `SetTransport`（默认 `UnityWebRequestDownloadTransport`） |
| `IDownloadRequester` / `GetUriFileSizeAsync` | 取消：长度从下载响应头直接得到，无需预请求 |
| `IDownloadUrlHelper` / `GetUrlFilesSizeAsync` | 取消：目录枚举属于业务层（示例里用 `WebRequestManager.AddUrlFileRequestTask`） |
| `DownloadDataProxy`（静态全局配置） | `DownloadManager.Settings`（`DownloadSettings`） |
| `AddDownload(url, path, offset, append)` | `AddDownload(url, path)` / `(…, DownloadMode)` / `(…, DownloadOptions)` |
| `DownloadTask`（struct） | `DownloadTask`（class，内部状态机）；对外用 `DownloadInfo` 快照 |
| `DownloadInfo` 6 字段 | 新增 `DownloadState` / `ExpectedLength` / `SpeedBytesPerSecond` / `AttemptCount` |
