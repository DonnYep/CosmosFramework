namespace Cosmos.Download
{
    public class DownloadSuccessEventArgs : GameEventArgs
    {
        public DownloadInfo DownloadInfo { get; private set; }
        public int CurrentDownloadTaskIndex { get; private set; }
        public int DownloadTaskCount { get; private set; }
        public override void Release()
        {
            DownloadInfo = default;
            CurrentDownloadTaskIndex = 0;
            DownloadTaskCount = 0;
        }
        public static DownloadSuccessEventArgs Create(DownloadInfo info, int currentTaskIndex, int taskCount)
        {
            var eventArgs = ReferencePool.Acquire<DownloadSuccessEventArgs>();
            eventArgs.DownloadInfo = info;
            eventArgs.CurrentDownloadTaskIndex = currentTaskIndex;
            eventArgs.DownloadTaskCount = taskCount;
            return eventArgs;
        }
        public static void Release(DownloadSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
