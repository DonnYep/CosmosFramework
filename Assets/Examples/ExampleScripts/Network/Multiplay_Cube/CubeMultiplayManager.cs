using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Cosmos.Test
{
    public class CubeMultiplayManager : MultiplayManager
    {
        Dictionary<int, NetworkIdentity> netIdentityDict = new Dictionary<int, NetworkIdentity>();
        Camera playerTraceCamera;
        MultiplayCubeCamera movementSphereCamera;
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
            playerTraceCamera = GameObject.Find("MovementSphereCamera").GetComponent<Camera>();
            movementSphereCamera = playerTraceCamera.gameObject.AddComponent<MultiplayCubeCamera>();

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
            var now = Time.time ;
            if (latestTime < now)
            {
                latestTime = now + NetworkSimulateConsts.SyncInterval;
                authorityIdentity.OnSerialize(out var transportData);
                SendAuthorityTransportData(transportData);
            }
        }
        void OnConnectHandler()
        {
            var go = GameObject.Instantiate(MultiplayManager.Instance.LocalPlayerPrefab);
            authorityIdentity = go.AddComponent<NetworkTransform>().NetworkIdentity;
            go.AddComponent<MultiplayCubeController>();
            authorityIdentity.NetId = MultiplayManager.Instance.AuthorityConv;
            authorityIdentity.IsAuthority = true;
            movementSphereCamera.SetCameraTarget(authorityIdentity.transform);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MultiplayManager.Instance.RemotePlayerPrefab);
            var comp = go.AddComponent<NetworkTransform>();
            comp.NetId = conv;
            comp.IsAuthority = false;
            netIdentityDict.Add(comp.NetId, comp.NetworkIdentity);
        }
        void OnPlayerExitHandler(int conv)
        {
            if (netIdentityDict.Remove(conv, out var networkIndentity))
            {
                MonoGameManager.KillObject(networkIndentity.gameObject);
            }
        }
        void OnPlayerInputHandler(FixTransportData[] inputDatas)
        {
            var length = inputDatas.Length;
            for (int i = 0; i < length; i++)
            {
                if (netIdentityDict.TryGetValue(inputDatas[i].Conv, out var netIdentity))
                {
                    netIdentity.OnDeserialize(inputDatas[i]);
                }
            }
        }
        void OnDisconnectHandler()
        {
            try
            {
                foreach (var netObj in netIdentityDict.Values)
                {
                    MonoGameManager.KillObject(netObj.gameObject);
                }
                MonoGameManager.KillObject(authorityIdentity.gameObject);
                authorityIdentity = null;
                netIdentityDict.Clear();
                movementSphereCamera.ReleaseTarget();
            }
            catch { }
        }
    }
}