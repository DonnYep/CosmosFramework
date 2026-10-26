![CosmosGitLogo](Docs/CosmosGitLogo.png)

[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE)
[![Issues:Welcome](https://img.shields.io/badge/Issues-welcome-blue.svg)](https://github.com/DonnYep/CosmosFramework/issues)

# [中文](README.md)

# CosmosFramework

CosmosFramework is a medium-lightweight Unity framework: a unified module lifecycle, pluggable resource providers, centralized object ticking, async/await support and multiple network channels — plus a large set of utilities, data structures and editor tools.

- [Full manual](Docs/MANUAL_EN.md): modules, utilities, foundation, usage and boundaries (written against the current code)
- [CosmosFramework Wiki](https://github.com/DonnYep/CosmosFramework/wiki)

> ## ⚠️ This release is a breaking update
>
> It is **not backward compatible**. Check the [breaking changes](#breaking-changes) before upgrading.
>
> - **Controller module removed**: entity ticking moved into Entity; `CosmosEntry.ControllerManager`, `IControllerManager`, `[TickRefresh]` / `[LateRefresh]` / `[FixedRefresh]` are deleted.
> - **Entity contract changed**: `ShowEntity(string, out EntityObject)` is now `ShowEntity<T>(string)`; new `RegisterLogicEntity<T>` (logic entities without a GameObject), `PauseEntityGroup`, `GetEntityObject`.
> - **Module system**: `[Module]` default priority is now `10000` (built-ins occupy `100~1300`); duplicate contracts fail at startup; contract resolution is deterministic.
> - **Module access**: `GetModule<T>()` throws `InvalidOperationException` when missing (`TryGetModule<T>()` added).
> - **Network**: `WaitConnectedAwaiter` / `WaitReceiveDataAwaiter` / `WaitForChannelExts` are deleted; use `ConnectAsync` / `ReceiveDataAsync` / `WaitDisconnectedAsync` (same names, new cancellation/timeout parameters).
> - **Event is reactive now**: `Subscribe` returns `IDisposable`, `Observe<T>()` supports `Where` / `Select` / `DistinctUntilChanged`; `AddEvent` / `RemoveEvent` / `Dispatch` are removed, and `Runtime/Base/Reactive` is added as the reactive foundation.
> - **MonoSingleton**: the first access (including pre-placed instances) now calls `OnInitialize`, i.e. `DontDestroyOnLoad`.

<a name="navigation"></a>

# Navigation
- [中文](README.md)
- [CosmosFramework](#cosmosframework)
- [Breaking changes](#breaking-changes)
- [Environment](#environment)
- [Quick start](#quick-start)
- [Project structure](#project-structure)
- [Core mechanisms](#core-mechanisms)
- [Modules](#modules)
- [Utilities](#utilities)
- [Foundation](#foundation)
- [Architecture templates](#architecture-templates)
- [Resource integrations](#resource-integrations)
- [Editor tools](#editor-tools)
- [Documentation](#documentation)
- [Known boundaries](#known-boundaries)
- [UPM](#upm)
- [Other](#other)
- [Library link](#library-link)

<a name="breaking-changes"></a>

## Breaking changes

| Change | Impact | Action |
| --- | --- | --- |
| Controller module removed | `[TickRefresh]` polling no longer works | Implement `IEntityTickable` / `IEntityFixedTickable` / `IEntityLateTickable`; plain MonoBehaviours can use `Update` |
| `CosmosEntry.ControllerManager` removed | Compile error | Use `CosmosEntry.EntityManager`: `Paused` for per-instance pause, `PauseEntityGroup` for group freeze |
| `ShowEntity(string, out EntityObject)` removed | Compile error | Use `ShowEntity<T>(entityName)` |
| Event API is reactive | `AddEvent` / `RemoveEvent` / `Dispatch` removed | Use `Subscribe` (returns `IDisposable`) / `Publish` / `Observe` |
| `LimitedQueue<T>` is composition-based | No longer a `Queue<T>` subclass; passing it as `Queue<T>` breaks | Use its own `Enqueue` / `Dequeue` / `TryDequeue` / `Peek` / `Clear` |
| Unified module priority | Custom modules with a small explicit `Priority` initialize earlier | Omit priority to stay after every built-in; opt in with `[Module(Priority = n)]` |
| Duplicate contracts rejected | A custom module sharing a built-in contract will not start | Remove one, drop the built-in from `BuiltinModuleRegistry`, or use `CosmosEntry.LaunchModules` |
| `GetModule<T>()` throws | Code relying on a null return now throws | Use `TryGetModule<T>()` for optional modules |
| Network awaiters rewritten | References to old awaiter types break | Use the same-named extensions `ConnectAsync` / `ReceiveDataAsync`, plus `WaitDisconnectedAsync` |
| `MonoSingleton` init timing | Pre-placed instances also become `DontDestroyOnLoad` | No action needed |
| `(long)Fix64` truncates toward zero | Negative results change (`(long)(Fix64)(-1.5)` is `-1` instead of `-2`) | Use `Fix64.Floor` before converting when you need floor semantics |
| `(Fix64)` overloads saturate out-of-range values | No more wraparound (`(Fix64)long.MaxValue` no longer yields `-1`); `NaN` maps to `0`, `±Infinity` / out-of-range saturates to `MaxValue` / `MinValue` | No action needed; relying on wraparound was undefined usage |
| `AStarFix64.Clear()` clears the grid | `GetNode` returns `null` after `Clear()` (same as `AStar`) | Use `ClearNodeCache()` to drop pooled buffers only |
| `QuadTree` leaves no longer drop objects | A full leaf at max depth keeps accumulating (may exceed `NodeObjectCapacity`); `Insert` no longer returns `false` | No action needed; capacity is a split threshold, not a hard limit |
| AOI inserts/removes by node identity | Entities sharing an X coordinate or a position no longer delete each other; `AOISkipList.Add` returns the lowest node for O(1) removal | No action needed |
| `Circle` relation fixes | Concentric or contained circles are no longer reported as separate / crossing | No action needed |

<a name="environment"></a>

## Environment

- Unity 2018+; .NET API level 4.x (Unity 2021.3 LTS recommended).

<a name="quick-start"></a>

## Quick start

1. **Launch**: add the `CosmosConfig` component to your boot scene (with `launchAppDomainModules` enabled), or call:

```csharp
CosmosEntry.LaunchAppDomainModules();                                   // built-ins + scan every assembly
CosmosEntry.LaunchAssemblyModules(typeof(MyModule).Assembly);           // built-ins + given assemblies
CosmosEntry.LaunchModules(new MyModule(), new OtherModule());           // fully manual (no built-ins)
```

2. **Get modules**: typed properties for built-ins, contract interfaces for custom modules:

```csharp
var ui = CosmosEntry.UIManager;                                  // built-in
var mine = CosmosEntry.GetModule<IInventoryManager>();           // custom (throws when missing)
if (CosmosEntry.TryGetModule<IOptionalManager>(out var opt)) { } // optional
```

3. **Write a module**:

```csharp
public interface IInventoryManager : IModuleManager { void AddItem(int id, int count); }

[Module(Priority = 150)]                                   // optional: default 10000, i.e. after built-ins
public class InventoryManager : Module, IInventoryManager
{
    protected override void OnPreparatory() { }            // resolve cross-module dependencies here
    protected override void OnUpdate() { }                 // joins the per-frame loop automatically
    public void AddItem(int id, int count) { }
}
```

See [custom modules](Docs/MODULE.md).

4. **Use entities**:

```csharp
CosmosEntry.EntityManager.RegisterEntityAsync<EnemyEntity>(new EntityAssetInfo("Enemy"));
var enemy = CosmosEntry.EntityManager.ShowEntity<EnemyEntity>("Enemy");

CosmosEntry.EntityManager.RegisterLogicEntity<BattleLogic>("battle", "logic");  // no GameObject
var logic = CosmosEntry.EntityManager.ShowEntity<BattleLogic>("battle");
```

<a name="project-structure"></a>

## Project structure

```text
Assets/CosmosFramework
├─ Runtime
│  ├─ CosmosEntry.cs            App-level facade: launch, module access, global refresh events
│  ├─ CosmosConfig.cs           Startup configuration (ScriptableObject)
│  ├─ Base
│  │  ├─ Async                  Unified async: OperationBase / OperationSystem / Async
│  │  ├─ Attribute              Shared attributes ([Module], ...)
│  │  ├─ Behaviour              ILifecycle contract
│  │  ├─ Constants              Global constants
│  │  ├─ Dataset                Dataset base class
│  │  ├─ DataStructure          Pools, trees, skip lists, quadtree, grids, A*, AOI, Fix64, geometry
│  │  ├─ EventCore              Event core
│  │  ├─ Extensions             Extension methods
│  │  ├─ ReferencePool          Reference pool
│  │  ├─ Reactive               Reactive foundation (Observable / Subject / ReactiveProperty)
│  │  ├─ ResourceAbstractions   Resource contracts (Provider / Handle / Operation)
│  │  ├─ Singleton              Singleton / ConcurrentSingleton / MonoSingleton
│  │  ├─ Timer                  TickTimer
│  │  └─ Utility                15 static utility classes
│  ├─ Modules                  13 built-in modules + Main (module system)
│  ├─ Integrations             Addressables / YooAsset / QuarkAsset
│  ├─ Architeture              LiteMVC / PureMVC
│  ├─ ModulesDebuger           Module debug window
│  └─ 3rdParty                 Hydrogen (TCP), LitJson (JSON)
└─ Editor                       Config panel, search, script headers, editor coroutines, fallback provider
```

<a name="core-mechanisms"></a>

## Core mechanisms

### Module lifecycle and priority

```text
Register(OnInitialization) → Active(OnActive) → Preparatory(OnPreparatory)
  → per frame(OnUpdate / OnFixedUpdate / OnLateUpdate / OnElapseUpdate)
  → Deactive(OnDeactive) → Termination(OnTermination)
```

- `[Module(Priority = n)]`: lower goes first; built-ins occupy `100~1300`, custom modules default to `10000`;
- `OnInitialization` only sees earlier modules; resolve cross-module dependencies in `OnPreparatory`;
- Overriding `OnUpdate` joins the loop automatically, driven by `MonoGameManager` (`[DefaultExecutionOrder(-20000)]`);
- A contract is an `IModuleManager`-derived interface; resolution order: `I{ClassName}` → suffix → single candidate → error. One contract per module.

### Ticking

| Mechanism | Use for |
| --- | --- |
| `MonoBehaviour.Update` | Plain presentation scripts |
| Entity centralized ticking | Entities implementing `IEntityTickable` etc.; supports group freeze and per-instance pause |
| Global events | `CosmosEntry.TickRefreshHandler` / `FixedRefreshHandler` / `LateRefreshHandler` / `ElapseRefreshHandler` |
| `TickTimer` | Independent timers |

### Threading

- All framework ticking happens on the main thread;
- Background callbacks go through `UnityMainThread.Post` / `UnityMainThread.Send`, drained by `MonoGameManager.Update`;
- Network awaiters already use this fallback.

<a name="modules"></a>

## Modules

| Module | Contract | Responsibility |
| --- | --- | --- |
| Event | `IEventManager` | Reactive event hub: `Subscribe` returns `IDisposable`, `Observe<T>()` supports operators |
| Resource | `IResourceManager` | Pluggable resource bridge: handles and coroutines, ref counting, diagnostics |
| WebRequest | `IWebRequestManager` | HTTP: handles with timeout / retry / priority / cancellation |
| DataTable | `IDataTableManager` | Read-only shared rows + key index; state-machine loading, reloadable |
| ObjectPool | `IObjectPoolManager` | Prefab pools with expiration |
| Entity | `IEntityManager` | Lifetime + centralized ticking: `EntityObject` (GameObject) / `EntityLogic` (pure logic), group freeze |
| UI | `IUIManager` | Form loading / activation / groups / priority (`IUIForm` lifecycle) |
| Audio | `IAudioManager` | Asset registration, playback, groups, fades, envelopes, stealing |
| Input | `IInputManager` | Virtual axes/buttons, input blocking, replaceable `IInputHelper` |
| FSM | `IFSMManager` | State machines keyed by owner type (`FSMState<T>`) |
| Procedure | `IProcedureManager` | Procedure nodes for boot / login / scene flow |
| Network | `INetworkManager` | Channel registration and ticking; protocols live in `Assets/CosmosLibs` (KCP / TCP / SUDP / RUDP) |
| Download | `IDownloadManager` | File downloads: resume, concurrency limits, pause / resume / cancel |

> Want a file → Download; want data → WebRequest. See [networking](Docs/NETWORKING.md).

<a name="utilities"></a>

## Utilities

Static classes under `Cosmos.Utility`:

| Class | Contents |
| --- | --- |
| Text / Converter | String checks and concatenation, hex, Base64, byte formatting |
| IO / Compression | File and directory IO, copy, size, MD5; Zip compression |
| Time | Timestamps, day/week/month math, conversions |
| Encryption | MD5 / HMAC / AES / XOR / GUID |
| Algorithm | Binary search, sorting, shuffle, random and normal distribution |
| Assert | Assertions and exception capture |
| Debug / Json | Logging and JSON (replaceable helpers) |
| Net | Request checks, content length, local/public IP, ping |
| Unity | Destroy, screenshots, random points, Gizmos, coroutine tasks (delay / frame / condition) |
| Assembly / Program | Reflection helpers, process start and memory usage |

See [manual §4](Docs/MANUAL_EN.md) for common APIs and samples.

<a name="foundation"></a>

## Foundation

- **Data structures**: `Pool<T>` / `ConcurrentPool<T>`, `BiDictionary`, `LruDictionary`, `LimitedQueue`, `SkipList` / `AOISkipList`, `QuadTree`, `RectangleGrid` / `SquareGrid`, `AStar` (Diagonal / Manhattan / Euclidean), `AOIZone`, `Fix64` (deterministic fixed point), `BasicFsm` / `SimpleFsm`, geometry (with Fix64 variants).
- **Async**: `OperationBase` / `OperationSystem` / `Async.WaitUntil` / `WaitForSeconds` / `NextFrame` / `WaitForTask`; coroutines are awaitable.
- **Pools**: `Pool<T>` object pool, `ReferencePool` reference pool.
- **Singletons**: `Singleton<T>` / `ConcurrentSingleton<T>` / `MonoSingleton<T>`.
- **Reactive**: `Observable<T>` / `Subject<T>` / `ReactiveProperty<T>` / `CompositeDisposable`; the Event module is built on it.
- **Extensions**: primitives, collections, Unity components, reflection, files.
- **Lifecycle**: `ILifecycle`; `EventCore`; `DatasetBase`.

<a name="architecture-templates"></a>

## Architecture templates

| Template | Parts | Use when |
| --- | --- | --- |
| LiteMVC | `Model` / `View` / `Controller` + `CommandBinder` / `CommandPool` | Lightweight, event-driven |
| PureMVC | `Model` / `View` / `Controller` + `Proxy` / `Mediator` / `Command` / `Notification`, with `[MVCProxy]`-style attributes and `MVC.RegisterAttributedMVC()` | Standard PureMVC layering |

<a name="resource-integrations"></a>

## Resource integrations

| Provider | Implementation | Notes |
| --- | --- | --- |
| Addressables | `AddressablesProvider` | Unity official addressables |
| YooAsset | `YooAssetProvider` | Package-based resources |
| QuarkAsset | `QuarkAssetProvider` | Lightweight option |
| AssetDatabase | `Editor/ResourceFallback/AssetDatabaseProvider` | Editor fallback, no build artifacts |

Wire up through `IResourceProvider`; `SetProvider(name, settings)` or `RegisterProvider(name, factory)` for custom implementations. See [resource documentation](Docs/RESOURCE_EN.md).

<a name="editor-tools"></a>

## Editor tools

`CosmosConfig` panel, script headers, hierarchy finder, AssetDatabase / GitHub search, editor coroutines, dataset panel, module window base, AssetBundle and shortcut commands. The runtime module debug window lives in `ModulesDebuger`.

<a name="documentation"></a>

## Documentation

| Document | Contents |
| --- | --- |
| [Manual](Docs/MANUAL_EN.md) | Modules, utilities, foundation, usage and boundaries (primary reference) |
| [Custom modules](Docs/MODULE.md) | Contract naming, priority, lifecycle, error reference |
| [Bootstrap and module registration](Docs/BOOTSTRAP.md) | Launch chain, registry, scanning and ordering |
| [Entity](Docs/ENTITY.md) | Two carriers, centralized ticking, group freeze |
| [Reactive](Docs/REACTIVE.md) | Subject / ReactiveProperty / operators / composite disposal |
| [Resource](Docs/RESOURCE_EN.md) | Provider contracts, handle API, diagnostics |
| [Async](Docs/ASYNC.md) | Unified async |
| [Audio](Docs/AUDIO.md) / [DataTable](Docs/DATATABLE.md) / [Input](Docs/INPUT.md) / [Download](Docs/DOWNLOAD.md) | Module details |
| [Network channel async](Docs/NETWORK_ASYNC.md) | Channel awaits, timeout, cancellation, thread fallback |
| [WebRequest vs Download](Docs/NETWORKING.md) | Selection rules |

<a name="known-boundaries"></a>

## Known boundaries

- `IObjectPool` does not expose `Spawn()`; use the concrete `ObjectPool`.
- Channels must be driven by `TickRefresh` (registered with `NetworkManager` or called manually), otherwise awaits only time out.
- Module access requires a completed launch; `GetModule<T>()` throws when missing.
- Hiding entities during a tick is allowed; registering/deregistering entities is not.
- The `AStar` open set is scanned linearly (O(open-set size) per expansion, no heap); on a large unreachable map the cost sits in the expansion phase (~20 ms for 100×100).
- Removing an AOI entity does not strip it from other entities' `ViewEntities` (instances are pooled), so consumers should drive off `Exited` / `Entered` instead of holding entity references.

<a name="upm"></a>

## UPM

CosmosFramework follows the Unity Package convention; the package root is `Assets/CosmosFramework` (the version lives in its `package.json`).

**Release build (recommended)** — pinned to a tag, immune to later commits. Add this to `dependencies` in `Packages/manifest.json`:

```json
"com.cosmosframework": "https://github.com/DonnYep/CosmosFramework.git?path=/Assets/CosmosFramework#v2.0.1"
```

- Replace `#v2.0.1` with `#main` to follow the latest release on the stable line.
- Replace `#v2.0.1` with `#dev` to try the development line — it may contain unreleased (and possibly non-compiling) changes; use it for feedback only.
- Without a Git URL: copy the whole `Assets/CosmosFramework` folder into your project's `Packages` folder (Unity treats it as a local package).

> `?path=` requires Unity 2019.3+; Unity 2021.3 LTS is recommended. See [CHANGELOG](CHANGELOG.md) for version history.

<a name="other"></a>

## Other

- Built-in examples: `Assets/Examples/` (Entity, UI, Audio, Input, Network, Procedure, ...)
- Module demo videos:
    - https://www.bilibili.com/video/BV11P4y1U7vB
    - https://www.bilibili.com/video/BV1x741157eR
    - https://www.bilibili.com/video/BV17u411Z7Ni

<a name="library-link"></a>

## Library link

- Hydrogen: https://github.com/DonnYep/Hydrogen
- HybridCLR: https://github.com/focus-creative-games/hybridclr
- CosmosEngine: https://github.com/DonnYep/CosmosEngine
- QuarkAsset: https://github.com/DonnYep/QuarkAsset
- KCP C: https://github.com/skywind3000/kcp
- KCP CSharp: https://github.com/vis2k/kcp2k
- TCP: https://github.com/vis2k/Telepathy
- Mirror: https://github.com/vis2k/Mirror

**[Back to top](#navigation)**
