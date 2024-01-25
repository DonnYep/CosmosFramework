using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包对象包装者
    /// </summary>
    internal class ResourceBundleWarpper
    {
        int referenceCount;
        ResourceBundle resourceBundle;
        string bundlePath;
        string bundleExtension;
        ulong bundleOffset;
        /// <summary>
        /// 包体所在的根目录
        /// </summary>
        public string BundlePath
        {
            get { return bundlePath; }
        }
        /// <summary>
        /// AB的后缀
        /// </summary>
        public string BundleExtension
        {
            get { return bundleExtension; }
        }
        /// <summary>
        /// 包体的信息
        /// </summary>
        public ResourceBundle ResourceBundle
        {
            get { return resourceBundle; }
        }
        public AssetBundle AssetBundle { get; set; }
        /// <summary>
        /// 包体的引用计数
        /// </summary>
        public int ReferenceCount
        {
            get { return referenceCount; }
            set
            {
                referenceCount = value;
                if (referenceCount < 0)
                    referenceCount = 0;
            }
        }
        /// <summary>
        /// 包体的偏移量
        /// </summary>
        public ulong BundleOffset
        {
            get { return bundleOffset; }
        }

        public ResourceBundleWarpper(ResourceBundle resourceBundle)
        {
            this.resourceBundle = resourceBundle;
        }
        public ResourceBundleWarpper(ResourceBundle resourceBundle, string bundlePath, string bundleExtension, ulong bundleOffset)
        {
            this.resourceBundle = resourceBundle;
            this.bundlePath = bundlePath;
            this.bundleExtension = bundleExtension;
            this.bundleOffset = bundleOffset;
        }
    }
}
