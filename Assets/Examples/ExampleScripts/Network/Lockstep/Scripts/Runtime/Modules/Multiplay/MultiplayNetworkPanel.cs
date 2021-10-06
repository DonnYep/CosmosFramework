using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Network;
using System.IO;
namespace Cosmos.Lockstep
{
    public class MultiplayNetworkPanel : MonoBehaviour
    {
        Button btnConnect;
        Button btnDisconnect;
        InputField iptHost;
        protected void Awake()
        {
            btnConnect = transform.GetComponentInChildren<Button>("BtnConnect");
            btnConnect.onClick.AddListener(ConnectClick);
            btnDisconnect = transform.GetComponentInChildren<Button>("BtnDisconnect");
            btnDisconnect.onClick.AddListener(DisconnectClick);
            iptHost = transform.GetComponentInChildren<InputField>("IptHost");
        }
        void ConnectClick()
        {
            var hostStr = iptHost.text;
            var hosts = hostStr.Split(':');
            var ip = hosts[0];
            var port = ushort.Parse(hosts[1]);
            GameEntry.ServiceManager.Connect(ip, port);
        }
        void DisconnectClick()
        {
            GameEntry.ServiceManager.Disconnect();
        }
    }
}