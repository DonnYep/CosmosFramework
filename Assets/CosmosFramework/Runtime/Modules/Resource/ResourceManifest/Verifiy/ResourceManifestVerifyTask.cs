using System;
namespace Cosmos.Resource.Verify
{
    internal class ResourceManifestVerifyTask : IEquatable<ResourceManifestVerifyTask>, IReference
    {
        public string Url { get; private set; }
        public string ResourceBundleName { get; private set; }
        public long ResourceBundleSize { get; private set; }
        public bool Equals(ResourceManifestVerifyTask other)
        {
            return other.Url == this.Url &&
                other.ResourceBundleName == this.ResourceBundleName;
        }
        public void Release()
        {
            Url = string.Empty;
            ResourceBundleName = string.Empty;
            ResourceBundleSize = 0;
        }
        public static ResourceManifestVerifyTask Create(string url, string resourceBundleName, long resourceBundleSize)
        {
            var task = ReferencePool.Acquire<ResourceManifestVerifyTask>();
            task.Url = url;
            task.ResourceBundleName = resourceBundleName;
            task.ResourceBundleSize = resourceBundleSize;
            return task;
        }
        public static void Release(ResourceManifestVerifyTask task)
        {
            ReferencePool.Release(task);
        }
    }
}
