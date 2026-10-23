using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源系统静态门面。
    /// <para>提供与Addressables类似的handle式资源加载API，并支持多资源包裹（分包）。</para>
    /// <code>
    /// // 初始化（默认包）
    /// var initHandle = CFResources.InitializeAsync(new ResourceInitParameters() { PlayMode = ..., ManifestPath = ..., BundleDirectory = ... });
    /// // 分包：再加载一个可独立下载/卸载的包
    /// var uiInit = CFResources.LoadPackageAsync(new ResourceInitParameters() { PackageName = "ui", ManifestPath = ..., BundleDirectory = ... });
    /// // 异步加载资产
    /// var handle = CFResources.LoadAssetAsync&lt;GameObject&gt;("Assets/Prefabs/Cube.prefab");
    /// handle.Completed += h =&gt; { var go = h.Asset; };
    /// // 批量加载（同一包体只加载一次）
    /// var batch = CFResources.LoadAssetsAsync&lt;GameObject&gt;(new[] { "Prefabs/A", "Prefabs/B" });
    /// // 或协程等待 / await
    /// yield return handle;
    /// var cube = await CFResources.LoadAssetAsync&lt;GameObject&gt;("Cube").Task;
    /// </code>
    /// </summary>
    public static class CFResources
    {
        static readonly Dictionary<string, ResourcePackage> packages = new Dictionary<string, ResourcePackage>();
        static ResourcePackage defaultPackage;

        /// <summary>
        /// 当前默认的资源包裹
        /// </summary>
        public static ResourcePackage DefaultPackage
        {
            get { return defaultPackage; }
        }
        /// <summary>
        /// 已注册的全部资源包裹（分包）
        /// </summary>
        public static IReadOnlyDictionary<string, ResourcePackage> Packages
        {
            get { return packages; }
        }
        /// <summary>
        /// 默认包裹的初始化参数
        /// </summary>
        public static ResourceInitParameters InitParameters
        {
            get { return defaultPackage != null ? defaultPackage.GetInitParameters() : null; }
        }
        /// <summary>
        /// 当前运行模式（默认包裹）
        /// </summary>
        public static CFResourcePlayMode ResourcePlayMode
        {
            get
            {
                var parameters = InitParameters;
                if (parameters == null)
                    return CFResourcePlayMode.None;
                return parameters.PlayMode;
            }
        }
        /// <summary>
        /// 默认包裹是否初始化完成
        /// </summary>
        public static bool IsInitialized
        {
            get { return defaultPackage != null && defaultPackage.IsInitialized; }
        }

        /// <summary>
        /// 异步初始化资源系统（同时把该包裹设为默认包裹）。
        /// <para>重复调用同名包裹会先重置再重新初始化。</para>
        /// </summary>
        public static InitializationHandle InitializeAsync(ResourceInitParameters parameters)
        {
            var handle = LoadPackageAsync(parameters);
            defaultPackage = packages[parameters.PackageName];
            return handle;
        }

        /// <summary>
        /// 异步加载一个资源包裹（分包）。
        /// <para>每个包裹是独立单元：独立清单、独立包体目录、独立卸载；第一次加载的包裹会成为默认包裹。</para>
        /// </summary>
        public static InitializationHandle LoadPackageAsync(ResourceInitParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("ResourceInitParameters is invalid !");
            if (string.IsNullOrEmpty(parameters.PackageName))
                parameters.PackageName = CFResourceConstants.DEFAULT_PACKAGE_NAME;
            ResourcePackage package;
            if (packages.TryGetValue(parameters.PackageName, out package))
            {
                package.Reset();
            }
            else
            {
                package = new ResourcePackage(parameters.PackageName);
                packages.Add(parameters.PackageName, package);
            }
            // 包体加载预算按最新参数生效
            ResourceLoadBudget.MaxConcurrentBundleLoads = parameters.MaxConcurrentBundleLoads;
            ResourceLoadBudget.Reset();
            if (defaultPackage == null)
                defaultPackage = package;
            return package.InitializeAsync(parameters);
        }

        /// <summary>
        /// 获取已注册的资源包裹
        /// </summary>
        public static ResourcePackage GetPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return null;
            packages.TryGetValue(packageName, out var package);
            return package;
        }
        /// <summary>
        /// 尝试获取已注册的资源包裹
        /// </summary>
        public static bool TryGetPackage(string packageName, out ResourcePackage package)
        {
            package = null;
            if (string.IsNullOrEmpty(packageName))
                return false;
            return packages.TryGetValue(packageName, out package);
        }
        /// <summary>
        /// 卸载并移除一个资源包裹（会先释放该包内全部资产与包体）
        /// </summary>
        public static void UnloadPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;
            if (!packages.TryGetValue(packageName, out var package))
                return;
            packages.Remove(packageName);
            if (defaultPackage == package)
                defaultPackage = null;
            package.Reset();
        }
        /// <summary>
        /// 卸载全部资源包裹
        /// </summary>
        public static void UnloadAllPackages()
        {
            var names = new List<string>(packages.Keys);
            for (int i = 0; i < names.Count; i++)
            {
                UnloadPackage(names[i]);
            }
            packages.Clear();
            defaultPackage = null;
        }

        #region 加载（默认包裹）
        /// <summary>
        /// 异步加载资产
        /// </summary>
        public static AssetHandle<T> LoadAssetAsync<T>(string assetKey)
            where T : Object
        {
            return ResolvePackage(null).LoadAssetAsync<T>(assetKey);
        }
        /// <summary>
        /// 异步加载资产及其子资产
        /// </summary>
        public static SubAssetsHandle<T> LoadSubAssetsAsync<T>(string assetKey)
            where T : Object
        {
            return ResolvePackage(null).LoadSubAssetsAsync<T>(assetKey);
        }
        /// <summary>
        /// 异步加载资源包内全部资产
        /// </summary>
        public static AllAssetsHandle<T> LoadAllAssetsAsync<T>(string bundleName)
            where T : Object
        {
            return ResolvePackage(null).LoadAllAssetsAsync<T>(bundleName);
        }
        /// <summary>
        /// 异步加载场景
        /// </summary>
        public static SceneHandle LoadSceneAsync(string sceneKey, bool additive = true, bool activateOnLoad = true)
        {
            return ResolvePackage(null).LoadSceneAsync(sceneKey, additive, activateOnLoad);
        }
        /// <summary>
        /// 批量异步加载资产（同一包体只加载一次）
        /// </summary>
        public static AssetsHandle<T> LoadAssetsAsync<T>(IEnumerable<string> assetKeys)
            where T : Object
        {
            return ResolvePackage(null).LoadAssetsAsync<T>(assetKeys);
        }
        /// <summary>
        /// 通过分类标签异步加载资产集合
        /// </summary>
        public static AssetsHandle<T> LoadAssetsByTagAsync<T>(string tag)
            where T : Object
        {
            return ResolvePackage(null).LoadAssetsByTagAsync<T>(tag);
        }
        #endregion

        #region 加载（指定包裹）
        /// <summary>
        /// 从指定包裹异步加载资产
        /// </summary>
        public static AssetHandle<T> LoadAssetAsync<T>(string packageName, string assetKey)
            where T : Object
        {
            return ResolvePackage(packageName).LoadAssetAsync<T>(assetKey);
        }
        /// <summary>
        /// 从指定包裹异步加载资产及其子资产
        /// </summary>
        public static SubAssetsHandle<T> LoadSubAssetsAsync<T>(string packageName, string assetKey)
            where T : Object
        {
            return ResolvePackage(packageName).LoadSubAssetsAsync<T>(assetKey);
        }
        /// <summary>
        /// 从指定包裹异步加载全部资产
        /// </summary>
        public static AllAssetsHandle<T> LoadAllAssetsAsync<T>(string packageName, string bundleName)
            where T : Object
        {
            return ResolvePackage(packageName).LoadAllAssetsAsync<T>(bundleName);
        }
        /// <summary>
        /// 从指定包裹异步加载场景
        /// </summary>
        public static SceneHandle LoadSceneAsync(string packageName, string sceneKey, bool additive = true, bool activateOnLoad = true)
        {
            return ResolvePackage(packageName).LoadSceneAsync(sceneKey, additive, activateOnLoad);
        }
        /// <summary>
        /// 从指定包裹批量异步加载资产
        /// </summary>
        public static AssetsHandle<T> LoadAssetsAsync<T>(string packageName, IEnumerable<string> assetKeys)
            where T : Object
        {
            return ResolvePackage(packageName).LoadAssetsAsync<T>(assetKeys);
        }
        /// <summary>
        /// 从指定包裹按分类标签异步加载资产集合
        /// </summary>
        public static AssetsHandle<T> LoadAssetsByTagAsync<T>(string packageName, string tag)
            where T : Object
        {
            return ResolvePackage(packageName).LoadAssetsByTagAsync<T>(tag);
        }
        #endregion

        /// <summary>
        /// 卸载场景
        /// </summary>
        public static void UnloadSceneAsync(SceneHandle handle)
        {
            handle?.Release();
        }
        /// <summary>
        /// 释放资产句柄
        /// </summary>
        public static void UnloadAsset(HandleBase handle)
        {
            handle?.Release();
        }
        /// <summary>
        /// 释放批量句柄（等同于 handle.Release()）
        /// </summary>
        public static void UnloadAssets<T>(AssetsHandle<T> handle)
            where T : Object
        {
            handle?.Release();
        }
        /// <summary>
        /// 释放默认包裹的全部资产与包体
        /// </summary>
        public static void UnloadAllAssets()
        {
            if (defaultPackage == null)
                return;
            defaultPackage.UnloadAllAssets();
        }
        /// <summary>
        /// 获取资产信息（默认包裹）
        /// </summary>
        public static AssetInfo GetAssetInfo(string assetKey)
        {
            return ResolvePackage(null).GetAssetInfo(assetKey);
        }
        /// <summary>
        /// 通过分类标签获取资产信息集合（默认包裹）
        /// </summary>
        public static AssetInfo[] GetAssetInfosByTag(string tag)
        {
            return ResolvePackage(null).GetAssetInfosByTag(tag);
        }

        static ResourcePackage ResolvePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                if (defaultPackage == null || !defaultPackage.IsInitialized)
                    throw new InvalidOperationException("CFResources is not initialized, call InitializeAsync first !");
                return defaultPackage;
            }
            if (!packages.TryGetValue(packageName, out var package))
                throw new InvalidOperationException($"Resource package not found : {packageName} !");
            if (!package.IsInitialized)
                throw new InvalidOperationException($"Resource package is not initialized : {packageName} !");
            return package;
        }

        #region Diagnostics
        /// <summary>
        /// 开启/关闭资源诊断（默认编辑器开启，真机默认关闭）
        /// </summary>
        public static void SetDiagnosticsEnabled(bool value)
        {
            ResourceDiagnostics.SetEnabled(value);
        }
        /// <summary>
        /// 诊断是否开启
        /// </summary>
        public static bool IsDiagnosticsEnabled
        {
            get { return ResourceDiagnostics.Enabled; }
        }
        /// <summary>
        /// 开启/关闭加载调用栈捕获（定位泄漏源头，有少量开销）
        /// </summary>
        public static void SetDiagnosticsCaptureStackTrace(bool value)
        {
            ResourceDiagnostics.CaptureStackTrace = value;
        }
        /// <summary>
        /// 清空诊断记录
        /// </summary>
        public static void ClearDiagnosticsRecords()
        {
            ResourceDiagnostics.ClearRecords();
        }
        /// <summary>
        /// 获取历史加载记录
        /// </summary>
        public static System.Collections.Generic.IReadOnlyList<ResourceLoadRecord> GetLoadRecords()
        {
            return ResourceDiagnostics.Records;
        }
        /// <summary>
        /// 获取失败的加载记录
        /// </summary>
        public static System.Collections.Generic.List<ResourceLoadRecord> GetFailedLoadRecords()
        {
            return ResourceDiagnostics.GetFailedRecords();
        }
        /// <summary>
        /// 获取潜在泄漏记录（句柄未释放的已加载资产）
        /// </summary>
        public static System.Collections.Generic.List<ResourceLoadRecord> GetLeakedRecords()
        {
            return ResourceDiagnostics.GetLeakedRecords();
        }
        /// <summary>
        /// 获取当前运行时加载状态（调试窗口用）
        /// </summary>
        public static ResourceRuntimeInfo GetRuntimeInfo()
        {
            var info = new ResourceRuntimeInfo();
            if (defaultPackage != null)
            {
                info.LoadedBundleCount = defaultPackage.LoadedBundleCount;
                var providers = defaultPackage.GetActiveProviders();
                foreach (var provider in providers)
                {
                    if (provider == null || provider.IsDestroyed)
                        continue;
                    var assetInfo = new LoadedAssetDebugInfo()
                    {
                        AssetPath = provider.MainAssetInfo?.AssetPath,
                        AssetName = provider.MainAssetInfo?.Name,
                        BundleName = provider.MainAssetInfo?.BundleName,
                        AssetTypeName = provider.AssetType?.Name,
                        LoadMode = provider.LoadMode.ToString(),
                        RefCount = provider.RefCount,
                        LoadDuration = provider.LoadDurationSeconds,
                        Succeeded = provider.IsDone && provider.Status == OperationStatus.Succeeded,
                        Error = provider.Error,
                    };
                    if (provider.AssetObject != null)
                        assetInfo.MemorySize = ResourceUtility.GetRuntimeMemorySizeLong(provider.AssetObject);
                    else if (provider.AllAssetObjects != null)
                    {
                        long memory = 0;
                        for (int i = 0; i < provider.AllAssetObjects.Length; i++)
                        {
                            if (provider.AllAssetObjects[i] != null)
                                memory += ResourceUtility.GetRuntimeMemorySizeLong(provider.AllAssetObjects[i]);
                        }
                        assetInfo.MemorySize = memory;
                    }
                    info.LoadedAssets.Add(assetInfo);
                }
            }
            info.ActiveProviderCount = ResourceDiagnostics.ActiveProviderCount;
            info.TotalLoadCount = ResourceDiagnostics.TotalLoadCount;
            info.FailedLoadCount = ResourceDiagnostics.FailedLoadCount;
            info.RecordCount = ResourceDiagnostics.Records.Count;
            return info;
        }
        /// <summary>
        /// 生成诊断摘要文本
        /// </summary>
        public static string GenerateDiagnosticsReport()
        {
            return ResourceDiagnostics.GenerateReport();
        }
        #endregion
    }
}
