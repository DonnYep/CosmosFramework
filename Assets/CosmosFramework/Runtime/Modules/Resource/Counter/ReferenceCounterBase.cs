using System.Collections.Generic;
using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// 资源对象引用计数器；
    /// </summary>
    internal abstract class ReferenceCounterBase : IDisposable
    {
        /// <summary>
        /// assetName===resourceObjectWarpper
        /// </summary>
        protected readonly Dictionary<string, ResourceObjectWarpper> resourceObjectDict
            = new Dictionary<string, ResourceObjectWarpper>();
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// </summary>
        protected readonly Dictionary<string, ResourceBundleWarpper> resourceBundleDict
            = new Dictionary<string, ResourceBundleWarpper>();

        public bool PeekResourceObjectWarpper(string assetName, out ResourceObjectWarpper objectWarpper)
        {
            return resourceObjectDict.TryGetValue(assetName, out objectWarpper);
        }
        public bool PeekResourceBundleWarpper(string bundleName, out ResourceBundleWarpper bundleWarpper)
        {
            return resourceBundleDict.TryGetValue(bundleName, out bundleWarpper);
        }
        public virtual void Dispose() { }
        /// <summary>
        /// 资产被加载一次；
        /// </summary>
        /// <param name="assetName">资产名</param>
        public virtual void OnLoad(string assetName)
        {
            if (resourceObjectDict.TryGetValue(assetName, out var objectWarpper))
            {
                objectWarpper.ReferenceCount++;
                var bundleName = objectWarpper.ResourceObject.BundleName;
                if (resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper))
                {
                    bundleWarpper.ReferenceCount++;
                }
            }
        }
        /// <summary>
        /// 资产被卸载一次；
        /// </summary>
        /// <param name="assetName">资产名</param>
        public virtual void OnUnload(string assetName)
        {
            if (resourceObjectDict.TryGetValue(assetName, out var objectWarpper))
            {
                objectWarpper.ReferenceCount--;
                var bundleName = objectWarpper.ResourceObject.BundleName;
                if (resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper))
                {
                    bundleWarpper.ReferenceCount--;
                }
            }
        }
        public virtual void OnUnloadAll()
        {
            foreach (var obj in resourceObjectDict.Values)
            {
                obj.ReferenceCount = 0;
            }
            foreach (var bundleWarpper in resourceBundleDict.Values)
            {
                bundleWarpper.ReferenceCount = 0;
            }
        }
    }
}
