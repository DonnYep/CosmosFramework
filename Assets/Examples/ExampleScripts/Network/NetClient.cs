using System.Collections;
using System.Collections.Generic;
using Cosmos;
using Cosmos.Network;
using System;
public class NetClient : IDisposable
{
    Action networkConnect;
    Action networkDisconnect;
    Action<ArraySegment<byte>> networkReceiveData;
    public event Action NetworkConnect
    {
        add
        {
            networkConnect += value;
        }
        remove
        {
            try
            {
                networkConnect -= value;
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
    }
    public event Action NetworkDisconnect
    {
        add
        {
            networkDisconnect += value;
        }
        remove
        {
            try
            {
                networkDisconnect -= value;
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
    }
    public event Action<ArraySegment<byte>> NetworkReceiveData
    {
        add { networkReceiveData += value; }
        remove{ networkReceiveData -= value; }
    }

    INetworkManager networkManager;
    public NetClient()
    {
        MessagePacket.SetHelper(new MessagePacketJsonHelper());
        networkManager = CosmosEntry.NetworkManager;
    }
    public void SendMessage(MessagePacket packet)
    {
        var packetBuffer = MessagePacket.Serialize(packet);
        networkManager.SendNetworkMessage( packetBuffer);
    }
    public void Dispose()
    {
        networkManager.OnConnect -= OnConnect;
        networkManager.OnDisconnect -= OnDisconnect;
        networkConnect = null;
        networkDisconnect = null;
    }
    public void Connect(string ip, int port)
    {
        networkManager.OnConnect += OnConnect;
        networkManager.OnDisconnect += OnDisconnect;
        networkManager.OnReceiveData += OnReceiveData;
        networkManager.Connect(ip, (ushort)port, NetworkProtocolType.UDP);
    }
    public void Disconnect()
    {
        networkManager.Disconnect();
    }
    /// <summary>
    /// 空虚函数
    /// </summary>
    /// <param name="packet">消息体</param>
    protected virtual void OnReceiveMessage(MessagePacket packet) { }
    void OnConnect()
    {
        networkConnect?.Invoke();
    }
    void OnDisconnect()
    {
        networkDisconnect?.Invoke();
    }
    void OnReceiveData(ArraySegment<byte> arrSeg)
    {
        networkReceiveData?.Invoke(arrSeg);
    }
}
