# 启动与模块注册

> 适用范围：`Assets/CosmosFramework/Runtime/Modules/Main`
> 相关文档：[自定义模块](MODULE.md)、[使用手册 §2](MANUAL.md)

## 1. 启动链

```
CosmosConfig.Awake
 └→ CosmosEntry.LaunchAppDomainModules()
     └→ GameManager.InitiateAppDomainModule()
         ├─ 取 AppDomain 程序集列表
         ├─ BuiltinModuleRegistry.CreateModules()      // 内置模块（显式 new）
         ├─ DiscoverModules(assemblies)                // 外部 [Module]（按需反射）
         ├─ ModuleOrdering.Sort(...)                   // 统一优先级排序
         └─ Launch(modules)
             ├─ RegisterModule × n                     // OnInitialization
             ├─ ActiveModule()                         // OnActive
             ├─ PrepareModule()                        // OnPreparatory + 挂载轮询
             └─ ThrowModuleInitException()             // 聚合异常
```

驱动层：`MonoGameManager`（`MonoSingleton`，`[DefaultExecutionOrder(-20000)]`）
每帧调用 `GameManager.OnRefresh / OnElapseRefresh / OnLateRefresh / OnFixedRefresh`，
并 `UnityMainThread.Drain()`。

## 2. 启动 API

| API | 模块集合 | 扫描 |
| --- | --- | --- |
| `LaunchAppDomainModules()` | 内置 + 扫描到的外部模块 | 全部 AppDomain 程序集 |
| `LaunchAssemblyModules(params Assembly[])` | 内置 + 指定程序集里的外部模块 | 仅指定程序集 |
| `LaunchModules(params Module[])` | 仅传入的模块 | 无；不排序、不校验 |
| `ReleaseLaunchedModules()` | — | 终止全部模块 |

- 启动有 `Launched` 守卫，重复调用会被忽略并告警；
- 启动失败会回滚守卫，允许修复后重试。

## 3. 内置模块注册表

`BuiltinModuleRegistry.CreateModules()` 只决定"有哪些内置模块"，**不决定顺序**：

```csharp
new EventManager(), new ResourceManager(), new WebRequestManager(),
new DataTableManager(), new ObjectPoolManager(), new EntityManager(),
new UIManager(), new AudioManager(), new InputManager(),
new FSMManager(), new ProcedureManager(), new NetworkManager(), new DownloadManager(),
```

新增 / 删除内置模块时，需要同步在 `ModuleConstant` 里分配或移除一个未占用的优先级。

## 4. 优先级与排序

- 优先级来自 `[Module(Priority = n)]`，未标注时取 `ModuleAttribute.DefaultPriority`（10000）；
- 内置模块占 `100~1300`（`ModuleConstant`，步长 100，留出插队空隙）；
- 排序用稳定排序：同优先级保持传入顺序（内置模块先放入 → 内置优先）；
- 启动前校验内置模块：必须标注、优先级唯一、且小于 10000，否则直接抛异常。

```csharp
[Module]                 // 10000：排在全部内置模块之后
[Module(Priority = 50)]  // 全部内置模块之前
[Module(Priority = 150)] // Event(100) 与 Resource(200) 之间
```

## 5. 外部模块发现

`DiscoverModules(assemblies)`：

1. 跳过框架自身程序集、动态程序集，以及**没有直接引用框架程序集**的程序集（`MayContainModules`）；
2. `GetTypes()` 用 `ReflectionTypeLoadException` 兜底（部分类型加载失败不影响启动）；
3. 过滤：非抽象类、实现 `Module`、标注 `[Module]`；
4. 按 `Priority` + 类型全名排序后 `Activator.CreateInstance` 实例化（需要无参构造）。

实例化失败会进入启动异常集合，不会中断其它模块。

## 6. 契约解析与重复注册

`ModuleContractResolver` 只考虑 `IModuleManager` 派生接口，按以下顺序解析：

1. 精确名 `I{类名}`（`UIManager` → `IUIManager`）；
2. 接口名以类名结尾（`AudioManager` → `ICoreAudioManager`）；
3. 只有一个候选时直接采用；
4. 仍歧义 → 抛异常（列出候选）。

同一契约只能由一个模块提供，重复注册会抛 `ArgumentException` 并进入启动异常集合。
要替换内置实现：从 `BuiltinModuleRegistry` 移除该项，或用 `CosmosEntry.LaunchModules` 自行组织集合。

## 7. 生命周期调用

注册阶段只调 `OnInitialization`；随后统一走：

```
InvokeActive → InvokePreparatory → （每帧）Update / FixedUpdate / LateUpdate / ElapseUpdate
→ InvokeDeactive → InvokeTermination
```

`Module` 基类暴露的是 `internal` 直调入口，框架侧不做 `MethodInfo.Invoke`。
`OnInitialization` 期间只有更早（优先级更小）的模块可用；跨模块依赖请放 `OnPreparatory`。

## 8. 轮询挂载与终止

- `AddRefreshListener()` 把每个模块的 `Update / FixedUpdate / LateUpdate / ElapseUpdate` 挂到
  `GameManager` 的四个委托上；
- `TerminateModules()` → `RemoveRefreshListener()` → `OnDeactive` → `OnTermination`
  → 清空模块表、契约缓存与模块挂载点（`MonoGameManager.ClearModuleInstanceObjects()`）。

## 9. 可观测与错误处理

| 项 | 说明 |
| --- | --- |
| `CosmosEntry.PrintModulePreparatory` | 是否打印模块准备日志与注册耗时 |
| `ModuleCount` | 已注册模块数量 |
| 启动异常 | 初始化 / 激活 / 准备阶段的异常收集后统一抛 `AggregateException` |
| 终止异常 | 失活 / 终止阶段的异常同样聚合抛出 |

常见启动报错见 [MODULE.md §6](MODULE.md)。

## 10. CosmosConfig 中的启动

`CosmosConfig.Awake` 依次执行：应用静态配置 → 装配 Debug / Json 帮助体 →
（`launchAppDomainModules` 为真时）启动模块 → 按配置设置资源 Provider 与输入帮助体。
关闭自动启动时，需要自行调用上表的 `Launch*`，并在之后设置 Provider / 输入帮助体。
