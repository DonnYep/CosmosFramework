using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadStartEventArgs:GameEventArgs
    {
        public string URI { get; private set; }
        public string DownloadPath { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
            CustomeData = null;
        }
        public static DownloadStartEventArgs Create(string uri, string downloadPath,object customeData)
        {
            var eventArgs = ReferencePool.Accquire<DownloadStartEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadPath = downloadPath;
            eventArgs.CustomeData = customeData;
            return eventArgs;
        }
        public static void Release(DownloadStartEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
