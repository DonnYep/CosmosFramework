using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadUpdateEventArgs : GameEventArgs
    {
        public string URI { get; private set; }
        /// <summary>
        /// 0~100%；
        /// </summary>
        public  int ProgressPercentage{ get; private set; }
        public string DownloadPath { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath= null;
            CustomeData = null;
            ProgressPercentage = 0;
        }
        public static DownloadUpdateEventArgs Create(string uri, string downloadPath, int progressPercentage, object customeData)
        {
            var eventArgs = ReferencePool.Accquire<DownloadUpdateEventArgs>();
            eventArgs.URI = uri;
            eventArgs.ProgressPercentage= progressPercentage;
            eventArgs.DownloadPath = downloadPath;
            eventArgs.CustomeData = customeData;
            return eventArgs;
        }
        public static void Release(DownloadUpdateEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
