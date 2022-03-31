using System;
using System.Runtime.CompilerServices;
namespace Cosmos.Network
{
    public class WaitReceiveDataAwaiter : INotifyCompletion
    {
        INetworkClientChannel networkChannel;
        Action continuation;
        byte[] rcvData;
        public WaitReceiveDataAwaiter(INetworkClientChannel networkChannel)
        {
            this.networkChannel = networkChannel;
            networkChannel.OnDataReceived += OnReceiveData;
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
        void OnReceiveData(byte[] data)
        {
            rcvData = data;
            continuation?.Invoke();
            IsCompleted = true;
            networkChannel.OnDataReceived-= OnReceiveData;
        }
    }
}
