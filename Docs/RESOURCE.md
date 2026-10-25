# 资源模块（可插拔 Provider）

> 重构后，框架**不再内置任何资源管理方案**。资源加载由外部 Provider 提供，框架只保留一个"桥"，负责 Provider 的接入、引用计数、名称索引与诊断。

## 1. 设计核心

> **资源加载方案可插拔；框架内所有模块的资源生命周期，统一建立在 Resource 模块之上；而 Resource 模块本身，建立在可插拔的资源管理方案之上。**

这是一条依赖倒置链：每一层只认识下一层的契约，不认识对方的具体实现。

```text
UI / Audio / Entity / Scene / DataTable / ObjectsPool
        │  只依赖 IResourceManager
        ▼
Modules/Resource（桥）   Provider 选择 · 引用计数 · 名称索引 · 场景登记 · 诊断
        │  IResourceProvider（4 个方法）
        ▼
Addressables   │   YooAsset   │   QuarkAsset   │   AssetDatabase（编辑器兜底）
```

### 1.1 方案可插拔

框架不实现资源管理：没有自研 AB 构建、没有清单格式、没有版本校验。
`IResourceProvider` 只要求四件事——初始化、创建一次加载操作、卸载未用资源、销毁；
具体方案的概念（包、catalog、清单、加密、下载）全部留在适配器内部。

| 方案 | 适配器 | 说明 |
| --- | --- | --- |
| Addressables | `Cosmos.Resource.Addressables` | 映射最完整，含 `InstantiateAsync` 对象池 |
| YooAsset | `Cosmos.Resource.YooAsset` | `ResourcePackage` + `*OperationHandle` |
| QuarkAsset | `Cosmos.Resource.QuarkAsset` | 原生 name-keyed 引用计数，与桥的兼容层天然对应 |
| AssetDatabase | `Cosmos.Resource.EditorFallback` | 仅编辑器，零构建产物 |

### 1.2 模块生命周期建立在桥之上

| 模块 | 通过桥获取 | 归还时机 |
| --- | --- | --- |
| UI | 面板预制体 | 面板关闭时 `UnloadAsset` |
| Audio | `AudioClip` | 音效卸载时 `UnloadAsset` |
| DataTable | 表文件 `TextAsset` | 表卸载时 `UnloadAsset` |
| ObjectsPool | 池对象预制体 | 池卸载时 `UnloadAsset` |
| Entity | 实体预制体 | 注册后常驻，框架终止时统一回收 |
| Scene | 场景 | `UnloadSceneAsync` 或统一回收 |

模块因此获得三个确定的行为：**换方案零改动**、**引用计数与重复加载合并只有一份实现**、**公开 API 稳定**。

### 1.3 桥只做两件事

`key → 句柄`、`句柄 → 引用计数`。桥不知道 AB 包、不知道 catalog、不知道 package；
方案之间能力的差异用能力位显式声明，缺失时桥直接报错，而不是让适配器"假装支持"。


## 2. 程序集

| 程序集 | 位置 | 说明 |
| --- | --- | --- |
| `Cosmos.Async` | `Runtime/Base/Async` | **通用异步底座**（`OperationBase` / `OperationSystem` / `OperationDriver` / `GameAsyncOperation`），只依赖 UnityEngine |
| `Cosmos.Resource.Abstractions` | `Runtime/Base/ResourceAbstractions` | 资源契约（句柄 / Provider 基类 / `ResourceProviderOperation`），不依赖任何三方库；引用 `Cosmos.Async` |
| `Cosmos.Runtime` | `Runtime/Modules/Resource` | 资源桥 `IResourceManager` 实现 |
| `Cosmos.Resource.EditorFallback` | `Editor/ResourceFallback` | 编辑器兜底 Provider（仅 Editor） |
| `Cosmos.Resource.Addressables` | `Runtime/Integrations/Addressables` | Addressables 适配器（可选） |
| `Cosmos.Resource.YooAsset` | `Runtime/Integrations/YooAsset` | YooAsset 适配器（可选） |
| `Cosmos.Resource.QuarkAsset` | `Runtime/Integrations/QuarkAsset` | QuarkAsset 适配器（可选） |

三个适配器都通过 `versionDefines` + `defineConstraints` 按包是否存在自动启停：
未安装对应资源库时程序集不会被编译，工程不会报错。

## 3. 快速开始

挂载 `CosmosConfig`，在 `Resource` 折叠项里填：

| 字段 | 示例 |
| --- | --- |
| Provider | `Addressables` / `YooAsset` / `QuarkAsset` / `AssetDatabase` |
| Settings | `key=value;key2=value2`（见下表） |

| Provider | Settings |
| --- | --- |
| `AssetDatabase` | 无（编辑器兜底，直接按资源名/路径加载） |
| `Addressables` | `updateCatalog=true`（可选） |
| `YooAsset` | `package=DefaultPackage;mode=offline`；`mode=editor` 时需 `manifest=<SimulateManifest 路径>` |
| `QuarkAsset` | `mode=assetbundle;path=<目录>;aesKey=xxx;offset=0`；编辑器可用 `mode=assetdatabase;dataset=QuarkAssetDataset` |

> 未配置时默认使用 `AssetDatabase`，因此示例工程在编辑器内可直接运行；真机请配置真实 Provider，否则会输出明确的报错。

## 4. 契约（写 Provider 只需要实现这些）

```csharp
public interface IResourceProvider
{
    string Name { get; }
    ResourceCapabilities Capabilities { get; }   // 能力位，缺能力时由桥统一报错/降级
    bool IsInitialized { get; }
    ResourceInitParameters InitParameters { get; }

    InitializationOperation InitializeAsync(ResourceInitParameters parameters);
    ResourceProviderOperation CreateOperation(ResourceRequest request);
    void UnloadUnusedAssets();
    void Destroy();
}

// 适配器只实现三个钩子：发起请求、推进、释放
public abstract class ResourceProviderOperation : OperationBase
{
    protected abstract void OnStartLoad();
    protected virtual  void OnUpdateLoad() { }
    protected abstract void OnRelease();

    protected void SetAsset(Object asset);
    protected void SetAssets(Object[] assets);
    protected void SetInstance(GameObject instance);
    protected void SetScene(Scene scene, string sceneName);
    protected void SetProgress(float progress);
    protected void Finish(string error = null);
}
```

引用计数、句柄创建、`Completed` 事件、协程与 `await` 支持全部由基类完成，Provider 不需要重复实现。

### 能力位

`ResourceCapabilities`：`Asset` / `SubAssets` / `AllAssets` / `Scene` / `Instance` / `Sync` / `Download` / `Priority`。

桥在调用前检查能力位，缺失时直接给出明确错误，而不是让某个方案"假装支持"。

## 5. 两套 API

### 5.1 句柄 API（推荐，语义最清晰）

```csharp
var handle = CosmosEntry.ResourceManager.LoadAssetHandle<GameObject>("Prefabs/Cube");
handle.Completed += h => Instantiate(h.Asset);
yield return handle;               // 协程
var cube = await handle.Task;      // async/await
var asset = handle.WaitForCompletion();
handle.Release();                  // 释放一次引用，归零即卸载底层资源
```

其它：`LoadSubAssetsHandle` / `LoadAllAssetsHandle` / `LoadAssetsHandle`（批量）/
`LoadSceneHandle` / `InstantiateHandle`（仅支持 `Instance` 能力的 Provider，如 Addressables）。

### 5.2 模块兼容 API（name-keyed + Coroutine）

业务模块（UI / Entity / Audio / DataTable / ObjectsPool）内部使用这一套，公开 API 与重构前保持一致：

```csharp
CosmosEntry.ResourceManager.LoadPrefabAsync("UI/Main", go => { ... }, null, true);
CosmosEntry.ResourceManager.LoadAssetAsync<AudioClip>("BGM_01", clip => { ... });
CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo("Level_01"), null, p => { }, null, () => { });

CosmosEntry.ResourceManager.UnloadAsset("UI/Main");   // 与加载配对，减少一次引用
```

规则：

- 每调用一次加载，增加一次引用；每调用一次 `UnloadAsset`，减少一次；
- 同一标识 + 类型 + 加载方式共享同一个底层操作，不会重复发起请求；
- 标识原样透传给 Provider（资源名 / 资源名.后缀 / 资源路径 / Provider 自定义地址），**框架不再发明一层地址映射**。

## 6. 诊断

```csharp
var manager = CosmosEntry.ResourceManager;
manager.DiagnosticsEnabled = true;          // 编辑器默认开启
manager.CaptureStackTrace = true;           // 记录调用栈，定位泄漏

Debug.Log(manager.GetRuntimeInfo());        // 当前 Provider / 活跃操作 / 已加载资产
Debug.Log(manager.GenerateDiagnosticsReport());   // 最慢、失败、疑似泄漏
```

## 7. 自行扩展一个 Provider

1. 新建程序集，引用 `Cosmos.Resource.Abstractions`、`Cosmos.Async`（以及你的资源库）；
2. 实现 `IResourceProvider`：`InitializeAsync` 里启动初始化操作，`CreateOperation` 里按 `LoadMode` 返回对应的 `ResourceProviderOperation`；
3. 实现操作：`OnStartLoad` 发起请求，异步方案在回调里 `SetAsset` + `Finish`，轮询式方案在 `OnUpdateLoad` 里判断完成；`OnRelease` 释放底层句柄；
4. 注册（`[RuntimeInitializeOnLoadMethod]` 里一行即可），随后 `CosmosConfig` 里填名字就能用：

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
static void Register()
{
    ResourceProviderRegistry.Register("MyProvider", () => new MyProvider());
}
```

参考实现：`Runtime/Integrations/` 下的三个适配器（每个约 150~300 行）。

## 8. 能力对照

| 能力 | Addressables | YooAsset | QuarkAsset | AssetDatabase |
| --- | --- | --- | --- | --- |
| 单资产 | ✔ | ✔ | ✔ | ✔ |
| 子资产 | ✔ | ✔ | ✔ | ✔ |
| 全部资产 | ✔ | ✔ | ✔ | ✔ |
| 场景 | ✔ | ✔ | ✔ | ✔ |
| 实例化（释放即销毁） | ✔ | ✗ | ✗ | ✔ |
| 优先级 | ✔ | ✔ | ✗ | ✗ |
| 下载/更新 | 原生支持 | 原生支持 | 原生支持 | ✗ |

> 下载/更新目前没有并入桥的 API：各方案的下载器语义差异较大，建议直接使用对应方案的 Downloader（如 `ResourcePackage.CreateResourceDownloader`、`Addressables.DownloadDependenciesAsync`）。

## 9. 与旧版的差异

已移除：

- `Modules/Resource/Legacy`（AssetBundle 清单、下载、校验、版本对比等整套自研方案）；
- `Base/ResourceCore`（自研的 Package / Manifest / BundleLoader 实现）；
- `Window/Cosmos/Module/Resource/*` 两套资源编辑器与构建流水线；
- `IResourceLoadHelper`、`ResourceLoadMode`、`ResourceDataset`、`SceneAssetInfo` 之外的旧资源类型。

保留 / 新增：

- `IResourceManager` 名称不变，模块调用方式不变（公开 API 兼容）；
- 新增 `IResourceProvider` / `ResourceProviderOperation` / `ResourceProviderRegistry` / 能力位与诊断。
