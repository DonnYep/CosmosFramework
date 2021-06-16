using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Cosmos
{
    public class CoroutineAwaiterWaitForMainThread:CoroutineAwaiter<WaitForMainThread>
    {
        public CoroutineAwaiterWaitForMainThread()
        {
            Instruction = default(WaitForMainThread);
        }
        public override void OnCompleted(Action continuation)
        {
            base.OnCompleted(continuation);
            if (SynchronizationContext.Current != null)
            {
                IsCompleted = true;
            }
            else
            {
                CoroutineAwaiterMonitor.Instance.PostToMainThread(state =>
                {
                    IsCompleted = true;
                });
            }
        }
    }
}
