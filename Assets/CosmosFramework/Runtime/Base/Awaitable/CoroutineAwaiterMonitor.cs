using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cosmos
{
    internal class CoroutineAwaiterMonitor : MonoSingleton<CoroutineAwaiterMonitor>
    {
        /// <summary>
        ///  用于线程同步；多线程的数据发送到unity的主线程中；
        /// </summary>
        SynchronizationContext synchronizationContext;
        protected override void Awake()
        {
            base.Awake();
            gameObject.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
            DontDestroyOnLoad(gameObject);
            synchronizationContext = SynchronizationContext.Current;
        }
        public void PostToMainThread(Action<object> sendOrPostCallback)
        {
            synchronizationContext.Post(state => sendOrPostCallback.Invoke(state), null);
        }
        public void StartAwaitableCoroutine<T>(CoroutineAwaiter<T> awaiterCoroutine)
        {
            StartCoroutine(awaiterCoroutine.Coroutine);
        }
        public void StopAwaitableCoroutine<T>(CoroutineAwaiter<T> awaiterCoroutine)
        {
            StopCoroutine(awaiterCoroutine.Coroutine);
        }
    }
}
