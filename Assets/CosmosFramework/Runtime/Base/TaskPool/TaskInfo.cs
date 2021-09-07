using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 任务状态信息；
    /// </summary>
    public struct TaskInfo
    {
        readonly int taskSerialId;
        readonly string tag;
        readonly object customeData;
        readonly TaskStatus taskStatus;
        readonly string description;
        public TaskInfo( int serialId, string tag, object customeData, TaskStatus taskStatus, string description)
        {
            this.taskSerialId = serialId;
            this.tag = tag;
            this.customeData = customeData;
            this.taskStatus = taskStatus;
            this.description = description;
        }
        /// <summary>
        /// 任务的序列号；
        /// </summary>
        public int TaskSerialId { get { return taskSerialId; } }
        /// <summary>
        /// 任务标签；
        /// </summary>
        public string Tag { get { return tag; } }
        /// <summary>
        /// 自定义数据；
        /// </summary>
        public object CustomeData { get { return customeData; } }
        /// <summary>
        /// 任务状态；
        /// </summary>
        public TaskStatus TaskStatus { get { return taskStatus; } }
        /// <summary>
        /// 任务描述；
        /// </summary>
        public string Description { get { return description; } }
    }
}
