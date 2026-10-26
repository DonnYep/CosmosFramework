# Changelog

记录对外可感知的变更。格式参考 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)，版本号遵循 [SemVer](https://semver.org/lang/zh-CN/)。

> 2.0 之前的版本（v1.3.8 及更早）未逐条留档，历史见[提交记录](https://github.com/DonnYep/CosmosFramework/commits/main)。

## [Unreleased]

## [2.0.1] - 2026-10-26

本版**不向后兼容**。完整对照表见 [README · 破坏性更新](README.md#破坏性更新)，升级前请逐项检查。

### 破坏性变更

- 移除 Controller 模块：实体刷新并入 Entity，`CosmosEntry.ControllerManager`、`IControllerManager`、`[TickRefresh]` / `[LateRefresh]` / `[FixedRefresh]` 全部删除。改用 `IEntityTickable` / `IEntityFixedTickable` / `IEntityLateTickable`，普通 MonoBehaviour 直接用 `Update`。
- Entity 入口变更：`ShowEntity(string, out EntityObject)` 改为 `ShowEntity<T>(entityName)`；新增 `RegisterLogicEntity<T>`（无 GameObject 的逻辑实体）、`PauseEntityGroup`、`GetEntityObject`。
- 模块系统：`[Module]` 默认优先级改为 `10000`（内置模块占 `100~1300`）；同一契约重复注册在启动时报错；契约解析改为确定性规则。
- 取模块语义：`GetModule<T>()` 未注册时抛 `InvalidOperationException`，可选模块请用新增的 `TryGetModule<T>()`。
- Event 改为响应式：`AddEvent` / `RemoveEvent` / `Dispatch` 删除，改用 `Subscribe`（返回 `IDisposable`）/ `Publish` / `Observe`。
- 网络 awaiter 重写：删除 `WaitConnectedAwaiter` / `WaitReceiveDataAwaiter` / `WaitForChannelExts`，改用 `ConnectAsync` / `ReceiveDataAsync` / `WaitDisconnectedAsync`（新增取消与超时参数）。
- `LimitedQueue<T>` 改为组合实现，不再是 `Queue<T>` 子类。
- `(long)Fix64` 改为向零截断：`(long)(Fix64)(-1.5)` 由 `-2` 变为 `-1`，需要向下取整时先 `Fix64.Floor`。
- `(Fix64)` 转换越界饱和：超界不再回绕，`NaN` 归一为 `0`，`±Infinity` 饱和到 `MaxValue` / `MinValue`。
- `AStarFix64.Clear()` 会清空网格（`GetNode` 随后返回 `null`），只想清缓冲请用 `ClearNodeCache()`。
- `MonoSingleton` 首次获取（含场景预置实例）统一调用 `OnInitialize` 与 `DontDestroyOnLoad`。

### 新增

- 资源方案可插拔：`IResourceProvider` 契约 + `SetProvider(name, settings)` / `RegisterProvider(name, factory)`，内置 Addressables / YooAsset / QuarkAsset 适配器与编辑器 `AssetDatabaseProvider` 兜底。
- 响应式基座 `Runtime/Base/Reactive`：`Subject` / `ReactiveProperty` / 操作符，`Observe<T>()` 支持 `Where` / `Select` / `DistinctUntilChanged`。
- 模块文档体系：`Docs/MANUAL.md`、`MODULE.md`、`BOOTSTRAP.md`、`ENTITY.md`、`REACTIVE.md`、`RESOURCE.md`、`ASYNC.md`。
- 工程内编译校验工具：`tools/RoslynCheck`（按 asmdef 边界用 Roslyn 编译，`core` / `runtime` / `editor` / `adapters` / `examples` 目标）。
- CI：push / PR 跑 Roslyn 编译校验与包一致性检查，打 tag 自动创建 GitHub Release（见 `Docs/RELEASING.md`）。

### 修复

- `QuadTree` 到达最大深度的满叶不再丢弃对象，`Insert` 不再返回 `false`（容量是分裂阈值而非硬上限）。
- `AStarFix64` 与 `AStar` 的清空行为对齐。
- AOI 按节点身份增删，同 X 坐标 / 同坐标的实体不再互相误删；`AOISkipList.Add` 返回最底层节点以便 O(1) 删除。
- `Circle` 关系判定修正：同心、包含不再被判为「相离」或「相交」。
- 负值 `Fix64` 转 `long`、超界饱和等数值边界行为修正。

### 内部

- Unity 工程升级至 2021.3.45f1c2。
- 移除旧资源加载方案及其编辑器构建流水线，改为 Provider 契约。
- 代码风格统一为 CSharpier（`.config/dotnet-tools.json`）。

[Unreleased]: https://github.com/DonnYep/CosmosFramework/compare/v2.0.1...dev
[2.0.1]: https://github.com/DonnYep/CosmosFramework/releases/tag/v2.0.1
