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
        public string BundlePath
        {
            get { return bundlePath; }
            set { bundlePath = value; }
        }
        public ResourceBundleWarpper(ResourceBundle resourceBundle)
        {
            this.resourceBundle = resourceBundle;
        }
        public ResourceBundleWarpper(ResourceBundle resourceBundle, string bundlePath)
        {
            this.resourceBundle = resourceBundle;
            this.bundlePath = bundlePath;
        }
        public ResourceBundle ResourceBundle { get { return resourceBundle; } }
        public AssetBundle AssetBundle { get; set; }
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
    }
}
