﻿using System;
using Telepathy;
namespace Cosmos.Network
{
    public class TcpClientChannel : INetworkClientChannel
    {
        Client client;
        Action onAbort;
        ///<inheritdoc/>
        public string ChannelName { get; set; }
        ///<inheritdoc/>
        public bool IsConnect { get { return client.Connected; } }
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action OnConnected
        {
            add { client.OnConnected += value; }
            remove { client.OnConnected -= value; }
        }
        event Action<byte[]> onDataReceived;
        public event Action<byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action OnDisconnected
        {
            add { client.OnDisconnected += value; }
            remove { client.OnDisconnected -= value; }
        }
        ///<inheritdoc/>
        public int Port { get; private set; }
        ///<inheritdoc/>
        public string IPAddress { get; private set; }
        public TcpClientChannel(string channelName)
        {
            this.ChannelName = channelName;
            client = new Client(TcpConstants.MaxMessageSize);
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogWarning(s);
            Log.Error = (s) => Utility.Debug.LogError(s);
        }
        ///<inheritdoc/>
        public void Connect(string ip, int port)
        {
            this.IPAddress = ip;
            this.Port = port;
            client.Connect(IPAddress, Port);
            client.OnData = OnReceiveDataHandler;
        }
        ///<inheritdoc/>
        public void TickRefresh()
        {
            client.Tick(100);
        }
        ///<inheritdoc/>
        public bool SendMessage(byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return client.Send(segment);
        }
        ///<inheritdoc/>
        public void Disconnect()
        {
            client.Disconnect();
            client.OnData = null;
            onDataReceived = null;
        }
        ///<inheritdoc/>
        public void AbortChannnel()
        {
            Disconnect();
            onAbort?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg)
        {
            onDataReceived?.Invoke(arrSeg.Array);
        }
    }
}
