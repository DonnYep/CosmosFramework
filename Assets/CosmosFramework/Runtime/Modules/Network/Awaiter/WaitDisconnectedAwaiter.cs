using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitDisconnectedAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action continuation;
        public WaitDisconnectedAwaiter(INetworkChannel networkChannel, int conv)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnDisconnected += OnDisconnected;
            networkChannel.Disconnect(conv);
        }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        public bool IsCompleted { get; private set; }
        public void GetResult() { }
        public WaitDisconnectedAwaiter GetAwaiter()
        {
            return this;
        }
        void OnDisconnected(int conv)
        {
            continuation?.Invoke();
            IsCompleted = true;
            networkChannel.OnConnected -= OnDisconnected;
        }
    }
}
