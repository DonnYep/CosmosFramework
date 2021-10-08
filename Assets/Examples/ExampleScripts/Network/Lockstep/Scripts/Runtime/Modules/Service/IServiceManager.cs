using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace Cosmos.Lockstep
{
    public interface IServiceManager : IModuleManager
    {
        bool IsConnected { get; }
        string IP { get; }
        ushort Port { get; }
        event Action OnConnected;
        event Action OnDisconnected;
        event Action<byte[]> OnReceiveData;
        void Connect(string ip, ushort port);
        void Disconnect();
        void SendMessage(byte[] data);
    }
}
