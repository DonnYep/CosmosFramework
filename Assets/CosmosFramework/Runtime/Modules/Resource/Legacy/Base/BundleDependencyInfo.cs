using System;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// ab包的依赖信息。
    /// </summary>
    [Serializable]
    public class BundleDependencyInfo : IEquatable<BundleDependencyInfo>
    {
        [SerializeField]
        string bundleKey;
        [SerializeField]
        string bundleName;
        public string BundleKey
        {
            get { return bundleKey; }
            set { bundleKey = value; }
        }
        public string BundleName
        {
            get { return bundleName; }
            set { bundleName = value; }
        }
        public bool Equals(BundleDependencyInfo other)
        {
            return other.bundleKey == bundleKey
                      && other.bundleName == bundleName;
        }
    }
}
