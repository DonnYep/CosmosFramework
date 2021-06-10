using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public struct FutureTaskInfo
    {
        public int FutureTaskId { get; set; }
        public float ElapseTime { get; set; }
        public static FutureTaskInfo None { get { return default(FutureTaskInfo); } }
    }
}
