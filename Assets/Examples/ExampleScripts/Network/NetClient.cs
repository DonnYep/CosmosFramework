using System.Collections;
using System.Collections.Generic;
using Cosmos;
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
                Utility.DebugError(e);
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
                Utility.DebugError(e);
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
                Utility.DebugError(e);
            }
        }
    }
    public NetClient()
    {
        MessagePacket.SetHelper(new MessagePacketJsonHelper());
    }
    public void SendMessage(MessagePacket packet)
    {
        var packetBuffer = MessagePacket.Serialize(packet);
        Facade.SendNetworkMessage(MessagePacketPort.GATE_MSG_PORT, packetBuffer);
    }
    public void Dispose()
    {
        Facade.NetworkOnConnect -= OnConnect;
        Facade.NetworkOnDisconnect -= OnDisconnect;
        networkReceive = null;
        networkConnect = null;
        networkDisconnect = null;
    }
    public void Connect(string ip, int port)
    {
        NetworkMessageEventCore.Instance.AddEventListener(MessagePacketPort.GATE_MSG_PORT, OnReceiveNetMessage);
        Facade.NetworkOnConnect += OnConnect;
        Facade.NetworkOnDisconnect += OnDisconnect;
        Facade.NetworkConnect(ip, port, System.Net.Sockets.ProtocolType.Udp);
    }
    public void Disconnect()
    {
        Facade.NetworkDisconnect();
    }
    public void RunHeartBeat(uint interval, byte maxRecur)
    {
        Facade.RunHeartbeat(interval, maxRecur);
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
