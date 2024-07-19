namespace Cosmos.Resource
{
    public class ResourceDownloadCompeleteEventArgs : GameEventArgs
    {
        /// <summary>
        /// 下载成功的任务
        /// </summary>
        public ResourceDownloadTask[] DownloadSuccessTasks { get; private set; }
        /// <summary>
        /// 下载失败的任务
        /// </summary>
        public ResourceDownloadTask[] DownloadFailureTasks { get; private set; }
        public override void Release()
        {
            DownloadSuccessTasks = new ResourceDownloadTask[0];
            DownloadFailureTasks = new ResourceDownloadTask[0];
        }
        public static ResourceDownloadCompeleteEventArgs Create(ResourceDownloadTask[] downloadSuccessTasks, ResourceDownloadTask[] downloadFailureTasks)
        {
            var eventArgs = ReferencePool.Acquire<ResourceDownloadCompeleteEventArgs>();
            eventArgs.DownloadSuccessTasks = downloadSuccessTasks;
            eventArgs.DownloadFailureTasks = downloadFailureTasks;
            return eventArgs;
        }
        public static void Release(ResourceDownloadCompeleteEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
