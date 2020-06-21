using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using Cosmos.Network;
namespace Cosmos
{
    /// <summary>
    /// 通信协议基类
    /// </summary>
    public abstract class ProtocolChannel
    {
        //TODO ProtocolChannel
        public Socket Client { get; private set; }
        /// <summary>
        /// 通信协议，TCP/UDP
        /// </summary>
        public virtual ProtocolType Protocol { get { return ProtocolType.Tcp; } }
        /// <summary>
        /// 信息模式，Stream/Dgram
        /// </summary>
        public virtual SocketType MsgMode { get { return SocketType.Stream; } }
        public virtual bool IsNeedConnect { get { return true; } }
        List<byte[]> sendDataBuffer = new List<byte[]>();
        bool isCanSend = false;
        public bool IsConnect
        {
            get
            {
                try
                {
                    return Client != null && Client.Connected;
                }
                catch 
                {
                    throw new ArgumentNullException("Client is invalid.");
                }
            }
        }
        public override string ToString()
        {
            return  Utility.Text.Format(Protocol.ToString() , "协议通道");
        }
        public void ConnectServer()
        {
            if (Client == null)
            {
                Client = new Socket(AddressFamily.InterNetwork, MsgMode, Protocol);
                Client.Connect(GameManager.NetworkManager.ServerEndPoint);
            }
        }
        public void DisconnectServer()
        {
            if (Client != null)
            {
                if (IsNeedConnect && IsConnect)
                {
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Disconnect(false);
                }
                Client.Close();
                Client.Dispose();
                Client = null;
            }
        }
        /// <summary>
        /// 消息注入
        /// </summary>
        /// <param name="msg"></param>
        public void InjectMessage(byte[] msg)
        {
            isCanSend = false;
            sendDataBuffer.Add((msg));
            isCanSend = true;
        }
        /// <summary>
        /// 封装消息，包头包体
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract byte[] EncapsulatedMessage(INetworkMessage message);
        public abstract INetworkMessage ReceiveMessage(Socket client);
    }
}