using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Test
{
    public class NetworkSimulateConsts
    {
        /// <summary>
        /// 服务器的同步帧率；
        /// </summary>
        public static float SyncInterval { get { return syncInterval; } }
        static float syncInterval = 0;
        /// <summary>
        /// 同步的毫秒间隔;
        /// </summary>
        public static int IntervalMS
        {
            get { return intervalMS; }
            set
            {
                intervalMS = value;
                syncInterval = (float)intervalMS / 1000;
            }
        }
        static int intervalMS = 100;
    }
}
