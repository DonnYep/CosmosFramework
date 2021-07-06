using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载并写入完成事件；
    /// </summary>
    public class DownloadAndWriteFinishEventArgs : GameEventArgs
    {
        public string[] SuccessURIs { get; private set; }
        public string[] FailureURIs { get; private set; }
        /// <summary>
        /// 下载所使用的时间；
        /// </summary>
        public TimeSpan DownloadAndWriteTimeSpan { get; private set; }
        public override void Release()
        {
            SuccessURIs = null;
            FailureURIs = null;
            DownloadAndWriteTimeSpan = TimeSpan.Zero; 
        }
        public static DownloadAndWriteFinishEventArgs Create(string[] successURIs , string[] failureURIs, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Accquire<DownloadAndWriteFinishEventArgs>();
            eventArgs.SuccessURIs= successURIs;
            eventArgs.FailureURIs= failureURIs;
            eventArgs.DownloadAndWriteTimeSpan = timeSpan;
            return eventArgs;
        }
        public static void Release(DownloadAndWriteFinishEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
