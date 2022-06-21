using System.Collections.Generic;
using System;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包，AssetBundle；
    /// 存储AssetBundle信息；
    /// </summary>
    [Serializable]
    public class ResourceBundle : IEquatable<ResourceBundle>
    {
        [SerializeField]
        string bundleName;
        [SerializeField]
        List<string> dependList;
        [SerializeField]
        List<ResourceObject> resourceObjectList;
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName { get { return bundleName; } }
        /// <summary>
        /// 资源的依赖项；
        /// </summary>
        public List<string> DependList
        {
            get
            {
                if (dependList == null)
                    dependList = new List<string>();
                return dependList;
            }
        }
        /// <summary>
        /// 资源对象列表；
        /// </summary>
        public List<ResourceObject> ResourceObjectList
        {
            get
            {
                if (resourceObjectList == null)
                    resourceObjectList = new List<ResourceObject>();
                return resourceObjectList;
            }
        }
        public ResourceBundle(string bundleName)
        {
            this.bundleName = bundleName;
        }
        public bool Equals(ResourceBundle other)
        {
            return other.BundleName == this.BundleName;
        }
        public override int GetHashCode()
        {
            return $"{bundleName}".GetHashCode();
        }
    }
}
