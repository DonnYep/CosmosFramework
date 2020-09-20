using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Cosmos;
using Cosmos.Reference;
using TMPro;

namespace Cosmos.Network
{
    /// <summary>
    /// UDP socket服务；
    /// 这里管理其他接入的远程对象；
    /// </summary>
    public abstract class UdpService : INetworkService
    {
        /// <summary>
        /// 由服务器分配的会话ID
        /// </summary>
        public long Conv { get; protected set; } = 0;
        public event Action OnConnect
        {
            add { onConnect += value;}
            remove
            {
                try
                {
                    onConnect -= value;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
        }
        public event Action OnDisconnect
        {
            add { onDisconnect += value; }
            remove
            {
                try
                {
                    onDisconnect -= value;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// udpSocket对象
        /// </summary>
        protected UdpClient udpSocket;
        /// <summary>
        /// IP对象；
        /// </summary>
        protected IPEndPoint serverEndPoint;
        protected ConcurrentQueue<UdpReceiveResult> awaitHandle = new ConcurrentQueue<UdpReceiveResult>();
        protected Action onConnect;
        protected Action onDisconnect;
        public UdpService()
        {
            //构造传入0表示接收任意端口收发的数据
            udpSocket = new UdpClient(0);
        }
        public virtual void SetHeartbeat(IHeartbeat heartbeat) { }
        /// <summary>
        /// 异步接收网络消息接口
        /// </summary>
        public virtual async void OnReceive()
        {
            if (udpSocket != null)
            {
                try
                {
                    UdpReceiveResult result = await udpSocket.ReceiveAsync();
                    awaitHandle.Enqueue(result);
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"Receive net KCP_MSG exception ：{e}");
                }
            }
        }
        /// <summary>
        /// 非空虚函数;
        /// 发送报文信息；
        /// 发送给特定的endpoint对象，可不局限于一个服务器点；
        /// </summary>
        /// <param name="netMsg">消息体</param>
        /// <param name="endPoint">远程对象</param>
        public virtual async void SendMessageAsync(INetworkMessage netMsg, IPEndPoint endPoint)
        {
            UdpNetMessage udpNetMsg = netMsg as UdpNetMessage;
            udpNetMsg.Conv = Conv;
            if (udpSocket != null)
            {
                try
                {
                    var buffer = udpNetMsg.GetBuffer();
                    int length = await udpSocket.SendAsync(buffer, buffer.Length, endPoint);
                    if (length != buffer.Length)
                    {
                        //消息未完全发送，则重新发送
                        SendMessageAsync(udpNetMsg, endPoint);
                    }
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"Send netKCP_MSG Exception:{e.Message}");
                }
            }
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        /// <param name="netMsg">消息体</param>
        public virtual void SendMessageAsync(INetworkMessage netMsg)
        {
            SendMessageAsync(netMsg, serverEndPoint);
        }
        /// <summary>
        /// 轮询更新;
        /// </summary>
        public abstract void OnRefresh();
        /// <summary>
        /// 建立网络连接；
        /// </summary>
        public abstract void Connect(string ip, int port);
        /// <summary>
        /// 断开网络连接
        /// </summary>
        public abstract void Disconnect();
    }
}
