# 模块启动（CosmosConfig / 模块注册）

> 重构目标：**启动路径上不再有"全量 AppDomain 扫描 + 每次生命周期反射 Invoke"**，
> 同时保留"用户自定义模块自动发现"这个便利。

## 1. 启动链

```text
CosmosConfig.Awake        （[DefaultExecutionOrder(-10000)]，先于业务脚本）
  ├─ 静态配置：PrintModulePreparatory / runInBackground / DisableDebugLog
  ├─ LoadHelpers()         Debug / Json 帮助体（类型名解析 + 校验）
  └─ LaunchAppDomainModules()
        │
        ▼
CosmosEntry.LaunchAppDomainModules()
        │
        ▼
GameManager.InitiateAppDomainModule()
  ├─ 内置模块：BuiltinModuleRegistry.CreateModules()   ← 直接 new，零反射
  └─ 外部模块：DiscoverModules(assemblies)             ← 只扫"引用了框架程序集"的程序集
        │
        ▼
Launch()：注册 → OnActive → OnPreparatory → 挂 Tick/Fixed/Late/Elapse 委托
```

## 2. 旧实现的开销在哪

| 位置 | 问题 |
| --- | --- |
| `InitiateAppDomainModule` | 对 **AppDomain 里每一个程序集** 调 `GetTypes()`（mscorlib/System.*/UnityEngine.* 全部展开），然后 LINQ `Where` 过滤——启动期一次性的大块分配与 GC 峰值 |
| `GetInstancesByAttribute<T,K>` | 每个候选类型再调 `GetCustomAttributes<T>()`（分配特性数组），命中后 `Activator.CreateInstance` |
| 生命周期 | `Utility.Assembly.InvokeMethod(module, "OnInitialization")` 等：**每个模块每个阶段**都做一次 `type.BaseType.GetMethod(...)` + `MethodInfo.Invoke` |
| 模块接口 | `type.GetInterfaces()` + 字符串 `Contains` 每次都重算 |
| `CosmosConfig` | 帮助体类型名走 `AppDomain` 逐个 `assembly.GetType`；解析失败**静默忽略** |
| `CosmosConfigEditor.OnEnable` | `GetDerivedTypeHandlers<T>()` 每次选中 Inspector 都全量扫描一次（编辑器侧卡顿） |

> 注意：这些都属于**启动期一次性开销**，不影响每帧性能。但对"启动时间敏感"
> 的项目（移动端首包、Play 模式频繁重进）值得优化，而且顺手能去掉一堆反射失败的风险。

## 3. 新方案

### 3.1 内置模块：显式注册表（零反射）

`Runtime/Modules/Main/BuiltinModuleRegistry.cs`：

```csharp
internal static Module[] CreateModules()
{
    return new Module[]
    {
        new EventManager(), new ResourceManager(), new WebRequestManager(),
        new DataTableManager(), new ObjectPoolManager(), new EntityManager(),
        new UIManager(), new AudioManager(), new InputManager(),
        new ControllerManager(), new FSMManager(), new ProcedureManager(),
        new NetworkManager(), new DownloadManager(),
    };
}
```

- 顺序即初始化顺序（事件 → 资源 → 网络请求 → 其它），可读、可调、每次启动一致。
- 没有 `GetTypes`、没有 `Activator`、没有特性反射。
- 新增内置模块：加一行；删除：删一行。

### 3.2 外部模块：按"程序集引用"过滤后再扫

```csharp
static bool MayContainModules(Assembly assembly)   // 只做元数据读取，不展开类型
{
    foreach (var reference in assembly.GetReferencedAssemblies())
        if (reference.Name == frameworkAssemblyName) return true;   // 模块必然引用 Cosmos.Runtime
    return false;
}
```

- 典型工程里绝大多数程序集（mscorlib / System.* / UnityEngine.* / 第三方库）**根本不引用框架程序集**，
  在 `GetTypes()` 之前就被过滤掉；
- 剩下的程序集按 `[Module]` 优先级 + 类型全名排序后实例化，顺序稳定；
- `GetTypes()` 用 `ReflectionTypeLoadException` 兜底（部分类型加载失败不会让整个启动崩掉）。

### 3.3 生命周期：直接调用，不再反射

`Module` 增加了 internal 直调入口（虚分派语义不变）：

```csharp
internal void InvokeInitialization() { OnInitialization(); }
internal void InvokeActive() { OnActive(); }
internal void InvokePreparatory() { OnPreparatory(); }
internal void InvokeDeactive() { OnDeactive(); }
internal void InvokeTermination() { OnTermination(); }
```

`GameManager` 因此可以 `module.InvokeTermination()`，删掉全部 `Utility.Assembly.InvokeMethod` 调用。
（旧实现还有一个隐患：`InvokeMethod` 取的是 `type.BaseType` 上的方法，靠虚分派才落到子类；
现在没有这层间接了。）

### 3.4 启动可观测

`printModulePreparatory = true` 时会打印：

```text
[Cosmos] modules : 14 , registration & preparation elapsed 0 ms
```

用 `System.Diagnostics.Stopwatch` 统计，方便在真实工程里对比优化前后。

### 3.5 CosmosConfig 的调整

| 项 | 旧 | 新 |
| --- | --- | --- |
| 执行顺序 | `Awake` 与业务脚本同级 | `[DefaultExecutionOrder(-10000)]` |
| 帮助体解析 | 类型名找不到时静默跳过 | `TryResolveHelper<T>`：校验接口/无参构造，失败 `Debug.LogWarning` |
| 帮助体缓存 | 每次 `AppDomain` 逐个 `GetType` | `Utility.Assembly.GetType` 带**命中缓存**（未命中不缓存，避免程序集延迟加载时缓存错误结果） |
| 死字段 | `messagePackHelperIndex`（声明后从未使用） | 删除 |
| 重复启动 | 可能重复注册 | `CosmosEntry` + `GameManager` 双重 `Launched` 守卫，重复调用会告警并忽略 |

### 3.6 编辑器侧

`EditorUtil.GetDerivedTypeHandlers<T>()` 增加按类型的静态缓存（域重载即失效，不会脏），
`GetDerivedTypeHandlers<T>(reload: true)` 可强制重扫。Inspector 反复选中不再重复扫描 AppDomain。

## 4. 启动 API

```csharp
CosmosEntry.LaunchAppDomainModules();                    // 内置 + 自动发现（推荐，保底行为）
CosmosEntry.LaunchAssemblyModules(asmA, asmB);           // 内置 + 只扫这两个程序集
CosmosEntry.LaunchModules(new MyModule(), new Other());  // 只注册传入模块（完全显式控制）
```

自定义模块两种写法：

```csharp
// A. 自动发现：程序集引用 Cosmos.Runtime 且类型带 [Module]
[Module]                                  // Priority 越小越先初始化（默认 100）
public class MyManager : Module, IMyManager { protected override void OnInitialization() { } }

// B. 显式注册
CosmosEntry.LaunchModules(BuildinAndMine());

static Module[] BuildinAndMine()
{
    var modules = new List<Module> { new MyModule() };   // 内置模块可自行 new
    return modules.ToArray();
}
```

## 5. 迁移注意

| 变更 | 说明 |
| --- | --- |
| `LaunchAssemblyModules` | **现在也会注册内置模块**（旧版只注册目标程序集里的，等于没有 `ResourceManager`/`AudioManager`，基本不可用） |
| 重复 `Launch*` | 第二次起被忽略并告警（旧版会重复注册/重复挂委托） |
| 没有匹配接口的模块 | 启动时告警并跳过（旧版静默跳过） |
| `ModuleAttribute.Priority` | 现在真正生效（旧版字段存在但从未使用） |
| `Module` 基类 | 新增 internal 直调入口；自定义模块无感知 |
