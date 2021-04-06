using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class MovementSphereView : MonoBehaviour
    {
        Dictionary<int, NetworkIndentity> netIndentityDict = new Dictionary<int, NetworkIndentity>();
        Camera playerTraceCamera;
        MovementSphereCamera movementSphereCamera;
        NetworkIndentity authorityIndentity;
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
            authorityIndentity = go.AddComponent<NetworkIndentity>();
            go.AddComponent<NetworkPlayerController>();
            authorityIndentity.NetId = MovementSphereManager.Instance.AuthorityConv;
            authorityIndentity.IsAuthority = true;
            movementSphereCamera.SetTarget(authorityIndentity.transform);
        }
        void OnPlayerEnterHandler(int conv)
        {
            var go = GameObject.Instantiate(MovementSphereManager.Instance.RemotePlayerPrefab);
            var comp = go.AddComponent<NetworkIndentity>();
            comp.NetId = conv;
            comp.IsAuthority = false;
            netIndentityDict.Add(comp.NetId, comp);
        }
        void OnPlayerExitHandler(int conv)
        {
            if (netIndentityDict.Remove(conv, out var networkIndentity))
            {
                MonoGameManager.KillObject(networkIndentity.gameObject);
            }
        }
        void OnPlayerInputHandler(Dictionary<int, byte[]> inputData)
        {
            foreach (var  iptData in inputData)
            {
                if( netIndentityDict.TryGetValue(iptData.Key, out var indentity))
                {
                    indentity.OnDeserializeAllSafely(new NetworkReader(iptData.Value),false);
                }
            }
                authorityIndentity.OnSerializeAllSafely(writer);
                MovementSphereManager.Instance.SendAuthorityData(writer.ToArray());
                writer.Reset();
     
        }
        void OnDisconnectHandler()
        {
            try
            {
                foreach (var netObj in netIndentityDict.Values)
                {
                    MonoGameManager.KillObject(netObj.gameObject);
                }
                MonoGameManager.KillObject(authorityIndentity.gameObject);
                authorityIndentity = null;
                netIndentityDict.Clear();
                movementSphereCamera.ReleaseTarget();
            }
            catch { }
        }
    }
}