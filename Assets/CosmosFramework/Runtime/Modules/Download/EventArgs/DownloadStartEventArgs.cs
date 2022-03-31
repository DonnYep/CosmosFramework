namespace Cosmos.Download
{
    public class DownloadStartEventArgs:GameEventArgs
    {
        public string URI { get; private set; }
        public string DownloadPath { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
        }
        public static DownloadStartEventArgs Create(string uri, string downloadPath)
        {
            var eventArgs = ReferencePool.Acquire<DownloadStartEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadPath = downloadPath;
            return eventArgs;
        }
        public static void Release(DownloadStartEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
