using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    ///  泛型Task类型的迭代指令；
    /// </summary>
    /// <typeparam name="T">task的任务返回类型</typeparam>
    public class WaitForTask<T> : WaitForTask
    {
        public new Task<T> Task { get; private set; }
        public T Result
        {
            get { return Task.Result; }
        }
        public WaitForTask(Task<T> task)
            : base(task)
        {
            Task = task;
        }
    }
}
