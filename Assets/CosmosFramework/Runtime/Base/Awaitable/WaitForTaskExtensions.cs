using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public static class WaitForTaskExtensions
    {
        public static WaitForTask AsCoroutine(this Task task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            return new WaitForTask(task);
        }
        public static WaitForTask<T> AsCoroutine<T>(this Task<T> task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            return new WaitForTask<T>(task);
        }
    }
}
