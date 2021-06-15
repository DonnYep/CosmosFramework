using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

 namespace Cosmos
{
    public partial class CoroutineAwaiter<T> : INotifyCompletion
    {
        Action continuation;
        bool _isCompleted;
        public T Instruction { get; protected set; }
        public AwaitableEnumerator Coroutine { get; private set; }
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            protected set
            {
                _isCompleted = value;

                if (value && continuation != null)
                {
                    continuation();
                    continuation = null;
                }
            }
        }
        public CoroutineAwaiter() { }
        public CoroutineAwaiter(T instruction) 
        {
            Instruction = instruction;
            Coroutine = new AwaitableEnumerator(this);
            CoroutineAwaiterMonitor.Instance.StartAwaitableCoroutine(this);
        }
        public T GetResult()
        {
            return Instruction;
        }
        public void OnCompleted(Action continuation)
        {
            ProcessOnCompleted(continuation);
        }
        /// <summary>
        /// 执行处理OnCompleted；
        /// </summary>
        /// <param name="continuation">OnCompleted的回调</param>
        protected virtual void ProcessOnCompleted(Action continuation)
        {
            this.continuation= continuation;
        }
    }
}
