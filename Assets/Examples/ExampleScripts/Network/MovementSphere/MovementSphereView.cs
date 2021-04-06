using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class MovementSphereView : MonoBehaviour
    {
        Dictionary<int, NetworkIndentity> netConvDict = new Dictionary<int, NetworkIndentity>();
        Camera playerTraceCamera;
        MovementSphereCamera movementSphereCamera;
        private void Start()
        {
            playerTraceCamera = GameObject.Find("MovementSphereCamera").GetComponent<Camera>();
            movementSphereCamera = playerTraceCamera.gameObject.AddComponent<MovementSphereCamera>();

            MovementSphereManager.Instance.OnConnect += OnConnectHandler;
            MovementSphereManager.Instance.OnPlayerEnter += OnPlayerEnterHandler;
            MovementSphereManager.Instance.OnPlayerExit+= OnPlayerExitHandler;
            MovementSphereManager.Instance.OnDisconnect+= OnDisconnectHandler;

        }
        void OnConnectHandler()
        {
            var go = GameObject.Instantiate(MovementSphereManager.Instance.LocalPlayerPrefab);
            var comp = go.AddComponent<NetworkIndentity>();
            go.AddComponent<NetworkPlayerController>();
            comp.NetId = MovementSphereManager.Instance.AuthorityConv;
            comp.IsAuthority = true;
            netConvDict.Add(comp.NetId, comp);
            movementSphereCamera.SetTarget(comp.transform);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MovementSphereManager.Instance.RemotePlayerPrefab);
            var comp = go.AddComponent<NetworkIndentity>();
            comp.NetId = conv;
            comp.IsAuthority = false;
            netConvDict.Add(comp.NetId, comp);
        }
        void OnPlayerExitHandler(int conv)
        {
            if( netConvDict.Remove(conv, out var networkIndentity))
            {
                MonoGameManager.KillObject(networkIndentity.gameObject);
            }
        }
        void OnDisconnectHandler()
        {
            try
            {
                foreach (var netObj in netConvDict.Values)
                {
                    MonoGameManager.KillObject(netObj.gameObject);
                }
                netConvDict.Clear();
                movementSphereCamera.ReleaseTarget();
            }
            catch { }
        }
    }
}