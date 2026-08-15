using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源系统静态门面。
    /// <para>提供与Addressables类似的handle式资源加载API。</para>
    /// <code>
    /// // 初始化
    /// var initHandle = CFResources.InitializeAsync(new ResourceInitParameters() { PlayMode = ..., ManifestPath = ..., BundleDirectory = ... });
    /// // 异步加载资产
    /// var handle = CFResources.LoadAssetAsync&lt;GameObject&gt;("Assets/Prefabs/Cube.prefab");
    /// handle.Completed += h =&gt; { var go = h.Asset; };
    /// // 或协程等待 / await
    /// yield return handle;
    /// var cube = await CFResources.LoadAssetAsync&lt;GameObject&gt;("Cube").Task;
    /// </code>
    /// </summary>
    public static class CFResources
    {
        static ResourcePackage defaultPackage;
        static ResourceInitParameters initParameters;

        /// <summary>
        /// 当前默认的资源包裹
        /// </summary>
        public static ResourcePackage DefaultPackage
        {
            get { return defaultPackage; }
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        public static ResourceInitParameters InitParameters
        {
            get { return initParameters; }
        }
        /// <summary>
        /// 当前运行模式
        /// </summary>
        public static CFResourcePlayMode ResourcePlayMode
        {
            get
            {
                if (initParameters == null)
                    return CFResourcePlayMode.None;
                return initParameters.PlayMode;
            }
        }
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public static bool IsInitialized
        {
            get { return defaultPackage != null && defaultPackage.IsInitialized; }
        }

        /// <summary>
        /// 异步初始化资源系统
        /// </summary>
        public static InitializationHandle InitializeAsync(ResourceInitParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("ResourceInitParameters is invalid !");
            initParameters = parameters;
            if (defaultPackage != null)
                defaultPackage.Reset();
            defaultPackage = new ResourcePackage(parameters.PackageName);
            return defaultPackage.InitializeAsync(parameters);
        }

        /// <summary>
        /// 异步加载资产
        /// </summary>
        public static AssetHandle<T> LoadAssetAsync<T>(string assetKey)
            where T : Object
        {
            CheckInitialized();
            return defaultPackage.LoadAssetAsync<T>(assetKey);
        }
        /// <summary>
        /// 异步加载资产及其子资产
        /// </summary>
        public static SubAssetsHandle<T> LoadSubAssetsAsync<T>(string assetKey)
            where T : Object
        {
            CheckInitialized();
            return defaultPackage.LoadSubAssetsAsync<T>(assetKey);
        }
        /// <summary>
        /// 异步加载资源包内全部资产
        /// </summary>
        public static AllAssetsHandle<T> LoadAllAssetsAsync<T>(string bundleName)
            where T : Object
        {
            CheckInitialized();
            return defaultPackage.LoadAllAssetsAsync<T>(bundleName);
        }
        /// <summary>
        /// 异步加载场景
        /// </summary>
        public static SceneHandle LoadSceneAsync(string sceneKey, bool additive = true, bool activateOnLoad = true)
        {
            CheckInitialized();
            return defaultPackage.LoadSceneAsync(sceneKey, additive, activateOnLoad);
        }
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
        /// 释放全部资产与包体
        /// </summary>
        public static void UnloadAllAssets()
        {
            if (defaultPackage == null)
                return;
            defaultPackage.UnloadAllAssets();
        }
        /// <summary>
        /// 获取资产信息
        /// </summary>
        public static AssetInfo GetAssetInfo(string assetKey)
        {
            CheckInitialized();
            return defaultPackage.GetAssetInfo(assetKey);
        }
        /// <summary>
        /// 通过分类标签获取资产信息集合
        /// </summary>
        public static AssetInfo[] GetAssetInfosByTag(string tag)
        {
            CheckInitialized();
            return defaultPackage.GetAssetInfosByTag(tag);
        }
        /// <summary>
        /// 通过分类标签异步加载资产集合（Addressables Label 风格）。
        /// <para>同一标签下的资产会逐个异步加载，全部完成后句柄完成；单个失败不影响其余资产。</para>
        /// </summary>
        public static TagAssetsHandle<T> LoadAssetsByTagAsync<T>(string tag)
            where T : Object
        {
            CheckInitialized();
            return defaultPackage.LoadAssetsByTagAsync<T>(tag);
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

        static void CheckInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("CFResources is not initialized, call InitializeAsync first !");
        }
    }
}
