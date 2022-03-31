using System;
using UnityEngine;
namespace Cosmos.ObjectPool
{
    public interface IObjectPool 
    {
        event Action<GameObject> OnObjectSpawn;
        event Action<GameObject> OnObjectDespawn;
        int ExpireTime { get; set; }
        int ReleaseInterval { get; set; }
        int Capacity { get; set; }
        ObjectPoolKey ObjectKey { get; }
        GameObject Spawn();
        void Despawn(object go);
        void ClearPool();
        void OnElapseRefresh(float deltatime);
    }
}
