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
        public object CustomeData { get; private set; }
        public string ErrorMessage{ get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
            CustomeData = null;
            ErrorMessage = null;
        }
        public static DownloadFailureEventArgs Create(string uri, string downloadPath, string errorMessage, object customeData)
        {
            var eventArgs = ReferencePool.Accquire<DownloadFailureEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadPath = downloadPath;
            eventArgs.ErrorMessage= errorMessage;
            eventArgs.CustomeData = customeData;
            return eventArgs;
        }
        public static void Release(DownloadFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
