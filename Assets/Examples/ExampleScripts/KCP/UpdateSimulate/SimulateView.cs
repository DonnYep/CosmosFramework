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
        private void Start()
        {
            KCPNetwork.Instance.OnClientConnected += () => SpawnLocalPlayer();
        }
        void SpawnLocalPlayer()
        {
            localPlayerInstance = GameObject.Instantiate(KCPNetwork.Instance.LocalPlayerPrefab);
        }
    }
}