using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源收集器。
    /// <para>负责将一个ResourcePackageSO下的所有资产收集为PackageManifest数据，同时完成：</para>
    /// <para>1、GUID去重：同一资源只保留一份，避免重复打进多个包；</para>
    /// <para>2、资源整合：未被显式收集但被显式资产引用的依赖资源，自动归纳到共享依赖包中，供多个包复用；</para>
    /// <para>3、依赖分析：生成包体之间的依赖关系，保证运行时加载顺序正确。</para>
    /// </summary>
    public class ResourceCollector
    {
        /// <summary>
        /// 收集结果
        /// </summary>
        public class CollectResult
        {
            public PackageManifest Manifest = new PackageManifest();
            /// <summary>
            /// assetPath === bundleName
            /// </summary>
            public readonly Dictionary<string, string> AssetBundleMap = new Dictionary<string, string>();
            /// <summary>
            /// 被去重移除的资源路径集合
            /// </summary>
            public readonly List<string> DuplicatedAssets = new List<string>();
            /// <summary>
            /// 收集过程中的警告信息
            /// </summary>
            public readonly List<string> Warnings = new List<string>();
        }

        readonly ResourcePackageSO packageSO;
        /// <summary>
        /// guid === asset
        /// </summary>
        readonly Dictionary<string, PackageAsset> explicitAssetDict = new Dictionary<string, PackageAsset>();
        /// <summary>
        /// assetPath === bundleName
        /// </summary>
        readonly Dictionary<string, string> explicitAssetBundleMap = new Dictionary<string, string>();
        /// <summary>
        /// 共享依赖资产：assetPath === asset
        /// </summary>
        readonly Dictionary<string, PackageAsset> dependAssetDict = new Dictionary<string, PackageAsset>();
        /// <summary>
        /// 共享依赖包：folderKey === bundleName
        /// </summary>
        readonly Dictionary<string, string> sharedBundleByFolderDict = new Dictionary<string, string>();
        /// <summary>
        /// bundleName === 依赖的bundle名集合
        /// </summary>
        readonly Dictionary<string, HashSet<string>> bundleDependencyDict = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// 收集过程中的警告信息
        /// </summary>
        public readonly List<string> Warnings = new List<string>();

        ResourceCollector(ResourcePackageSO packageSO)
        {
            this.packageSO = packageSO;
        }

        /// <summary>
        /// 收集资源包裹数据
        /// </summary>
        public static CollectResult Collect(ResourcePackageSO packageSO)
        {
            if (packageSO == null)
                return null;
            var collector = new ResourceCollector(packageSO);
            return collector.CollectInternal();
        }

        CollectResult CollectInternal()
        {
            var result = new CollectResult();
            result.Manifest.PackageName = packageSO.PackageName;
            result.Manifest.PackageVersion = packageSO.PackageVersion;
            // 1、收集显式资产
            CollectExplicitAssets(result);
            // 2、收集共享依赖资产
            CollectSharedDependencies(result);
            // 3、构建包体信息
            BuildBundleInfos(result);
            // 同步警告信息
            if (Warnings.Count > 0)
                result.Warnings.AddRange(Warnings);
            return result;
        }
        void CollectExplicitAssets(CollectResult result)
        {
            var bundleSOList = packageSO.BundleSOList;
            var bundleCount = bundleSOList.Count;
            for (int i = 0; i < bundleCount; i++)
            {
                var bundleSO = bundleSOList[i];
                if (bundleSO == null)
                    continue;
                var bundleName = ResolveBundleName(bundleSO);
                var assetEntries = bundleSO.GetAssetEntries();
                var entryCount = assetEntries.Count;
                for (int j = 0; j < entryCount; j++)
                {
                    var entry = assetEntries[j];
                    if (string.IsNullOrEmpty(entry.Guid))
                        continue;
                    var assetPath = AssetDatabase.GUIDToAssetPath(entry.Guid);
                    if (string.IsNullOrEmpty(assetPath))
                    {
                        Warnings.Add($"Asset entry guid {entry.Guid} not found in AssetDatabase !");
                        continue;
                    }
                    if (explicitAssetDict.ContainsKey(entry.Guid))
                    {
                        // GUID去重：同一资源只保留第一个包内的引用
                        result.DuplicatedAssets.Add(assetPath);
                        Warnings.Add($"Duplicate asset detected : {assetPath} , it has been removed from bundle : {bundleName}");
                        continue;
                    }
                    var packageAsset = CreatePackageAsset(entry, assetPath, bundleName, bundleSO.PackSeparately);
                    explicitAssetDict.Add(entry.Guid, packageAsset);
                    explicitAssetBundleMap[assetPath] = packageAsset.BundleName;
                    result.AssetBundleMap[assetPath] = packageAsset.BundleName;
                    result.Manifest.AssetList.Add(packageAsset);
                }
            }
        }

        void CollectSharedDependencies(CollectResult result)
        {
            // 对所有显式资产做依赖分析
            var explicitAssets = new List<PackageAsset>(explicitAssetDict.Values);
            var assetCount = explicitAssets.Count;
            for (int i = 0; i < assetCount; i++)
            {
                var asset = explicitAssets[i];
                var dependencies = AssetDatabase.GetDependencies(asset.AssetPath, true);
                var depCount = dependencies.Length;
                for (int j = 0; j < depCount; j++)
                {
                    var depPath = dependencies[j];
                    if (string.IsNullOrEmpty(depPath))
                        continue;
                    if (depPath == asset.AssetPath)
                        continue;
                    if (!depPath.StartsWith("Assets/"))
                        continue;
                    // 已是显式资产，跳过
                    if (explicitAssetBundleMap.ContainsKey(depPath))
                        continue;
                    // 非可收集资源（如脚本），跳过
                    if (!IsCollectableAsset(depPath))
                        continue;
                    // 共享依赖去重
                    if (dependAssetDict.ContainsKey(depPath))
                        continue;
                    var sharedBundleName = GetSharedBundleName(depPath);
                    var packageAsset = new PackageAsset()
                    {
                        Name = Path.GetFileNameWithoutExtension(depPath),
                        AssetPath = depPath,
                        Extension = Path.GetExtension(depPath).ToLower(),
                        AssetGuid = AssetDatabase.AssetPathToGUID(depPath),
                        BundleName = sharedBundleName,
                    };
                    dependAssetDict.Add(depPath, packageAsset);
                    result.AssetBundleMap[depPath] = sharedBundleName;
                    result.Manifest.AssetList.Add(packageAsset);
                }
            }
            // 处理共享依赖包之间的依赖关系
            ResolveSharedDependencies();
        }

        void ResolveSharedDependencies()
        {
            var sharedAssets = new List<PackageAsset>(dependAssetDict.Values);
            var count = sharedAssets.Count;
            for (int i = 0; i < count; i++)
            {
                var asset = sharedAssets[i];
                var dependencies = AssetDatabase.GetDependencies(asset.AssetPath, false);
                var depCount = dependencies.Length;
                for (int j = 0; j < depCount; j++)
                {
                    var depPath = dependencies[j];
                    if (depPath == asset.AssetPath)
                        continue;
                    if (!dependAssetDict.TryGetValue(depPath, out var depAsset))
                        continue;
                    var srcBundle = asset.BundleName;
                    var dstBundle = depAsset.BundleName;
                    if (srcBundle == dstBundle)
                        continue;
                    if (IsReachable(dstBundle, srcBundle))
                        continue;
                    AddBundleDependency(srcBundle, dstBundle);
                }
            }
        }
        void BuildBundleInfos(CollectResult result)
        {
            // 显式包体
            var bundleSOList = packageSO.BundleSOList;
            var bundleCount = bundleSOList.Count;
            for (int i = 0; i < bundleCount; i++)
            {
                var bundleSO = bundleSOList[i];
                if (bundleSO == null)
                    continue;
                var bundleName = ResolveBundleName(bundleSO);
                result.Manifest.BundleList.Add(CreateBundleInfo(bundleName));
            }
            // 共享依赖包体
            foreach (var kv in sharedBundleByFolderDict)
            {
                result.Manifest.BundleList.Add(CreateBundleInfo(kv.Value));
            }
        }

        PackageBundle CreateBundleInfo(string bundleName)
        {
            var dependSet = GetBundleDependencies(bundleName);
            var dependBundles = new string[dependSet.Count];
            dependSet.CopyTo(dependBundles);
            return new PackageBundle()
            {
                BundleName = bundleName,
                DependBundles = dependBundles,
            };
        }

        string ResolveBundleName(PackageBundleSO bundleSO)
        {
            var name = bundleSO.BundleName;
            if (string.IsNullOrEmpty(name))
                name = bundleSO.name;
            if (string.IsNullOrEmpty(name))
                name = "UnnamedBundle";
            return name;
        }

        PackageAsset CreatePackageAsset(AssetEntry entry, string assetPath, string bundleName, bool packSeparately)
        {
            var finalBundleName = bundleName;
            if (packSeparately)
            {
                // 每个资产独立成包
                var assetName = Path.GetFileNameWithoutExtension(assetPath);
                finalBundleName = $"{bundleName}_{ResourceUtility.FilterName(assetName)}";
            }
            return new PackageAsset()
            {
                Name = entry.Name,
                AssetPath = assetPath,
                Extension = entry.Extension,
                AssetGuid = entry.Guid,
                AssetTags = entry.AssetTags,
                BundleName = finalBundleName,
            };
        }

        string GetSharedBundleName(string assetPath)
        {
            var folder = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(folder))
                folder = "Assets";
            if (sharedBundleByFolderDict.TryGetValue(folder, out var bundleName))
                return bundleName;
            bundleName = $"{ResourceEditorConstants.SHARED_DEPENDENCY_BUNDLE_PREFIX}{ResourceUtility.FilterName(folder)}";
            sharedBundleByFolderDict.Add(folder, bundleName);
            return bundleName;
        }

        void AddBundleDependency(string srcBundle, string dstBundle)
        {
            if (!bundleDependencyDict.TryGetValue(srcBundle, out var set))
            {
                set = new HashSet<string>();
                bundleDependencyDict.Add(srcBundle, set);
            }
            set.Add(dstBundle);
        }

        HashSet<string> GetBundleDependencies(string bundleName)
        {
            if (bundleDependencyDict.TryGetValue(bundleName, out var set))
                return set;
            return new HashSet<string>();
        }

        /// <summary>
        /// 判断source是否可达target（用于避免依赖环）
        /// </summary>
        bool IsReachable(string source, string target)
        {
            if (!bundleDependencyDict.TryGetValue(source, out var dependSet))
                return false;
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(source);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (!visited.Add(node))
                    continue;
                if (!bundleDependencyDict.TryGetValue(node, out var nodeDeps))
                    continue;
                foreach (var dep in nodeDeps)
                {
                    if (dep == target)
                        return true;
                    queue.Enqueue(dep);
                }
            }
            return false;
        }

        static bool IsCollectableAsset(string assetPath)
        {
            var extension = Path.GetExtension(assetPath).ToLower();
            var extensions = ResourceEditorConstants.Extensions;
            var length = extensions.Length;
            for (int i = 0; i < length; i++)
            {
                if (extensions[i] == extension)
                    return true;
            }
            return false;
        }
    }
}
