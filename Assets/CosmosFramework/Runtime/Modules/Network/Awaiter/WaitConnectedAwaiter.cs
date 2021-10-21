using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitConnectedAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action continuation;
        public WaitConnectedAwaiter(INetworkChannel networkChannel)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnConnected += OnConnected;
            networkChannel.Connect();
        }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        public bool IsCompleted { get; private set; }
        public void GetResult() { }
        public WaitConnectedAwaiter GetAwaiter()
        {
            return this;
        }
        void OnConnected(int conv)
        {
            continuation?.Invoke();
            IsCompleted = true;
            networkChannel.OnConnected -= OnConnected;
        }
    }
}
