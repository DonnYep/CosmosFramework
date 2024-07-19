namespace Cosmos.Resource
{
    public class ResourceDownloadFailureEventArgs : GameEventArgs
    {
        public ResourceDownloadTask ResourceDownloadTask { get; private set; }
        public override void Release()
        {
            ResourceDownloadTask = default;
        }
        public static ResourceDownloadFailureEventArgs Create(ResourceDownloadTask task)
        {
            var eventArgs = ReferencePool.Acquire<ResourceDownloadFailureEventArgs>();
            eventArgs.ResourceDownloadTask = task;
            return eventArgs;
        }
        public static void Release(ResourceDownloadFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
