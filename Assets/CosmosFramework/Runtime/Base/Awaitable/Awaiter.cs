using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Cosmos
{
    public  abstract class Awaiter<T> : INotifyCompletion
    {
        public abstract bool IsCompleted { get; protected set; }
        public abstract void OnCompleted(Action continuation);
        public Awaiter<T> GetAwaiter() { return this;}
        public abstract T GetResult();
    }
}
