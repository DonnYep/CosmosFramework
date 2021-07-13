using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Network;
using System.IO;
namespace Cosmos.Test
{
    public class MultiplayNetworkPanel : UIForm
    {
        Button btnConnect;
        Button btnDisconnect;
        InputField inputMsg;

        protected override void Awake()
        {
            btnConnect = GetUILable<Button>("BtnConnect");
            btnConnect.onClick.AddListener(ConnectClick);
            btnDisconnect = GetUILable<Button>("BtnDisconnect");
            btnDisconnect.onClick.AddListener(DisconnectClick);
            inputMsg = GetUILable<InputField>("InputMsg");
        }
        void ConnectClick()
        {
            MultiplayManager.Instance.Connect();
        }
        void DisconnectClick()
        {
            MultiplayManager.Instance.Disconnect();
        }
    }
}