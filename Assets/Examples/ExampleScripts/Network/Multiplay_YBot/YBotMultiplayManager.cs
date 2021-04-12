using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Cosmos.Test
{
    public class YBotMultiplayManager : MultiplayManager
    {
        Dictionary<int, NetworkIdentity> netTransDict = new Dictionary<int, NetworkIdentity>();
        MultiplayYBotCamera  multiplayerYBotCamera;
        NetworkIdentity authorityIdentity;
        float latestTime = 0;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
        protected override void Start()
        {
            base.Start();
            OnConnect += OnConnectHandler;
            OnPlayerEnter += OnPlayerEnterHandler;
            OnPlayerExit += OnPlayerExitHandler;
            OnDisconnect += OnDisconnectHandler;
            OnPlayerInput += OnPlayerInputHandler;
        }
        private void FixedUpdate()
        {
            if (!IsConnected)
                return;

            var now = Time.time;
            if (latestTime <= now)
            {
                latestTime = now + NetworkSimulateConsts.SyncInterval;
                authorityIdentity.OnSerialize(out var transportData);
                SendAuthorityTransportData(transportData);
            }
        }
        void OnConnectHandler()
        {
            multiplayerYBotCamera = GameObject.Find("YBotCamera").AddComponent<MultiplayYBotCamera>();
            var go = GameObject.Instantiate(MultiplayManager.Instance.LocalPlayerPrefab);
            go.AddComponent<NetworkAnimator>();
            authorityIdentity = go.AddComponent<NetworkTransform>().NetworkIdentity;
           var ctrlComp= go.AddComponent<MultiplayYBotController>();
            authorityIdentity.NetId = MultiplayManager.Instance.AuthorityConv;
            authorityIdentity.IsAuthority = true;
            multiplayerYBotCamera.SetCameraTarget(ctrlComp.CameraTarget);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MultiplayManager.Instance.RemotePlayerPrefab);
            go.AddComponent<NetworkAnimator>();
            var comp = go.AddComponent<NetworkTransform>();
            comp.NetId = conv;
            comp.IsAuthority = false;
            netTransDict.Add(comp.NetId, comp.NetworkIdentity);
        }
        void OnPlayerExitHandler(int conv)
        {
            if (netTransDict.Remove(conv, out var networkIndentity))
            {
                MonoGameManager.KillObject(networkIndentity.gameObject);
            }
        }
        /// <summary>
        /// int表示Conv，string表示FixTransform的json
        /// </summary>
        void OnPlayerInputHandler( FixTransportData[] inputDatas)
        {
            var length = inputDatas.Length;
            for (int i = 0; i < length; i++)
            {
                if (netTransDict.TryGetValue(inputDatas[i].Conv, out var netIdentity))
                {
                    netIdentity.OnDeserialize(inputDatas[i]);
                }
            }
        }
        void OnDisconnectHandler()
        {
            try
            {
                foreach (var netObj in netTransDict.Values)
                {
                    MonoGameManager.KillObject(netObj.gameObject);
                }
                MonoGameManager.KillObject(authorityIdentity.gameObject);
                authorityIdentity = null;
                netTransDict.Clear();
                multiplayerYBotCamera.ReleaseTarget();
                MonoGameManager.KillObject(multiplayerYBotCamera);
            }
            catch { }
        }
    }
}