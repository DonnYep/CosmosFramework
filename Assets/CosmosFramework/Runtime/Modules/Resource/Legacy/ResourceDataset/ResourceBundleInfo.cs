using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Resource
{
    [Serializable]
    public class ResourceBundleInfo : IEquatable<ResourceBundleInfo>
    {
        [SerializeField]
        string bundleName;
        [SerializeField]
        string bundleKey;
        [SerializeField]
        long bundleSize;
        [SerializeField]
        string bundleFormatBytes;
        [SerializeField]
        List<ResourceObjectInfo> resourceObjectInfoList = new List<ResourceObjectInfo>();
        [SerializeField]
        List<BundleDependencyInfo> bundleDependencies = new List<BundleDependencyInfo>();
        [SerializeField]
        List<ResourceSubBundleInfo> resourceSubBundleInfoList = new List<ResourceSubBundleInfo>();
        bool packSeparately;
        /// <summary>
        /// 资源包的名称，AsseBundleName。
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
        /// AB加载时候使用的名称。
        /// </summary>
        public string BundleKey
        {
            get { return bundleKey; }
            set { bundleKey = value; }
        }
        /// <summary>
        /// Bundle尺寸
        /// </summary>
        public long BundleSize
        {
            get { return bundleSize; }
            set { bundleSize = value; }
        }
        /// <summary>
        /// Bundle比特文件大小。
        /// </summary>
        public string BundleFormatBytes
        {
            get { return bundleFormatBytes; }
            set { bundleFormatBytes = value; }
        }
        /// <summary>
        /// 资源对象列表。
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
        /// 资源的依赖项。
        /// </summary>
        public List<BundleDependencyInfo> BundleDependencyList
        {
            get
            {
                if (bundleDependencies == null)
                    bundleDependencies = new List<BundleDependencyInfo>();
                return bundleDependencies;
            }
            set { bundleDependencies = value; }
        }
        /// <summary>
        /// 此bundle的子bundle文件。
        /// </summary>
        public List<ResourceSubBundleInfo> ResourceSubBundleInfoList
        {
            get
            {
                if (resourceSubBundleInfoList == null)
                    resourceSubBundleInfoList = new List<ResourceSubBundleInfo>();
                return resourceSubBundleInfoList;
            }
            set { resourceSubBundleInfoList = value; }
        }

        /// <summary>
        /// extract all files from the folder as an individual assetbundle.
        /// </summary>
        public bool PackSeparately
        {
            get { return packSeparately; }
            set { packSeparately = value; }
        }

        public bool Equals(ResourceBundleInfo other)
        {
            return other.BundleName == this.BundleName;
        }
    }
}
