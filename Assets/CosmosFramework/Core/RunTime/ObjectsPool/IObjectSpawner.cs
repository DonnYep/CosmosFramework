using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public interface IObjectSpawner :IBehaviour
    {
        string SpawnObjectPoolKey { get; set; }
        GameObject SpawnItem { get; set; }
        GameObject ActivatedObjectMounting { get; set; }
        GameObject DeactivatedObjectMounting { get; set; }
        void OnSpawn(GameObject go);
        void OnDespawn(GameObject go);
        void SetObjectMounting(GameObject activated, GameObject deactivated);
        void SetSpawnData(string key, GameObject spawnItem);
    }
}