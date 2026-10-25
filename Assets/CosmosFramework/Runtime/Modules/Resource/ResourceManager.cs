using System;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Operation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源模块实现（资源桥）。
    /// <para>把“资源怎么加载”完全交给 <see cref="IResourceProvider"/>，本类只负责：</para>
    /// <para>· Provider 的注册、选择、初始化与切换；</para>
    /// <para>· 引用计数与 name-keyed 兼容层（业务模块只认资源名）；</para>
    /// <para>· 场景句柄的登记与卸载；</para>
    /// <para>· 诊断记录。</para>
    /// </summary>
    [Module(Priority = ModuleConstant.PRIORITY_RESOURCE)]
    internal sealed class ResourceManager : Module, IResourceManager
    {
        /// <summary>
        /// 未配置 Provider 时的默认名称，编辑器兜底 Provider 会以此名注册。
        /// </summary>
        public const string DEFAULT_PROVIDER_NAME = "AssetDatabase";

        readonly Dictionary<string, Func<IResourceProvider>> providerFactories = new Dictionary<string, Func<IResourceProvider>>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, ResourceNameEntry> entries = new Dictionary<string, ResourceNameEntry>(StringComparer.Ordinal);
        readonly Dictionary<string, List<SceneHandle>> loadedScenes = new Dictionary<string, List<SceneHandle>>(StringComparer.Ordinal);

        IResourceProvider provider;
        InitializationOperation initializationOperation;
        string defaultProviderName = DEFAULT_PROVIDER_NAME;

        #region Provider
        /// <inheritdoc/>
        public IResourceProvider Provider
        {
            get { return provider; }
        }
        /// <inheritdoc/>
        public string ProviderName
        {
            get { return provider != null ? provider.Name : null; }
        }
        /// <inheritdoc/>
        public bool IsInitialized
        {
            get { return provider != null && provider.IsInitialized; }
        }
        /// <inheritdoc/>
        public string DefaultProviderName
        {
            get { return defaultProviderName; }
            set { defaultProviderName = string.IsNullOrEmpty(value) ? DEFAULT_PROVIDER_NAME : value; }
        }
        /// <inheritdoc/>
        public void RegisterProvider(string name, Func<IResourceProvider> factory)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name is invalid !");
            if (factory == null)
                throw new ArgumentNullException("factory is invalid !");
            providerFactories[name] = factory;
        }
        /// <inheritdoc/>
        public bool HasProvider(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return providerFactories.ContainsKey(name) || ResourceProviderRegistry.Contains(name);
        }
        /// <inheritdoc/>
        public void SetProvider(string name, string settings = null)
        {
            if (string.IsNullOrEmpty(name))
                name = defaultProviderName;
            IResourceProvider target = null;
            if (providerFactories.TryGetValue(name, out var factory))
            {
                target = factory();
            }
            else if (!ResourceProviderRegistry.TryCreate(name, out target))
            {
                try
                {
                    target = Utility.Assembly.GetTypeInstance(name) as IResourceProvider;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Resource] create provider failed : {name} : {e}");
                }
            }
            if (target == null)
            {
                Debug.LogError($"[Resource] provider not found : {name}\n" +
                    $"registered : {string.Join(", ", providerFactories.Keys)} {string.Join(", ", ResourceProviderRegistry.Names)}\n" +
                    "若使用三方方案，请确认对应程序集已挂载（Addressables / YooAsset / QuarkAsset 适配器）。");
                return;
            }
            SetProvider(target, settings);
        }
        /// <inheritdoc/>
        public void SetProvider(IResourceProvider target, string settings = null)
        {
            if (target == null)
            {
                Debug.LogError("[Resource] provider is invalid !");
                return;
            }
            if (provider != null)
                UnsetProvider();
            provider = target;
            var parameters = ResourceInitParameters.Parse(settings, new ResourceInitParameters(target.Name));
            parameters.ProviderName = target.Name;
            try
            {
                initializationOperation = provider.InitializeAsync(parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] provider initialize failed : {provider.Name} : {e}");
                initializationOperation = null;
                return;
            }
            if (initializationOperation != null && initializationOperation.Status == OperationStatus.None)
                OperationSystem.StartOperation(initializationOperation);
        }
        /// <inheritdoc/>
        public void UnsetProvider()
        {
            UnloadAllAsset(false);
            if (provider != null)
            {
                try
                {
                    provider.Destroy();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Resource] provider destroy failed : {e}");
                }
            }
            provider = null;
            initializationOperation = null;
        }
        #endregion

        #region 句柄 API
        /// <inheritdoc/>
        public AssetHandle<T> LoadAssetHandle<T>(string assetKey, uint priority = 0) where T : Object
        {
            var operation = CreateOperation(assetKey, typeof(T), AssetLoadMode.SingleAsset, priority);
            return operation != null ? operation.CreateAssetHandle<T>() : null;
        }
        /// <inheritdoc/>
        public SubAssetsHandle<T> LoadSubAssetsHandle<T>(string assetKey, uint priority = 0) where T : Object
        {
            if (!CheckCapability(ResourceCapabilities.SubAssets, "LoadSubAssets", assetKey))
                return null;
            var operation = CreateOperation(assetKey, typeof(T), AssetLoadMode.SubAssets, priority);
            return operation != null ? operation.CreateSubAssetsHandle<T>() : null;
        }
        /// <inheritdoc/>
        public AllAssetsHandle<T> LoadAllAssetsHandle<T>(string assetKey, uint priority = 0) where T : Object
        {
            if (!CheckCapability(ResourceCapabilities.AllAssets, "LoadAllAssets", assetKey))
                return null;
            var operation = CreateOperation(assetKey, typeof(T), AssetLoadMode.AllAssets, priority);
            return operation != null ? operation.CreateAllAssetsHandle<T>() : null;
        }
        /// <inheritdoc/>
        public AssetsHandle<T> LoadAssetsHandle<T>(IEnumerable<string> assetKeys, uint priority = 0) where T : Object
        {
            if (assetKeys == null)
                return new AssetsHandle<T>(new AssetHandle<T>[0], new string[0]);
            var handles = new List<AssetHandle<T>>();
            var keys = new List<string>();
            foreach (var key in assetKeys)
            {
                var handle = LoadAssetHandle<T>(key, priority);
                if (handle == null)
                    continue;
                keys.Add(key);
                handles.Add(handle);
            }
            return new AssetsHandle<T>(handles.ToArray(), keys.ToArray());
        }
        /// <inheritdoc/>
        public SceneHandle LoadSceneHandle(string sceneKey, bool additive = true, bool activateOnLoad = true, uint priority = 0)
        {
            if (!CheckCapability(ResourceCapabilities.Scene, "LoadScene", sceneKey))
                return null;
            if (provider == null)
                return null;
            var assetInfo = new AssetInfo(sceneKey, provider.Name);
            var request = ResourceRequest.Scene(assetInfo, additive ? LoadSceneMode.Additive : LoadSceneMode.Single, activateOnLoad, priority);
            var operation = CreateOperation(request);
            return operation != null ? operation.CreateSceneHandle() : null;
        }
        /// <inheritdoc/>
        public GameObjectHandle InstantiateHandle(string assetKey, Transform parent = null, uint priority = 0)
        {
            if (!CheckCapability(ResourceCapabilities.Instance, "Instantiate", assetKey))
                return null;
            if (provider == null)
                return null;
            var assetInfo = new AssetInfo(assetKey, provider.Name);
            var request = ResourceRequest.Instance(assetInfo, parent, priority);
            var operation = CreateOperation(request);
            return operation != null ? operation.CreateInstanceHandle() : null;
        }
        #endregion

        #region 模块兼容 API
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(LoadAssetRoutine(assetName, typeof(T), handle => callback?.Invoke(handle.Asset as T), progress));
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(LoadAssetRoutine(assetName, type, handle => callback?.Invoke(handle.AssetObject), progress));
        }
        /// <summary>
        /// 资产加载协程：先等 Provider 就绪，再取（或复用）操作并等待句柄完成。
        /// </summary>
        IEnumerator LoadAssetRoutine(string assetName, Type type, Action<AssetHandle<Object>> callback, Action<float> progress)
        {
            yield return WaitForReady();
            var entry = GetOrCreateEntry(assetName, type, AssetLoadMode.SingleAsset, 0);
            if (entry == null)
            {
                callback?.Invoke(null);
                yield break;
            }
            var handle = entry.Operation.CreateAssetHandle<Object>();
            entry.Handles.Add(handle);
            while (!handle.IsDone)
            {
                progress?.Invoke(handle.Progress);
                yield return null;
            }
            progress?.Invoke(1f);
            callback?.Invoke(handle);
        }
        /// <inheritdoc/>
        public Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(LoadSubAssetsRoutine(assetName, typeof(T), assets => callback?.Invoke(ToArray<T>(assets)), progress));
        }
        /// <inheritdoc/>
        public Coroutine LoadMainAndSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(LoadSubAssetsRoutine(assetName, type, callback, progress));
        }
        IEnumerator LoadSubAssetsRoutine(string assetName, Type type, Action<Object[]> callback, Action<float> progress)
        {
            yield return WaitForReady();
            if (!CheckCapability(ResourceCapabilities.SubAssets, "LoadMainAndSubAssets", assetName))
            {
                callback?.Invoke(null);
                yield break;
            }
            var entry = GetOrCreateEntry(assetName, type, AssetLoadMode.SubAssets, 0);
            if (entry == null)
            {
                callback?.Invoke(null);
                yield break;
            }
            var handle = entry.Operation.CreateSubAssetsHandle<Object>();
            entry.Handles.Add(handle);
            while (!handle.IsDone)
            {
                progress?.Invoke(handle.Progress);
                yield return null;
            }
            progress?.Invoke(1f);
            callback?.Invoke(handle.Assets);
        }
        /// <inheritdoc/>
        public Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false)
        {
            return Utility.Unity.StartCoroutine(LoadPrefabRoutine(assetName, instantiate, callback, progress));
        }
        IEnumerator LoadPrefabRoutine(string assetName, bool instantiate, Action<GameObject> callback, Action<float> progress)
        {
            yield return WaitForReady();
            var entry = GetOrCreateEntry(assetName, typeof(GameObject), AssetLoadMode.SingleAsset, 0);
            if (entry == null)
            {
                callback?.Invoke(null);
                yield break;
            }
            var handle = entry.Operation.CreateAssetHandle<GameObject>();
            entry.Handles.Add(handle);
            while (!handle.IsDone)
            {
                progress?.Invoke(handle.Progress);
                yield return null;
            }
            progress?.Invoke(1f);
            var asset = handle.Asset;
            if (instantiate && asset != null)
                callback?.Invoke(Object.Instantiate(asset));
            else
                callback?.Invoke(asset);
        }
        /// <inheritdoc/>
        public Coroutine LoadAllAssetAsync(string assetPack, Action<Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(LoadAllAssetsRoutine(assetPack, callback, progress));
        }
        IEnumerator LoadAllAssetsRoutine(string assetPack, Action<Object[]> callback, Action<float> progress)
        {
            yield return WaitForReady();
            if (!CheckCapability(ResourceCapabilities.AllAssets, "LoadAllAsset", assetPack))
            {
                callback?.Invoke(null);
                yield break;
            }
            var entry = GetOrCreateEntry(assetPack, typeof(Object), AssetLoadMode.AllAssets, 0);
            if (entry == null)
            {
                callback?.Invoke(null);
                yield break;
            }
            var handle = entry.Operation.CreateAllAssetsHandle<Object>();
            entry.Handles.Add(handle);
            while (!handle.IsDone)
            {
                progress?.Invoke(handle.Progress);
                yield return null;
            }
            progress?.Invoke(1f);
            callback?.Invoke(handle.Assets);
        }
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(LoadSceneRoutine(sceneAssetInfo, progressProvider, progress, condition, callback));
        }
        IEnumerator LoadSceneRoutine(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            yield return WaitForReady();
            if (string.IsNullOrEmpty(sceneAssetInfo.SceneName))
            {
                Debug.LogError("[Resource] scene name is invalid !");
                callback?.Invoke();
                yield break;
            }
            var handle = LoadSceneHandle(sceneAssetInfo.SceneName, sceneAssetInfo.Additive, true, ToPriority(sceneAssetInfo.Priority));
            if (handle == null)
            {
                callback?.Invoke();
                yield break;
            }
            if (!loadedScenes.TryGetValue(sceneAssetInfo.SceneName, out var sceneHandles))
            {
                sceneHandles = new List<SceneHandle>();
                loadedScenes[sceneAssetInfo.SceneName] = sceneHandles;
            }
            sceneHandles.Add(handle);
            while (!handle.IsDone)
            {
                progress?.Invoke(progressProvider != null ? progressProvider() : handle.Progress);
                yield return null;
            }
            if (handle.Status != OperationStatus.Succeeded)
                Debug.LogError($"[Resource] scene load failed : {sceneAssetInfo.SceneName} : {handle.Error}");
            if (condition != null)
            {
                while (!condition())
                    yield return null;
            }
            progress?.Invoke(1f);
            callback?.Invoke();
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(UnloadSceneRoutine(sceneAssetInfo, progress, condition, callback));
        }
        IEnumerator UnloadSceneRoutine(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback)
        {
            if (!string.IsNullOrEmpty(sceneAssetInfo.SceneName) && loadedScenes.TryGetValue(sceneAssetInfo.SceneName, out var sceneHandles))
            {
                var index = sceneHandles.Count - 1;
                if (index >= 0)
                {
                    var handle = sceneHandles[index];
                    sceneHandles.RemoveAt(index);
                    handle?.Release();
                }
                if (sceneHandles.Count == 0)
                    loadedScenes.Remove(sceneAssetInfo.SceneName);
            }
            if (condition != null)
            {
                while (!condition())
                    yield return null;
            }
            progress?.Invoke(1f);
            callback?.Invoke();
        }
        /// <inheritdoc/>
        public void UnloadAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                return;
            List<ResourceNameEntry> matched = null;
            foreach (var entry in entries.Values)
            {
                if (string.Equals(entry.AssetKey, assetName, StringComparison.Ordinal))
                    (matched ?? (matched = new List<ResourceNameEntry>())).Add(entry);
            }
            if (matched == null)
                return;
            // 一次卸载只释放一个引用，保证与一次加载严格配对
            // （同一标识可能以不同类型/方式加载，各自持有独立的引用计数）
            for (int i = 0; i < matched.Count; i++)
            {
                if (matched[i].Handles.Count == 0)
                    continue;
                ReleaseOneHandle(matched[i]);
                return;
            }
        }
        /// <inheritdoc/>
        public void UnloadAssets(IEnumerable<string> assetNames)
        {
            if (assetNames == null)
                return;
            foreach (var assetName in assetNames)
                UnloadAsset(assetName);
        }
        /// <inheritdoc/>
        public void UnloadAllAsset(bool unloadAllLoadedObjects)
        {
            if (entries.Count > 0)
            {
                var snapshot = new List<ResourceNameEntry>(entries.Values);
                entries.Clear();
                for (int i = 0; i < snapshot.Count; i++)
                {
                    try
                    {
                        snapshot[i].Operation.ReleaseAllHandles();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[Resource] release all handles failed : {e}");
                    }
                }
            }
            if (loadedScenes.Count > 0)
            {
                foreach (var sceneHandles in loadedScenes.Values)
                {
                    for (int i = 0; i < sceneHandles.Count; i++)
                        sceneHandles[i]?.Release();
                }
                loadedScenes.Clear();
            }
            if (provider != null)
            {
                try
                {
                    provider.UnloadUnusedAssets();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Resource] unload unused assets failed : {e}");
                }
            }
            if (unloadAllLoadedObjects)
                Resources.UnloadUnusedAssets();
        }
        #endregion

        #region 内部
        ResourceProviderOperation CreateOperation(string assetKey, Type assetType, AssetLoadMode loadMode, uint priority)
        {
            if (provider == null)
            {
                Debug.LogError($"[Resource] no provider configured, load failed : {assetKey}");
                return null;
            }
            var assetInfo = new AssetInfo(assetKey, provider.Name);
            var request = BuildRequest(assetInfo, assetType, loadMode, priority);
            return CreateOperation(request);
        }
        ResourceProviderOperation CreateOperation(ResourceRequest request)
        {
            ResourceProviderOperation operation;
            try
            {
                operation = provider.CreateOperation(request);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Resource] create operation failed : {request.AssetInfo} : {e}");
                return null;
            }
            if (operation == null)
            {
                Debug.LogError($"[Resource] provider returned a null operation : {request.AssetInfo}");
                return null;
            }
            operation.Destroyed += OnOperationDestroyed;
            operation.Completed += OnOperationCompleted;
            ResourceDiagnostics.OnOperationStart(operation);
            OperationSystem.StartOperation(operation);
            return operation;
        }
        ResourceNameEntry GetOrCreateEntry(string assetKey, Type assetType, AssetLoadMode loadMode, uint priority)
        {
            if (provider == null)
            {
                Debug.LogError($"[Resource] no provider configured, load failed : {assetKey}");
                return null;
            }
            var entryKey = BuildEntryKey(assetKey, assetType, loadMode);
            if (entries.TryGetValue(entryKey, out var entry) && entry.Operation != null && !entry.Operation.IsDestroyed)
                return entry;
            var assetInfo = new AssetInfo(assetKey, provider.Name);
            var request = BuildRequest(assetInfo, assetType, loadMode, priority);
            var operation = CreateOperation(request);
            if (operation == null)
                return null;
            entry = new ResourceNameEntry(entryKey, assetKey, assetType, loadMode, operation);
            entries[entryKey] = entry;
            return entry;
        }
        static ResourceRequest BuildRequest(AssetInfo assetInfo, Type assetType, AssetLoadMode loadMode, uint priority)
        {
            switch (loadMode)
            {
                case AssetLoadMode.SubAssets:
                    return ResourceRequest.SubAssets(assetInfo, assetType, priority);
                case AssetLoadMode.AllAssets:
                    return ResourceRequest.AllAssets(assetInfo, assetType, priority);
                case AssetLoadMode.Instance:
                    return ResourceRequest.Instance(assetInfo, null, priority);
                case AssetLoadMode.Scene:
                    return ResourceRequest.Scene(assetInfo, LoadSceneMode.Additive, true, priority);
                default:
                    return ResourceRequest.Asset(assetInfo, assetType, priority);
            }
        }
        static string BuildEntryKey(string assetKey, Type assetType, AssetLoadMode loadMode)
        {
            return $"{assetKey}|{(assetType != null ? assetType.FullName : string.Empty)}|{loadMode}";
        }
        static uint ToPriority(int priority)
        {
            return priority > 0 ? (uint)priority : 0u;
        }
        void ReleaseOneHandle(ResourceNameEntry entry)
        {
            if (entry == null || entry.Handles.Count == 0)
                return;
            var index = entry.Handles.Count - 1;
            var handle = entry.Handles[index];
            entry.Handles.RemoveAt(index);
            if (handle != null)
                handle.Release();
            if (entry.Handles.Count == 0)
                entries.Remove(entry.EntryKey);
        }
        void OnOperationCompleted(OperationBase operation)
        {
            var providerOperation = operation as ResourceProviderOperation;
            if (providerOperation == null)
                return;
            ResourceDiagnostics.OnOperationFinish(providerOperation);
        }
        void OnOperationDestroyed(ResourceProviderOperation operation)
        {
            ResourceDiagnostics.OnOperationDestroyed(operation);
            if (entries.Count == 0)
                return;
            List<string> stale = null;
            foreach (var kv in entries)
            {
                if (ReferenceEquals(kv.Value.Operation, operation))
                    (stale ?? (stale = new List<string>())).Add(kv.Key);
            }
            if (stale == null)
                return;
            for (int i = 0; i < stale.Count; i++)
                entries.Remove(stale[i]);
        }
        bool CheckCapability(ResourceCapabilities capability, string action, string assetKey)
        {
            if (provider == null)
            {
                Debug.LogError($"[Resource] no provider configured, {action} failed : {assetKey}");
                return false;
            }
            if ((provider.Capabilities & capability) == 0)
            {
                Debug.LogError($"[Resource] provider '{provider.Name}' does not support {capability} : {action} : {assetKey}");
                return false;
            }
            return true;
        }
        IEnumerator WaitForReady()
        {
            if (provider == null)
            {
                Debug.LogError("[Resource] no provider configured. 请在 CosmosConfig 上配置 resourceProviderName（编辑器可直接留空使用 AssetDatabase 兜底）。");
                yield break;
            }
            while (initializationOperation != null && !initializationOperation.IsDone && !provider.IsInitialized)
                yield return null;
            if (!provider.IsInitialized)
                Debug.LogError($"[Resource] provider is not ready : {provider.Name} : {initializationOperation?.Error}");
        }
        static T[] ToArray<T>(Object[] source) where T : Object
        {
            if (source == null)
                return new T[0];
            var result = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i] as T;
            return result;
        }
        #endregion

        #region 诊断
        /// <inheritdoc/>
        public bool DiagnosticsEnabled
        {
            get { return ResourceDiagnostics.Enabled; }
            set { ResourceDiagnostics.Enabled = value; }
        }
        /// <inheritdoc/>
        public bool CaptureStackTrace
        {
            get { return ResourceDiagnostics.CaptureStackTrace; }
            set { ResourceDiagnostics.CaptureStackTrace = value; }
        }
        /// <inheritdoc/>
        public IReadOnlyList<ResourceLoadRecord> LoadRecords
        {
            get { return ResourceDiagnostics.Records; }
        }
        /// <inheritdoc/>
        public void ClearDiagnostics()
        {
            ResourceDiagnostics.Clear();
        }
        /// <inheritdoc/>
        public ResourceRuntimeInfo GetRuntimeInfo()
        {
            var info = new ResourceRuntimeInfo()
            {
                ProviderName = ProviderName,
                ProviderInitialized = IsInitialized,
                ActiveOperationCount = ResourceDiagnostics.ActiveOperationCount,
                TotalLoadCount = ResourceDiagnostics.TotalLoadCount,
                FailedLoadCount = ResourceDiagnostics.FailedLoadCount,
                RecordCount = ResourceDiagnostics.Records.Count,
            };
            foreach (var entry in entries.Values)
            {
                info.LoadedAssets.Add(new LoadedAssetInfo()
                {
                    Key = entry.AssetKey,
                    AssetTypeName = entry.AssetType != null ? entry.AssetType.Name : string.Empty,
                    LoadMode = entry.LoadMode.ToString(),
                    RefCount = entry.Operation != null ? entry.Operation.RefCount : 0,
                    DurationSeconds = entry.Operation != null ? entry.Operation.LoadDurationSeconds : 0,
                    Succeeded = entry.Operation != null && entry.Operation.Status == OperationStatus.Succeeded,
                    Error = entry.Operation != null ? entry.Operation.Error : null,
                });
            }
            return info;
        }
        /// <inheritdoc/>
        public string GenerateDiagnosticsReport()
        {
            return ResourceDiagnostics.GenerateReport();
        }
        #endregion

        #region 生命周期
        protected override void OnTermination()
        {
            UnsetProvider();
            entries.Clear();
            loadedScenes.Clear();
            providerFactories.Clear();
        }
        #endregion
    }
}
