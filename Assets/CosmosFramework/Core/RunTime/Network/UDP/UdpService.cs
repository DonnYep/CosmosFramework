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
    public class UdpService : INetworkService
    {
        /// <summary>
        /// 对象IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 对象端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 由服务器分配的会话ID
        /// </summary>
        public uint Conv { get; protected set; } = 0;
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Available { get; protected set; } = false;
        public Action OnConnect { get ; set; }
        public Action OnDisconnect { get ; set ; }

        /// <summary>
        /// udpSocket对象
        /// </summary>
        protected UdpClient udpSocket;
        /// <summary>
        /// IP对象；
        /// </summary>
        protected IPEndPoint serverEndPoint;
        protected ConcurrentQueue<UdpReceiveResult> awaitHandle = new ConcurrentQueue<UdpReceiveResult>();
        public UdpService()
        {
            //构造传入0表示接收任意端口收发的数据
            udpSocket = new UdpClient(0);
        }
        /// <summary>
        /// 非空虚函数；
        /// 开启这个服务；
        /// </summary>
        public virtual void OnActive()
        {
            Available = true;
            OnConnect?.Invoke();
        }
        public virtual void SetHeartbeat(IHeartbeat heartbeat){}
        /// <summary>
        /// 非空虚函数；
        /// 关闭这个服务；
        /// </summary>
        public virtual void OnDeactive()
        {
            Available = false;
            Conv = 0;
            OnDisconnect?.Invoke();
        }
        /// <summary>
        /// 异步接收网络消息接口
        /// </summary>
        public virtual async void OnReceive()
        {
            if (!Available)
                return;
            if (udpSocket != null)
            {
                try
                {
                    UdpReceiveResult result = await udpSocket.ReceiveAsync();
                    awaitHandle.Enqueue(result);
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"Receive net message exception ：{e}");
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
            if (!Available)
                return;
            UdpNetMessage udpNetMsg = netMsg as UdpNetMessage;
            udpNetMsg.Conv = Conv;
            Utility.Debug.LogInfo($"Send net message : {udpNetMsg} ");
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
                    Utility.Debug.LogError($"Send net message Exception:{e.Message}");
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
        /// 空虚函数；
        /// 轮询更新;
        /// </summary>
        public virtual void OnRefresh(){}


    }
}
