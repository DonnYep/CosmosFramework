namespace Cosmos.Download
{
    public class DownloadSuccessEventArgs : GameEventArgs
    {
        public DownloadCompletedInfo DownloadCompletedInfo { get; private set; }
        public override void Release()
        {
            DownloadCompletedInfo = default;
        }
        public static DownloadSuccessEventArgs Create(DownloadCompletedInfo completedInfo)
        {
            var eventArgs = ReferencePool.Acquire<DownloadSuccessEventArgs>();
            eventArgs.DownloadCompletedInfo = completedInfo;
            return eventArgs;
        }
        public static void Release(DownloadSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
