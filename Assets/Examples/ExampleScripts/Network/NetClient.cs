using System.Collections;
using System.Collections.Generic;
using Cosmos;
using Cosmos.Network;
using System;
public class NetClient : IDisposable
{
    Action networkConnect;
    Action networkDisconnect;
    Action<MessagePacket> networkReceive;
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
    public event Action<MessagePacket> NetworkReceive
    {
        add
        {
            networkReceive += value;
        }
        remove
        {
            try
            {
                networkReceive -= value;
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
    }
    INetworkManager networkManager;
    public NetClient()
    {
        MessagePacket.SetHelper(new MessagePacketJsonHelper());
        networkManager = GameManager.GetModule<INetworkManager>();
    }
    public void SendMessage(MessagePacket packet)
    {
        var packetBuffer = MessagePacket.Serialize(packet);
        networkManager.SendNetworkMessage(MessagePacketPort.GATE_MSG_PORT, packetBuffer);
    }
    public void Dispose()
    {
        networkManager.NetworkOnConnect -= OnConnect;
        networkManager.NetworkOnDisconnect -= OnDisconnect;
        networkReceive = null;
        networkConnect = null;
        networkDisconnect = null;
    }
    public void Connect(string ip, int port)
    {
        NetworkMsgEventCore.Instance.AddEventListener(MessagePacketPort.GATE_MSG_PORT, OnReceiveNetMessage);
        networkManager.NetworkOnConnect += OnConnect;
        networkManager.NetworkOnDisconnect += OnDisconnect;
        networkManager.Connect(ip, port, System.Net.Sockets.ProtocolType.Udp);
    }
    public void Disconnect()
    {
        networkManager.Disconnect();
    }
    public void RunHeartBeat(uint interval, byte maxRecur)
    {
        networkManager.RunHeartbeat(interval, maxRecur);
    }
    /// <summary>
    /// 空虚函数
    /// </summary>
    /// <param name="packet">消息体</param>
    protected virtual void OnReceiveMessage(MessagePacket packet) { }
    void OnReceiveNetMessage(INetworkMessage netMsg)
    {
        var packet = MessagePacket.Deserialize(netMsg.ServiceMsg);
        OnReceiveMessage(packet);
        networkReceive?.Invoke(packet);
    }
    void OnConnect()
    {
        networkConnect?.Invoke();
    }
    void OnDisconnect()
    {
        networkDisconnect?.Invoke();
    }
}
