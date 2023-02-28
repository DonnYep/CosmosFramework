using System;

namespace Cosmos.Lockstep
{
    public interface IServiceManager : IModuleManager
    {
        bool IsConnected { get; }
        string Host { get; }
        ushort Port { get; }
        event Action OnConnected;
        event Action OnDisconnected;
        event Action<byte[]> OnReceiveData;
        void Connect(string host, ushort port);
        void Disconnect();
        void SendMessage(byte[] data);
    }
}
