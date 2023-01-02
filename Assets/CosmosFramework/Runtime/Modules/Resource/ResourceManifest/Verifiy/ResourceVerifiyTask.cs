using System;
namespace Cosmos.Resource.Verifiy
{
    internal class ResourceVerifiyTask : IEquatable<ResourceVerifiyTask>, IReference
    {
        public string Url { get; private set; }
        public string ResourceBundleName { get; private set; }
        public long ResourceBundleSize { get; private set; }
        public bool Equals(ResourceVerifiyTask other)
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
        public static ResourceVerifiyTask Create(string url, string resourceBundleName, long resourceBundleSize)
        {
            var task = ReferencePool.Acquire<ResourceVerifiyTask>();
            task.Url = url;
            task.ResourceBundleName = resourceBundleName;
            task.ResourceBundleSize = resourceBundleSize;
            return task;
        }
        public static void Release(ResourceVerifiyTask task)
        {
            ReferencePool.Release(task);
        }
    }
}
