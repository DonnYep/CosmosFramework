using System;
using UnityEngine;

namespace Cosmos.Resource
{
    [Serializable]
    public class ResourceBundleDependency : IEquatable<ResourceBundleDependency>
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
        public bool Equals(ResourceBundleDependency other)
        {
            return other.bundleKey == bundleKey
                      && other.bundleName == bundleName;
        }
    }
}
