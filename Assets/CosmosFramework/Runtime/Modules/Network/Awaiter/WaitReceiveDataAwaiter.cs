using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitReceiveDataAwaiter : INotifyCompletion
    {
        INetworkChannel networkChannel;
        Action continuation;
        byte[] rcvData;
        public WaitReceiveDataAwaiter(INetworkChannel networkChannel)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnReceiveData += OnReceiveData;
        }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
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
            continuation?.Invoke();
            IsCompleted = true;
            networkChannel.OnReceiveData -= OnReceiveData;
        }
    }
}
