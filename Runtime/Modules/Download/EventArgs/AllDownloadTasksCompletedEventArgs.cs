using System;
namespace Cosmos.Download
{
    public class AllDownloadTasksCompletedEventArgs : GameEventArgs
    {
        public DownloadCompletedInfo[] SuccessedInfos{ get; private set; }
        public DownloadCompletedInfo[] FailedInfos { get; private set; }
        /// <summary>
        /// 下载所使用的时间；
        /// </summary>
        public TimeSpan AllDownloadTasksCompletedTimeSpan { get; private set; }
        public override void Release()
        {
            SuccessedInfos = null;
            FailedInfos = null;
            AllDownloadTasksCompletedTimeSpan = TimeSpan.Zero; 
        }
        public static AllDownloadTasksCompletedEventArgs Create(DownloadCompletedInfo[] successedInfos, DownloadCompletedInfo[] failedInfos, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<AllDownloadTasksCompletedEventArgs>();
            eventArgs.SuccessedInfos = successedInfos;
            eventArgs.FailedInfos= failedInfos;
            eventArgs.AllDownloadTasksCompletedTimeSpan = timeSpan;
            return eventArgs;
        }
        public static void Release(AllDownloadTasksCompletedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
