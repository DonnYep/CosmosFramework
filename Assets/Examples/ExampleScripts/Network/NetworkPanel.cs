using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Network;
using System.IO;

public class NetworkPanel : UIResidentForm
{
    Button btnConnect;
    Button btnDisconnect;
    Button btnSend;
    Text info;
    InputField inputMsg;
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] ushort port = 8511;
    protected override void OnInitialization()
    {
        btnConnect = GetUIForm<Button>("BtnConnect");
        btnConnect.onClick.AddListener(ConnectClick);
        btnDisconnect = GetUIForm<Button>("BtnDisconnect");
        btnDisconnect.onClick.AddListener(DisconnectClick);
        btnSend = GetUIForm<Button>("BtnSend");
        btnSend.onClick.AddListener(SendClick);
        info = GetUIForm<Text>("Info");
        inputMsg = GetUIForm<InputField>("InputMsg");
        CosmosEntry.NetworkManager.OnConnect += ConnectCallback;
        CosmosEntry.NetworkManager.OnDisconnect += DisconnectCallback;
    }
    void ConnectClick()
    {
        CosmosEntry.NetworkManager.Connect(ip, port);
    }
    void DisconnectClick()
    {
        CosmosEntry.NetworkManager.Disconnect();
    }
    void SendClick()
    {
        string str = inputMsg.text;
        var data = Utility.Converter.ConvertToByte(str);
        CosmosEntry.NetworkManager.SendNetworkMessage(data);
    }
    void ServerMsg(INetworkMessage netMsg)
    {
        Utility.Debug.LogInfo($"{ Utility.Converter.GetString((netMsg as UdpNetMessage).ServiceData)}");
    }
    void ConnectCallback()
    {
        Utility.Debug.LogInfo("NetworkPanel回调，连接成功！");
    }
    void DisconnectCallback()
    {
        Utility.Debug.LogInfo("NetworkPanel回调，与服务器断开链接！");
    }
    protected override void OnTermination()
    {
        CosmosEntry.NetworkManager.Disconnect();
    }
}