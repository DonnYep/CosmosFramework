using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包对象包装者
    /// </summary>
    public class ResourceBundleWarpper
    {
        public ResourceBundleWarpper(ResourceBundle resourceBundle)
        {
            ResourceBundle = resourceBundle;
        }
        public ResourceBundle ResourceBundle { get; private set; }
        public AssetBundle AssetBundle { get; set; }
        public int ReferenceCount { get; set; }
    }
}
