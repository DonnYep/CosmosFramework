namespace Cosmos.Resource
{
    public class ResourceRequestManifestSuccessEventArgs : GameEventArgs
    {
        public string ManifestPath { get; private set; }
        public ResourceLoadMode ResourceLoadMode { get; private set; }
        public ResourceBundlePathType ResourceBundlePathType { get; private set; }
        public ResourceManifest ResourceManifest { get; private set; }
        public override void Release()
        {
            ManifestPath = string.Empty;
            ResourceLoadMode = ResourceLoadMode.Resource;
            ResourceBundlePathType = ResourceBundlePathType.StreamingAssets;
            ResourceManifest = null;
        }
        public static ResourceRequestManifestSuccessEventArgs Create(string manifestPath, ResourceLoadMode resourceLoadMode, ResourceBundlePathType resourceBundlePathType, ResourceManifest resourceManifest)
        {
            var eventArgs = ReferencePool.Acquire<ResourceRequestManifestSuccessEventArgs>();
            eventArgs.ManifestPath = manifestPath;
            eventArgs.ResourceLoadMode = resourceLoadMode;
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
