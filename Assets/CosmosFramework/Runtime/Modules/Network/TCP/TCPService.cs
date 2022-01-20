﻿using System;
using System.Net;
using System.Net.Sockets;

namespace Cosmos
{
    public class TCPService : INetworkService
    {
        TcpClient tcpClient;
        TCPClientPeer tcpClientPeer;
        public long Conv { get; private set; }
        IReferencePoolManager referencePoolManager;
        public event Action<ArraySegment<byte>> OnReceiveData
        {
            add { onReceiveData += value; }
            remove { onReceiveData -= value; }
        }
        protected Action<ArraySegment<byte>> onReceiveData;
        public TCPService()
        {
            referencePoolManager = GameManager.GetModule<IReferencePoolManager>();
            tcpClientPeer = new TCPClientPeer();
        }
        public void Connect(string ip, int port)
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.ConnectAsync(ip, port);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        public void Disconnect()
        {
            try
            {
                tcpClient.Close();
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        public virtual async void OnReceive()
        {
            try
            {
                //缓存接收到的数据 byte[]
                byte[] buffer = new byte[4096];
                await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
                var  netMsg = referencePoolManager.Spawn<TcpNetMessage>();
                netMsg.DecodeMessage(buffer);
                if (netMsg.IsFull)
                {
                    tcpClientPeer.MessageHandler(netMsg);
                }
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
                tcpClient.Close();
            }
        }
        public void OnRefresh()
        {
        }
        public async void SendMessageAsync(INetworkMessage netMsg, IPEndPoint endPoint)
        {

        }
        public void SendMessageAsync(INetworkMessage netMsg)
        {
            SendMessageAsync(netMsg.EncodeMessage());
        }
        public async void SendMessageAsync(byte[] buffer)
        {
            if (tcpClient.Connected)
            {
                try
                {
                    await tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
                    Console.WriteLine("发送成功!");
                }
                catch (Exception e)
                {
                    tcpClient.Close();
                    Utility.Debug.LogError(e);
                }
            }
        }
        public void SetHeartbeat(IHeartbeat heartbeat)
        {
        }
    }
}
