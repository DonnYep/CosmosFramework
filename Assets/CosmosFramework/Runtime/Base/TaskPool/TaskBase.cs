using System;
namespace Cosmos
{
    /// <summary>
    /// 任务基类；
    /// </summary>
    public abstract class TaskBase : IReference
    {
        string tag;
        object customeData;
        int taskSerialId;
        bool done;
        /// <summary>
        /// 任务的序列号；
        /// </summary>
        public int TaskSerialId { get { return taskSerialId; } }
        /// <summary>
        /// 任务标签；
        /// </summary>
        public string Tag { get { return tag; }  }
        /// <summary>
        /// 任务是否完成；
        /// </summary>
        public bool Done { get { return done; } set { done = value; } }
        /// <summary>
        /// 自定义数据；
        /// </summary>
        public object CustomeData { get { return customeData; } }
        /// <summary>
        /// 任务描述；
        /// </summary>
        public virtual string Description { get { return string.Empty; } }
        public void Initialize(int taskSerialId, string tag,object customeData)
        {
            this.taskSerialId = taskSerialId;
            this.tag = tag;
            this.customeData = customeData;
            this.done = true;
        }
        public void Release()
        {
            this.taskSerialId = 0;
            this.tag = string.Empty; ;
            this.customeData = null;
            this.done = false;
        }
    }
}
