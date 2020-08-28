using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Cosmos
{
    public class KcpChannel : NetworkChannel
    {
        public override byte[] EncodingMessage(INetworkMessage message)
        {
            return null;
        }
        public override INetworkMessage ReceiveMessage(Socket client)
        {
            return null;
        }
        public override void OnRefresh()
        {
            base.OnRefresh();
        }
        protected override void ReceiveMessage()
        {
        }
        protected override void SendMessage()
        {
        }
    }
}
