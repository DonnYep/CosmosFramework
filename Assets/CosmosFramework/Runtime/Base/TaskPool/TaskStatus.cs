using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 任务状态；
    /// </summary>
    public enum TaskStatus:byte
    {
        /// <summary>
        /// 等待开始；
        /// </summary>
        Prepare = 0x0,
        /// <summary>
        /// 执行中；
        /// </summary>
        Running =0x1,
        /// <summary>
        /// 已经完成；
        /// </summary>
        Done=0x2,
        /// <summary>
        /// 终止任务；
        /// </summary>
        Abort=0x3,
    }
}
