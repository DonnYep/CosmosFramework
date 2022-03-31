using System;
using System.Runtime.CompilerServices;

namespace Cosmos.Awaitable
{
    public  abstract class Awaiter<T> : INotifyCompletion
    {
        public abstract bool IsCompleted { get; protected set; }
        public abstract void OnCompleted(Action continuation);
        public Awaiter<T> GetAwaiter() { return this;}
        public abstract T GetResult();
    }
}
