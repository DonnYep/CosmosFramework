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
        InputField iptHost;

        protected override void Awake()
        {
            btnConnect = GetUILable<Button>("BtnConnect");
            btnConnect.onClick.AddListener(ConnectClick);
            btnDisconnect = GetUILable<Button>("BtnDisconnect");
            btnDisconnect.onClick.AddListener(DisconnectClick);
            iptHost = GetUILable<InputField>("IptHost");
        }
        void ConnectClick()
        {
            var hostStr = iptHost.text;
            var hosts = hostStr.Split(':');
            MultiplayManager.Instance.IP = hosts[0];
            MultiplayManager.Instance.Port= int.Parse( hosts[1]);
            MultiplayManager.Instance.Connect();
        }
        void DisconnectClick()
        {
            MultiplayManager.Instance.Disconnect();
        }
    }
}