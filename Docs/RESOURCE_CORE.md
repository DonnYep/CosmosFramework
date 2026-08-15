# Cosmos.Resource.Core — 独立异步资源加载核心库

> 本文档介绍重构后的资源加载体系。旧版 `Modules/Resource/Legacy` 已废弃，仅作兼容保留。

## 1. 架构总览

重构后资源加载分为三层：

```
┌─────────────────────────────────────────────────────────┐
│ 业务层：CFResources 静态门面（Addressables 风格）        │
│   LoadAssetAsync<T>(key) / LoadSceneAsync / LoadByTag   │
├─────────────────────────────────────────────────────────┤
│ 核心库：Assets/CosmosFramework/Runtime/Base/ResourceCore│
│   程序集：Cosmos.Resource.Core（独立 asmdef，仅依赖     │
│   UnityEngine，不依赖框架任何其他程序集）                │
│   · OperationSystem / OperationBase —— 通用异步操作驱动  │
│   · ResourcePackage / PackageManifest —— 包裹与清单      │
│   · ProviderBase + 各类 Provider —— 加载流程与引用计数   │
│   · HandleBase + 各类 Handle —— 对外交互句柄             │
│   · ResourceDiagnostics —— 加载诊断与泄漏检测            │
├─────────────────────────────────────────────────────────┤
│ 工具层：Window/Cosmos/Module/Resource/*                  │
│   ResourceEditor   —— 资源编辑分类（分组/寻址/标签）     │
│   ResourceBuild    —— AB 构建与清单导出                  │
│   CFResourceAnalyzer —— 去重与检索分析                   │
└─────────────────────────────────────────────────────────┘
```

设计参考：[YooAsset](https://github.com/tuyoogame/YooAsset)（操作句柄 + 引用计数 + 资源包模型）、
[Unity Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@latest/manual/Groups.html)（Group/Label 编辑工具）。

## 2. 为什么独立成库

- **零框架依赖**：`Cosmos.Resource.Core.asmdef` 不引用 `Cosmos.Runtime`，可直接移植到其他 Unity 工程；
- **核心底座**：`OperationSystem`/`OperationBase` 作为通用异步驱动，WebRequest 模块（并发调度、超时重试）同样构建于其上；
- **职责清晰**：核心库只做“加载”，模块层只做“集成”，编辑器只做“工具”。

## 3. 快速上手

### 3.1 初始化

```csharp
using Cosmos.Resource;

var initHandle = CFResources.InitializeAsync(new ResourceInitParameters()
{
    PackageName = "DefaultPackage",
#if UNITY_EDITOR
    PlayMode = CFResourcePlayMode.AssetDatabase,   // 编辑器模拟：无需打AB包
#else
    PlayMode = CFResourcePlayMode.AssetBundle,     // 真机：加载StreamingAssets/persistentDataPath下的AB包
#endif
    ManifestPath = "CosmosResources/DefaultPackage/PackageManifest.json",
    BundleDirectory = "CosmosResources/DefaultPackage",
    ManifestEncryptionKey = "CosmosBundlesKey",
});
initHandle.Completed += h => { if (h.IsSucceeded) Debug.Log($"version : {h.Version}"); };
await initHandle.Task;
```

### 3.2 加载资产（四种用法）

```csharp
// 事件回调
var handle = CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube");
handle.Completed += h => { var go = Instantiate(h.Asset); h.Release(); };

// 协程等待
var handle = CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube");
yield return handle;
Instantiate(handle.Asset);
handle.Release();

// async/await
var cube = await CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube").Task;

// 同步等待（仅建议非频繁调用）
var cube = CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube").WaitForCompletion();
```

- assetKey 支持三种写法：资产名 `ResCube`、带后缀 `ResCube.prefab`、完整路径 `Assets/Prefabs/ResCube.prefab`；
- **同一资产的多路加载共享同一个 Provider，不会重复发起加载请求**；
- 每个句柄持有一次引用计数，`Release()` 归零后自动卸载对应资产与包体。

### 3.3 分类标签加载（Addressables Label 风格）

```csharp
// 编辑器中对资产打上标签（如 "UI", "Level1"），运行时按标签整组加载
var handle = CFResources.LoadAssetsByTagAsync<GameObject>("UI");
handle.Completed += h => { var list = h.Assets; ... };
```

### 3.4 场景加载

```csharp
var sceneHandle = CFResources.LoadSceneAsync("Assets/Scenes/MyScene.unity", additive: true);
sceneHandle.Completed += h => Debug.Log($"Scene loaded : {h.SceneName}");
sceneHandle.UnloadAsync();   // 卸载
```

## 4. 调试与诊断（方便调试）

核心库内置 `ResourceDiagnostics`，默认编辑器开启、真机可手动开启：

```csharp
CFResources.SetDiagnosticsEnabled(true);
CFResources.SetDiagnosticsCaptureStackTrace(true);   // 捕获调用栈，定位泄漏源头

// 运行时信息（当前加载的资产/引用计数/内存占用）
ResourceRuntimeInfo info = CFResources.GetRuntimeInfo();

// 历史加载记录（耗时/成败/错误）
var records = CFResources.GetLoadRecords();

// 失败记录 / 潜在泄漏（句柄未释放）
var failed = CFResources.GetFailedLoadRecords();
var leaks   = CFResources.GetLeakedRecords();

// 一键输出诊断摘要到控制台
Debug.Log(CFResources.GenerateDiagnosticsReport());
```

## 5. 编辑器工具（资源编辑分类）

### 5.1 ResourceEditor（Window/Cosmos/Module/Resource/ResourceEditor）

Addressables Groups 风格的资源管理窗口：

1. **Package**：选择/新建资源包裹，编辑包名与版本；
2. **Bundle**：左侧创建/删除资源包，可勾选 PackSeparately（每资产独立成包）；
3. **Assets**：右侧为选中包添加资产 —— 支持拖放单个资产或按文件夹批量收集；
4. **Address**：编辑资产的寻址名（运行时加载 key）；
5. **Tags**：编辑资产的分类标签（逗号分隔，配合 `LoadAssetsByTagAsync`）；
6. 一键 **Assign/Clear BundleNames**，随后打开 Build 窗口构建。

### 5.2 ResourceBuild（Window/Cosmos/Module/Resource/ResourceBuild）

- `Build Package`：收集资产（GUID 去重 + 共享依赖自动归入 `~shared_` 依赖包）→ 分配 AB 名 → 构建 → 生成 `PackageManifest.json`（可选 AES 加密）→ 可选拷贝 StreamingAssets；
- `Export Manifest Only`：编辑器模拟模式，无需打 AB 包即可加载。

### 5.3 CFResourceAnalyzer（Window/Cosmos/Module/Resource/CFResourceAnalyzer）

- 重复 GUID 检测（同资源被多包引用）；
- 重复内容检测（不同路径相同内容，按 MD5）；
- 资源检索（名称/路径/标签关键字）。

## 6. 高性能设计要点

| 设计 | 说明 |
| --- | --- |
| Provider 共享 | 同一资产并发加载只发一次底层请求，句柄共享引用计数 |
| BundleLoader 共享 | 同一包体只创建一次 `LoadFromFileAsync`，依赖包递归先加载 |
| 引用计数卸载 | 句柄 Release 归零即卸载包体（`Unload(false)` 保留实例对象） |
| 优先级调度 | OperationSystem 按 Priority 排序驱动 |
| 诊断环形缓冲 | 记录上限 512 条，避免无限增长 |
| 无协程驱动 | 全部基于 OperationBase 状态机 + 每帧驱动，无每帧协程分配 |

## 7. WebRequest 模块（同底座并发升级）

`WebRequestManager` 新增句柄式并发 API（默认最大并发 4，可调）：

```csharp
var manager = CosmosEntry.WebRequestManager;
manager.MaxConcurrentRequests = 6;

// 文本/二进制/贴图/音频/AB/文件/长度/上传，统一句柄
var handle = manager.AddDownloadTextTaskAsync("http://.../config.json", timeoutSeconds: 15, retryCount: 2, priority: 10);
handle.Completed += h => { if (h.IsSucceeded) Debug.Log(h.Result); };

// 或 await
var text = await manager.AddDownloadTextTaskAsync(url).Task;

// 下载到本地文件
var fileHandle = manager.AddDownloadFileTaskAsync(url, $"{Application.persistentDataPath}/bundle.ab");
```

- 超时控制（`TimeoutSeconds`）、失败重试（`RetryCount`）、优先级排队（`Priority`）；
- 取消：`handle.Cancel()` / `manager.CancelAllRequests()` / `manager.RemoveTask(taskId)`；
- 旧版事件式 API（`OnSuccessCallback` 等）完整保留，Legacy 资源模块不受影响。

## 8. 目录结构

```
Runtime/Base/ResourceCore/
├── Cosmos.Resource.Core.asmdef
├── CFResources.cs                 # 静态门面（对外主入口）
├── ResourceInitParameters.cs
├── ResourceHelper.cs
├── Base/                          # 包裹/清单/寻址数据
├── Bundle/                        # 包体加载器与引用计数
├── Handle/                        # 各类句柄（含 TagAssetsHandle）
├── Operations/                    # 初始化/卸载场景操作
├── Provider/                      # 资产加载流程（AB/编辑器模拟/失败）
├── Utility/                       # 自包含工具（JsonUtility+AES+IO）
├── Diagnostics/                   # 加载记录/运行时信息/泄漏检测
└── Operation/                     # OperationSystem 异步驱动核心
```
