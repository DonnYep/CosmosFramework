using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Awaitable
{
    public class WaitForCoroutineAwaiter : INotifyCompletion
    {
        Coroutine coroutine;
        Action continuation;
        public WaitForCoroutineAwaiter(Coroutine coroutine)
        {
            this.coroutine = coroutine;
            CoroutineAwaiterMonitor.Instance.StartCoroutine(EnumRun());
        }
        public bool IsCompleted { get; private set; }
        public void GetResult() { }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        IEnumerator EnumRun()
        {
            yield return coroutine;
            IsCompleted = true;
            continuation?.Invoke();
        }
    }
}
