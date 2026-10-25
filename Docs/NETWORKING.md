# 网络模块的分工：WebRequest 与 Download

> 结论：**两个模块不合并。** 它们共用的只是"HTTP 传输"这一层；上层语义完全不同——
> 一个是"一次请求换一份内存数据"，另一个是"把文件安全地落到磁盘"。
> 合并会得到一个既要管内存结果、又要管断点/校验/原子提交的调度器，两边都会变难维护。
>
> 本轮做的是**把重叠的部分真的收干净**：删掉 WebRequest 里那个"能落盘但不续传/不校验"的
> 文件下载 API，并把两个模块重复的 HTTP 判定抽到 `Utility.Net`。
> 参考实现（GameFramework）也是同样的取舍，见 §2。

## 1. 两者的本质差异

| 维度 | WebRequest | Download |
| --- | --- | --- |
| 产物 | 内存对象（`string` / `byte[]` / `Texture2D` / `AudioClip` / `AssetBundle`） | 磁盘上的**文件工件** |
| 单位 | 一次请求（handle） | 一个文件任务（task，可重试/暂停/续传） |
| 断点续传 | 不需要 | **核心**（临时文件 + Range/206 + 长度校验） |
| 失败语义 | 请求失败 → 该 handle 失败（可按次数重试整个请求） | 一次尝试失败 → 保留断点 + 指数退避重试，必要时丢弃脏文件 |
| 超时语义 | **总时长**超时（`TimeoutSeconds`，默认 30s） | **空闲**超时（`UnityWebRequest.timeout`，大文件可下几分钟） |
| 完成动作 | 把结果交给调用方，用完即弃 | 校验长度 → `File.Move` 原子提交到目标路径 |
| 典型用途 | 网关版本号、配置、公告、排行榜、头像贴图 | 热更包、语音包、视频、Mod、离线资源 |
| 驱动 | `OperationSystem`（`WebRequestHandle<T>` 是 `OperationBase`） | 模块 `OnUpdate` 的 tick 状态机 |

把这两者塞进一个模块，等于让一个调度器同时承担"请求生命周期"和"文件工件生命周期"两套状态机，
并且必须用一堆开关区分超时/重试语义——这正是难维护的典型形态。

## 2. 参考实现：GameFramework 也是两个模块

`EllanJiang/GameFramework`（本框架架构的主要参照）的模块列表里 `Download` 与 `WebRequest` **同时存在**，
且职责划分与上面一致：[GameFramework/Download](https://github.com/EllanJiang/GameFramework/tree/master/GameFramework/Download)
（`DownloadAgent` / `DownloadTask` / `DownloadTaskStatus` / `DownloadCounter`）与
[GameFramework/WebRequest](https://github.com/EllanJiang/GameFramework/tree/master/GameFramework/WebRequest)
（`WebRequestAgent` / `WebRequestTask` / `WebRequestTaskStatus`）。

关键细节：**GameFramework 的 WebRequest 没有任何"下载文件到路径"的 API**——落盘是 Download 的专属职责。
本框架之前多出来的 `AddDownloadFileTaskAsync` 正是两者唯一的真重叠（见 §3）。

（顺带：TEngine 把 Download / WebRequest 都裁掉了，只留 11 个模块——说明"要不要这两个"取决于项目，
但"这两个是不是一个"在参考实现里是明确的：不是。）

## 3. 本轮改动

### 3.1 删除重叠 API：`AddDownloadFileTaskAsync`

它用 `DownloadHandlerFile(savePath)` 一次性落盘，**没有**断点续传、**没有**长度校验、**没有**原子提交，
一旦网络中断就留下半截文件，而且 `removeFileOnAbort` 默认为 true 会把已下载数据删掉。
它与 Download 模块构成"两个都能下载文件、行为还不一样"的歧义。

| 需求 | 现在应该用 |
| --- | --- |
| 小文件/配置落到磁盘 | `DownloadManager.AddDownload(url, path)`（默认断点续传 + 校验） |
| 只要数据、不需要文件 | `WebRequestManager.AddDownloadBytesTaskAsync(url)` |

### 3.2 抽出共用 HTTP 判定：`Utility.Net`

两个模块各自重复了「成功判定」「Content-Length 解析」「Content-Range 推导」，其中成功判定还带着
`#if UNITY_2020_1_OR_NEWER` 的两套代码。现在统一到 `Utility.Net`：

```csharp
bool ok      = Utility.Net.IsRequestSucceeded(request);   // 2020+ 用 result == Success（数据解析失败也算失败）
long length  = Utility.Net.GetContentLength(request);     // 缺失/非法 → -1
long bytes   = Utility.Net.GetTransferLength(request);    // Content-Length 优先，其次 Content-Range
bool parsed  = Utility.Net.TryParseContentRange("bytes 100-999/1000", out var start, out var end, out var total);
```

替换点：`WebRequestHandle`（请求完成判定）、`WebUrlFileRequestTask`（页面抓取判定）、
`WebRequestManager.AddGetContentLengthTaskAsync`、`WebRequester.CreateContentLengthHandle`、
`Download/UnityWebRequestDownloadTransport`（长度推导 + 成功判定）。

> 行为变化：`result == DataProcessingError` 之前被当成成功（旧判定只排除 Connection/Protocol），
> 现在按失败处理——这本来就是错误分类。

## 4. 选择规则（写进团队规范）

```text
要的是"数据"？         → WebRequest（handle，可 await/取消/优先级/超时重试）
要的是"磁盘上的文件"？  → Download（task，可断点续传/暂停恢复/批量进度）
要的是"资源（AB/预制体）"？ → Resource 模块（Provider 负责下载与缓存）
```

三条都不要交叉：**WebRequest 不再提供落盘**，**Download 不解析业务数据**，**Resource 不碰原始文件下载**。

## 5. 验证

- 全量编译：`Cosmos.Runtime` 310 文件、Editor、25 个示例文件全绿。
- 下载引擎行为工程扩充了 **HTTP 头解析/成功判定** 用例（与 WebRequest 共用同一实现）：
  `17 HTTP 头解析与成功判定` —— 共 **72 断言**全过。
- 音频（84）、输入（53）回归全过。

```powershell
dotnet run --project E:\DeekSeekSetting\_dshcache\20-projects\cosmos\download\harness\DownloadHarness.csproj
```

## 6. 不做的事（以及为什么）

| 想法 | 为什么不 |
| --- | --- |
| 把 Download 并进 WebRequest 的调度器 | WebRequest 的 handle 超时是"总时长"、重试是"整个请求重发"；文件下载需要"空闲超时"和"保留断点重试"。合并要加一堆开关区分语义，且会牺牲刚建立并验证过的文件任务状态机 |
| 让 WebRequest 支持断点续传 | 内存结果没有"断点"概念；续传需要临时文件、206 校验、原子提交——那是 Download 的完整职责 |
| 两个模块共用一个 `MaxConcurrent` | 可以按需在业务层统一设置两个值；强行共享会让"网关请求"被"大文件下载"挤占并发槽 |
