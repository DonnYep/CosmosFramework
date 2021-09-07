using System;
using System.Collections.Generic;
 using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kcp
{
    public class KcpClientService:KcpServiceBase
    {
        KcpClient client;

        public Action OnClientConnected;

        public Action<ArraySegment<byte>, byte> OnClientDataReceived;

        public Action OnClientDisconnected;

        public bool Connected { get { return client.connected; } }

        public override void ServiceSetup()
        {
            client = new KcpClient(
    () => OnClientConnected?.Invoke(),
    (message) => OnClientDataReceived?.Invoke(message, (byte)KcpChannel.Reliable),
    () => OnClientDisconnected?.Invoke()
);
        }
        public override void ServiceConnect(string address)
        {
            client.Connect(address, Port, NoDelay, Interval, FastResend, CongestionWindow, SendWindowSize, ReceiveWindowSize);
        }
        public override void ServiceDisconnect(int connectionId=0)
        {
            client.Disconnect();
        }
        public override void ServiceSend(KcpChannel channelId, ArraySegment<byte> segment, int connectionId=0)
        {
            // switch to kcp channel.
            // unreliable or reliable.
            // default to reliable just to be sure.
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    client.Send(segment, KcpChannel.Unreliable);
                    break;
                default:
                    client.Send(segment, KcpChannel.Reliable);
                    break;
            }
        }
        public override void ServiceTick()
        {
            client.Tick();
        }
        public override void ServiceUnpause()
        {
            client.Unpause();
        }
        public override void ServicePause()
        {
            client.Pause();
        }
    }
}
