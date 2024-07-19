namespace Cosmos.Resource
{
    public class ResourceDownloadSuccessEventArgs : GameEventArgs
    {
        public ResourceDownloadTask  ResourceDownloadTask{ get; private set; }
        public override void Release()
        {
            ResourceDownloadTask=default;
        }
        public static ResourceDownloadSuccessEventArgs Create(ResourceDownloadTask task)
        {
            var eventArgs = ReferencePool.Acquire<ResourceDownloadSuccessEventArgs>();
            eventArgs.ResourceDownloadTask = task;
            return eventArgs;
        }
        public static void Release(ResourceDownloadSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
