using System;

namespace Cosmos.Awaitable
{
    public partial class CoroutineAwaiter<T> : Awaiter<T>
    {
        Action continuation;
        bool isCompleted;
        public T Instruction { get; protected set; }
        public AwaitableEnumerator Coroutine { get; private set; }
        public override bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
            protected set
            {
                isCompleted = value;
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
        public override T GetResult()
        {
            return Instruction;
        }
        public override void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
    }
}
