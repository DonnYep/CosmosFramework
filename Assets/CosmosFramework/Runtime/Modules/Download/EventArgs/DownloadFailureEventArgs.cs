using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadFailureEventArgs : GameEventArgs
    {
        public string URI { get; private set; }
        public string DownloadPath { get; private set; }
        public string ErrorMessage{ get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
            ErrorMessage = null;
        }
        public static DownloadFailureEventArgs Create(string uri, string downloadPath, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<DownloadFailureEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadPath = downloadPath;
            eventArgs.ErrorMessage= errorMessage;
            return eventArgs;
        }
        public static void Release(DownloadFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
