namespace Cosmos.Download
{
    public class DownloadSuccessEventArgs : GameEventArgs
    {
        public string URI { get; private set; }
        public string DownloadPath { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
        }
        public static DownloadSuccessEventArgs Create(string uri, string downloadPath)
        {
            var eventArgs = ReferencePool.Acquire<DownloadSuccessEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadPath = downloadPath;
            return eventArgs;
        }
        public static void Release(DownloadSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
