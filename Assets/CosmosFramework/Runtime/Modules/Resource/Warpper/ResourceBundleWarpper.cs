using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包对象包装者
    /// </summary>
    public class ResourceBundleWarpper
    {
        int referenceCount;
        ResourceBundle resourceBundle;
        public ResourceBundleWarpper(ResourceBundle resourceBundle)
        {
            this.resourceBundle = resourceBundle;
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
