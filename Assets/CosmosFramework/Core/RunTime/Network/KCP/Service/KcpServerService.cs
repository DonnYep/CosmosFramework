using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace kcp
{
    public class KcpServerService: KcpServiceBase
    {
        KcpServer server;
        public event Action<int> OnServerConnected
        {
            add { onServerConnected += value; }
            remove { onServerConnected -= value; }
        }
        Action<int> onServerConnected;

        public event Action<int, ArraySegment<byte>, int> OnServerDataReceived
        {
            add { onServerDataReceived += value; }
            remove { onServerDataReceived -= value; }
        }
        Action<int, ArraySegment<byte>, int> onServerDataReceived;

        public event Action<int, Exception> OnServerError
        {
            add { onServerError += value; }
            remove { onServerError -= value; }
        }
        Action<int, Exception> onServerError;

        public event Action<int> OnServerDisconnected
        {
            add { onServerDisconnected += value; }
            remove { onServerDisconnected -= value; }
        }
        Action<int> onServerDisconnected;

        public bool ServerActive { get { return server.IsActive(); } }
        public override void ServiceSetup()
        {
            server = new KcpServer(
    (connectionId) => onServerConnected?.Invoke(connectionId),
    (connectionId, message) => onServerDataReceived?.Invoke(connectionId, message, (byte)KcpChannel.Reliable),
    (connectionId) => onServerDisconnected?.Invoke(connectionId),
    NoDelay,
    Interval,
    FastResend,
    CongestionWindow,
    SendWindowSize,
    ReceiveWindowSize
);
        }
        public override void ServiceConnect(string address="localhost") 
        { server.Start(Port); }
        public override void ServiceSend( KcpChannel channelId, ArraySegment<byte> segment, int connectionId)
        {
            // switch to kcp channel.
            // unreliable or reliable.
            // default to reliable just to be sure.
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    server.Send(connectionId, segment, KcpChannel.Unreliable);
                    break;
                default:
                    server.Send(connectionId, segment, KcpChannel.Reliable);
                    break;
            }
        }
        public override void  ServiceDisconnect(int connectionId)
        {
            server.Disconnect(connectionId);
        }
        public void ServerServiceStop() { server.Stop(); }
        public override void ServiceTick()
        {
            server.Tick();
        }
        public override void ServiceUnpause()
        {
            server.Unpause();
        }
        public override void ServicePause()
        {
            server.Pause();
        }
    }
}
