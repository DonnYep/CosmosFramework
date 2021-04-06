using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kcp;
using System;
using System.Text;

namespace Cosmos.Test
{
    public class MovementSphereManager : MonoSingleton<MovementSphereManager>
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

        Dictionary<int, NetworkIndentity> networkIdentityDict;
        List<NetworkIndentity> netIdCache;
        NetworkWriter writer;
        Dictionary<int, Pool<NetworkWriter>> writePool;

        Action onConnect;
        public event Action OnConnect
        {
            add { onConnect += value; }
            remove { onConnect -= value; }
        }
        Action onDisconnect;
        public event Action OnDisconnect
        {
            add { onDisconnect += value; }
            remove { onDisconnect -= value; }
        }

        Action<int> onPlayerEnter;
        public event Action<int> OnPlayerEnter
        {
            add { onPlayerEnter += value; }
            remove { onPlayerEnter -= value; }
        }
        Action<int> onPlayerExit;
        public event Action<int> OnPlayerExit
        {
            add { onPlayerExit += value; }
            remove { onPlayerExit -= value; }
        }
        public int AuthorityConv { get; private set; }
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
        protected override void Awake()
        {
            base.Awake();
            netIdCache = new List<NetworkIndentity>();
            networkIdentityDict = new Dictionary<int, NetworkIndentity>();
        }
        void Start()
        {
            CosmosEntry.NetworkManager.OnReceiveData += OnReceiveDataHandler;
            CosmosEntry.NetworkManager.OnDisconnect += OnDisconnectHandler;
        }
        void OnReceiveDataHandler(byte[] buffer)
        {
            var json = Encoding.UTF8.GetString(buffer);
            var opData = Utility.Json.ToObject<OperationData>(json);
            ProcessHandler(opData);
        }
        void OnDisconnectHandler()
        {
            AuthorityConv = 0;
            onDisconnect?.Invoke();
        }
        void ProcessHandler(OperationData opData)
        {
            var opCode = (OperationCode)opData.OperationCode;
            switch (opCode)
            {
                case OperationCode.PlayerEnter:
                    {
                        var messageDict = opData.DataContract as Dictionary<byte, object>;
                        var authorityConv = Utility.GetValue(messageDict, (byte)ParameterCode.AuthorityConv);
                        AuthorityConv = Convert.ToInt32(authorityConv);

                        onConnect?.Invoke();

                        var remoteConvs= Utility.GetValue(messageDict, (byte)ParameterCode.RemoteConvs) as List<int>;
                        var length = remoteConvs.Count;
                        for (int i = 0; i < length; i++)
                        {
                            onPlayerEnter(remoteConvs[i]);
                        }
                    }
                    break;
                case OperationCode.PlayerExit:
                    {
                        var exitNetId = Convert.ToInt32(opData.DataContract);
                        onPlayerExit?.Invoke(exitNetId);
                    }
                    break;
                case OperationCode.PlayerInput:
                    {

                    }
                    break;
                case OperationCode.FIN:
                    {
                        Utility.Debug.LogError(opData.DataContract);
                        Disconnect();
                    }
                    break;
            }
        }
    }
}