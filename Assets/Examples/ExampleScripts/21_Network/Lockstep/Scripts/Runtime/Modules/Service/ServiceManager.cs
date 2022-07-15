using System;
namespace Cosmos.Lockstep
{
    [Module]
    public class ServiceManager : Module, IServiceManager
    {
        public bool IsConnected { get; private set; }
        public string IP { get; private set; }
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
        KCPClientChannel networkChannel;
        public void Connect(string ip, ushort port)
        {
            if (IsConnected)
                return;
            networkChannel = new KCPClientChannel("Lockstep");
            networkChannel.OnDataReceived+= OnReceiveDataHandle;
            networkChannel.OnDisconnected += OnDisconnectedHandle;
            networkChannel.OnConnected += OnConnectedHandle;
            networkChannel.Connect(ip,port);
            CosmosEntry.NetworkManager.AddOrUpdateChannel(networkChannel);
            IP = ip;
            Port = port;
        }
        public void Disconnect()
        {
            if (!IsConnected)
                return;
            CosmosEntry.NetworkManager.RemoveChannel(networkChannel.NetworkChannelKey, out _);
            networkChannel?.Disconnect();
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
