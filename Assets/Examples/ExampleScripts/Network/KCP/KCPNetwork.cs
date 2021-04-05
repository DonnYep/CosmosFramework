using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kcp;
using System;
using System.Text;

namespace Cosmos.Test
{
    public class KCPNetwork : MonoSingleton<KCPNetwork>
    {
        KcpClientService kcpClientService = new KcpClientService();
        public KcpClientService KcpClientService { get { return kcpClientService; } }
        [SerializeField] string ip = "127.0.0.1";
        public string IP { get { return ip; } }
        [SerializeField] int port = 8521;
        public int Port { get { return port; } }
        [SerializeField] GameObject localPlayerPrefab;
        public GameObject LocalPlayerPrefab { get { return localPlayerPrefab; } }
        [SerializeField] GameObject remotePlayerPrefab;
        public GameObject RemotePlayerPrefab { get { return remotePlayerPrefab; } }
        private void Start()
        {
            CosmosEntry.NetworkManager.OnReceiveData += NetworkManager_OnReceiveData;
        }
        void NetworkManager_OnReceiveData(byte[] buffer)
        {
            Utility.Debug.LogInfo(Encoding.UTF8.GetString(buffer));
        }
        public void SendKcpMessage(string msg)
        {
            var buffer = Encoding.UTF8.GetBytes(msg);
            CosmosEntry.NetworkManager.SendNetworkMessage(buffer);
        }
        public void Connect()
        {
            CosmosEntry.NetworkManager.Connect(IP, (ushort)Port);
        }
        public void Disconnect()
        {
            CosmosEntry.NetworkManager.Disconnect();
        }
        protected override void OnDestroy()
        {
            CosmosEntry.NetworkManager.Disconnect();
        }

    }
}