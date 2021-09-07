using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using Cosmos.Network;
using System.Configuration;

namespace Cosmos
{
    /// <summary>
    /// 通信协议基类
    /// </summary>
    public abstract class NetworkChannel:IBehaviour,IRefreshable
    {
        #region Properties
        //TODO NetworkChannel
        public Socket Peer { get; protected set; }
        /// <summary>
        /// 通信协议，TCP/UDP
        /// </summary>
        public virtual ProtocolType Protocol { get { return ProtocolType.Tcp; } }
        /// <summary>
        /// 信息模式，Stream/Dgram
        /// </summary>
        public virtual SocketType SocketType { get { return SocketType.Stream; } }
        /// <summary>
        /// 是否保持长连接
        /// </summary>
        public virtual bool IsNeedConnect { get { return true; } }
        protected List<byte[]> sendDataBuffer = new List<byte[]>();
        bool isCanSend = false;
        /// <summary>
        /// 通道建立连接事件
        /// </summary>
        public Action<NetworkChannel,object> NetworkChannelOnConnected { get; set; }
        /// <summary>
        /// 通道接收消息事件
        /// </summary>
        public Action<NetworkChannel,INetworkMessage> NetworkChannelOnReceive { get; set; }
        /// <summary>
        /// 通道关闭事件
        /// </summary>
        public Action<NetworkChannel> NetworkChannelOnClose { get; set; }
        ///// <summary>
        ///// 通道丢失心跳事件
        ///// </summary>
        //public Action<NetworkChannel,int> NetworkChannelOnMissHeartBeat { get; set; }
        ///// <summary>
        ///// 通道错误事件;
        ///// 通道，自定义通道错误码，socketError，错误消息
        ///// </summary>
        //public Action<NetworkChannel,byte,SocketError,string> NetworkChannelOnError { get; set; }
        ///// <summary>
        ///// 通道自定义错误事件
        ///// </summary>
        //public Action<NetworkChannel,object> NetworkChannelOnCustomError { get; set; }
        public bool IsConnect
        {
            get
            {
                try
                {
                    return Peer != null && Peer.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        #endregion
        #region Methods
        public virtual void OnInitialization()
        {
            if (!IsNeedConnect)
            {
                Peer = new Socket(AddressFamily.InterNetwork, SocketType, Protocol);

            }
        }
        public virtual void OnTermination()
        {
            sendDataBuffer.Clear();
            isCanSend = false;
        }
        /// <summary>
        /// 非空虚函数；
        /// 轮询事件,在主线程中调用;
        /// </summary>
        public virtual void OnRefresh()
        {
            //只有建立连接之后才进行消息收发
            if (IsConnect)
            {
                SendMessage();
                ReceiveMessage();
            }
        }
        public override string ToString()
        {
            return  Utility.Text.Format(Protocol.ToString() , "协议通道");
        }
        /// <summary>
        /// 建立远程连接
        /// </summary>
        /// <param name="iPAddress">远程主机IP地址</param>
        /// <param name="port">远程主机端口</param>
        /// <param name="userData">用户数据</param>
        public void Connect(IPAddress iPAddress,int port,object userData)
        {
            if (Peer !=null)
            {
                Disconnect();
                Peer = null;
            }
            Peer = new Socket(AddressFamily.InterNetwork, SocketType, Protocol);
            Peer.Connect(iPAddress, port);
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (Peer != null)
            {
                if (IsNeedConnect && IsConnect)
                {
                    Peer.Shutdown(SocketShutdown.Both);
                    Peer.Disconnect(false);
                }
                Peer.Close();
                Peer.Dispose();
                Peer = null;
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
        /// 对消息进行编码
        /// </summary>
        /// <param name="message">消息体</param>
        /// <returns>编码后的消息数组</returns>
        public abstract byte[] EncodingMessage(INetworkMessage message);
        /// <summary>
        /// 接收网络消息，并对消息进行解码
        /// </summary>
        /// <param name="client">连接的客户端</param>
        /// <returns>解码后的消息对象</returns>
        public abstract INetworkMessage ReceiveMessage(Socket client);
        /// <summary>
        /// 轮询事件发送网络消息
        /// </summary>
        protected abstract void SendMessage();
        /// <summary>
        /// 轮询事件接收网络消息
        /// </summary>
        protected abstract void ReceiveMessage();
        #endregion
    }
}