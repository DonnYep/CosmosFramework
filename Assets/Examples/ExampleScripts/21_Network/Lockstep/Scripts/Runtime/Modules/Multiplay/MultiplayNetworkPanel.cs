using UnityEngine;
using UnityEngine.UI;
namespace Cosmos.Lockstep
{
    public class MultiplayNetworkPanel : MonoBehaviour
    {
        Button btnConnect;
        Button btnDisconnect;
        InputField iptHost;
        protected void Awake()
        {
            btnConnect = gameObject.GetComponentInChildren<Button>("BtnConnect");
            btnConnect.onClick.AddListener(ConnectClick);
            btnDisconnect = gameObject.GetComponentInChildren<Button>("BtnDisconnect");
            btnDisconnect.onClick.AddListener(DisconnectClick);
            iptHost = gameObject.GetComponentInChildren<InputField>("IptHost");
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