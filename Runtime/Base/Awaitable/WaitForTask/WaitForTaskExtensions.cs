using System;
using System.Threading.Tasks;

namespace Cosmos.Awaitable
{
    public static class WaitForTaskExtensions
    {
        public struct WaitForMainThread { }
        public static WaitForMainThreadAwaiter GetAwaiter(this WaitForMainThread waitForMainThread)
        {
            return new WaitForMainThreadAwaiter();
        }
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
