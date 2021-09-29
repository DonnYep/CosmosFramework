using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitReceiveDataAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action onConnected;
        byte[] rcvData;
        public WaitReceiveDataAwaiter(INetworkChannel networkChannel)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnReceiveData += OnReceiveData;
        }
        public void OnCompleted(Action onConnected)
        {
            this.onConnected = onConnected;
        }
        public bool IsCompleted { get; private set; }
        public byte[] GetResult() { return rcvData; }
        public WaitReceiveDataAwaiter GetAwaiter()
        {
            return this;
        }
        void OnReceiveData(int conv, byte[] data)
        {
            rcvData = data;
            onConnected?.Invoke();
            IsCompleted = true;
            networkChannel.OnReceiveData -= OnReceiveData;
        }
    }
}
