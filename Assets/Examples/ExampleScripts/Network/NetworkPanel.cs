using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using System.IO;

public class NetworkPanel : UILogicResident
{
    Button btnConnect;
    Button btnDisconnect;
    Button btnSend;
    Text info;
    InputField inputMsg;
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 8511;
    [SerializeField] uint heartbeatInterval = 45;
    protected override void OnInitialization()
    {
        btnConnect = GetUIPanel<Button>("BtnConnect");
        btnConnect.onClick.AddListener(ConnectClick);
        btnDisconnect = GetUIPanel<Button>("BtnDisconnect");
        btnDisconnect.onClick.AddListener(DisconnectClick);
        btnSend = GetUIPanel<Button>("BtnSend");
        btnSend.onClick.AddListener(SendClick);
        info = GetUIPanel<Text>("Info");
        inputMsg = GetUIPanel<InputField>("InputMsg");
        NetworkMsgEventCore.Instance.AddEventListener(10, ServerMsg);
    }
    void ConnectClick()
    {
        Facade.NetworkConnect(ip,port,System.Net.Sockets.ProtocolType.Udp);
        Facade.RunHeartbeat(heartbeatInterval,5); 
    }
    void DisconnectClick()
    {
        Facade.NetworkDisconnect();
    }
    void SendClick()
    {
        string str = inputMsg.text;
        var data= Utility.Encode.ConvertToByte(str);
        Facade.SendNetworkMessage(10, data);

        //UdpNetMessage msg = new UdpNetMessage(0,0,KcpProtocol.MSG,10,data);
        //Facade.SendNetworkMessage(msg);
    }
    void ServerMsg(INetworkMessage netMsg)
    {
        var data = (netMsg as UdpNetMessage).ServiceMsg;
        if(data!=null)
        Utility.Debug.LogInfo($"{ Utility.Converter.GetString(data)}");
    }
}
