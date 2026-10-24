using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.EditorFallback
{
    /// <summary>
    /// 编辑器兜底 Provider：直接用 AssetDatabase 加载，不需要任何构建产物。
    /// <para>用途：示例工程开箱即跑；没有接入任何三方方案时，编辑器内仍然可以正常开发。</para>
    /// <para>仅编辑器可用（程序集 includePlatforms = Editor），真机请配置 Addressables / YooAsset / QuarkAsset。</para>
    /// </summary>
    public sealed class AssetDatabaseProvider : IResourceProvider
    {
        /// <summary>
        /// 注册名
        /// </summary>
        public const string PROVIDER_NAME = "AssetDatabase";

        readonly Dictionary<string, string> pathCache = new Dictionary<string, string>(StringComparer.Ordinal);
        ResourceInitParameters initParameters;

        /// <summary>
        /// 程序集加载时自动注册，因此 CosmosConfig 只要写 "AssetDatabase" 即可。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            ResourceProviderRegistry.Register(PROVIDER_NAME, () => new AssetDatabaseProvider());
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return PROVIDER_NAME; }
        }
        /// <inheritdoc/>
        public ResourceCapabilities Capabilities
        {
            get { return ResourceCapabilities.Asset | ResourceCapabilities.SubAssets | ResourceCapabilities.AllAssets | ResourceCapabilities.Scene | ResourceCapabilities.Instance | ResourceCapabilities.Sync; }
        }
        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }
        /// <inheritdoc/>
        public ResourceInitParameters InitParameters
        {
            get { return initParameters; }
        }
        /// <inheritdoc/>
        public InitializationOperation InitializeAsync(ResourceInitParameters parameters)
        {
            initParameters = parameters;
            IsInitialized = true;
            var operation = new InitializationOperation(Name);
            operation.Complete("editor");
            return operation;
        }
        /// <inheritdoc/>
        public ResourceProviderOperation CreateOperation(ResourceRequest request)
        {
            return new AssetDatabaseOperation(this, request);
        }
        /// <inheritdoc/>
        public void UnloadUnusedAssets()
        {
            // AssetDatabase 资源由编辑器管理，无需手动卸载
        }
        /// <inheritdoc/>
        public void Destroy()
        {
            IsInitialized = false;
            pathCache.Clear();
        }

        #region 路径解析
        /// <summary>
        /// 把资源标识解析为资产路径，支持：
        /// <para>· Assets/ 开头的完整路径；</para>
        /// <para>· 资源名；</para>
        /// <para>· 资源名.后缀。</para>
        /// </summary>
        internal string ResolvePath(string key, Type assetType)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (pathCache.TryGetValue(key, out var cached))
                return cached;
            string result = null;
            if (key.StartsWith("Assets/", StringComparison.Ordinal) && File.Exists(key))
                result = key;
            if (string.IsNullOrEmpty(result))
                result = SearchAssetPath(key, assetType);
            if (!string.IsNullOrEmpty(result))
                pathCache[key] = result;
            return result;
        }
        static string SearchAssetPath(string key, Type assetType)
        {
            AssetInfo.ParseKey(key, out var name, out var extension);
            if (string.IsNullOrEmpty(name))
                return null;
            var filter = string.IsNullOrEmpty(extension) ? name : $"{name}.{extension}";
            var guids = AssetDatabase.FindAssets(filter);
            if (guids == null || guids.Length == 0)
                guids = AssetDatabase.FindAssets(name);
            string fallback = null;
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path))
                    continue;
                if (!string.Equals(Path.GetFileNameWithoutExtension(path), name, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!string.IsNullOrEmpty(extension) && !string.Equals(Path.GetExtension(path).TrimStart('.'), extension, StringComparison.OrdinalIgnoreCase))
                    continue;
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset == null)
                    continue;
                if (assetType != null && assetType != typeof(Object) && assetType != typeof(Scene) && !assetType.IsInstanceOfType(asset))
                {
                    if (fallback == null)
                        fallback = path;
                    continue;
                }
                return path;
            }
            return fallback;
        }
        #endregion

        /// <summary>
        /// AssetDatabase 加载操作：同步完成，仅用于编辑器开发期。
        /// </summary>
        internal sealed class AssetDatabaseOperation : ResourceProviderOperation
        {
            readonly AssetDatabaseProvider provider;
            AsyncOperation sceneOperation;
            string scenePath;

            public AssetDatabaseOperation(AssetDatabaseProvider provider, ResourceRequest request) : base(provider, request)
            {
                this.provider = provider;
            }

            protected override void OnStartLoad()
            {
                var key = AssetInfo.Key;
                var path = provider.ResolvePath(key, AssetType);
                if (string.IsNullOrEmpty(path))
                {
                    Finish($"asset not found : {key}");
                    return;
                }
                switch (LoadMode)
                {
                    case AssetLoadMode.Scene:
                        StartLoadScene(path);
                        break;
                    case AssetLoadMode.Instance:
                        LoadInstance(path);
                        break;
                    case AssetLoadMode.SubAssets:
                        SetAssets(AssetDatabase.LoadAllAssetRepresentationsAtPath(path));
                        Finish();
                        break;
                    case AssetLoadMode.AllAssets:
                        SetAssets(AssetDatabase.LoadAllAssetsAtPath(path));
                        Finish();
                        break;
                    default:
                        {
                            var asset = AssetDatabase.LoadAssetAtPath(path, AssetType);
                            if (asset == null)
                            {
                                Finish($"asset type mismatch : {key} ({AssetType})");
                                return;
                            }
                            SetAsset(asset);
                            Finish();
                            break;
                        }
                }
            }

            void LoadInstance(string path)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    Finish($"prefab not found : {AssetInfo.Key}");
                    return;
                }
                var instance = Object.Instantiate(prefab, Request.InstanceParent);
                instance.name = prefab.name;
                SetInstance(instance);
                Finish();
            }

            void StartLoadScene(string path)
            {
                scenePath = path;
                var loadType = Request.SceneMode == LoadSceneMode.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
                try
                {
                    sceneOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters(loadType));
                }
                catch (Exception e)
                {
                    Finish($"scene load failed : {path} : {e.Message}");
                }
            }

            protected override void OnUpdateLoad()
            {
                if (sceneOperation == null)
                    return;
                SetProgress(sceneOperation.progress);
                if (!sceneOperation.isDone)
                    return;
                var scene = SceneManager.GetSceneByPath(scenePath);
                SetScene(scene, Path.GetFileNameWithoutExtension(scenePath));
                sceneOperation = null;
                Finish();
            }

            protected override void OnRelease()
            {
                if (LoadMode == AssetLoadMode.Instance)
                {
                    if (InstanceObject != null)
                        Object.Destroy(InstanceObject);
                    return;
                }
                if (LoadMode == AssetLoadMode.Scene && SceneObject.IsValid() && SceneObject.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(SceneObject);
                }
            }

            protected override void OnAbortLoad()
            {
                sceneOperation = null;
            }
        }
    }
}
