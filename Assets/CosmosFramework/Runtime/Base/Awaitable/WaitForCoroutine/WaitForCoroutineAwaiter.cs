using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    public class WaitForCoroutineAwaiter : INotifyCompletion
    {
        bool isCompleted = false;
        Coroutine coroutine;
        Action continuation;
        public WaitForCoroutineAwaiter(Coroutine coroutine)
        {
            this.coroutine = coroutine;
            CoroutineAwaiterMonitor.Instance.StartCoroutine(EnumRun());
        }
        public bool IsCompleted
        {
            get
            {
                return isCompleted; 
            }
           private set
            {
                isCompleted = value;
                if (value && continuation != null)
                {
                    continuation();
                    continuation = null;
                }
            }
        }
        public void GetResult() { }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        IEnumerator EnumRun()
        {
            yield return coroutine;
            IsCompleted = true;
        }
    }
}
