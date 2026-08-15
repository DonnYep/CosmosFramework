using Cosmos.Operation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包裹。一个资源包裹包含多个PackageBundle。
    /// <para>包裹是资源加载的独立单元，支持分包加载、独立卸载。</para>
    /// </summary>
    public class ResourcePackage
    {
        readonly PackageAddress packageAddress = new PackageAddress();
        readonly string packageName;
        readonly Bundles bundles;
        readonly Dictionary<string, ProviderBase> providerMap = new Dictionary<string, ProviderBase>();
        PackageManifest manifest;
        ResourceInitParameters initParameters;

        public string PackageName
        {
            get { return packageName; }
        }
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool IsInitialized { get; private set; }
        /// <summary>
        /// 资源清单
        /// </summary>
        public PackageManifest Manifest
        {
            get { return manifest; }
        }
        /// <summary>
        /// 当前激活的Provider数量
        /// </summary>
        public int LoadedProviderCount
        {
            get { return providerMap.Count; }
        }
        /// <summary>
        /// 当前加载的包体数量
        /// </summary>
        public int LoadedBundleCount
        {
            get { return bundles.Count; }
        }

        private ResourcePackage()
        {
        }
        internal ResourcePackage(string packageName)
        {
            this.packageName = packageName;
            bundles = new Bundles(this);
        }

        /// <summary>
        /// 异步初始化资源包裹。
        /// <para>加载文件清单并构建寻址索引，完成后即可进行资源加载。</para>
        /// </summary>
        public InitializationHandle InitializeAsync(ResourceInitParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("ResourceInitParameters is invalid !");
            initParameters = parameters;
            var operation = new PackageInitializationOperation(this, parameters);
            InternalStartOperation(operation);
            return new InitializationHandle(operation);
        }

        /// <summary>
        /// 异步加载资产。
        /// <para>assetKey能够识别以下几种类型：assetName，assetName.ext，assetPath</para>
        /// <para>assetName: 资产文件名，e.g. myText</para>
        /// <para>assetName.ext: 资产文件名加上后缀，e.g. myText.txt</para>
        /// <para>assetPath: 资产路径，e.g. Assets/Text/myText.txt</para>
        /// <para>同一资产的多路加载共享同一Provider，不会重复发起加载请求。</para>
        /// </summary>
        /// <typeparam name="T">资产类型</typeparam>
        /// <param name="assetKey">资产识别标识</param>
        /// <returns>加载的handle</returns>
        public AssetHandle<T> LoadAssetAsync<T>(string assetKey)
            where T : Object
        {
            var assetInfo = GetAssetInfo(assetKey);
            if (assetInfo == null || assetInfo.Invalid)
                return ResourceHelper.CreateFailedHandle<T>(this, assetKey, $"Asset not found : {assetKey}");
            return LoadAssetAsync<T>(assetInfo);
        }

        /// <summary>
        /// 通过资产信息异步加载资产
        /// </summary>
        public AssetHandle<T> LoadAssetAsync<T>(AssetInfo assetInfo)
            where T : Object
        {
            var provider = GetProvider(assetInfo, typeof(T), AssetLoadMode.SingleAsset);
            return new AssetHandle<T>(provider);
        }

        /// <summary>
        /// 异步加载资产及其子资产
        /// </summary>
        public SubAssetsHandle<T> LoadSubAssetsAsync<T>(string assetKey)
            where T : Object
        {
            var assetInfo = GetAssetInfo(assetKey);
            if (assetInfo == null || assetInfo.Invalid)
                return ResourceHelper.CreateFailedSubAssetsHandle<T>(this, assetKey, $"Asset not found : {assetKey}");
            var provider = GetProvider(assetInfo, typeof(T), AssetLoadMode.SubAssets);
            return new SubAssetsHandle<T>(provider);
        }

        /// <summary>
        /// 异步加载资源包内全部资产
        /// </summary>
        public AllAssetsHandle<T> LoadAllAssetsAsync<T>(string bundleName)
            where T : Object
        {
            var assetInfo = CreateBundleAssetInfo(bundleName);
            var provider = GetProvider(assetInfo, typeof(T), AssetLoadMode.AllAssets);
            return new AllAssetsHandle<T>(provider);
        }

        /// <summary>
        /// 异步加载场景。
        /// <para>sceneKey支持资产名与资产路径。</para>
        /// </summary>
        /// <param name="sceneKey">场景识别标识</param>
        /// <param name="additive">是否叠加加载</param>
        /// <param name="activateOnLoad">加载完成后是否立即激活场景</param>
        public SceneHandle LoadSceneAsync(string sceneKey, bool additive = true, bool activateOnLoad = true)
        {
            var assetInfo = GetAssetInfo(sceneKey);
            if (assetInfo == null || assetInfo.Invalid)
                return ResourceHelper.CreateFailedSceneHandle(this, sceneKey, $"Scene not found : {sceneKey}", additive, activateOnLoad);
            var providerKey = $"{assetInfo.UID}_Scene";
            ProviderBase provider = null;
            if (!providerMap.TryGetValue(providerKey, out provider) || provider.IsDestroyed)
            {
                provider = new SceneProvider(this, assetInfo, additive, activateOnLoad);
                providerMap[providerKey] = provider;
                provider.OnProviderDestroy += () => { providerMap.Remove(providerKey); };
                InternalStartOperation(provider);
            }
            var sceneProvider = provider as SceneProvider;
            if (sceneProvider == null)
                return ResourceHelper.CreateFailedSceneHandle(this, sceneKey, $"Scene provider is invalid : {sceneKey}", additive, activateOnLoad);
            return new SceneHandle(sceneProvider);
        }

        /// <summary>
        /// 通过分类标签异步加载资产集合（Addressables Label 风格）。
        /// <para>同一标签下的资产会逐个异步加载，全部完成后句柄完成；单个失败不影响其余资产。</para>
        /// </summary>
        public TagAssetsHandle<T> LoadAssetsByTagAsync<T>(string tag)
            where T : Object
        {
            var assets = GetAssetInfosByTag(tag);
            if (assets == null || assets.Length == 0)
                return new TagAssetsHandle<T>(new AssetHandle<T>[0]);
            var handles = new AssetHandle<T>[assets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                handles[i] = LoadAssetAsync<T>(assets[i]);
            }
            return new TagAssetsHandle<T>(handles);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        public void UnloadSceneAsync(SceneHandle handle)
        {
            handle?.Release();
        }
        /// <summary>
        /// 释放资产句柄，减少一个引用计数
        /// </summary>
        public void UnloadAsset(HandleBase handle)
        {
            handle?.Release();
        }
        /// <summary>
        /// 释放全部资产与包体
        /// </summary>
        public void UnloadAllAssets()
        {
            foreach (var provider in providerMap.Values)
            {
                if (provider != null && !provider.IsDestroyed)
                    provider.ReleaseAllHandles();
            }
            providerMap.Clear();
            bundles.UnloadAll();
        }

        /// <summary>
        /// 获取资产信息
        /// </summary>
        public AssetInfo GetAssetInfo(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
                return null;
            packageAddress.PeekAssetInfo(assetKey, out var assetInfo);
            return assetInfo;
        }

        /// <summary>
        /// 通过分类标签获取资产信息集合
        /// </summary>
        public AssetInfo[] GetAssetInfosByTag(string tag)
        {
            var assets = manifest?.GetAssetsByTag(tag);
            if (assets == null || assets.Count == 0)
                return null;
            var result = new AssetInfo[assets.Count];
            for (int i = 0; i < assets.Count; i++)
            {
                result[i] = new AssetInfo(packageName, assets[i]);
            }
            return result;
        }

        /// <summary>
        /// 重置资源包裹
        /// </summary>
        public void Reset()
        {
            UnloadAllAssets();
            manifest = null;
            IsInitialized = false;
        }

        internal ProviderBase GetProvider(AssetInfo assetInfo, Type assetType, AssetLoadMode loadMode)
        {
            var providerKey = $"{assetInfo.UID}_{assetType.FullName}_{loadMode}";
            if (providerMap.TryGetValue(providerKey, out var provider))
            {
                if (!provider.IsDestroyed)
                    return provider;
                providerMap.Remove(providerKey);
            }
            provider = ResourceHelper.CreateProvider(this, assetInfo, assetType, loadMode);
            providerMap.Add(providerKey, provider);
            provider.OnProviderDestroy += () => { providerMap.Remove(providerKey); };
            InternalStartOperation(provider);
            return provider;
        }

        internal void InternalStartOperation(OperationBase operationBase)
        {
            OperationSystem.StartOperation(operationBase);
        }
        /// <summary>
        /// 当前激活的Provider集合（内部调试用）
        /// </summary>
        internal IReadOnlyCollection<ProviderBase> GetActiveProviders()
        {
            return providerMap.Values;
        }
        internal PackageManifest GetManifest()
        {
            return manifest;
        }
        internal Bundles GetBundles()
        {
            return bundles;
        }
        internal ResourceInitParameters GetInitParameters()
        {
            return initParameters;
        }
        internal void SetManifest(PackageManifest manifest)
        {
            this.manifest = manifest;
            packageAddress.Init(packageName, manifest.AssetList);
            IsInitialized = true;
        }
        AssetInfo CreateBundleAssetInfo(string bundleName)
        {
            var packageAsset = new PackageAsset()
            {
                Name = bundleName,
                AssetPath = bundleName,
                Extension = string.Empty,
                AssetGuid = bundleName,
                BundleName = bundleName,
            };
            return new AssetInfo(packageName, packageAsset);
        }
    }
}
