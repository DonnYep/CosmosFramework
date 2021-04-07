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
        /// 同步的帧率；
        /// </summary>
        public static float SyncInterval { get { return  IntervalMS/ 1000 ; } }
        /// <summary>
        /// 同步的毫秒间隔;
        /// </summary>
        public static int IntervalMS { get; set; } = 100;
    }
}
