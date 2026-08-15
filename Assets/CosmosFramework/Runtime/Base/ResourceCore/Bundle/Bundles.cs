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
            if (string.IsNullOrEmpty(bundleName))
                return null;
            if (loaderMap.TryGetValue(bundleName, out var loader))
            {
                loader.AddRef();
                return loader;
            }
            var bundleInfo = package.GetManifest()?.GetBundle(bundleName);
            var filePath = ResourceUtility.GetBundleFilePath(package.GetInitParameters(), bundleName);
            var newLoader = new BundleLoader(bundleName, filePath);
            loaderMap.Add(bundleName, newLoader);
            newLoader.AddRef();
            // 先加载依赖包
            if (bundleInfo != null && bundleInfo.DependBundles != null)
            {
                var dependLength = bundleInfo.DependBundles.Length;
                for (int i = 0; i < dependLength; i++)
                {
                    var depLoader = LoadBundle(bundleInfo.DependBundles[i]);
                    if (depLoader != null)
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
