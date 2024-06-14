using System;
using System.Threading;

namespace Cosmos
{
    /// <summary>
    /// unity线程同步类
    /// </summary>
    public class UnityThreadSync : MonoSingleton<UnityThreadSync>
    {
        SynchronizationContext synchronizationContext;
        public void PostToUnityThread(Action<object> postCallback)
        {
            synchronizationContext.Post(state => postCallback.Invoke(state), null);
        }
        public void SendToUnityThread(Action<object> postCallback)
        {
            synchronizationContext.Send(state => postCallback.Invoke(state), null);
        }
        void Awake()
        {
            gameObject.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
            synchronizationContext = SynchronizationContext.Current;
        }
    }
}
