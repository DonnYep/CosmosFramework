using System;
namespace Cosmos.Resource.Verifiy
{
    internal class ResourceManifestVerifiyTask : IEquatable<ResourceManifestVerifiyTask>, IReference
    {
        public string Url { get; private set; }
        public string ResourceBundleName { get; private set; }
        public long ResourceBundleSize { get; private set; }
        public bool Equals(ResourceManifestVerifiyTask other)
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
        public static ResourceManifestVerifiyTask Create(string url, string resourceBundleName, long resourceBundleSize)
        {
            var task = ReferencePool.Acquire<ResourceManifestVerifiyTask>();
            task.Url = url;
            task.ResourceBundleName = resourceBundleName;
            task.ResourceBundleSize = resourceBundleSize;
            return task;
        }
        public static void Release(ResourceManifestVerifiyTask task)
        {
            ReferencePool.Release(task);
        }
    }
}
