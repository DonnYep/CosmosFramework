using Cosmos.Network;
using Cosmos.Reference;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cosmos
{
    public class UdpClientService : UdpService
    {
        /// <summary>
        /// 轮询委托
        /// </summary>
        UdpClientPeer peer;

        public UdpClientService():base()
        {
            //构造传入0表示接收任意端口收发的数据
            peer = new UdpClientPeer();
            peer.SetValue(SendMessage, OnDeactive, 0, null);
        }
        public override void OnActive()
        {
            base.OnActive();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            peer.OnActive();
            SendSYNMessage();
        }
        public override void OnDeactive()
        {
            SendFINMessage();
            peer.OnDeactive();
            base.OnDeactive();
        }
        public override void SendMessage(INetworkMessage netMsg)
        {
            if (!Available)
                return;
            UdpNetworkMessage udpNetMsg = netMsg as UdpNetworkMessage;
            var result = peer.EncodeMessage(ref udpNetMsg);
            if (result)
            {
               SendMessage(udpNetMsg, serverEndPoint);
            }
            else
                Utility.Debug.LogError("INetworkMessage 消息编码失败");
        }
        public override void OnRefresh()
        {
            if (!Available)
                return;
            OnReceive();
            peer.OnRefresh();
            if (awaitHandle.Count > 0)
            {
                UdpReceiveResult data;
                if (awaitHandle.TryDequeue(out data))
                {
                    UdpNetworkMessage netMsg = Facade.SpawnReference<UdpNetworkMessage>();
                    netMsg.CacheDecodeBuffer(data.Buffer);
                    if (Conv == 0)
                    {
                        Conv = netMsg.Conv;
                        peer.Conv = Conv;
                        peer.AllocateConv(Conv);
                    }
                    if (netMsg.IsFull)
                    {
                        peer.MessageHandler(netMsg);
                    }
                }
            }
        }
        void SendSYNMessage()
        {
            UdpNetworkMessage udpNetMsg = new UdpNetworkMessage(0,KcpProtocol.SYN); 
            SendMessage(udpNetMsg);
        }
        void SendFINMessage()
        {
            UdpNetworkMessage udpNetMsg = new UdpNetworkMessage(0, KcpProtocol.FIN);
            SendMessage(udpNetMsg);
        }
    }
}
