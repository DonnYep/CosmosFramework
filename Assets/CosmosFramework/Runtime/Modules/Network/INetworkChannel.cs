using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Configuration;

namespace Cosmos.Network
{
    //================================================
    /*
    1、网络通道指当前local实例与remote进行建立网络连接的一个消息通道；
    
    2、当通道为sever时，则此通道作为服务器，管理进入的链接对象；
    
    3、当通道为clinet时，则通道作为client对remote进行链接。
    
    4、允许存在多通道，通道与通道之间通讯需要自定义实现；
    */
    //================================================
    /// <summary>
    /// 网络通道；
    /// </summary>
    public interface INetworkChannel
    {
        /// <summary>
        /// 建立连接回调；
        /// </summary>
        event Action<int> OnConnected;
        /// <summary>
        /// 断开连接回调；
        /// </summary>
        event Action<int> OnDisconnected;
        /// <summary>
        /// 接收数据回调；
        /// </summary>
        event Action<int,byte[]> OnReceiveData;
        /// <summary>
        /// 通道销毁事件；
        /// </summary>
        event Action OnAbort;
        /// <summary>
        /// 通道的唯一识别key；
        /// </summary>
        NetworkChannelKey NetworkChannelKey { get; }
        /// <summary>
        /// 是否已建立连接；
        /// </summary>
        bool IsConnect { get; }
        /// <summary>
        /// 与remote建立连接；
        /// </summary>
        void Connect();
        /// <summary>
        /// 发送数据到remote;
        /// 默认为可靠类型；
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="connectionId">连接Id</param>
        void SendMessage( byte[] data, int connectionId);
        /// <summary>
        /// 发送数据到remote;
        /// </summary>
        /// <param name="reliableType">数据可靠类型</param>
        /// <param name="data">数据</param>
        /// <param name="connectionId">连接Id</param>
        void SendMessage(NetworkReliableType reliableType, byte[] data, int connectionId);
        /// <summary>
        /// 断开连接；
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        void Disconnect(int connectionId=-1);
        /// <summary>
        /// 弃用&销毁通道；
        /// </summary>
        void Abort();
        /// <summary>
        /// 轮询消息通道；
        /// </summary>
        void TickRefresh();
        /// <summary>
        /// 获取连接Id的地址；
        /// </summary>
        /// <param name="connectionId">连接Id</param>
        /// <returns></returns>
        string GetConnectionAddress(int connectionId);
    }
}