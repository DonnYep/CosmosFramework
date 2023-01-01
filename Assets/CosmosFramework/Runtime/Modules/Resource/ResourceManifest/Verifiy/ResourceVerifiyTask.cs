using System;
using System.Runtime.InteropServices;
namespace Cosmos.Resource.Verifiy
{
    [StructLayout(LayoutKind.Auto)]
    internal struct ResourceVerifiyTask : IEquatable<ResourceVerifiyTask>
    {
        public string Url { get; private set; }
        public string ResourceBundleName { get; private set; }
        public long ResourceBundleSize { get; private set; }
        public ResourceVerifiyTask(string url, string resourceBundleName, long resourceBundleSize)
        {
            Url = url;
            ResourceBundleName = resourceBundleName;
            ResourceBundleSize = resourceBundleSize;
        }
        public bool Equals(ResourceVerifiyTask other)
        {
            return other.Url == this.Url &&
                other.ResourceBundleName == this.ResourceBundleName &&
                other.ResourceBundleSize == this.ResourceBundleSize;
        }
    }
}
