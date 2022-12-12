namespace Cosmos.Download
{
    public class DownloadSuccessEventArgs : GameEventArgs
    {
        public string URI { get; private set; }
        public string DownloadPath { get; private set; }
        public int DownloadLength { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
            DownloadLength = 0;
        }
        public static DownloadSuccessEventArgs Create(string uri, string downloadPath,int downloadLength)
        {
            var eventArgs = ReferencePool.Acquire<DownloadSuccessEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadPath = downloadPath;
            eventArgs.DownloadLength = downloadLength;
            return eventArgs;
        }
        public static void Release(DownloadSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
