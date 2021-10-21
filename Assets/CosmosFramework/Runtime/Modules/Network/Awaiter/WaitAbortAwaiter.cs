using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitAbortAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action continuation;
        public WaitAbortAwaiter(INetworkChannel networkChannel)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnAbort += OnAbort;
            networkChannel.Abort();
        }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        public bool IsCompleted { get; private set; }
        public void GetResult() { }
        public WaitAbortAwaiter GetAwaiter()
        {
            return this;
        }
        void OnAbort()
        {
            continuation?.Invoke();
            IsCompleted = true;
            networkChannel.OnAbort -= OnAbort;
        }
    }
}
