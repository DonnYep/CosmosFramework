using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Cosmos.Test
{
    public class CubeMultiplayerManager : MultiplayerManager
    {
        Dictionary<int, NetworkTransform> netTransDict = new Dictionary<int, NetworkTransform>();
        Camera playerTraceCamera;
        MultiplayerCubeCamera movementSphereCamera;
        NetworkTransform authorityTrans;
        NetworkWriter writer = new NetworkWriter();
        Dictionary<int, Pool<NetworkWriter>> writePool;
        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
        protected override void Start()
        {
            base.Start();
            playerTraceCamera = GameObject.Find("MovementSphereCamera").GetComponent<Camera>();
            movementSphereCamera = playerTraceCamera.gameObject.AddComponent<MultiplayerCubeCamera>();

            MultiplayerManager.Instance.OnConnect += OnConnectHandler;
            MultiplayerManager.Instance.OnPlayerEnter += OnPlayerEnterHandler;
            MultiplayerManager.Instance.OnPlayerExit += OnPlayerExitHandler;
            MultiplayerManager.Instance.OnDisconnect += OnDisconnectHandler;
            MultiplayerManager.Instance.OnPlayerInput += OnPlayerInputHandler;

        }
        void OnConnectHandler()
        {
            var go = GameObject.Instantiate(MultiplayerManager.Instance.LocalPlayerPrefab);
            authorityTrans = go.AddComponent<NetworkTransform>();
            go.AddComponent<MultiplayerCubeController>();
            authorityTrans.NetId = MultiplayerManager.Instance.AuthorityConv;
            authorityTrans.IsAuthority = true;
            movementSphereCamera.SetCameraTarget(authorityTrans.transform);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MultiplayerManager.Instance.RemotePlayerPrefab);
            var comp = go.AddComponent<NetworkTransform>();
            comp.NetId = conv;
            comp.IsAuthority = false;
            netTransDict.Add(comp.NetId, comp);
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
        void OnPlayerInputHandler(Dictionary<int, string> inputData)
        {
            foreach (var  iptData in inputData)
            {
                if(netTransDict.TryGetValue(iptData.Key, out var netTrans))
                {
                    netTrans.DeserializeNetworkTransform(Utility.Json.ToObject<FixTransform>(iptData.Value));
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
                MonoGameManager.KillObject(authorityTrans.gameObject);
                authorityTrans = null;
                netTransDict.Clear();
                movementSphereCamera.ReleaseTarget();
            }
            catch { }
        }
    }
}