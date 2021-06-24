using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadOverallEventArgs : GameEventArgs
    {
        /// <summary>
        /// 整体进度百分比；
        /// 0~100%；
        /// </summary>
        public int OverallPercentage { get; private set; }
        /// <summary>
        /// 单个进度的百分比；
        /// 0~100%；
        /// </summary>
        public int IndividualPercentage { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            OverallPercentage = 0;
            IndividualPercentage = 0;
            CustomeData = 0;
        }
        public static DownloadOverallEventArgs Create(int overallPercentage, int IndividualPercentage, object customeData)
        {
            var eventArgs = ReferencePool.Accquire<DownloadOverallEventArgs>();
            eventArgs.OverallPercentage = overallPercentage;
            eventArgs.IndividualPercentage= IndividualPercentage;
            eventArgs.CustomeData = customeData;
            return eventArgs;
        }
        public static void Release(DownloadOverallEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
