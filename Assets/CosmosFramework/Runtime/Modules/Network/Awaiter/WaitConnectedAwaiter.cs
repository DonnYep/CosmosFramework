using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitConnectedAwaiter : INotifyCompletion
    {
        INetworkClientChannel channel;
        Action continuation;
        public WaitConnectedAwaiter(INetworkClientChannel clientChannel, string host, int port)
        {
            this.channel = clientChannel;
            clientChannel.OnConnected += OnConnected;
            clientChannel.Connect(host,port);
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
        void OnConnected()
        {
            continuation?.Invoke();
            IsCompleted = true;
            channel.OnConnected -= OnConnected;
        }
    }
}
