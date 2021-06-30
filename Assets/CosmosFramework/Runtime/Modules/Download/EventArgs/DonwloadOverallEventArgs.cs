using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DonwloadOverallEventArgs : GameEventArgs
    {
        /// <summary>
        /// 当前正在下载的uri资源；
        /// </summary>
        public string URI { get; private set; }
        /// <summary>
        ///整体下载百分比进度 0~100%；
        /// </summary>
        public float OverallProgress { get; private set; }
        /// <summary>
        /// 当前下载文件的百分比；
        /// </summary>
        public float IndividualProgress { get; private set; }
        /// <summary>
        /// 当前资源的下载缓存路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadPath = null;
            CustomeData = null;
            OverallProgress = 0;
        }
        public static DonwloadOverallEventArgs Create(string uri, string downloadPath, float overallProgress,float individualProgress, object customeData)
        {
            var eventArgs = ReferencePool.Accquire<DonwloadOverallEventArgs>();
            eventArgs.URI = uri;
            eventArgs.OverallProgress= overallProgress;
            eventArgs.DownloadPath = downloadPath;
            eventArgs.IndividualProgress= individualProgress;
            eventArgs.CustomeData = customeData;
            return eventArgs;
        }
        public static void Release(DonwloadOverallEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
