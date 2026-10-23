using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包加载器集合。
    /// <para>以bundle名称为键，维护整个资源包裹内所有包体的加载与卸载。</para>
    /// </summary>
    internal class Bundles
    {
        readonly Dictionary<string, BundleLoader> loaderMap = new Dictionary<string, BundleLoader>();
        readonly ResourcePackage package;

        public int Count
        {
            get { return loaderMap.Count; }
        }
        public IEnumerable<BundleLoader> Loaders
        {
            get { return loaderMap.Values; }
        }

        public Bundles(ResourcePackage package)
        {
            this.package = package;
        }

        /// <summary>
        /// 加载资源包，并自动递归加载其依赖包。
        /// <para>若资源包正在加载或已经加载，则复用已有加载器，仅增加引用计数。</para>
        /// </summary>
        public BundleLoader LoadBundle(string bundleName)
        {
            return GetOrCreateLoader(bundleName, true);
        }

        /// <summary>
        /// 获取或创建包体加载器。
        /// <para>addRefForCaller 为 true 时表示调用方持有一个引用（由调用方负责 Release）；</para>
        /// <para>false 用于依赖加载：依赖引用由 AddDependency 统一增加，避免引用计数被重复累加。</para>
        /// </summary>
        BundleLoader GetOrCreateLoader(string bundleName, bool addRefForCaller)
        {
            if (string.IsNullOrEmpty(bundleName))
                return null;
            if (loaderMap.TryGetValue(bundleName, out var loader))
            {
                if (addRefForCaller)
                    loader.AddRef();
                return loader;
            }
            var bundleInfo = package.GetManifest()?.GetBundle(bundleName);
            var filePath = ResourceUtility.GetBundleFilePath(package.GetInitParameters(), bundleName);
            var newLoader = new BundleLoader(bundleName, filePath);
            loaderMap.Add(bundleName, newLoader);
            if (addRefForCaller)
                newLoader.AddRef();
            // 先建立依赖关系（依赖加载器会并行加载），再启动自身
            if (bundleInfo != null && bundleInfo.DependBundles != null)
            {
                var dependLength = bundleInfo.DependBundles.Length;
                for (int i = 0; i < dependLength; i++)
                {
                    var depLoader = GetOrCreateLoader(bundleInfo.DependBundles[i], false);
                    if (depLoader == null || depLoader == newLoader)
                        continue;
                    newLoader.AddDependency(depLoader);
                }
            }
            package.InternalStartOperation(newLoader);
            return newLoader;
        }

        /// <summary>
        /// 释放资源包加载器，引用计数归零时卸载包体并递归释放依赖
        /// </summary>
        public void UnloadBundle(BundleLoader loader)
        {
            if (loader == null)
                return;
            loader.Release();
            if (loader.RefCount > 0)
                return;
            loaderMap.Remove(loader.BundleName);
            var dependLoaders = loader.DependLoaders;
            for (int i = 0; i < dependLoaders.Count; i++)
            {
                UnloadBundle(dependLoaders[i]);
            }
            loader.Destroy();
        }

        /// <summary>
        /// 卸载全部资源包
        /// </summary>
        public void UnloadAll()
        {
            var loaders = new List<BundleLoader>(loaderMap.Values);
            loaderMap.Clear();
            var count = loaders.Count;
            for (int i = 0; i < count; i++)
            {
                loaders[i].Destroy();
            }
        }
    }
}
