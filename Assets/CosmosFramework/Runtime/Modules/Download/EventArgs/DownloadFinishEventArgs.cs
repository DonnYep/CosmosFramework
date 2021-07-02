using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class DownloadFinishEventArgs : GameEventArgs
    {
        public string[] SuccessURIs { get; private set; }
        public string[] FailureURIs { get; private set; }
        /// <summary>
        /// 下载所使用的时间；
        /// </summary>
        public TimeSpan DownloadTimeSpan { get; private set; }
        public override void Release()
        {
            SuccessURIs = null;
            FailureURIs = null;
            DownloadTimeSpan = TimeSpan.Zero; ;
        }
        public static DownloadFinishEventArgs Create(string[] successURIs , string[] failureURIs, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Accquire<DownloadFinishEventArgs>();
            eventArgs.SuccessURIs= successURIs;
            eventArgs.FailureURIs= failureURIs;
            eventArgs.DownloadTimeSpan = timeSpan;
            return eventArgs;
        }
        public static void Release(DownloadFinishEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
