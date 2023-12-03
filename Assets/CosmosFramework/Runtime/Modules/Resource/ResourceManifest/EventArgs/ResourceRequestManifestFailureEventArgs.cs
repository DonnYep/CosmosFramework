namespace Cosmos.Resource
{
    public class ResourceRequestManifestFailureEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string ManifestPath { get; private set; }
        public ResourceLoadMode ResourceLoadMode { get; private set; }
        public ResourceBundlePathType ResourceBundlePathType { get; private set; }
        public string ErrorMessage { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            ManifestPath = string.Empty;
            ResourceLoadMode = ResourceLoadMode.Resource;
            ResourceBundlePathType = ResourceBundlePathType.StreamingAssets;
            ErrorMessage = string.Empty;
        }
        public static ResourceRequestManifestFailureEventArgs Create(long taskId, string manifestPath, ResourceLoadMode resourceLoadMode, ResourceBundlePathType resourceBundlePathType, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<ResourceRequestManifestFailureEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.ManifestPath = manifestPath;
            eventArgs.ResourceLoadMode = resourceLoadMode;
            eventArgs.ResourceBundlePathType = resourceBundlePathType;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        public static void Release(ResourceRequestManifestFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
