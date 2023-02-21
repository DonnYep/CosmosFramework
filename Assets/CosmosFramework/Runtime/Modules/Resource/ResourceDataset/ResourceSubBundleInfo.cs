using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Resource
{
    [Serializable]
    public class ResourceSubBundleInfo : IEquatable<ResourceSubBundleInfo>
    {
        [SerializeField]
        string bundleName;
        [SerializeField]
        string bundlePath;
        [SerializeField]
        string bundleKey;
        [SerializeField]
        long bundleSize;
        [SerializeField]
        string bundleFormatBytes;
        [SerializeField]
        List<ResourceObjectInfo> resourceObjectInfoList = new List<ResourceObjectInfo>();
        [SerializeField]
        List<string> dependentBundleKeyList = new List<string>();
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName
        {
            get { return bundleName; }
            set
            {
                var srcValue = value;
                if (!string.IsNullOrEmpty(srcValue))
                {
                    srcValue = ResourceUtility.FilterName(srcValue);
                }
                bundleName = srcValue;
            }
        }
        /// <summary>
        /// AB包在Assets目录下的地址；
        /// </summary>
        public string BundlePath
        {
            get { return bundlePath; }
            set { bundlePath = value; }
        }
        /// <summary>
        /// AB加载时候使用的名称；
        /// </summary>
        public string BundleKey
        {
            get { return bundleKey; }
            set { bundleKey = value; }
        }
        /// <summary>
        /// Bundle尺寸；
        /// </summary>
        public long BundleSize
        {
            get { return bundleSize; }
            set { bundleSize = value; }
        }
        /// <summary>
        /// Bundle比特文件大小；
        /// </summary>
        public string BundleFormatBytes
        {
            get { return bundleFormatBytes; }
            set { bundleFormatBytes = value; }
        }
        /// <summary>
        /// 资源对象列表；
        /// </summary>
        public List<ResourceObjectInfo> ResourceObjectInfoList
        {
            get
            {
                if (resourceObjectInfoList == null)
                    resourceObjectInfoList = new List<ResourceObjectInfo>();
                return resourceObjectInfoList;
            }
            set { resourceObjectInfoList = value; }
        }
        /// <summary>
        /// 资源的依赖项；
        /// </summary>
        public List<string> DependentBundleKeyList
        {
            get
            {
                if (dependentBundleKeyList == null)
                    dependentBundleKeyList = new List<string>();
                return dependentBundleKeyList;
            }
            set { dependentBundleKeyList = value; }
        }
        public bool Equals(ResourceSubBundleInfo other)
        {
            return other.BundleName == this.BundleName ||
                other.BundlePath == this.BundlePath;
        }
    }
}
