using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitDisconnectedAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action onDisconnected;
        public WaitDisconnectedAwaiter(INetworkChannel networkChannel,int conv)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnDisconnected+= OnDisconnected;
            networkChannel.Disconnect(conv);
        }
        public void OnCompleted(Action onDisconnected)
        {
            this.onDisconnected = onDisconnected;
        }
        public bool IsCompleted { get; private set; }
        public void GetResult() { }
        public WaitDisconnectedAwaiter GetAwaiter()
        {
            return this;
        }
        void OnDisconnected(int conv)
        {
            onDisconnected?.Invoke();
            IsCompleted = true;
            networkChannel.OnConnected -= OnDisconnected;
        }
    }
}
