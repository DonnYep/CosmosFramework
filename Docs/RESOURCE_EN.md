# Resource Module (Pluggable Providers)

> The framework no longer ships a resource-management implementation. Loading is delegated to a Provider;
> the framework keeps only a **bridge** that wires providers, reference counting, name indexing and diagnostics.

## 1. Design Core

> **Loading solutions are pluggable; every module's resource lifecycle is built on the Resource module; and the Resource module itself is built on a pluggable loading solution.**

One dependency-inversion chain: each layer knows only the next layer's contract, never its implementation.

```text
UI / Audio / Entity / Scene / DataTable / ObjectsPool
        │  depends on IResourceManager only
        ▼
Modules/Resource (bridge)   provider selection · ref count · name index · scene registry · diagnostics
        │  IResourceProvider (4 methods)
        ▼
Addressables   │   YooAsset   │   QuarkAsset   │   AssetDatabase (editor fallback)
```

| Module | Loaded through the bridge | Released when |
| --- | --- | --- |
| UI | panel prefab | panel closes → `UnloadAsset` |
| Audio | `AudioClip` | audio unloaded → `UnloadAsset` |
| DataTable | table `TextAsset` | table unloaded → `UnloadAsset` |
| ObjectsPool | pool prefab | pool unloaded → `UnloadAsset` |
| Entity | entity prefab | resident; reclaimed on framework shutdown |
| Scene | scene | `UnloadSceneAsync`, or reclaimed by the bridge |

The bridge handles only two things: `key → handle` and `handle → reference count`. Bundles, catalogs,
packages, manifests, download and encryption stay inside adapters; capability differences are declared
with capability flags and checked by the bridge, which reports a clear error instead of letting an
adapter pretend to support something it does not.

## 2. Assemblies

| Assembly | Location | Note |
| --- | --- | --- |
| `Cosmos.Resource.Abstractions` | `Runtime/Base/ResourceAbstractions` | contract + shared async core, no third-party dependency |
| `Cosmos.Runtime` | `Runtime/Modules/Resource` | the bridge (`IResourceManager`) |
| `Cosmos.Resource.EditorFallback` | `Editor/ResourceFallback` | `AssetDatabase` fallback, Editor only |
| `Cosmos.Resource.Addressables` | `Runtime/Integrations/Addressables` | optional adapter |
| `Cosmos.Resource.YooAsset` | `Runtime/Integrations/YooAsset` | optional adapter |
| `Cosmos.Resource.QuarkAsset` | `Runtime/Integrations/QuarkAsset` | optional adapter |

Optional adapters use `versionDefines` + `defineConstraints`, so they are simply not compiled when the
corresponding package is absent.

## 3. Configure

On `CosmosConfig`:

| Field | Example |
| --- | --- |
| Provider | `Addressables`, `YooAsset`, `QuarkAsset`, `AssetDatabase` |
| Settings | `key=value;key2=value2` |

| Provider | Settings |
| --- | --- |
| `AssetDatabase` | none (editor fallback) |
| `Addressables` | optional `updateCatalog=true` |
| `YooAsset` | `package=DefaultPackage;mode=offline` (or `mode=editor;manifest=<path>`) |
| `QuarkAsset` | `mode=assetbundle;path=<dir>;aesKey=xxx;offset=0`, or `mode=assetdatabase;dataset=QuarkAssetDataset` |

Defaults to `AssetDatabase`, so samples run in the editor without any build step.

## 4. Usage

```csharp
// handle API (recommended)
var handle = CosmosEntry.ResourceManager.LoadAssetHandle<GameObject>("Prefabs/Cube");
handle.Completed += h => Instantiate(h.Asset);
yield return handle;               // coroutine
var cube = await handle.Task;      // async/await
handle.Release();

// module-compatible API (same shape as before the refactor)
CosmosEntry.ResourceManager.LoadPrefabAsync("UI/Main", go => { }, null, true);
CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo("Level_01"), null, p => { }, null, () => { });
CosmosEntry.ResourceManager.UnloadAsset("UI/Main");
```

Keys are passed to the provider unchanged: asset name / name.ext / asset path / provider-specific address.
The framework does not add another addressing layer.

## 5. Writing a provider

1. Reference `Cosmos.Resource.Abstractions`;
2. Implement `IResourceProvider` (`InitializeAsync` + `CreateOperation`) and a `ResourceProviderOperation`
   per load mode (`OnStartLoad` / `OnUpdateLoad` / `OnRelease`);
3. Register it once:

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
static void Register() => ResourceProviderRegistry.Register("MyProvider", () => new MyProvider());
```

Reference implementations live in `Runtime/Integrations/` (about 100~200 lines each).

## 6. Capability matrix

| Capability | Addressables | YooAsset | QuarkAsset | AssetDatabase |
| --- | --- | --- | --- | --- |
| Asset | ✔ | ✔ | ✔ | ✔ |
| Sub assets | ✔ | ✔ | ✔ | ✔ |
| All assets | ✔ | ✔ | ✔ | ✔ |
| Scene | ✔ | ✔ | ✔ | ✔ |
| Instance (release destroys) | ✔ | ✗ | ✗ | ✔ |
| Priority | ✔ | ✔ | ✗ | ✗ |
| Download / update | native | native | native | ✗ |

Download/update is intentionally not part of the bridge API; use each solution's downloader directly.

## 7. Removed in this refactor

- `Modules/Resource/Legacy` (self-built manifest / bundle / download / verify pipeline);
- `Base/ResourceCore` (self-built package & bundle loader);
- both resource editor windows and build pipelines;
- old types such as `IResourceLoadHelper`, `ResourceLoadMode`, `ResourceDataset`.

`IResourceManager` keeps its name and its module-facing API, so module code was not affected.
