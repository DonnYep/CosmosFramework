![CosmosGitLogo](Docs/CosmosGitLogo.png)

[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE)
[![Issues:Welcome](https://img.shields.io/badge/Issues-welcome-blue.svg)](https://github.com/DonnYep/CosmosFramework/issues)

# [English](README_EN.md)

# CosmosFramework

CosmosFramework 是一款中轻量级的 Unity 开发框架：统一模块生命周期、可插拔资源方案、集中式对象刷新、async/await 语法支持与多网络通道，并附带大量工具类、数据结构与编辑器工具。

- [完整使用手册](Docs/MANUAL.md)：模块、工具类、底座、用法与边界（按当前代码结构编写）
- [CosmosFramework Wiki](https://github.com/DonnYep/CosmosFramework/wiki)

> ## ⚠️ 本次为破坏性更新
>
> 本版**不向后兼容**。升级前请对照 [破坏性更新清单](#破坏性更新)。
>
> - **Controller 模块已移除**：实体刷新并入 Entity 模块，`CosmosEntry.ControllerManager`、`IControllerManager`、`[TickRefresh]` / `[LateRefresh]` / `[FixedRefresh]` 全部删除。
> - **Entity 契约调整**：`ShowEntity(string, out EntityObject)` 改为 `ShowEntity<T>(string)`；新增 `RegisterLogicEntity<T>`（无 GameObject 的逻辑实体）、`PauseEntityGroup`、`GetEntityObject` 等。
> - **模块系统**：`[Module]` 默认优先级改为 `10000`（内置模块占 `100~1300`），同一契约重复注册会启动报错，契约解析改为确定性规则。
> - **取模块语义**：`GetModule<T>()` 未注册时抛 `InvalidOperationException`（新增 `TryGetModule<T>()`）。
> - **网络**：删除 `WaitConnectedAwaiter` / `WaitReceiveDataAwaiter` / `WaitForChannelExts`；改用 `ConnectAsync` / `ReceiveDataAsync` / `WaitDisconnectedAsync`（同名扩展保留，新增取消与超时参数）。
> - **Event 改为 reactive**：`Subscribe` 返回 `IDisposable`，`Observe<T>()` 支持 `Where` / `Select` / `DistinctUntilChanged`；`AddEvent` / `RemoveEvent` / `Dispatch` 删除，新增 `Runtime/Base/Reactive` 响应式基座。
> - **MonoSingleton**：首次获取（含场景预置）都会调用 `OnInitialize`，即 `DontDestroyOnLoad`。

<a name="导航"></a>

# 导航
- [English](#english)
- [CosmosFramework](#cosmosframework)
- [破坏性更新](#破坏性更新)
- [环境](#环境)
- [快速开始](#快速开始)
- [项目结构](#项目结构)
- [核心机制](#核心机制)
- [模块一览](#模块一览)
- [工具类](#工具类)
- [基础底座](#基础底座)
- [架构模板](#架构模板)
- [资源集成](#资源集成)
- [编辑器工具](#编辑器工具)
- [文档索引](#文档索引)
- [已知边界](#已知边界)
- [UPM](#upm)
- [其他](#其他)
- [Library link](#library-link)

<a name="破坏性更新"></a>

## 破坏性更新

| 变更 | 影响 | 处理方式 |
| --- | --- | --- |
| Controller 模块删除 | 旧 `[TickRefresh]` 轮询不再生效 | 实体实现 `IEntityTickable` / `IEntityFixedTickable` / `IEntityLateTickable`；普通 MonoBehaviour 直接用 `Update` |
| `CosmosEntry.ControllerManager` 删除 | 编译失败 | 用 `CosmosEntry.EntityManager`：`Paused` 个体暂停、`PauseEntityGroup` 组冻结 |
| `ShowEntity(string, out EntityObject)` 删除 | 编译失败 | 改为 `ShowEntity<T>(entityName)` |
| Event API 改为 reactive | `AddEvent` / `RemoveEvent` / `Dispatch` 删除 | 改用 `Subscribe`（返回 `IDisposable`）/ `Publish` / `Observe` |
| `LimitedQueue<T>` 改为组合实现 | 不再是 `Queue<T>` 子类，按 `Queue<T>` 传递会编译失败 | 改用自身的 `Enqueue` / `Dequeue` / `TryDequeue` / `Peek` / `Clear` |
| 模块优先级语义统一 | 显式声明小 `Priority` 的自定义模块会提前初始化 | 默认不写即排在全部内置模块之后；需要插队再用 `[Module(Priority = n)]` |
| 重复契约报错 | 与内置模块同契约的自定义模块无法启动 | 删其一，或 `BuiltinModuleRegistry` 移除内置项 / 用 `CosmosEntry.LaunchModules` 自定义集合 |
| `GetModule<T>()` 抛异常 | 依赖 null 判断的代码会抛 | 可选模块改用 `TryGetModule<T>()` |
| 网络 awaiter 重写 | 旧 awaiter 类型引用编译失败 | 使用同名扩展 `ConnectAsync` / `ReceiveDataAsync`，或新增的 `WaitDisconnectedAsync` |
| `MonoSingleton` 初始化时机 | 场景预置实例也会 `DontDestroyOnLoad` | 无需处理，行为更一致 |
| `(long)Fix64` 改为向零截断 | 负值结果变化（`(long)(Fix64)(-1.5)` 由 `-2` 变 `-1`） | 需要向下取整时先 `Fix64.Floor` 再转 `long` |
| `(Fix64)` 数值转换越界饱和 | 超界不再回绕（`(Fix64)long.MaxValue` 不再得到 `-1`）；`NaN` 归一为 `0`，`±Infinity` / 超界值饱和到 `MaxValue` / `MinValue` | 无需处理；依赖回绕结果的写法属未定义用法 |
| `AStarFix64.Clear()` 清空网格 | `Clear()` 后 `GetNode` 返回 `null`（与 `AStar` 一致） | 只想清缓冲用 `ClearNodeCache()` |
| `QuadTree` 满叶不再丢对象 | 到达最大深度的满叶会继续累积（可超过 `NodeObjectCapacity`），`Insert` 不再返回 `false` | 无需处理；容量是分裂阈值而非硬上限 |
| AOI 按节点身份增删 | 同 X 坐标 / 同坐标的多个实体不再互相误删；`AOISkipList.Add` 返回最底层节点，可用于 O(1) 删除 | 无需处理 |
| `Circle` 关系判定修正 | 同心、包含不再被判为「相离」或「相交」 | 无需处理 |

<a name="环境"></a>

## 环境

- Unity 版本：2018 及以上；.NET API 版本：4.x（推荐 Unity 2021.3 LTS）。

<a name="快速开始"></a>

## 快速开始

1. **启动**：在启动场景放置 `CosmosConfig` 组件（`launchAppDomainModules` 勾选），或手动调用：

```csharp
CosmosEntry.LaunchAppDomainModules();                                   // 内置 + 扫描所有程序集
CosmosEntry.LaunchAssemblyModules(typeof(MyModule).Assembly);           // 内置 + 指定程序集
CosmosEntry.LaunchModules(new MyModule(), new OtherModule());           // 完全手动（不含内置）
```

2. **取模块**：内置模块有强类型属性，自定义模块用契约接口：

```csharp
var ui = CosmosEntry.UIManager;                                  // 内置
var mine = CosmosEntry.GetModule<IInventoryManager>();           // 自定义（未注册会抛异常）
if (CosmosEntry.TryGetModule<IOptionalManager>(out var opt)) { } // 可选
```

3. **写一个模块**：

```csharp
public interface IInventoryManager : IModuleManager { void AddItem(int id, int count); }

[Module(Priority = 150)]                                   // 可省略：默认 10000，排在内置模块之后
public class InventoryManager : Module, IInventoryManager
{
    protected override void OnPreparatory() { }            // 跨模块依赖在这里取
    protected override void OnUpdate() { }                 // 自动进入每帧轮询
    public void AddItem(int id, int count) { }
}
```

详见 [自定义模块](Docs/MODULE.md)。

4. **用实体**：

```csharp
CosmosEntry.EntityManager.RegisterEntityAsync<EnemyEntity>(new EntityAssetInfo("Enemy"));
var enemy = CosmosEntry.EntityManager.ShowEntity<EnemyEntity>("Enemy");

CosmosEntry.EntityManager.RegisterLogicEntity<BattleLogic>("battle", "logic");  // 无 GameObject
var logic = CosmosEntry.EntityManager.ShowEntity<BattleLogic>("battle");
```

<a name="项目结构"></a>

## 项目结构

```text
Assets/CosmosFramework
├─ Runtime
│  ├─ CosmosEntry.cs            应用级门面：启动、取模块、全局轮询事件
│  ├─ CosmosConfig.cs           启动配置（ScriptableObject）
│  ├─ Base
│  │  ├─ Async                  统一异步：OperationBase / OperationSystem / Async
│  │  ├─ Attribute              通用特性（[Module] 等）
│  │  ├─ Behaviour              ILifecycle 生命周期契约
│  │  ├─ Constants              全局常量
│  │  ├─ Dataset                数据集基类
│  │  ├─ DataStructure          池、树、跳表、四叉树、网格、A*、AOI、Fix64、几何
│  │  ├─ EventCore              事件内核
│  │  ├─ Extensions             扩展方法
│  │  ├─ ReferencePool          引用池
│  │  ├─ Reactive               响应式基座（Observable / Subject / ReactiveProperty）
│  │  ├─ ResourceAbstractions   资源契约（Provider / Handle / Operation）
│  │  ├─ Singleton              Singleton / ConcurrentSingleton / MonoSingleton
│  │  ├─ Timer                  TickTimer
│  │  └─ Utility                15 个静态工具分册
│  ├─ Modules                  13 个内置模块 + Main（模块系统）
│  ├─ Integrations             Addressables / YooAsset / QuarkAsset
│  ├─ Architeture              LiteMVC / PureMVC
│  ├─ ModulesDebuger           模块调试窗口
│  └─ 3rdParty                 Hydrogen（TCP）、LitJson（JSON）
└─ Editor                       配置面板、搜索、脚本头、编辑器协程、资源兜底 Provider
```

<a name="核心机制"></a>

## 核心机制

### 模块生命周期与优先级

```text
注册(OnInitialization) → 激活(OnActive) → 准备(OnPreparatory)
  → 每帧(OnUpdate / OnFixedUpdate / OnLateUpdate / OnElapseUpdate)
  → 失活(OnDeactive) → 终止(OnTermination)
```

- `[Module(Priority = n)]`：数值越小越先；内置模块占 `100~1300`，自定义模块默认 `10000`；
- `OnInitialization` 只能依赖更早的模块，跨模块依赖放 `OnPreparatory`；
- 重写 `OnUpdate` 即自动进入轮询，由 `MonoGameManager`（`[DefaultExecutionOrder(-20000)]`）统一驱动；
- 契约是 `IModuleManager` 派生接口，解析顺序：`I{类名}` → 后缀匹配 → 唯一候选 → 报错；同一契约只能有一个模块。

### 刷新体系

| 方式 | 适用 |
| --- | --- |
| `MonoBehaviour.Update` | 普通表现脚本 |
| Entity 集中刷新 | 实体：实现 `IEntityTickable` 等接口，由 Entity 模块统一派发，支持组冻结与个体暂停 |
| 全局事件 | `CosmosEntry.TickRefreshHandler` / `FixedRefreshHandler` / `LateRefreshHandler` / `ElapseRefreshHandler` |
| `TickTimer` | 需要独立计时器的逻辑 |

### 线程

- 框架轮询全部在主线程；
- 后台线程回调统一走 `UnityMainThread.Post` / `UnityMainThread.Send`，由 `MonoGameManager.Update` 每帧 `Drain`；
- 网络通道的 await 等待器已内置该兜底。

<a name="模块一览"></a>

## 模块一览

| 模块 | 契约 | 职责 |
| --- | --- | --- |
| Event | `IEventManager` | 响应式事件中心：`Subscribe` 返回 `IDisposable`，`Observe<T>()` 可接操作符 |
| Resource | `IResourceManager` | 可插拔资源桥：句柄与协程 API、引用计数、加载诊断 |
| WebRequest | `IWebRequestManager` | HTTP 请求：句柄 + 超时 / 重试 / 优先级 / 取消 |
| DataTable | `IDataTableManager` | 只读共享行 + 主键索引，状态机加载、可重载 |
| ObjectPool | `IObjectPoolManager` | 预制体对象池，支持过期回收 |
| Entity | `IEntityManager` | 对象存活 + 集中刷新：`EntityObject`（有 GameObject）/ `EntityLogic`（纯逻辑）、分组冻结 |
| UI | `IUIManager` | 界面加载 / 激活 / 分组 / 优先级（`IUIForm` 生命周期） |
| Audio | `IAudioManager` | 音效注册与播放、分组、淡入淡出、音量包络、抢占 |
| Input | `IInputManager` | 虚拟轴 / 虚拟按钮、输入屏蔽、可替换 `IInputHelper` |
| FSM | `IFSMManager` | 按 owner 类型管理的状态机（`FSMState<T>`） |
| Procedure | `IProcedureManager` | 流程节点：启动 / 登录 / 进场景等阶段 |
| Network | `INetworkManager` | 通道注册与 Tick；协议实现位于 `Assets/CosmosLibs`（KCP / TCP / SUDP / RUDP） |
| Download | `IDownloadManager` | 文件下载：断点续传、并发上限、暂停 / 恢复 / 取消 |

> 要文件用 Download，要数据用 WebRequest；两者分工见 [网络文档](Docs/NETWORKING.md)。

<a name="工具类"></a>

## 工具类

`Cosmos.Utility` 下的静态分册，随取随用：

| 分册 | 内容 |
| --- | --- |
| Text / Converter | 字符串校验与拼接、十六进制、Base64、字节格式化 |
| IO / Compression | 文件与目录读写、复制、大小、MD5；Zip 压缩解压 |
| Time | 时间戳、日/周/月计算、时区转换 |
| Encryption | MD5 / HMAC / AES / XOR / GUID |
| Algorithm | 二分查找、排序、洗牌、随机与正态分布 |
| Assert | 断言、异常捕获 |
| Debug / Json | 日志与 JSON（帮助体可替换） |
| Net | 请求判定、内容长度、本机 IP、公网 IP、Ping |
| Unity | 销毁、截图、随机点、Gizmos 绘制、协程任务（延时 / 帧 / 条件） |
| Assembly / Program | 反射取类型与实例、进程启动与内存占用 |

常用 API 与示例见 [手册 §4](Docs/MANUAL.md)。

<a name="基础底座"></a>

## 基础底座

- **数据结构**：`Pool<T>` / `ConcurrentPool<T>`、`BiDictionary`、`LruDictionary`、`LimitedQueue`、`SkipList` / `AOISkipList`、`QuadTree`、`RectangleGrid` / `SquareGrid`、`AStar`（含 Diagonal / Manhattan / Euclidean）、`AOIZone`、`Fix64`（确定性定点数）、`BasicFsm` / `SimpleFsm`、几何体（均含 Fix64 版本）。
- **异步**：`OperationBase` / `OperationSystem` / `Async.WaitUntil` / `WaitForSeconds` / `NextFrame` / `WaitForTask`；协程可 `await`。
- **池**：`Pool<T>` 对象池、`ReferencePool` 引用池。
- **单例**：`Singleton<T>` / `ConcurrentSingleton<T>` / `MonoSingleton<T>`。
- **响应式**：`Observable<T>` / `Subject<T>` / `ReactiveProperty<T>` / `CompositeDisposable`，Event 模块建立在其上。
- **扩展方法**：基础类型、集合、Unity 组件、反射、文件等分组。
- **生命周期**：`ILifecycle` 统一契约；`EventCore` 事件内核；`DatasetBase` 数据集基类。

<a name="架构模板"></a>

## 架构模板

| 模板 | 组成 | 适用 |
| --- | --- | --- |
| LiteMVC | `Model` / `View` / `Controller` + `CommandBinder` / `CommandPool` | 轻量、事件驱动 |
| PureMVC | `Model` / `View` / `Controller` + `Proxy` / `Mediator` / `Command` / `Notification`，支持 `[MVCProxy]` 等特性与 `MVC.RegisterAttributedMVC()` 自动注册 | 标准 PureMVC 分层 |

<a name="资源集成"></a>

## 资源集成

| 方案 | 实现 | 说明 |
| --- | --- | --- |
| Addressables | `AddressablesProvider` | Unity 官方可寻址资源 |
| YooAsset | `YooAssetProvider` | 包管理式资源 |
| QuarkAsset | `QuarkAssetProvider` | 轻量方案 |
| AssetDatabase | `Editor/ResourceFallback/AssetDatabaseProvider` | 编辑器兜底，无需构建产物 |

通过 `IResourceProvider` 接入；`SetProvider(name, settings)` 或 `RegisterProvider(name, factory)` 注册自定义实现。契约见 [资源模块文档](Docs/RESOURCE.md)。

<a name="编辑器工具"></a>

## 编辑器工具

`CosmosConfig` 配置面板、脚本头生成、层级查找、AssetDatabase / GitHub 搜索、编辑器协程、数据集面板、模块窗口基类、AssetBundle 与快捷命令。运行时另有模块调试窗口（`ModulesDebuger`）。

<a name="文档索引"></a>

## 文档索引

| 文档 | 内容 |
| --- | --- |
| [使用手册](Docs/MANUAL.md) | 模块、工具类、底座、用法与边界（主参考） |
| [自定义模块](Docs/MODULE.md) | 契约命名、优先级、生命周期、报错对照 |
| [启动与模块注册](Docs/BOOTSTRAP.md) | 启动链、注册表、扫描与排序 |
| [实体](Docs/ENTITY.md) | 两种载体、集中刷新、分组冻结 |
| [响应式](Docs/REACTIVE.md) | Subject / ReactiveProperty / 操作符 / 组合释放 |
| [资源](Docs/RESOURCE.md) | Provider 契约、句柄 API、诊断 |
| [异步](Docs/ASYNC.md) | 统一异步方案 |
| [音频](Docs/AUDIO.md) / [数据表](Docs/DATATABLE.md) / [输入](Docs/INPUT.md) / [下载](Docs/DOWNLOAD.md) | 对应模块细节 |
| [网络通道异步](Docs/NETWORK_ASYNC.md) | 通道 await、超时、取消、线程兜底 |
| [WebRequest 与 Download 分工](Docs/NETWORKING.md) | 选型规则 |
| [CHANGELOG](CHANGELOG.md) | 版本变更记录 |
| [分支与发布](Docs/RELEASING.md) | 分支约定、发版清单、仓库设置 |

<a name="已知边界"></a>

## 已知边界

- `IObjectPool` 接口未暴露 `Spawn()`，需要具体类型 `ObjectPool`。
- 网络通道必须被 `TickRefresh` 驱动（注册到 `NetworkManager` 或自行调用），否则 await 只会超时。
- 模块取用必须在启动之后；`GetModule<T>()` 未注册会抛异常。
- 实体刷新中允许隐藏实例，但不允许注册 / 注销实体。
- `AStar` 的开集是线性选点（每次展开 O(开集大小)），未使用堆；大面积不可达地图的开销集中在展开阶段（100×100 约 20 ms）。
- AOI 移除实体时不会把它从其它实体的 `ViewEntities` 里摘除（实例会被池复用），消费侧应以 `Exited` / `Entered` 为准，不要长期持有实体引用。

<a name="UPM"></a>

## UPM

CosmosFramework 遵循 Unity Package 规范，包根为 `Assets/CosmosFramework`（版本号见其中的 `package.json`）。

**发行版（推荐）** —— 固定到 tag，不受后续提交影响。在 `Packages/manifest.json` 的 `dependencies` 中加入：

```json
"com.cosmosframework": "https://github.com/DonnYep/CosmosFramework.git?path=/Assets/CosmosFramework#v2.0.1"
```

- 把 `#v2.0.1` 换成 `#main`：跟随稳定线的最新发布版。
- 把 `#v2.0.1` 换成 `#dev`：尝鲜开发线，可能含未发布改动，**不保证可编译**，仅建议试用与反馈。
- 不用 Git URL 时：把 `Assets/CosmosFramework` 整个目录拷进工程的 `Packages` 目录（Unity 视为本地包）。

> `?path=` 需要 Unity 2019.3 及以上；推荐 Unity 2021.3 LTS。版本变更见 [CHANGELOG](CHANGELOG.md)。

<a name="其他"></a>

## 其他

- 内置案例：`Assets/Examples/`（含 Entity、UI、Audio、Input、Network、Procedure 等示例）
- 模块演示视频：
    - https://www.bilibili.com/video/BV11P4y1U7vB
    - https://www.bilibili.com/video/BV1x741157eR
    - https://www.bilibili.com/video/BV17u411Z7Ni

<a name="Library-link"></a>

## Library link

- Hydrogen: https://github.com/DonnYep/Hydrogen
- HybridCLR: https://github.com/focus-creative-games/hybridclr
- CosmosEngine: https://github.com/DonnYep/CosmosEngine
- QuarkAsset: https://github.com/DonnYep/QuarkAsset
- KCP C: https://github.com/skywind3000/kcp
- KCP CSharp: https://github.com/vis2k/kcp2k
- TCP: https://github.com/vis2k/Telepathy
- Mirror: https://github.com/vis2k/Mirror

**[回到最上层](#导航)**
