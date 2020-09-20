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
    NetClient netClient;
    bool isConnected;
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
        netClient = new NetClient();
        netClient.NetworkConnect += ConnectCallback;
        netClient.NetworkDisconnect += DisconnectCallback;
    }
    void ConnectClick()
    {
        netClient.Connect(ip, port);
    }
    void DisconnectClick()
    {
        netClient.Disconnect();
    }
    void SendClick()
    {
        string str = inputMsg.text;
        var data = Utility.Encode.ConvertToByte(str);
        Facade.SendNetworkMessage(10, data);
    }
    void ServerMsg(INetworkMessage netMsg)
    {
        Utility.Debug.LogInfo($"{ Utility.Converter.GetString((netMsg as UdpNetMessage).ServiceMsg)}");
    }
    void ConnectCallback()
    {
        netClient.RunHeartBeat(heartbeatInterval, 5);
        Utility.Debug.LogInfo("NetworkPanel回调，连接成功！");
        isConnected = true;
    }
    void DisconnectCallback()
    {
        Utility.Debug.LogInfo("NetworkPanel回调，与服务器断开链接！");
        isConnected = false;
    }
    protected override void OnTermination()
    {
        if (isConnected)
            netClient.Disconnect();
    }
}