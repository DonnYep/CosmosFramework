using System.Collections.Generic;
using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包，AssetBundle；
    /// 存储AssetBundle信息；
    /// </summary>
    [Serializable]
    public class ResourceBundle : IEquatable<ResourceBundle>
    {
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName;
        /// <summary>
        /// 资源的依赖项；
        /// </summary>
        public IList<string> DependList;
        /// <summary>
        /// 资源对象列表；
        /// </summary>
        public List<ResourceObject> ResourceObjectList;

        public bool Equals(ResourceBundle other)
        {
            return other.BundleName == this.BundleName;
        }
    }
}
