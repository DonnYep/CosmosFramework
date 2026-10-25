# CosmosFramework Manual

> Scope: `Assets/CosmosFramework` (Runtime / Editor).
> This manual is written against the **current code structure**: what exists, how to use it, and where the boundaries are.
> Design rationale and migration notes live in the per-topic documents under `Docs/`.

> ## ⚠️ Breaking update
>
> This release is **not backward compatible**. The full list is in the
> [README breaking changes](../README_EN.md#breaking-changes). Pay attention to:
>
> - **Controller module removed**: entity ticking moved into Entity (implement `IEntityTickable` and friends);
>   `[TickRefresh]` attributes are gone;
> - **Entity contract**: `ShowEntity<T>(entityName)`, new `RegisterLogicEntity<T>` / `PauseEntityGroup`;
> - **Module system**: `[Module]` defaults to priority `10000`, built-ins occupy `100~1300`, duplicate contracts fail;
> - **Module access**: `GetModule<T>()` throws when missing; use `TryGetModule<T>()` for optional modules;
> - **Network**: old awaiter types removed; use `ConnectAsync` / `ReceiveDataAsync` / `WaitDisconnectedAsync`.
> - **Event is reactive now**: `AddEvent` / `RemoveEvent` / `Dispatch` removed; use `Subscribe` (returns `IDisposable`) / `Publish` / `Observe`.

## 1. Layout and layers

```
Assets/CosmosFramework
├─ Runtime
│  ├─ CosmosEntry.cs            App-level static facade: launch, module access, global refresh events
│  ├─ CosmosConfig.cs           Startup configuration (ScriptableObject)
│  ├─ Base
│  │  ├─ Async                  Unified async: OperationBase / OperationSystem / Async extensions
│  │  ├─ Attribute              Shared attributes ([Module], [Implementer], ...)
│  │  ├─ Behaviour              ILifecycle contract
│  │  ├─ Constants              Global constants
│  │  ├─ Dataset                Dataset base class
│  │  ├─ DataStructure          Pools, trees, skip lists, quadtree, grids, A*, AOI, Fix64, geometry
│  │  ├─ EventCore              Event core
│  │  ├─ Extensions             Extension methods (primitives/collections/Unity/reflection/IO)
│  │  ├─ ReferencePool          Reference pool
│  │  ├─ Reactive               Reactive foundation (Observable / Subject / ReactiveProperty)
│  │  ├─ ResourceAbstractions   Resource contracts (Provider / Handle / Operation)
│  │  ├─ Singleton              Singleton / ConcurrentSingleton / MonoSingleton
│  │  ├─ Timer                  TickTimer
│  │  └─ Utility                15 static utility classes (see §4)
│  ├─ Modules                  13 built-in modules + Main (the module system itself)
│  ├─ Integrations              Resource providers: Addressables / YooAsset / QuarkAsset
│  ├─ Architeture              LiteMVC / PureMVC templates
│  ├─ ModulesDebuger           Module debug window
│  └─ 3rdParty                 Hydrogen (TCP), LitJson (JSON)
└─ Editor                       Config panel, search tools, script headers, editor coroutines, fallback provider
```

Three layers:

| Layer | Responsibility | Entry point |
| --- | --- | --- |
| Application | Launch, get modules, global refresh | `CosmosEntry` / `CosmosConfig` |
| Modules | Business capabilities (resource, UI, audio, entity, ...) with one lifecycle | `I*Manager` contracts |
| Foundation | Data structures, utilities, async, pools, extensions | Static calls |

## 2. Startup and the module system

### 2.1 Three ways to launch

```csharp
CosmosEntry.LaunchAppDomainModules();                                 // Built-ins + scan every assembly (most common)
CosmosEntry.LaunchAssemblyModules(typeof(MyModule).Assembly);         // Built-ins + scan the given assemblies
CosmosEntry.LaunchModules(new MyModule(), new OtherModule());         // Fully manual: no scan, no sorting, no built-ins
CosmosEntry.ReleaseLaunchedModules();                                 // Tear down every module
```

- Launch runs once (`Launched` guard); on failure the flag is rolled back so it can be retried.
- Module initialization errors are collected and thrown together as an `AggregateException`.
- `CosmosConfig` (a scene component) launches modules in `Awake`; you normally do not call `Launch*` yourself.

### 2.2 CosmosConfig

| Setting | Purpose |
| --- | --- |
| `launchAppDomainModules` | Whether to launch modules automatically in `Awake` |
| `resourceProviderName` / `resourceProviderSettings` | Resource provider (`Addressables` / `YooAsset` / `QuarkAsset` / `AssetDatabase`) |
| `resourceDiagnostics` | Resource load diagnostics |
| `inputHelperName` / `debugHelperName` / `jsonHelperName` | Helper type names (resolved by name; failures are reported) |
| `printModulePreparatory` | Log module preparation |
| `runInBackground` / `disableDebugLog` | Runtime options |

### 2.3 Module lifecycle

```
Register (OnInitialization) → Active (OnActive) → Preparatory (OnPreparatory)
  → per frame (OnUpdate / OnFixedUpdate / OnLateUpdate / OnElapseUpdate)
  → Deactive (OnDeactive) → Termination (OnTermination)
```

- `OnInitialization` runs inside the registration loop: **only modules with a lower priority are available**;
- Resolve cross-module dependencies in `OnPreparatory` (all modules are registered by then);
- Overriding `OnUpdate` is enough to join the per-frame loop, driven by `MonoGameManager`
  (`[DefaultExecutionOrder(-20000)]`);
- `Module.Pause` pauses that module's ticking.

### 2.4 Priority

`[Module(Priority = n)]`, **lower goes first**. Built-in modules occupy `100~1300` (step 100, see `ModuleConstant`);
custom modules default to `ModuleAttribute.DefaultPriority = 10000`, i.e. after every built-in module.

```csharp
[Module]                 // 10000: after built-ins
[Module(Priority = 50)]  // before every built-in
[Module(Priority = 150)] // between Event(100) and Resource(200)
```

Equal priorities keep the incoming order (built-ins first). Built-in priorities must be explicit, unique and lower
than 10000, otherwise startup fails. See [MODULE.md](MODULE.md) for writing custom modules.

### 2.5 Contracts and access

A module's contract is an `IModuleManager`-derived interface, resolved by `ModuleContractResolver`:

1. exact name `I{ClassName}` (`InventoryManager` → `IInventoryManager`, recommended);
2. interface name ending with the class name;
3. only one candidate;
4. otherwise an error — never "pick one at random".

One contract can only be provided by one module; duplicates fail at startup.

```csharp
var ui = CosmosEntry.UIManager;                          // Built-in: typed property (sugar over GetModule<IUIManager>())
var mine = CosmosEntry.GetModule<IInventoryManager>();   // Custom: always access through the contract interface
if (CosmosEntry.TryGetModule<IOptionalManager>(out var opt)) { }   // Optional module
```

**`GetModule<T>()` throws `InvalidOperationException` when the module is not registered** (it never returns null);
use `TryGetModule` only for genuinely optional modules.

### 2.6 Module instance object

Modules that need a `Transform` parent (UI root canvas, audio attachment, multiplayer instances) use:

```csharp
var root = CosmosEntry.UIManager.InstanceObject();   // Lazy, parented to CosmosRoot, survives scene loads
```

Instance objects are cached per module type, recreated if destroyed externally, and released with the module table.

### 2.7 Threading and global refresh

- All framework ticking happens on the main thread (`MonoGameManager`);
- Background-thread callbacks go through `UnityMainThread`:

```csharp
UnityMainThread.Post(() => { /* runs on the main thread */ });
UnityMainThread.Send(() => { /* runs on the main thread and waits; throws on timeout */ });
UnityMainThread.IsMainThread;
```

`MonoGameManager.Update` drains the queue every frame. Hosts without `MonoGameManager` must call `Drain()` themselves.

- Global refresh events (bypassing modules):

```csharp
CosmosEntry.TickRefreshHandler += OnTick;
CosmosEntry.FixedRefreshHandler += OnFixed;
CosmosEntry.LateRefreshHandler += OnLate;
CosmosEntry.ElapseRefreshHandler += OnElapse;   // real frame delta
```

## 3. Built-in modules

> Contracts live in `Modules/<Name>/I<Name>Manager.cs`; implementations are `internal sealed class <Name>Manager`.
> Only common APIs are listed below — see the interface files for full signatures.

### 3.1 Event (reactive event hub)

- Contract: `IEventManager`; payload types must derive from `GameEventArgs`.
- `Subscribe` returns `IDisposable` (dispose to unsubscribe); `Observe<T>()` exposes the stream for operators.
- Main thread; subscribing/unsubscribing during dispatch is safe.

```csharp
public class LoginEventArgs : GameEventArgs
{
    public string Account;
    public override void Release() { Account = null; }
}

var subscription = CosmosEntry.EventManager.Subscribe<LoginEventArgs>(OnLogin);
CosmosEntry.EventManager.Publish(new LoginEventArgs { Account = "a" });
subscription.Dispose();

CosmosEntry.EventManager.Observe<LoginEventArgs>()
    .Where(args => !string.IsNullOrEmpty(args.Account))
    .Subscribe(OnLogin);
```

API: `Subscribe<T>` / `Publish<T>` / `Observe<T>` / `HasEvent<T>` / `GetEventInfo<T>` / `RemoveAllEvents<T>` / `RemoveAllEvents` / `EventCount`.
Reactive foundation (`Subject` / `ReactiveProperty` / operators / composite disposal): [REACTIVE.md](REACTIVE.md).

### 3.2 Resource

- Contract: `IResourceManager`; the core idea is a **pluggable provider** — the module depends on no resource library.
- Two API styles: handles (`InstantiateHandle` / `LoadSceneHandle`) and coroutine-compatible methods (`LoadPrefabAsync`, ...).
- Coroutine results are awaitable (the framework ships coroutine await extensions).

```csharp
// Provider switch (usually configured through CosmosConfig)
CosmosEntry.ResourceManager.SetProvider("AssetDatabase");   // editor fallback

var prefab = await CosmosEntry.ResourceManager.LoadPrefabAsync("EntityPlayer", true);
var handle = CosmosEntry.ResourceManager.InstantiateHandle("Prefabs/Cube", parent);
var go = handle.WaitForCompletion();
CosmosEntry.ResourceManager.UnloadAsset("EntityPlayer");
```

Diagnostics: `DiagnosticsEnabled`, `LoadRecords`, `GenerateDiagnosticsReport()`, `GetRuntimeInfo()`.
Details and writing your own provider: [RESOURCE.md](RESOURCE.md) / [RESOURCE_EN.md](RESOURCE_EN.md).

### 3.3 WebRequest (HTTP)

- Contract: `IWebRequestManager`; two generations of API — callback tasks and handle/await. Use handles in new code.
- Handles support timeout, retry, priority and cancellation; concurrency is bounded by `MaxConcurrentRequests`.

```csharp
var handle = CosmosEntry.WebRequestManager.AddDownloadTextTaskAsync(url, timeoutSeconds: 10, retryCount: 2);
string text = handle.WaitForCompletion();     // or await it
handle.Cancel();
```

Also: `AddDownloadBytesTaskAsync`, `AddDownloadAudioTaskAsync`, `AddDownloadAssetBundleTaskAsync`,
`AddGetContentLengthTaskAsync`, `AddUploadTaskAsync`.

### 3.4 DataTable

- Contract: `IDataTableManager`; rows are **read-only and shared**, stored in an array with a primary-key index;
  loading is a state machine (idempotent, reloadable).
- An `IDataTableHelper` decides how an asset becomes rows.

```csharp
CosmosEntry.DataTableManager.ReadDataTableAssetAsync(new DataTableAssetInfo("item"), "item");
var table = (DataTable<ItemRow>)CosmosEntry.DataTableManager.GetDataTable("item");
var row = table.GetRowData(1001);
foreach (var r in table.Rows) { }
```

API: `ReadDataTableAssetAsync` / `ReloadDataTableAsync` / `GetDataTable` / `IsDataTableLoaded` / `ReleaseDataTable`.
Details: [DATATABLE.md](DATATABLE.md).

### 3.5 ObjectPool

- Contract: `IObjectPoolManager`; pools are registered by name against a prefab and support expiration
  (`ExpireTime` / `ReleaseInterval` / `Capacity`).

```csharp
var pool = CosmosEntry.ObjectPoolManager.RegisterObjectPool("bullet", bulletPrefab) as ObjectPool;
var go = pool.Spawn();
pool.Despawn(go);
CosmosEntry.ObjectPoolManager.DeregisterObjectPool("bullet");
```

> Known gap: `Spawn()` lives on the concrete `ObjectPool`; `IObjectPool` only exposes `Despawn/ClearPool/capacity/events`.

### 3.6 Entity (object lifetime + centralized ticking)

- Contract: `IEntityManager`; two carriers:
  - `EntityObject` (owns a GameObject, for presentation);
  - `EntityLogic` (plain C#, for logic frames / server-side simulation).
- Ticking is opt-in: implement `IEntityTickable` / `IEntityFixedTickable` / `IEntityLateTickable`; no reflection.
- Group freeze and per-instance pause are independent and compose as a union.

```csharp
// Prefab entity
CosmosEntry.EntityManager.RegisterEntityAsync<EnmeyEntity>(new EntityAssetInfo("Enemy"));
var e = CosmosEntry.EntityManager.ShowEntity<EnmeyEntity>("Enemy");
CosmosEntry.EntityManager.HideEntityObject(e);

// Logic entity (no GameObject)
CosmosEntry.EntityManager.RegisterLogicEntity<BattleLogic>("battle", "logic");
var logic = CosmosEntry.EntityManager.ShowEntity<BattleLogic>("battle");

// Group freeze
CosmosEntry.EntityManager.PauseEntityGroup("monsters");
CosmosEntry.EntityManager.UnPauseEntityGroup("monsters");
```

Details (tick chain, ordering, hiding during tick, migration): [ENTITY.md](ENTITY.md).

### 3.7 UI

- Contract: `IUIManager`; forms implement `IUIForm` (`OnInit/OnOpen/OnClose/OnRelease/OnFocusGained/OnFocusLost`).
- An `IUIFormAssetHelper` decides instantiation/attachment (UGUI uses `UGUIAssetHelper`).
- Supports groups, priority (stacking), activation, queries.

```csharp
var canvas = await CosmosEntry.ResourceManager.LoadPrefabAsync("UICanvas", true);
CosmosEntry.UIManager.SetUIFormAssetHelper(new UGUIAssetHelper(canvas.transform));
await CosmosEntry.UIManager.OpenUIFormAsync<InventoryPanel>(new UIAssetInfo("InventoryPanel"));
CosmosEntry.UIManager.PeekUIForm("InventoryPanel", out var form);
CosmosEntry.UIManager.DeactiveUIForm("InventoryPanel");
```

### 3.8 Audio

- Contract: `IAudioManager`; register assets → `PlayAudio` returns a `serialId`; control by id.
- Supports groups, fade in/out, volume envelopes and priority stealing.

```csharp
CosmosEntry.AudioManager.RegisterAudioAsset("bgm_01");
int id = CosmosEntry.AudioManager.PlayAudio("bgm_01", AudioParams.Default);
CosmosEntry.AudioManager.StopAudio(id, fadeOutSeconds: 0.5f);
```

`AudioParams`: `PlayTime` / `Loop` / `Priority` / `Volume` / `Pitch` / `StereoPan` / `SpatialBlend` / `MaxDistance` / `FadeInSeconds`.
`AudioPositionParams` controls world position and bind parent. Details: [AUDIO.md](AUDIO.md).

### 3.9 Input

- Contract: `IInputManager`; virtual axes/buttons plus a replaceable `IInputHelper` (default `StandardInputHelper`).
- Supports input blocking (`PushInputBlock` / `PopInputBlock`) for modal UI.

```csharp
CosmosEntry.InputManager.SetInputHelper(new StandardInputHelper());
float h = CosmosEntry.InputManager.GetAxis(InputAxisType._Horizontal);
if (CosmosEntry.InputManager.GetButtonDown(InputButtonType._MouseLeft)) { }

CosmosEntry.InputManager.PushInputBlock(this);
CosmosEntry.InputManager.PopInputBlock(this);
```

Details: [INPUT.md](INPUT.md).

### 3.10 FSM

- Contract: `IFSMManager`; machines are keyed by owner type + name; states derive from `FSMState<T>`.

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

`FSMState<T>` also offers `AddTransition` / `RefreshTransition` / `OnInitialization` / `OnTermination`;
`IFSM<T>` offers `ChangeState`, `HasState`, `PeekState`, `OnStateChange`.

### 3.11 Procedure

- Contract: `IProcedureManager`; splits startup/login/scene-loading into nodes.

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

### 3.12 Network (channels + async waiting)

- Contract: `INetworkManager` only registers channels and ticks them. Concrete protocols live in `Assets/CosmosLibs`
  (KCP / TCP / SUDP / RUDP, each a separate assembly — not part of the framework itself).
- Channels are event-driven (`OnConnected` / `OnDataReceived` / `OnDisconnected`); the framework wraps them as awaitables.

```csharp
var channel = new TcpClientChannel("MAIN");     // implementation from CosmosLibs
CosmosEntry.NetworkManager.AddChannel(channel); // required, otherwise nothing calls TickRefresh

await channel.ConnectAsync("127.0.0.1", 7777, ct);   // failure/timeout/cancel all complete normally
var data = await channel.ReceiveDataAsync(ct);      // data arriving early is buffered, not lost
await channel.WaitDisconnectedAsync(ct);
```

Details (buffering, timeouts, thread fallback, channel fixes): [NETWORK_ASYNC.md](NETWORK_ASYNC.md).

### 3.13 Download

- Contract: `IDownloadManager`; the split with WebRequest: **want a file → Download, want data → WebRequest**.
- Supports resumable transfer, concurrency limits, timeouts, pause/resume/cancel and overall progress events.

```csharp
var id = CosmosEntry.DownloadManager.AddDownload(url, savePath, DownloadMode.Overwrite);
CosmosEntry.DownloadManager.LaunchDownload();
CosmosEntry.DownloadManager.PauseDownload();
bool found = CosmosEntry.DownloadManager.GetDownloadInfo(id, out var info);
```

Details: [DOWNLOAD.md](DOWNLOAD.md); the split: [NETWORKING.md](NETWORKING.md).

## 4. Utility classes

All are static partials under `Cosmos.Utility`; call directly (Debug / Json optionally take helpers).

| Class | Purpose | Common APIs |
| --- | --- | --- |
| `Utility.Text` | Strings | `IsStringValid` / `Combine` / `FormatCombine` / `GenerateRandomString` / `GetUTF8Length` / `Replace` / `StringSplit` |
| `Utility.IO` | Files and folders | `ReadTextFileContent` / `WriteTextFile` / `ReadBinaryFile` / `CopyDirectory` / `GetAllFilesRecursively` / `GetFileSize` / `GenerateFileMD5` / `CombineURL` |
| `Utility.Time` | Time and timestamps | `MillisecondTimeStamp` / `SecondNow` / `TimestampToDateTime` / `GetStartOfDay` / `GetTotalHours` / `GetWeekTime` |
| `Utility.Converter` | Bytes / objects / hex | `Object2Bytes` / `ConvertToHexString` / `FormatBytes` / `EncodeToBase64` |
| `Utility.Compression` | Compression | `CompressString` / `DecompressBytes` / `CreateZipFromDirectory` / `ExtractZipToDirectory` |
| `Utility.Encryption` | Digests and ciphers | `MD5Encrypt` / `GenerateHexMD5` / `HmacSHA256` / `AESEncrypt` / `AESDecrypt` / `XorEncrypt` / `GUID` |
| `Utility.Algorithm` | Algorithms and randomness | `BinarySearch` / `Shuffle` / `Disrupt` / `SortByAscending` / `RandomRange` / `RandomNormalDistribution` |
| `Utility.Assert` | Assertions | `NotNull` / `IsNull` / `Predicate` / `State` / `ExceptionCatcher` |
| `Utility.Debug` | Logging | `LogInfo` / `LogWarning` / `LogError` / `SetHelper` / `DisableDebugLog` |
| `Utility.Json` | JSON | `ToJson` / `ToObject` / `BytesToObject` / `SetHelper` |
| `Utility.Net` | Network helpers | `IsRequestSucceeded` / `GetContentLength` / `GetLocalIPv4Address` / `GetPublicIPAddress` / `PingURI` |
| `Utility.Unity` | Unity helpers | `DestroyObject` / `FindSceneGameObject` / `CameraScreenshotAsTexture` / `GetRandomPointInCircle` / `DrawCircleXZ` / `PostToUnityThread` / coroutine tasks |
| `Utility.Assembly` | Reflection | `GetDerivedTypes` / `GetInstancesByAttribute` / `GetPropertyValue` / `SetFieldValue` / `InvokeMethod` |
| `Utility.Program` | Processes | `StartProcess` / `GetMemoryUsage` |
| `Utility` | Misc | `DebugColor` and shared enums |

```csharp
if (!Utility.Text.IsStringValid(name)) return;
Utility.IO.WriteTextFile(path, Utility.Json.ToJson(data));
var now = Utility.Time.MillisecondTimeStamp();
var text = Utility.Text.FormatCombine("player/", id, "/avatar");
```

## 5. Foundation

### 5.1 Data structures (`Base/DataStructure`)

| Category | Types |
| --- | --- |
| Pools | `Pool<T>` (queue based), `ConcurrentPool<T>` |
| Dictionaries / queues | `BiDictionary` / `ConcurrentBiDictionary`, `LruDictionary`, `LimitedQueue`, `TinyLinkedList`, `LinkedNodeList` |
| Trees / skip lists | `BinaryTree`, `SkipList`, `SkipNodeList`, `AOISkipList` |
| Spatial | `QuadTree` / `QuadTreeFix64`, `RectangleGrid` / `RectangleGridFix64`, `SquareGrid` / `SquareGridFix64`, `AOIZone` / `AOIZoneFix64` |
| Pathfinding | `AStar` with `Diagonal` / `Manhattan` / `Euclidean` variants, all with Fix64 versions |
| Fixed point | `Fix64` (with sin/cos LUTs) for deterministic logic |
| Geometry | `Rectangle` / `Square` / `Circle` / `Point2D` / `Vector2D` (Fix64 counterparts) |
| Lightweight FSM | `BasicFsm` / `SimpleFsm` (no module dependency, unit-testable) |
| Concurrency | `AsyncLock` |

> The Fix64 variants of `AStar` / `QuadTree` / `AOI` are deterministic implementations for lockstep/server use,
> with APIs mirroring their floating-point versions.
>
> - **AOI**: inserts and removals go through the axis nodes each entity owns, so entities sharing an X coordinate or a position never delete each other; `AOISkipList.Add` returns the lowest node for O(1) removal.
> - **A\***: open/closed membership is O(1) and neighbor lists are reused across expansions; override `GetNeighborNodes` for a custom neighborhood, or `FillNeighborNodes` (`FillNeighboringNodes` on `AStarFix64`) to avoid even the temporary collection.
> - **Quadtree**: `NodeObjectCapacity` is a split threshold, not a hard limit — a leaf at max depth keeps accumulating objects instead of dropping them.
> - **Fix64**: `(long)Fix64` truncates toward zero (use `Fix64.Floor` first when you need floor); `(Fix64)` saturates out-of-range input and maps `NaN` to 0.

### 5.2 Async (`Base/Async`)

- `OperationBase`: drivable, abortable, prioritized operation base;
- `OperationSystem`: priority scheduling;
- `Async`: one-liners such as `NextFrame()`;
- `WaitOperations`: `WaitUntil` / `WaitWhile` / `WaitForSeconds`;
- Coroutines are awaitable via `AwaitableExtensions`.

Details: [ASYNC.md](ASYNC.md).

### 5.3 Timer (`Base/Timer/TickTimer`)

A lightweight frame/second timer; call `TickRefresh()` yourself (see example `25_TickTimer`).

### 5.4 Pools

- `Pool<T>`: object pool (`Spawn` / `Despawn` / `Clear`); `Clear()` only drops references — use `Clear(release)` when pooled items own resources;
- `ConcurrentPool<T>`: thread-safe variant; `Count` is pooled items, `TotalCount` includes in-flight reservations;
- `ReferencePool`: `Acquire<T>()` / `Release(IReference)`; `IReference.Release()` clears state.

### 5.5 Singletons

| Type | Purpose |
| --- | --- |
| `Singleton<T>` | Plain C# singleton |
| `ConcurrentSingleton<T>` | Thread-safe singleton |
| `MonoSingleton<T>` | MonoBehaviour singleton; auto-created and `DontDestroyOnLoad` on first access |

### 5.6 Extensions (`Base/Extensions`)

Grouped by target: primitives (`IntExts` / `DoubleExts` / `LongExts` / `StringExts` / `DateTimeExts` / `StreamExts`),
collections (`ListExts` / `IEnumerableExts` / `IDictionaryExts` / `ICollectionExts` / `QueueExts` / `StackExts`),
Unity (`GameObjectExts` / `TransformExts` / `ComponentExts` / `RectTransformExts` / `UGUIExts` / `AnimatorExts` /
`AudioSourceExts` / `CameraExts` / `TextureExts` / `SpriteExts` / `VectorExts`), and others
(`ObjectExts.As<T>/CastTo<T>`, `TypeExts`, `FileExts`, `KeyValuePairExts`).

### 5.7 Others

- `ILifecycle`: one contract for `OnInitialization / OnPreparatory / OnActive / OnPause / OnUnPause / OnRefresh /
  OnElapseRefresh / OnDeactive / OnRecycle / OnRenewal / OnTermination`;
- `EventCore`: the event core behind `EventManager`;
- `DatasetBase`: dataset base class (paired with `DatasetBaseEditor`);
- `Constants`: global constants;
- `Reactive`: `Observable<T>` / `Subject<T>` / `ReactiveProperty<T>` / `CompositeDisposable` plus `Where` / `Select` / `DistinctUntilChanged`; the Event module is built on it (see [REACTIVE.md](REACTIVE.md)).

## 6. Architecture templates (`Architeture`)

| Template | Parts | Use when |
| --- | --- | --- |
| LiteMVC | `Model` / `View` / `Controller` + `CommandBinder` / `CommandPool` | Lightweight, event-driven |
| PureMVC | `Model` / `View` / `Controller` + `Proxy` / `Mediator` / `Command` / `Notification`, with `[MVCProxy]`-style attributes and `MVCSingleton` | Standard PureMVC layering |

Both are decoupled from the module system (example `03_UI` uses PureMVC).

## 7. Resource integrations (`Integrations`)

| Provider | Implementation | Notes |
| --- | --- | --- |
| Addressables | `AddressablesProvider` | Unity official addressables |
| YooAsset | `YooAssetProvider` | Package-based resource management |
| QuarkAsset | `QuarkAssetProvider` | Lightweight option |
| AssetDatabase | `Editor/ResourceFallback/AssetDatabaseProvider` | Editor fallback, no build artifacts needed |

Wire-up: implement/choose an `IResourceProvider`, then `SetProvider(name, settings)` at startup, or register your own
through `RegisterProvider(name, factory)`. Contracts and capability flags: [RESOURCE_EN.md](RESOURCE_EN.md).

## 8. Editor tools (`Editor`)

| Tool | Location | Purpose |
| --- | --- | --- |
| CosmosConfig panel | `IndieEditor/CosmosConfig` | Visual configuration of startup, helpers and providers |
| Script header | `Tools/EditorConfig` | Adds file headers to new scripts (`IScriptHeaderProvider`) |
| Hierarchy finder | `Tools/HierachyFinder` | Locate scene objects by name/component |
| Search toolbar | `Tools/SearchToolBar` | AssetDatabase search / GitHub search |
| Editor coroutines | `EditorCoroutines` | Coroutines inside the editor |
| Dataset panel | `IndieEditor/DatasetBase` | Visual editing for `DatasetBase` |
| Module window base | `ModuleEditor/Base` | Base class for custom module debug windows |
| AssetBundle command | `Command/AssetBundleCommand` | Build AssetBundles |
| Shortcut command | `Command/ShortcutCommand` | Common menu items |

The runtime module debug window is `Runtime/ModulesDebuger/ModuleDebugWindow.cs`.

## 9. Conventions and pitfalls

1. **Getting modules**: `GetModule<T>()` throws when missing; use `TryGetModule<T>()` only for optional modules;
   custom modules must be fetched by contract interface, not by class name.
2. **Dependency timing**: `OnInitialization` sees only earlier modules; resolve cross-module dependencies in `OnPreparatory`.
3. **Contract naming**: prefer `I{ClassName}`; ambiguity fails at startup; one contract per module.
4. **Priority**: built-ins 100~1300, default 10000; missing or duplicate built-in priorities fail at startup.
5. **Ticking**: entity ticking is centralized (implement `IEntityTickable`); do not also write `Update`.
   Plain MonoBehaviours can simply use `Update`.
6. **Network**: channels must be driven by `TickRefresh` (register with `NetworkManager` or call it yourself),
   otherwise `await` only ends with a timeout.
7. **Threading**: background callbacks go through `UnityMainThread.Post/Send`; never touch Unity APIs directly.
8. **Object pools**: `IObjectPool.Spawn` is not on the interface yet; cast to the concrete `ObjectPool` (known gap).
9. **UI**: `SetUIFormAssetHelper` must be called before any `OpenUIFormAsync`, otherwise forms have no parent.
10. **Resource**: `IsInitialized` is false before `SetProvider`; switching providers unsets the previous one first.
