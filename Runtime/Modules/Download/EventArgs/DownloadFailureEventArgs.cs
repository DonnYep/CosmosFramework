namespace Cosmos.Download
{
    public class DownloadFailureEventArgs : GameEventArgs
    {
        public DownloadInfo DownloadInfo { get; private set; }
        public string ErrorMessage { get; private set; }
        public int CurrentDownloadTaskIndex { get; private set; }
        public int DownloadTaskCount { get; private set; }
        public override void Release()
        {
            DownloadInfo = default;
            CurrentDownloadTaskIndex = 0;
            DownloadTaskCount = 0;
            ErrorMessage = null;
        }
        public static DownloadFailureEventArgs Create(DownloadInfo info, int currentTaskIndex, int taskCount, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<DownloadFailureEventArgs>();
            eventArgs.DownloadInfo= info;
            eventArgs.CurrentDownloadTaskIndex = currentTaskIndex;
            eventArgs.DownloadTaskCount = taskCount;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        public static void Release(DownloadFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
