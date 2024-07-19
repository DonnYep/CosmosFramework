namespace Cosmos.Resource
{
    public class ResourceDownloadTasksCancelEventArgs : GameEventArgs
    {
        /// <summary>
        /// 取消的下载信息
        /// </summary>
        public ResourceDownloadTask[] CanceledInfos { get; private set; }
        public override void Release()
        {
            CanceledInfos = new ResourceDownloadTask[0];
        }
        public static ResourceDownloadTasksCancelEventArgs Create(ResourceDownloadTask[] canceledTasks)
        {
            var eventArgs = ReferencePool.Acquire<ResourceDownloadTasksCancelEventArgs>();
            eventArgs.CanceledInfos = canceledTasks;
            return eventArgs;
        }
        public static void Release(ResourceDownloadTasksCancelEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
