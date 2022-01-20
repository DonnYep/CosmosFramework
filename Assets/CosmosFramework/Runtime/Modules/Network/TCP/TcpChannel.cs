using System;
using System.Net.Sockets;

namespace Cosmos
{
    public class TcpChannel : NetworkChannel
    {
        public override ProtocolType Protocol { get { return ProtocolType.Tcp; } }
        public override SocketType SocketType { get { return SocketType.Stream; } }
        public override bool IsNeedConnect { get { return true; } }
        public override INetworkMessage ReceiveMessage(Socket client)
        {
            throw new NotImplementedException();
        }
        public override byte[] EncodingMessage(INetworkMessage message)
        {
            throw new NotImplementedException();
        }
        protected override void SendMessage()
        {
            throw new NotImplementedException();
        }
        protected override void ReceiveMessage()
        {
            throw new NotImplementedException();
        }
    }
}
