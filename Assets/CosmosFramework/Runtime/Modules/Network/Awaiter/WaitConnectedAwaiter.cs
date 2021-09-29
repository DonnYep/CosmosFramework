using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitConnectedAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action onConnected;
        public WaitConnectedAwaiter(INetworkChannel networkChannel)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnConnected += OnConnected;
            networkChannel.Connect();
        }
        public void OnCompleted(Action onConnected)
        {
            this.onConnected = onConnected;
        }
        public bool IsCompleted{ get;private set; }
        public void GetResult() { }
        public WaitConnectedAwaiter GetAwaiter()
        {
            return this;
        }
        void OnConnected(int conv)
        {
            onConnected?.Invoke();
            IsCompleted = true;
            networkChannel.OnConnected -= OnConnected;
        }
    }
}
