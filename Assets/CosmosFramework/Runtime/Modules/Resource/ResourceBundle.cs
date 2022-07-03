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
        [SerializeField]
        long bundleSize;
        [SerializeField]
        string bundlePath;
        [SerializeField]
        string bundleHash;
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName { get { return bundleName; } set { bundleName = value; } }
        /// <summary>
        /// AsseBundle的Hash；
        /// 这里采用bundle地址内涵的文件bytes生成；
        /// </summary>
        public string BundleHash { get { return bundleHash; } set { bundleHash = value; } }
        /// <summary>
        /// AB包在Assets目录下的地址；
        /// </summary>
        public string BundlePath { get { return bundlePath; } }
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
        /// <summary>
        ///  包体数据数据长度，用于验证数据完整性；
        /// </summary>
        public long BundleSize
        {
            get { return bundleSize; }
            set
            {
                bundleSize = value;
                if (bundleSize < 0)
                    bundleSize = 0;
            }
        }
        public ResourceBundle(string bundleName)
        {
            this.bundleName = bundleName;
        }
        public ResourceBundle(string bundleName, string bundlePath)
        {
            this.bundleName = bundleName;
            this.bundlePath = bundlePath;
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
