# 写一个自定义模块

> 适用范围：`Assets/CosmosFramework/Runtime/Modules/Main`
> 相关文档：[启动流程与模块注册](BOOTSTRAP.md)

自定义模块与内置模块共用同一套生命周期、轮询与优先级序列，区别只在"谁来 new"：
内置模块由 `BuiltinModuleRegistry` 显式创建，自定义模块由程序集扫描（`[Module]`）或
`CosmosEntry.LaunchModules(...)` 显式传入。

## 1. 最小可运行示例

```csharp
using Cosmos;

public interface IInventoryManager : IModuleManager
{
    void AddItem(int itemId, int count);
}

[Module(Priority = 150)]                 // 可省略；省略 = 10000，排在全部内置模块之后
public class InventoryManager : Module, IInventoryManager
{
    IEventManager eventManager;

    protected override void OnInitialization()
    {
        // 自身状态初始化；此时只有优先级更小的模块可用
    }
    protected override void OnPreparatory()
    {
        // 跨模块依赖在这里取：OnPreparatory 时所有模块都已注册完毕
        eventManager = CosmosEntry.EventManager;
    }
    protected override void OnUpdate()
    {
        // 每帧；由 MonoGameManager 统一驱动，无需 MonoBehaviour
    }
    protected override void OnTermination()
    {
        // 释放
    }
    public void AddItem(int itemId, int count) { }
}
```

启动（二选一，只调用一次）：

```csharp
CosmosEntry.LaunchAppDomainModules();                                  // 内置 + 扫描所有程序集
CosmosEntry.LaunchAssemblyModules(typeof(InventoryManager).Assembly);   // 内置 + 只扫指定程序集
```

使用：

```csharp
var inventory = CosmosEntry.GetModule<IInventoryManager>();
inventory?.AddItem(1001, 2);
```

## 2. 三条硬性要求

| 要求 | 说明 |
| --- | --- |
| 继承 `Module` | 生命周期钩子与轮询都来自它 |
| 实现一个 `IModuleManager` 派生接口 | 这是模块的对外契约，也是 `GetModule<T>()` 的键 |
| 可无参实例化 + `[Module]` | 扫描路径用 `Activator.CreateInstance`；类不能是抽象/泛型；特性用于被发现 |

程序集必须**直接引用** `Cosmos.Runtime`（普通游戏脚本/asmdef 默认满足），否则扫描会被跳过。

## 3. 契约命名规则

解析顺序（只考虑 `IModuleManager` 派生接口）：

1. 精确名 `I{类名}`：`InventoryManager` → `IInventoryManager`（推荐）；
2. 接口名以类名结尾：`InventoryManager` → `IInventoryManagerEx` 不匹配，但 `IOuterInventoryManager` 匹配；
3. 只有一个候选接口 → 直接采用；
4. 仍然歧义 → **启动报错**，不会"随便挑一个"。

两个模块提供同一契约 → **启动报错**，不会出现"内置模块继续运行、而 `GetModule<T>()` 返回另一个实例"的双份真相。

## 4. 优先级

- 数值越小越先初始化，也越先挂载轮询回调；
- 内置模块占用 `100 ~ 1300`（见 `ModuleConstant`），步长 100，便于插队；
- 自定义模块默认 `ModuleAttribute.DefaultPriority` = `10000`，即排在全部内置模块之后；
- 同优先级保持传入顺序（内置模块先放入，因此"内置优先"）。

```csharp
[Module]                                    // 10000：内置模块之后
[Module(Priority = 50)]                     // 全部内置模块之前
[Module(Priority = 150)]                    // Event(100) 与 Resource(200) 之间
[Module(Priority = 1350)]                   // 全部内置模块之后、其它默认模块之前
```

注意：`CosmosEntry.LaunchModules(...)` 是"完全由你决定"的入口——**不排序、不校验、也不包含内置模块**，
适合测试或刻意替换模块集合的场景。

## 5. 生命周期与时序

```
注册（OnInitialization）→ 激活（OnActive）→ 准备（OnPreparatory）
  → 每帧（OnUpdate / OnFixedUpdate / OnLateUpdate / OnElapseUpdate）
  → 失活（OnDeactive）→ 终止（OnTermination）
```

- `OnInitialization` 在注册循环里调用：**只能拿到优先级更小的模块**；
- 需要依赖其它模块（尤其是内置模块）时，放到 `OnPreparatory`——此时所有模块都已注册；
- 取模块统一用 `CosmosEntry.GetModule<T>()`：**未注册时抛 `InvalidOperationException`**（不返回 null）；
  模块确实可选用 `CosmosEntry.TryGetModule<T>(out var module)`；
- 框架内置的 13 个 `CosmosEntry.XxxManager` 属性只是 `GetModule<IXxxManager>()` 的一行糖，
  自定义模块没有对应属性，用泛型即可；
- 勾选/取消轮询不需要额外操作：重写了 `OnUpdate` 就会被每帧调用；
- `Module.Pause` 可暂停该模块的轮询（`protected set`，在模块内部设置）。

## 6. 常见启动报错对照

| 报错 | 原因 |
| --- | --- |
| `Module contract : IXxxManager is not registered.` | 模块没被启动（未 `Launch`、不在扫描程序集里），或该契约被其它模块占用 |
| `does not implement a contract derived from IModuleManager, skipped.` | 只实现了 `IModuleManager` 本身，或接口不是它的派生 |
| `implements multiple module contracts (...) and none of them can be chosen unambiguously` | 多接口且无法按命名规则区分 → 把契约改名为 `I{类名}`，或只保留一个 |
| `the contract IXxxManager is already provided by YyyManager` | 两个模块提供同一契约 → 删掉一个，或用 `LaunchModules` 自定义集合 |
| `Built-in module : Xxx must declare a priority lower than 10000` | 内置模块忘了给 `[Module(Priority = ...)]`，会破坏"用户模块默认排最后"的约定 |
| `Built-in modules : A and B share the same priority N` | 内置模块优先级重复 |

## 7. 想替换内置模块怎么办

框架不提供"运行时覆盖内置契约"的开关（那会留下两份实例）。正确做法二选一：

1. 从 `BuiltinModuleRegistry` 移除对应项，再让自定义模块提供同一契约；
2. 用 `CosmosEntry.LaunchModules(...)` 自行组织完整模块集合（需要自己 `new` 内置模块）。
