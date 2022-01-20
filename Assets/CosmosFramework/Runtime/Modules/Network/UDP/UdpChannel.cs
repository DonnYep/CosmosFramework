﻿using System;
using System.Net.Sockets;

namespace Cosmos
{
    public class UdpChannel : NetworkChannel
    {
        public override ProtocolType Protocol { get { return ProtocolType.Udp; } }
        public override SocketType SocketType { get { return SocketType.Dgram; } }
        public override bool IsNeedConnect { get { return false; } }
        public override byte[] EncodingMessage(INetworkMessage message)
        {
            UdpNetMessage udpNetMsg = message as UdpNetMessage;
            return udpNetMsg.EncodeMessage();
        }
        public override INetworkMessage ReceiveMessage(Socket client)
        {
            try
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void SendMessage()
        {
        }

        protected override void ReceiveMessage()
        {
        }
    }
}
