using System;
using Cosmos.Network;
namespace Cosmos.Lockstep
{
    [Module]
    public class ServiceManager : Module, IServiceManager
    {
        public bool IsConnected { get; private set; }
        public string Host { get; private set; }
        public ushort Port { get; private set; }
        Action onConnected;
        public event Action OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        Action onDisconnected;
        public event Action OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        Action<byte[]> onReceiveData;
        public event Action<byte[]> OnReceiveData
        {
            add { onReceiveData += value; }
            remove { onReceiveData -= value; }
        }
        KcpClientChannel networkChannel;
        public void Connect(string host, ushort port)
        {
            if (IsConnected)
                return;
            networkChannel = new KcpClientChannel("Lockstep");
            networkChannel.OnDataReceived+= OnReceiveDataHandle;
            networkChannel.OnDisconnected += OnDisconnectedHandle;
            networkChannel.OnConnected += OnConnectedHandle;
            networkChannel.Connect(host,port);
            CosmosEntry.NetworkManager.AddOrUpdateChannel(networkChannel);
            Host = host;
            Port = port;
        }
        public void Disconnect()
        {
            if (!IsConnected)
                return;
            CosmosEntry.NetworkManager.CloseChannel(networkChannel.ChannelName, out _);
        }
        public void SendMessage(byte[] data)
        {
            if (!IsConnected)
                return;
            networkChannel.SendMessage(data);
        }
        void OnReceiveDataHandle(byte[] data)
        {
            onReceiveData?.Invoke(data);
        }
        void OnDisconnectedHandle()
        {
            IsConnected = false;
            onDisconnected?.Invoke();
        }
        void OnConnectedHandle()
        {
            IsConnected = true;
            onConnected?.Invoke();
        }
    }
}
