using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public partial class CoroutineAwaiter<TInstruction>
    {
        /// <summary>
        /// 协程对象的计数器；
        /// </summary>
        public class AwaitableEnumerator : IEnumerator
        {
            /// <summary>
            /// 计数器的CoroutineAwaiter目标；
            /// </summary>
            CoroutineAwaiter<TInstruction> awaiter;
            IEnumerator nestedCoroutine;
            public object Current { get; private set; }
            public AwaitableEnumerator(CoroutineAwaiter<TInstruction> awaiter)
            {
                this.awaiter = awaiter;
                nestedCoroutine = awaiter.Instruction as IEnumerator;
            }
            public bool MoveNext()
            {
                if (nestedCoroutine != null)
                {
                    bool result = nestedCoroutine.MoveNext();
                    Current = nestedCoroutine.Current;
                    awaiter.IsCompleted = !result;
                    return result;
                }
                if (Current == null)
                {
                    Current = awaiter.Instruction;
                    return true;
                }
                awaiter.IsCompleted = true;
                return false;
            }
            public void Reset()
            {
                Current = null;
                awaiter.IsCompleted = false;
            }
        }
    }
}
