namespace Cosmos.Download
{
    public class DownloadFailureEventArgs : GameEventArgs
    {
        public DownloadCompletedInfo DownloadCompletedInfo { get; private set; }
        public string ErrorMessage { get; private set; }
        public override void Release()
        {
            DownloadCompletedInfo = default;
            ErrorMessage = null;
        }
        public static DownloadFailureEventArgs Create(DownloadCompletedInfo completedInfo, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<DownloadFailureEventArgs>();
            eventArgs.DownloadCompletedInfo= completedInfo;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        public static void Release(DownloadFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
