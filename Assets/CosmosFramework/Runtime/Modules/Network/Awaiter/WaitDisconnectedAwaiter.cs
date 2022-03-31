using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitDisconnectedAwaiter : INotifyCompletion
    {
        INetworkClientChannel channel;
        Action continuation;
        public WaitDisconnectedAwaiter(INetworkClientChannel networkChannel)
        {
            this.channel = networkChannel;
            networkChannel.OnDisconnected += OnDisconnected;
            networkChannel.Disconnect();
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
        void OnDisconnected()
        {
            continuation?.Invoke();
            IsCompleted = true;
            channel.OnConnected -= OnDisconnected;
        }
    }
}
