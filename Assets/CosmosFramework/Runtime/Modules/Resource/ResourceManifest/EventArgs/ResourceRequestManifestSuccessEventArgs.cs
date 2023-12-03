namespace Cosmos.Resource
{
    public class ResourceRequestManifestSuccessEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string ManifestPath { get; private set; }
        public string BundlePath { get; private set; }
        public ResourceBundlePathType ResourceBundlePathType { get; private set; }
        public ResourceManifest ResourceManifest { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            ManifestPath = string.Empty;
            BundlePath = string.Empty;
            ResourceBundlePathType = ResourceBundlePathType.StreamingAssets;
            ResourceManifest = null;
        }
        public static ResourceRequestManifestSuccessEventArgs Create(long taskId,string manifestPath, string bundlePath,  ResourceBundlePathType resourceBundlePathType, ResourceManifest resourceManifest)
        {
            var eventArgs = ReferencePool.Acquire<ResourceRequestManifestSuccessEventArgs>();
            eventArgs.TaskId= taskId;
            eventArgs.ManifestPath = manifestPath;
            eventArgs.BundlePath = bundlePath;
            eventArgs.ResourceBundlePathType = resourceBundlePathType;
            eventArgs.ResourceManifest = resourceManifest;
            return eventArgs;
        }
        public static void Release(ResourceRequestManifestSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
