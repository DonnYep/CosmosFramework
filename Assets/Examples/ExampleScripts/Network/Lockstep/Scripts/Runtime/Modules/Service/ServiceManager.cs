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
        Action<int> onConnected;
        public event Action<int> OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        Action<int> onDisconnected;
        public event Action<int> OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        Action<int, byte[]> onReceiveData;
        public event Action<int, byte[]> OnReceiveData
        {
            add { onReceiveData += value; }
            remove { onReceiveData -= value; }
        }
        INetworkChannel networkChannel;
        public void Connect(string ip, ushort port)
        {
            if (IsConnected)
                return;
            networkChannel = new KCPClientChannel("KCP", ip, port);
            if(CosmosEntry.NetworkManager.RemoveChannel(networkChannel.NetworkChannelKey,out var cnl))
            {
                cnl.Abort();
            }
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
            onReceiveData?.Invoke(conv, data);
        }
        void OnDisconnectedHandle(int conv)
        {
            IsConnected = false;
            onDisconnected?.Invoke(conv);
        }
        void OnConnectedHandle(int conv)
        {
            IsConnected = true;
            onConnected?.Invoke(conv);
        }
    }
}
