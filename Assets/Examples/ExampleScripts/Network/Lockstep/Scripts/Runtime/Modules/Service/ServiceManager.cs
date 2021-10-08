using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Network;
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
        INetworkChannel networkChannel;
        public void Connect(string ip, ushort port)
        {
            if (IsConnected)
                return;
            NetworkChannelKey channelKey = new NetworkChannelKey("Lockstep", $"{ip}:{port}");
            CosmosEntry.NetworkManager.AbortChannel(channelKey);
            networkChannel = new KCPClientChannel("Lockstep", ip, port);
            networkChannel.OnReceiveData += OnReceiveDataHandle;
            networkChannel.OnDisconnected += OnDisconnectedHandle;
            networkChannel.OnConnected += OnConnectedHandle;
            networkChannel.Connect();
            CosmosEntry.NetworkManager.AddChannel(networkChannel);
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
            networkChannel.SendMessage(data, 0);
        }
        void OnReceiveDataHandle(int conv, byte[] data)
        {
            onReceiveData?.Invoke(data);
        }
        void OnDisconnectedHandle(int conv)
        {
            IsConnected = false;
            onDisconnected?.Invoke();
        }
        void OnConnectedHandle(int conv)
        {
            IsConnected = true;
            onConnected?.Invoke();
        }
    }
}
