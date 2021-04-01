using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class SimulateView : MonoBehaviour
    {
        GameObject localPlayerInstance;
        GameObject remotePlayerInstance;
        Dictionary<int, NetworkIndentity> netObjDict = new Dictionary<int, NetworkIndentity>();
         Camera playerTraceCamera;
        SimulatePlayerCameraTracer simulatePlayerCameraTracer;
        private void Start()
        {
            playerTraceCamera = GameObject.Find("PlayerTraceCamera").GetComponent<Camera>();
            CosmosEntry.NetworkManager.OnConnect += () => SpawnLocalPlayer();
            CosmosEntry.NetworkManager.OnDisconnect+= () => ClearAlllPlayer();
            simulatePlayerCameraTracer = playerTraceCamera.gameObject.AddComponent<SimulatePlayerCameraTracer>();
        }
        void SpawnLocalPlayer()
        {
            localPlayerInstance = GameObject.Instantiate(KCPNetwork.Instance.LocalPlayerPrefab);
            var comp= localPlayerInstance.AddComponent<NetworkIndentity>();
            comp.NetId = (int)CosmosEntry.NetworkManager.Conv;
            netObjDict.Add(comp.NetId, comp);
            localPlayerInstance.AddComponent<SimulatePlayerController>();
            simulatePlayerCameraTracer.SetTracerTarget(comp.transform);

        }
        void ClearAlllPlayer()
        {
            foreach (var netObj in netObjDict.Values)
            {
                MonoGameManager.KillObject(netObj.gameObject);
            }
            netObjDict.Clear();
            simulatePlayerCameraTracer.ResetTracer();
        }
    }
}