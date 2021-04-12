using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kcp;
using System;
using System.Text;

namespace Cosmos.Test
{
    public class MultiplayManager : MonoSingleton<MultiplayManager>
    {
        [SerializeField] string ip = "127.0.0.1";
        public string IP { get { return ip; } }
        [SerializeField] int port = 8531;
        public int Port { get { return port; } }
        [SerializeField] GameObject localPlayerPrefab;
        public GameObject LocalPlayerPrefab { get { return localPlayerPrefab; } }
        [SerializeField] GameObject remotePlayerPrefab;
        public GameObject RemotePlayerPrefab { get { return remotePlayerPrefab; } }

        public NetworkWriter NetworkWriter { get; private set; }

        OperationData authorityInputOpdata;

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

        Action<FixTransportData[]> onPlayerInput;
        public event Action<FixTransportData[]> OnPlayerInput
        {
            add { onPlayerInput += value; }
            remove { onPlayerInput -= value; }
        }

        public int AuthorityConv { get; private set; }
       public bool IsConnected { get; private set; }
        FixTransportData fixTransportData;
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
        public void SendAuthorityTransportData(FixTransportData transportData)
        {
            fixTransportData = transportData;
            authorityInputOpdata.DataMessage = Utility.Json.ToJson(fixTransportData);
            var json = Utility.Json.ToJson(authorityInputOpdata);
            var data = Encoding.UTF8.GetBytes(json);
            CosmosEntry.NetworkManager.SendNetworkMessage(data);
        }
        protected override void OnDestroy()
        {
            CosmosEntry.NetworkManager.Disconnect();
        }
        protected override void Awake()
        {
            base.Awake();
            NetworkWriter = new NetworkWriter();
            authorityInputOpdata = new OperationData((byte)OperationCode.PlayerInput);
        }
        protected virtual void Start()
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
            IsConnected = false;
        }
        void ProcessHandler(OperationData opData)
        {
            var opCode = (OperationCode)opData.OperationCode;
            switch (opCode)
            {
                case OperationCode.SYN:
                    {
                        var messageDict = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(opData.DataMessage));
                        var authorityConv = Utility.GetValue(messageDict, (byte)ParameterCode.AuthorityConv);
                        var serverSyncInterval = Utility.GetValue(messageDict, (byte)ParameterCode.ServerSyncInterval);
                        AuthorityConv = Convert.ToInt32(authorityConv);
                        NetworkSimulateConsts.IntervalMS = Convert.ToInt32(serverSyncInterval);

                        onConnect?.Invoke();

                        var remoteConvsJson = Convert.ToString(Utility.GetValue(messageDict, (byte)ParameterCode.RemoteConvs));
                        var remoteConvs = Utility.Json.ToObject<List<int>>(remoteConvsJson);
                        if (remoteConvs != null)
                        {
                            var length = remoteConvs.Count;
                            for (int i = 0; i < length; i++)
                            {
                                onPlayerEnter(remoteConvs[i]);
                            }
                        }
                        IsConnected = true;
                    }
                    break;
                case OperationCode.PlayerEnter:
                    {
                        var enterNetId = Convert.ToInt32(opData.DataMessage);
                        onPlayerEnter(enterNetId);
                    }
                    break;
                case OperationCode.PlayerExit:
                    {
                        var exitNetId = Convert.ToInt32(opData.DataMessage);
                        onPlayerExit?.Invoke(exitNetId);
                    }
                    break;
                case OperationCode.PlayerInput:
                    {
                        var fixTransports= Utility.Json.ToObject<List< FixTransportData>>(Convert.ToString(opData.DataMessage));
                        if (fixTransports!= null)
                            onPlayerInput?.Invoke(fixTransports.ToArray());
                    }
                    break;
                case OperationCode.FIN:
                    {
                        Utility.Debug.LogError(opData.DataMessage);
                        Disconnect();
                    }
                    break;
            }
        }
    }
}