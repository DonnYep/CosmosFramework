using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Cosmos.Test
{
    public class MovementSphereView : MonoBehaviour
    {
        Dictionary<int, NetworkTransform> netTransDict = new Dictionary<int, NetworkTransform>();
        Camera playerTraceCamera;
        MovementSphereCamera movementSphereCamera;
        NetworkTransform authorityTrans;
        NetworkWriter writer = new NetworkWriter();
        Dictionary<int, Pool<NetworkWriter>> writePool;


        private void Start()
        {
            playerTraceCamera = GameObject.Find("MovementSphereCamera").GetComponent<Camera>();
            movementSphereCamera = playerTraceCamera.gameObject.AddComponent<MovementSphereCamera>();

            MovementSphereManager.Instance.OnConnect += OnConnectHandler;
            MovementSphereManager.Instance.OnPlayerEnter += OnPlayerEnterHandler;
            MovementSphereManager.Instance.OnPlayerExit += OnPlayerExitHandler;
            MovementSphereManager.Instance.OnDisconnect += OnDisconnectHandler;
            MovementSphereManager.Instance.OnPlayerInput += OnPlayerInputHandler;

        }
        void OnConnectHandler()
        {
            var go = GameObject.Instantiate(MovementSphereManager.Instance.LocalPlayerPrefab);
            authorityTrans = go.AddComponent<NetworkTransform>();
            go.AddComponent<NetworkPlayerController>();
            authorityTrans.NetId = MovementSphereManager.Instance.AuthorityConv;
            authorityTrans.IsAuthority = true;
            movementSphereCamera.SetTarget(authorityTrans.transform);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MovementSphereManager.Instance.RemotePlayerPrefab);
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
                authorityTrans.SerializeNetworkTransform(out var fixTransform);
                MovementSphereManager.Instance.SendAuthorityData(fixTransform);
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