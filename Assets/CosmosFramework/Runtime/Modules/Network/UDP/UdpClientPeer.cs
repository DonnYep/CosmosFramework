using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Cosmos.Network
{
    /// <summary>
    /// 停等ARQ协议
    /// </summary>
    public class UdpClientPeer : IRemotePeer, IRefreshable
    {
        /// <summary>
        /// 服务器分配的会话ID
        /// </summary>
        public long Conv { get; set; }
        /// <summary>
        /// 分配的endPoint
        /// </summary>
        public IPEndPoint PeerEndPoint { get; private set; }
        /// <summary>
        /// 处理的message序号，按1累次递增。
        /// </summary>
        public uint HandleSN { get; set; }
        /// <summary>
        /// 已经发送的消息序号
        /// </summary>
        public uint SendSN { get; set; }
        /// <summary>
        /// 下一个需要发送的序号
        /// </summary>
        public uint Snd_nxt { get; set; }
        /// <summary>
        /// 当前Peer是否处于连接状态
        /// </summary>
        public bool Available { get; private set; }
        /// <summary>
        /// 心跳机制
        /// </summary>
        public IHeartbeat Heartbeat { get; set; }
        /// <summary>
        /// 并发发送消息的字典；
        /// 整理错序报文；
        /// 临时起到ACK缓存的作用
        /// </summary>
        protected ConcurrentDictionary<uint, UdpNetMessage> sndMsgDict;
        /// <summary>
        /// 收到的待处理的错序报文
        /// </summary>
        protected ConcurrentDictionary<uint, UdpNetMessage> rcvMsgDict;
        /// <summary>
        /// 最后一次更新时间；
        /// 更新的时间戳；
        /// </summary>
        protected long latestPollingTime;
        /// <summary>
        /// 发送网络消息委托；
        /// 这里函数指针指向service的sendMessage
        /// </summary>
        Action<INetworkMessage> sendMessageHandler;
        Action onConnectHandler;
        Action onDisconnectHandler;
        Action<ArraySegment<byte>> onReceiveDataHandler;
        INetworkManager networkManager;
        public UdpClientPeer()
        {
            //Available = true;
            sndMsgDict = new ConcurrentDictionary<uint, UdpNetMessage>();
            rcvMsgDict = new ConcurrentDictionary<uint, UdpNetMessage>();
            networkManager = GameManager.GetModule<INetworkManager>();
        }
        public UdpClientPeer(Action onConnect, Action onDisconnect, Action<ArraySegment<byte>> onReceive) : this()
        {
            this.onConnectHandler = onConnect;
            this.onDisconnectHandler = onDisconnect;
            this.onReceiveDataHandler = onReceive;
        }
        public UdpClientPeer(uint conv) : this()
        {
            this.Conv = conv;
        }
        public void OnActive()
        {
            sndMsgDict.Clear();
            rcvMsgDict.Clear();
            Available = true;
            latestPollingTime = Utility.Time.MillisecondNow() + NetworkConsts.Interval;
        }
        public void OnDeactive()
        {
            Available = false;
            Conv = 0;
            HandleSN = 0;
            SendSN = 0;
            latestPollingTime = 0;
            if (Heartbeat != null)
            {
                Heartbeat.Conv = 0;
                Heartbeat.OnDeactive();
            }
            sndMsgDict.Clear();
            rcvMsgDict.Clear();
        }
        public void SetValue(Action<INetworkMessage> sendMsgCallback, Action abortPeerCallback, uint conv, IPEndPoint endPoint)
        {
            this.Conv = conv;
            this.PeerEndPoint = endPoint;
            this.sendMessageHandler += sendMsgCallback;
        }
        /// <summary>
        /// 为这个Peer分配会话ID；
        /// </summary>
        /// <param name="conv">会话ID</param>
        public void AllocateConv(long conv)
        {
            Conv = conv;
            Heartbeat.Conv = conv;
            Heartbeat.OnActive();
        }
        /// <summary>
        /// 空虚函数
        /// 发送消息给这个peer的远程对象
        /// </summary>
        /// <param name="netMsg">消息体</param>
        public virtual void SendMessage(INetworkMessage netMsg)
        {
            if (Available)
                sendMessageHandler?.Invoke(netMsg);
        }
        /// <summary>
        /// 处理进入这个peer的消息；
        /// 由service分发消息到这个方法；
        /// </summary>
        /// <param name="msg">消息体</param>
        public virtual void MessageHandler(INetworkMessage msg)
        {
            UdpNetMessage netMsg = msg as UdpNetMessage;
            switch (netMsg.Cmd)
            {
                //ACK报文
                case UdpProtocol.ACK:
                    {
                        UdpNetMessage tmpMsg;
                        if (netMsg.HeaderCode == UdpHeader.Hearbeat)
                            Heartbeat.OnRenewal();
                        if (netMsg.HeaderCode == UdpHeader.Connect)
                        {
                            OnActive();
                            onConnectHandler?.Invoke();
                        }
                        if (netMsg.HeaderCode == UdpHeader.Disconnect)
                        {
                            onDisconnectHandler?.Invoke();
                            OnDeactive();
                        }
                        if (sndMsgDict.TryRemove(netMsg.SN, out tmpMsg))
                            ReferencePool.Release(tmpMsg);
                    }
                    break;
                case UdpProtocol.MSG:
                    {
                        //生成一个ACK报文，并返回发送
                        var ack = UdpNetMessage.ConvertToACK(netMsg);
                        //这里需要发送ACK报文
                        sendMessageHandler?.Invoke(ack);
                        //发送后进行原始报文数据的处理
                        HandleMsgSN(netMsg);
                    }
                    break;
                case UdpProtocol.SYN:
                    {
                        //建立连接标志
                        Utility.Debug.LogInfo($"Conv : {Conv} ,Receive SYN Message");
                        //生成一个ACK报文，并返回发送
                        var ack = UdpNetMessage.ConvertToACK(netMsg);
                        //这里需要发送ACK报文
                        sendMessageHandler?.Invoke(ack);
                    }
                    break;
                case UdpProtocol.FIN:
                    {
                        //结束建立连接Cmd，这里需要谨慎考虑；
                        Utility.Debug.LogError($"Conv : {Conv} ,Receive FIN Message");
                        onDisconnectHandler?.Invoke();
                        OnDeactive();
                    }
                    break;
            }
        }
        /// <summary>
        /// 轮询更新，创建Peer对象时候将此方法加入监听；
        /// </summary>
        public void OnRefresh()
        {
            Heartbeat?.OnRefresh();
            long now = Utility.Time.MillisecondNow();
            if (now <= latestPollingTime)
                return;
            latestPollingTime = now + NetworkConsts.Interval;
            if (!Available)
                return;
            foreach (var msg in sndMsgDict.Values)
            {
                if (msg.RecurCount >= NetworkConsts.RTO_DEF)
                {
                    Available = false;
                    onDisconnectHandler?.Invoke();
                    return;
                }
                //重发次数+1
                msg.RecurCount += 1;
                //绕过编码消息，直接发送；
                networkManager.SendNetworkMessage(msg.GetBuffer());
            }
        }
        /// <summary>
        /// 对网络消息进行编码
        /// </summary>
        /// <param name="netMsg">生成的消息</param>
        /// <returns>是否编码成功</returns>
        public bool EncodeMessage(ref UdpNetMessage netMsg)
        {
            netMsg.TS = Utility.Time.MillisecondTimeStamp();
            if (netMsg.HeaderCode != UdpHeader.Hearbeat)
            {
                SendSN += 1;
                netMsg.SN = SendSN;
            }
            netMsg.Conv = Conv;
            netMsg.EncodeMessage();
            bool result = true;
            if (Conv != 0)
            {
                //若会话ID不为0，则缓存入ACK容器中，等接收成功后进行移除
                try
                {
                    if (netMsg.Cmd == UdpProtocol.MSG)
                        sndMsgDict.TryAdd(netMsg.SN, netMsg);
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
            return result;
        }

        public void Clear()
        {
            Available = false;
            Conv = 0;
            PeerEndPoint = null;
            HandleSN = 0;
            SendSN = 0;
            latestPollingTime = 0;
            sendMessageHandler = null;
            sndMsgDict.Clear();
            Heartbeat.Clear();
        }
        /// <summary>
        /// 处理报文序号
        /// </summary>
        /// <param name="netMsg">网络消息</param>
        protected void HandleMsgSN(UdpNetMessage netMsg)
        {
            //sn小于当前处理HandleSN则表示已经处理过的消息；
            if (netMsg.SN <= HandleSN)
            {
                return;
            }
            if (netMsg.SN - HandleSN > 1)
            {
                //对错序报文进行缓存
                rcvMsgDict.TryAdd(netMsg.SN, netMsg);
            }
            HandleSN = netMsg.SN;
            onReceiveDataHandler.Invoke(new ArraySegment<byte>(netMsg.ServiceData));
            //NetworkMsgEventCore.Instance.Dispatch(netMsg.OperationCode, netMsg);
            if (rcvMsgDict.TryRemove(HandleSN + 1, out var nxtNetMsg))
            {
                HandleMsgSN(nxtNetMsg);
            }
        }


    }
}
