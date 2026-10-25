# CosmosFramework 使用手册

> 适用范围：`Assets/CosmosFramework`（Runtime / Editor）。
> 本文按**当前代码结构**编写，讲"有什么、怎么用、边界在哪"；设计取舍与改版原因见各专题文档（`Docs/` 下按模块命名）。

> ## ⚠️ 破坏性更新
>
> 本版**不向后兼容**，完整清单见 [README 的破坏性更新](../README.md#破坏性更新)。升级时重点关注：
>
> - **Controller 模块已移除**：实体刷新并入 Entity（实现 `IEntityTickable` 等接口），`[TickRefresh]` 系列特性删除；
> - **Entity 契约调整**：`ShowEntity<T>(entityName)`、新增 `RegisterLogicEntity<T>` / `PauseEntityGroup`；
> - **模块系统**：`[Module]` 默认优先级 `10000`，内置占 `100~1300`，重复契约启动报错；
> - **取模块**：`GetModule<T>()` 未注册抛异常，可选模块用 `TryGetModule<T>()`；
> - **网络**：旧 awaiter 类型删除，改用 `ConnectAsync` / `ReceiveDataAsync` / `WaitDisconnectedAsync`。
> - **Event 改为 reactive**：`AddEvent` / `RemoveEvent` / `Dispatch` 删除，改用 `Subscribe`（返回 `IDisposable`）/ `Publish` / `Observe`。

## 1. 目录与分层

```
Assets/CosmosFramework
├─ Runtime
│  ├─ CosmosEntry.cs            应用级静态门面：启动、模块取用、全局轮询事件
│  ├─ CosmosConfig.cs           启动配置（ScriptableObject）
│  ├─ Base
│  │  ├─ Async                  统一异步：OperationBase / OperationSystem / Async 扩展
│  │  ├─ Attribute              通用特性（[Module]、[Implementer] 等）
│  │  ├─ Behaviour              ILifecycle 生命周期契约
│  │  ├─ Constants              全局常量
│  │  ├─ Dataset                数据集基类
│  │  ├─ DataStructure          数据结构：池、树、跳表、四叉树、网格、A*、AOI、Fix64、几何
│  │  ├─ EventCore              事件内核
│  │  ├─ Extensions             扩展方法（基础类型/集合/Unity/反射/IO…）
│  │  ├─ ReferencePool          引用池
│  │  ├─ Reactive               响应式基座（Observable / Subject / ReactiveProperty）
│  │  ├─ ResourceAbstractions   资源契约（Provider / Handle / Operation）
│  │  ├─ Singleton              Singleton / ConcurrentSingleton / MonoSingleton
│  │  ├─ Timer                  TickTimer
│  │  └─ Utility                静态工具类（15 个分册，见 §4）
│  ├─ Modules                  13 个内置模块 + Main（模块系统本身）
│  ├─ Integrations             资源方案实现：Addressables / YooAsset / QuarkAsset
│  ├─ Architeture              LiteMVC / PureMVC 两套 MVC 模板
│  ├─ ModulesDebuger           模块调试窗口
│  └─ 3rdParty                 Hydrogen（TCP）、LitJson（JSON）
└─ Editor                       配置面板、搜索工具、脚本头、编辑器协程、资源兜底 Provider
```

三个层次，先记住这个分工：

| 层 | 职责 | 入口 |
| --- | --- | --- |
| 应用层 | 启动、取模块、全局轮询 | `CosmosEntry` / `CosmosConfig` |
| 模块层 | 业务能力（资源、UI、音频、实体…），统一生命周期 | `I*Manager` 契约 |
| 底座 | 数据结构、工具、异步、池、扩展 | 直接静态调用 |

## 2. 启动与模块系统

### 2.1 三种启动方式

```csharp
CosmosEntry.LaunchAppDomainModules();                                 // 内置 + 扫描所有程序集（最常用）
CosmosEntry.LaunchAssemblyModules(typeof(MyModule).Assembly);         // 内置 + 只扫指定程序集
CosmosEntry.LaunchModules(new MyModule(), new OtherModule());         // 完全手动：不扫描、不排序、不含内置模块
CosmosEntry.ReleaseLaunchedModules();                                 // 释放（终止所有模块）
```

- 启动只会执行一次（`Launched` 守卫）；失败会回滚标记，允许修复后重试。
- 模块启动失败不会立刻抛，而是收集异常，最后统一抛 `AggregateException`——看到它说明有模块初始化出错。
- `CosmosConfig`（挂在场景里）会在 `Awake` 里按配置启动，通常不需要自己调 `Launch*`。

### 2.2 CosmosConfig

| 配置 | 作用 |
| --- | --- |
| `launchAppDomainModules` | 是否在 Awake 自动启动模块 |
| `resourceProviderName` / `resourceProviderSettings` | 资源方案（`Addressables` / `YooAsset` / `QuarkAsset` / `AssetDatabase`） |
| `resourceDiagnostics` | 资源加载诊断开关 |
| `inputHelperName` / `debugHelperName` / `jsonHelperName` | 帮助体类型名（按名字解析，失败会明确告警） |
| `printModulePreparatory` | 是否打印模块准备日志 |
| `runInBackground` / `disableDebugLog` | 常规运行配置 |

### 2.3 模块生命周期

```
注册（OnInitialization）→ 激活（OnActive）→ 准备（OnPreparatory）
  → 每帧（OnUpdate / OnFixedUpdate / OnLateUpdate / OnElapseUpdate）
  → 失活（OnDeactive）→ 终止（OnTermination）
```

要点：

- `OnInitialization` 在注册循环里调用：**只能拿到优先级更小的模块**；
- 跨模块依赖放到 `OnPreparatory`（此时所有模块都已注册）；
- 重写 `OnUpdate` 就会自动进入每帧轮询，由 `MonoGameManager`（`[DefaultExecutionOrder(-20000)]`）统一驱动；
- `Module.Pause` 可暂停该模块轮询。

### 2.4 优先级

`[Module(Priority = n)]`，**数值越小越先**。内置模块占用 `100~1300`（步长 100，见 `ModuleConstant`），
自定义模块默认 `ModuleAttribute.DefaultPriority = 10000`，即排在全部内置模块之后。

```csharp
[Module]                 // 10000：内置之后
[Module(Priority = 50)]  // 全部内置之前
[Module(Priority = 150)] // Event(100) 与 Resource(200) 之间
```

同优先级保持传入顺序（内置优先）。内置模块的优先级必须显式、唯一且小于 10000，否则**启动即报错**。
自定义模块写法与报错对照见 [MODULE.md](MODULE.md)。

### 2.5 契约与取用

模块的对外契约是 `IModuleManager` 派生接口，契约解析规则（`ModuleContractResolver`）：

1. 精确名 `I{类名}`（`InventoryManager` → `IInventoryManager`，推荐）；
2. 接口名以类名结尾；
3. 只有一个候选时直接采用；
4. 否则报错，不会"随便挑一个"。

同一契约只能由一个模块提供，重复注册会报错。

```csharp
var ui = CosmosEntry.UIManager;                          // 内置模块：强类型属性（GetModule<IUIManager>() 的一行糖）
var mine = CosmosEntry.GetModule<IInventoryManager>();   // 自定义模块：必须用契约接口取
if (CosmosEntry.TryGetModule<IOptionalManager>(out var opt)) { }  // 可选模块
```

**`GetModule<T>()` 在未注册时抛 `InvalidOperationException`，不返回 null**；只有"模块可选"才用 `TryGetModule`。

### 2.6 模块挂载点

需要 Transform 当父节点的模块（UI 根 Canvas、音频挂载、多播实例）通过挂载点拿：

```csharp
var root = CosmosEntry.UIManager.InstanceObject();   // 惰性创建，挂在 CosmosRoot 下，跨场景保留
```

挂载点按模块类型缓存；节点被外部销毁会自动重建；模块表清空时一并销毁。

### 2.7 线程与全局轮询

- 框架轮询全部在主线程（`MonoGameManager`）；
- 后台线程回调统一走 `UnityMainThread`：

```csharp
UnityMainThread.Post(() => { /* 主线程执行 */ });
UnityMainThread.Send(() => { /* 主线程执行并等待完成，超时抛异常 */ });
UnityMainThread.IsMainThread;   // 判定
```

`MonoGameManager.Update` 每帧 `Drain()` 队列；没有 `MonoGameManager` 的纯逻辑宿主需自行调用。

- 全局轮询事件（不经过模块）：

```csharp
CosmosEntry.TickRefreshHandler += OnTick;
CosmosEntry.FixedRefreshHandler += OnFixed;
CosmosEntry.LateRefreshHandler += OnLate;
CosmosEntry.ElapseRefreshHandler += OnElapse;   // 真实帧间隔
```

## 3. 内置模块

> 契约在 `Modules/<Name>/I<Name>Manager.cs`，实现是 `internal sealed class <Name>Manager`。
> 下面只列常用 API，完整签名看接口文件。

### 3.1 Event（响应式事件中心）

- 契约：`IEventManager`；参数类型必须继承 `GameEventArgs`。
- `Subscribe` 返回 `IDisposable`，释放即退订；`Observe<T>()` 返回事件流，可接操作符。
- 线程：主线程派发；派发过程中订阅 / 退订都是安全的。

```csharp
public class LoginEventArgs : GameEventArgs
{
    public string Account;
    public override void Release() { Account = null; }
}

// 订阅（记得在销毁时 dispose）
var subscription = CosmosEntry.EventManager.Subscribe<LoginEventArgs>(OnLogin);
CosmosEntry.EventManager.Publish(new LoginEventArgs { Account = "a" });
subscription.Dispose();

// 事件流 + 操作符
CosmosEntry.EventManager.Observe<LoginEventArgs>()
    .Where(args => !string.IsNullOrEmpty(args.Account))
    .Subscribe(OnLogin);
```

API：`Subscribe<T>` / `Publish<T>` / `Observe<T>` / `HasEvent<T>` / `GetEventInfo<T>` / `RemoveAllEvents<T>` / `RemoveAllEvents` / `EventCount`。
Rx 基座（`Subject` / `ReactiveProperty` / 操作符 / 组合释放）见 [REACTIVE.md](REACTIVE.md)。

### 3.2 Resource（资源）

- 契约：`IResourceManager`；核心是**可插拔 Provider**，模块本身不依赖任何资源库。
- 两套 API：句柄 API（`InstantiateHandle` / `LoadSceneHandle`）与协程兼容 API（`LoadPrefabAsync` 等）。
- 协程返回值可 `await`（框架提供协程 await 扩展）。

```csharp
// 方案切换（通常在 CosmosConfig 里配）
CosmosEntry.ResourceManager.SetProvider("AssetDatabase");   // 编辑器兜底

var prefab = await CosmosEntry.ResourceManager.LoadPrefabAsync("EntityPlayer", true);
var handle = CosmosEntry.ResourceManager.InstantiateHandle("Prefabs/Cube", parent);
var go = handle.WaitForCompletion();
CosmosEntry.ResourceManager.UnloadAsset("EntityPlayer");
```

诊断：`DiagnosticsEnabled`、`LoadRecords`、`GenerateDiagnosticsReport()`、`GetRuntimeInfo()`。
细节与自写 Provider：见 [RESOURCE.md](RESOURCE.md)。

### 3.3 WebRequest（HTTP 请求）

- 契约：`IWebRequestManager`；分"回调任务"与"句柄/await"两代 API，新代码用后者。
- 句柄带超时、重试、优先级、取消；并发受 `MaxConcurrentRequests` 控制。

```csharp
var handle = CosmosEntry.WebRequestManager.AddDownloadTextTaskAsync(url, timeoutSeconds: 10, retryCount: 2);
string text = handle.WaitForCompletion();     // 或 await（Cosmos.Async 扩展）
handle.Cancel();
```

常用：`AddDownloadBytesTaskAsync` / `AddDownloadAudioTaskAsync` / `AddDownloadAssetBundleTaskAsync` / `AddGetContentLengthTaskAsync` / `AddUploadTaskAsync`。

### 3.4 DataTable（数据表）

- 契约：`IDataTableManager`；表是**只读共享行**，数组 + 主键索引，加载走状态机（幂等、可重载）。
- 需要 `IDataTableHelper` 决定"资源 → 文本 → 行"的解析。

```csharp
CosmosEntry.DataTableManager.ReadDataTableAssetAsync(new DataTableAssetInfo("item"), "item");
var table = (DataTable<ItemRow>)CosmosEntry.DataTableManager.GetDataTable("item");
var row = table.GetRowData(1001);
foreach (var r in table.Rows) { }
```

API：`ReadDataTableAssetAsync` / `ReloadDataTableAsync` / `GetDataTable` / `IsDataTableLoaded` / `ReleaseDataTable`。
细节：见 [DATATABLE.md](DATATABLE.md)。

### 3.5 ObjectPool（对象池）

- 契约：`IObjectPoolManager`；池按名字注册，绑定一个预制体，支持过期回收（`ExpireTime` / `ReleaseInterval` / `Capacity`）。

```csharp
var pool = CosmosEntry.ObjectPoolManager.RegisterObjectPool("bullet", bulletPrefab) as ObjectPool;
var go = pool.Spawn();
pool.Despawn(go);
CosmosEntry.ObjectPoolManager.DeregisterObjectPool("bullet");
```

> 已知缺口：`Spawn()` 在具体类 `ObjectPool` 上，`IObjectPool` 接口只暴露 `Despawn/ClearPool/容量/事件`。

### 3.6 Entity（实体：对象存活 + 集中刷新）

- 契约：`IEntityManager`；两种载体：
  - `EntityObject`（持有 GameObject，表现用）；
  - `EntityLogic`（纯 C#，逻辑帧/服务器算用）。
- 刷新按需登记：实现 `IEntityTickable` / `IEntityFixedTickable` / `IEntityLateTickable` 才进列表，无反射。
- 组冻结与个体暂停互相独立、结果取并集。

```csharp
// 预制体实体
CosmosEntry.EntityManager.RegisterEntityAsync<EnmeyEntity>(new EntityAssetInfo("Enemy"));
var e = CosmosEntry.EntityManager.ShowEntity<EnmeyEntity>("Enemy");
CosmosEntry.EntityManager.HideEntityObject(e);

// 逻辑实体（无 GameObject）
CosmosEntry.EntityManager.RegisterLogicEntity<BattleLogic>("battle", "logic");
var logic = CosmosEntry.EntityManager.ShowEntity<BattleLogic>("battle");

// 组冻结（UI 打开时冻场景这类需求）
CosmosEntry.EntityManager.PauseEntityGroup("monsters");
CosmosEntry.EntityManager.UnPauseEntityGroup("monsters");
```

细节（刷新链、顺序、刷新中隐藏、迁移）：见 [ENTITY.md](ENTITY.md)。

### 3.7 UI（界面）

- 契约：`IUIManager`；界面类实现 `IUIForm`（生命周期 `OnInit/OnOpen/OnClose/OnRelease/OnFocusGained/OnFocusLost`）。
- 需要 `IUIFormAssetHelper` 决定"资源 → 实例化后的挂载"（UGUI 用 `UGUIAssetHelper`）。
- 支持分组、优先级（决定同组叠放顺序）、激活/失活、按名查询。

```csharp
var canvas = await CosmosEntry.ResourceManager.LoadPrefabAsync("UICanvas", true);
CosmosEntry.UIManager.SetUIFormAssetHelper(new UGUIAssetHelper(canvas.transform));
await CosmosEntry.UIManager.OpenUIFormAsync<InventoryPanel>(new UIAssetInfo("InventoryPanel"));
CosmosEntry.UIManager.PeekUIForm("InventoryPanel", out var form);
CosmosEntry.UIManager.DeactiveUIForm("InventoryPanel");
```

### 3.8 Audio（音频）

- 契约：`IAudioManager`；注册音效资源 → 播放返回 `serialId` → 按 id 控制；支持组、淡入淡出、音量包络、抢占（priority）。

```csharp
CosmosEntry.AudioManager.RegisterAudioAsset("bgm_01");
int id = CosmosEntry.AudioManager.PlayAudio("bgm_01", AudioParams.Default);
CosmosEntry.AudioManager.StopAudio(id, fadeOutSeconds: 0.5f);

var group = CosmosEntry.AudioManager.AddAudioGroup("SFX");
CosmosEntry.AudioManager.PauseAllAudios(0.2f);
```

`AudioParams`：`PlayTime` / `Loop` / `Priority` / `Volume` / `Pitch` / `StereoPan` / `SpatialBlend` / `MaxDistance` / `FadeInSeconds`。
`AudioPositionParams` 决定世界位置与绑定父节点。
细节：见 [AUDIO.md](AUDIO.md)。

### 3.9 Input（输入）

- 契约：`IInputManager`；虚拟轴 / 虚拟按钮 + 可替换 `IInputHelper`（默认 `StandardInputHelper`）。
- 支持输入屏蔽（`PushInputBlock` / `PopInputBlock`，UI 模态时常用）。

```csharp
CosmosEntry.InputManager.SetInputHelper(new StandardInputHelper());
float h = CosmosEntry.InputManager.GetAxis(InputAxisType._Horizontal);
if (CosmosEntry.InputManager.GetButtonDown(InputButtonType._MouseLeft)) { }

CosmosEntry.InputManager.PushInputBlock(this);   // 打开弹窗时屏蔽
CosmosEntry.InputManager.PopInputBlock(this);
```

细节：见 [INPUT.md](INPUT.md)。

### 3.10 FSM（有限状态机）

- 契约：`IFSMManager`；状态机按 owner 类型 + 名字管理，状态继承 `FSMState<T>`。

```csharp
public class IdleState : FSMState<Player>
{
    public override void OnStateEnter(IFSM<Player> fsm) { }
    public override void OnStateStay(IFSM<Player> fsm) { }
    public override void OnStateExit(IFSM<Player> fsm) { }
}

var fsm = CosmosEntry.FSMManager.CreateFSM(this, new IdleState(), new RunState());
fsm.ChangeState<RunState>();
```

`FSMState<T>` 还提供 `AddTransition` / `RefreshTransition` / `OnInitialization` / `OnTermination`；
`IFSM<T>` 提供 `ChangeState`、`HasState`、`PeekState`、`OnStateChange`。

### 3.11 Procedure（流程节点）

- 契约：`IProcedureManager`；把启动/登录/进场景等阶段拆成节点。

```csharp
public class BootState : ProcedureNode
{
    public override void OnInit(ProcedureProcessor p) { }
    public override void OnEnter(ProcedureProcessor p) { }
    public override void OnUpdate(ProcedureProcessor p) { }
    public override void OnExit(ProcedureProcessor p) { }
    public override void OnDestroy(ProcedureProcessor p) { }
}

CosmosEntry.ProcedureManager.AddProcedureNodes(new BootState(), new LoginState());
CosmosEntry.ProcedureManager.RunProcedureNode<BootState>();
```

### 3.12 Network（通道与 async 等待）

- 契约：`INetworkManager` 只负责**通道注册与 Tick**；具体协议实现在 `Assets/CosmosLibs`（KCP / TCP / SUDP / RUDP，均为独立程序集，不属于框架本体）。
- 通道是事件模型（`OnConnected` / `OnDataReceived` / `OnDisconnected`），框架在其上提供 await 包装。

```csharp
var channel = new TcpClientChannel("MAIN");     // CosmosLibs 里的实现
CosmosEntry.NetworkManager.AddChannel(channel); // 必须注册，否则没人调 TickRefresh

await channel.ConnectAsync("127.0.0.1", 7777, ct);            // 失败/超时/取消都会正常结束
var data = await channel.ReceiveDataAsync(ct);               // 数据先到会进缓冲，不会丢
await channel.WaitDisconnectedAsync(ct);
```

细节（缓冲、超时、线程兜底、通道实现修复）：见 [NETWORK_ASYNC.md](NETWORK_ASYNC.md)。

### 3.13 Download（文件下载）

- 契约：`IDownloadManager`；与 WebRequest 的分工：**要文件**用 Download，**要数据**用 WebRequest。
- 支持断点续传、并发上限、超时、暂停/恢复/取消、整体进度事件。

```csharp
var id = CosmosEntry.DownloadManager.AddDownload(url, savePath, DownloadMode.Overwrite);
CosmosEntry.DownloadManager.LaunchDownload();
CosmosEntry.DownloadManager.PauseDownload();
bool found = CosmosEntry.DownloadManager.GetDownloadInfo(id, out var info);
```

细节：见 [DOWNLOAD.md](DOWNLOAD.md)、分工见 [NETWORKING.md](NETWORKING.md)。

## 4. 工具类 Utility

全部是 `Cosmos.Utility` 下的静态分册，直接调用，无需初始化（Debug / Json 可选装帮助体）。

| 分册 | 用途 | 常用 API |
| --- | --- | --- |
| `Utility.Text` | 字符串 | `IsStringValid` / `Combine` / `FormatCombine` / `GenerateRandomString` / `GetUTF8Length` / `Replace` / `StringSplit` |
| `Utility.IO` | 文件与目录 | `ReadTextFileContent` / `WriteTextFile` / `ReadBinaryFile` / `CopyDirectory` / `GetAllFilesRecursively` / `GetFileSize` / `GenerateFileMD5` / `CombineURL` |
| `Utility.Time` | 时间与时间戳 | `MillisecondTimeStamp` / `SecondNow` / `TimestampToDateTime` / `GetStartOfDay` / `GetTotalHours` / `GetWeekTime` |
| `Utility.Converter` | 字节/对象/十六进制 | `Object2Bytes` / `ConvertToHexString` / `FormatBytes` / `EncodeToBase64` |
| `Utility.Compression` | 压缩 | `CompressString` / `DecompressBytes` / `CreateZipFromDirectory` / `ExtractZipToDirectory` |
| `Utility.Encryption` | 摘要与加解密 | `MD5Encrypt` / `GenerateHexMD5` / `HmacSHA256` / `AESEncrypt` / `AESDecrypt` / `XorEncrypt` / `GUID` |
| `Utility.Algorithm` | 算法与随机 | `BinarySearch` / `Shuffle` / `Disrupt` / `SortByAscending` / `RandomRange` / `RandomNormalDistribution` |
| `Utility.Assert` | 断言与异常 | `NotNull` / `IsNull` / `Predicate` / `State` / `ExceptionCatcher` |
| `Utility.Debug` | 日志 | `LogInfo` / `LogWarning` / `LogError` / `SetHelper` / `DisableDebugLog` |
| `Utility.Json` | JSON | `ToJson` / `ToObject` / `BytesToObject` / `SetHelper` |
| `Utility.Net` | 网络工具 | `IsRequestSucceeded` / `GetContentLength` / `GetLocalIPv4Address` / `GetPublicIPAddress` / `PingURI` |
| `Utility.Unity` | Unity 辅助 | `DestroyObject` / `FindSceneGameObject` / `CameraScreenshotAsTexture` / `GetRandomPointInCircle` / `DrawCircleXZ` / `PostToUnityThread` / 协程任务（`AddDelayTask` 等） |
| `Utility.Assembly` | 反射与类型 | `GetDerivedTypes` / `GetInstancesByAttribute` / `GetPropertyValue` / `SetFieldValue` / `InvokeMethod` / `GetInterfaceHigestImplementedType` |
| `Utility.Program` | 进程 | `StartProcess` / `GetMemoryUsage` |
| `Utility` | 杂项 | `DebugColor` 等公共枚举 |

示例：

```csharp
if (!Utility.Text.IsStringValid(name)) return;
Utility.IO.WriteTextFile(path, Utility.Json.ToJson(data));
var now = Utility.Time.MillisecondTimeStamp();
var text = Utility.Text.FormatCombine("player/", id, "/avatar");
```

## 5. 基础底座

### 5.1 数据结构（`Base/DataStructure`）

| 分类 | 内容 |
| --- | --- |
| 池 | `Pool<T>`（队列式）、`ConcurrentPool<T>` |
| 字典/队列 | `BiDictionary` / `ConcurrentBiDictionary`（双向）、`LruDictionary`、`LimitedQueue`、`TinyLinkedList`、`LinkedNodeList` |
| 树/跳表 | `BinaryTree` / `SkipList` / `SkipNodeList` / `AOISkipList`（AOI 用） |
| 空间 | `QuadTree` / `QuadTreeFix64`、`RectangleGrid` / `RectangleGridFix64`、`SquareGrid` / `SquareGridFix64`、`AOIZone` / `AOIZoneFix64` |
| 寻路 | `AStar` 及 `Diagonal` / `Manhattan` / `Euclidean` 变体，均有 Fix64 版本 |
| 定点数 | `Fix64`（含 sin/cos LUT），用于确定性逻辑 |
| 几何 | `Rectangle` / `Square` / `Circle` / `Point2D` / `Vector2D`（各含 Fix64 版） |
| FSM 轻量实现 | `BasicFsm` / `SimpleFsm`（不依赖模块，可单测） |
| 并发 | `AsyncLock` |

> 说明：`AStar` / `QuadTree` / `AOI` 的 Fix64 版本是为帧同步/服务器准备的确定性实现，与浮点版本 API 对称。
>
> - **AOI**：增删按实体自持的轴节点身份进行，同 X 坐标 / 同坐标的多个实体互不影响；`AOISkipList.Add` 返回最底层节点，可直接用于 O(1) 删除。
> - **A\***：开 / 闭集为 O(1) 判定，邻居列表在展开时复用；自定义邻域重写 `GetNeighborNodes` 即可，想连临时集合都省掉再重写 `FillNeighborNodes`（`AStarFix64` 对应 `FillNeighboringNodes`）。
> - **四叉树**：`NodeObjectCapacity` 是分裂阈值而非硬上限，到达最大深度的叶子会继续累积对象（不会丢）。
> - **Fix64**：`(long)Fix64` 向零截断（需要向下取整先 `Fix64.Floor`）；`(Fix64)` 越界饱和、`NaN` 归一为 0。

### 5.2 异步（`Base/Async`）

- `OperationBase`：可驱动、可 abort、带优先级的操作基类；
- `OperationSystem`：按优先级调度操作；
- `Async`：`NextFrame()` 等一行等待；
- `WaitOperations`：`WaitUntil` / `WaitWhile` / `WaitForSeconds` 等条件等待；
- 协程可 `await`（`Unity` 侧扩展）。

细节：见 [ASYNC.md](ASYNC.md)。

### 5.3 定时器（`Base/Timer/TickTimer`）

按帧/秒累计的轻量定时器，需手动 `TickRefresh()`（示例 `25_TickTimer`）。

### 5.4 池

- `Pool<T>`：对象池（`Spawn` / `Despawn` / `Clear`）；`Clear()` 只丢弃引用，池内若是需要销毁的资源请用 `Clear(release)`；
- `ConcurrentPool<T>`：线程安全版本，`Count` 为池内数量、`TotalCount` 含在途占位；
- `ReferencePool`：`Acquire<T>()` / `Release(IReference)`，`IReference.Release()` 负责清空状态。

### 5.5 单例

| 类型 | 用途 |
| --- | --- |
| `Singleton<T>` | 纯 C# 单例 |
| `ConcurrentSingleton<T>` | 线程安全单例 |
| `MonoSingleton<T>` | MonoBehaviour 单例；首次访问自动创建并 `DontDestroyOnLoad` |

### 5.6 扩展方法（`Base/Extensions`）

按类型分组：基础类型（`IntExts` / `DoubleExts` / `LongExts` / `StringExts` / `DateTimeExts` / `StreamExts`）、集合（`ListExts` / `IEnumerableExts` / `IDictionaryExts` / `ICollectionExts` / `QueueExts` / `StackExts`）、Unity（`GameObjectExts` / `TransformExts` / `ComponentExts` / `RectTransformExts` / `UGUIExts` / `AnimatorExts` / `AudioSourceExts` / `CameraExts` / `TextureExts` / `SpriteExts` / `VectorExts`）、其它（`ObjectExts.As<T>/CastTo<T>`、`TypeExts`、`FileExts`、`KeyValuePairExts`）。

### 5.7 其它

- `ILifecycle`：`OnInitialization / OnPreparatory / OnActive / OnPause / OnUnPause / OnRefresh / OnElapseRefresh / OnDeactive / OnRecycle / OnRenewal / OnTermination` 的统一契约；
- `EventCore`：事件内核（`EventManager` 的底座）；
- `DatasetBase`：数据集基类（配合编辑器 `DatasetBaseEditor`）；
- `Constants`：全局常量；
- `Reactive`：`Observable<T>` / `Subject<T>` / `ReactiveProperty<T>` / `CompositeDisposable` 与 `Where` / `Select` / `DistinctUntilChanged`，Event 模块即建立在其上（见 [REACTIVE.md](REACTIVE.md)）。

## 6. 架构模板（`Architeture`）

| 模板 | 组成 | 适用 |
| --- | --- | --- |
| LiteMVC | `Model` / `View` / `Controller` + `CommandBinder` / `CommandPool` | 轻量、事件驱动 |
| PureMVC | `Model` / `View` / `Controller` + `Proxy` / `Mediator` / `Command` / `Notification`，带 `[MVCProxy]` 等特性与 `MVCSingleton` | 需要标准 PureMVC 分层 |

两者与模块系统解耦，可单独使用（示例 `03_UI` 用 PureMVC）。

## 7. 资源集成（`Integrations`）

| 方案 | 实现 | 说明 |
| --- | --- | --- |
| Addressables | `AddressablesProvider` | 官方可寻址资源 |
| YooAsset | `YooAssetProvider` | 国产常用，支持包管理 |
| QuarkAsset | `QuarkAssetProvider` | 轻量方案 |
| AssetDatabase | `Editor/ResourceFallback/AssetDatabaseProvider` | 编辑器兜底，无需构建产物 |

接入方式：实现/选用 `IResourceProvider`，启动时 `SetProvider(name, settings)`，或用 `RegisterProvider(name, factory)` 注册自定义实现。契约与能力位见 [RESOURCE.md](RESOURCE.md)。

## 8. 编辑器工具（`Editor`）

| 工具 | 位置 | 作用 |
| --- | --- | --- |
| CosmosConfig 面板 | `IndieEditor/CosmosConfig` | 可视化配置启动参数、帮助体、资源方案 |
| 脚本头生成 | `Tools/EditorConfig` | 新建脚本自动加文件头（`IScriptHeaderProvider`） |
| 层级查找 | `Tools/HierachyFinder` | 按名字/组件快速定位场景对象 |
| 搜索工具条 | `Tools/SearchToolBar` | AssetDatabase 搜索 / GitHub 搜索 |
| 编辑器协程 | `EditorCoroutines` | 编辑器内 coroutine |
| 数据集面板 | `IndieEditor/DatasetBase` | `DatasetBase` 的可视化编辑 |
| 模块窗口基类 | `ModuleEditor/Base` | 自定义模块调试窗口的基类 |
| AssetBundle 命令 | `Command/AssetBundleCommand` | 构建 AssetBundle |
| 快捷命令 | `Command/ShortcutCommand` | 常用菜单项 |

运行时的模块调试窗口在 `Runtime/ModulesDebuger/ModuleDebugWindow.cs`（依赖 `ModuleWindowBase`）。

## 9. 约定与常见坑

1. **取模块**：`GetModule<T>()` 未注册即抛异常；可选模块用 `TryGetModule<T>()`；自定义模块必须按接口取，不能按类名。
2. **依赖时机**：`OnInitialization` 里只能依赖更早的模块；跨模块依赖放 `OnPreparatory`。
3. **契约命名**：优先 `I{类名}`，否则歧义会直接报错；同一契约只能有一个模块。
4. **优先级**：内置 100~1300、默认 10000；内置模块漏写或重复优先级会启动报错。
5. **刷新**：实体的刷新由 Entity 模块集中派发（实现 `IEntityTickable`），不要同时写 `Update`；普通 MonoBehaviour 直接用 `Update`。
6. **网络**：通道必须被 `TickRefresh` 驱动（注册到 `NetworkManager` 或自己 Update 调），否则 `await` 只会等到超时。
7. **线程**：后台线程回调统一 `UnityMainThread.Post/Send`，不要直接碰 Unity API。
8. **对象池**：`IObjectPool.Spawn` 目前不在接口上，需要具体类型 `ObjectPool`（已知缺口）。
9. **UI**：`SetUIFormAssetHelper` 必须先于任何 `OpenUIFormAsync`；否则界面无父节点。
10. **资源**：`SetProvider` 前模块处于未初始化状态，`IsInitialized` 为 false；切换方案会先 `UnsetProvider`。
