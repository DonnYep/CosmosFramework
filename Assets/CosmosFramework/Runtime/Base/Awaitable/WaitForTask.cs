using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// Task类型的迭代指令；
    /// </summary>
    public class WaitForTask : CustomYieldInstruction
    {
        public Task Task { get; private set; }
        public override bool keepWaiting
        {
            get
            {
                if (Task.Exception != null)
                    throw Task.Exception;
                return !Task.IsCompleted;
            }
        }
        public WaitForTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("Task is invalid !");
            Task = task;
        }
    }
}
