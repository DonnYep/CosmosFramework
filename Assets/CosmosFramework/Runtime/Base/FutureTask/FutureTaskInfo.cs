using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public struct FutureTaskInfo
    {
        public int FutureTaskId { get; private  set; }
        public float ElapsedTime { get; private  set; }
        public string Description { get; private  set; }
        public FutureTaskInfo(int futureTaskId, float elapseTime, string description)
        {
            FutureTaskId = futureTaskId;
            ElapsedTime = elapseTime;
            Description = description;
        }
        public static FutureTaskInfo None { get { return default(FutureTaskInfo); } }
    }
}
