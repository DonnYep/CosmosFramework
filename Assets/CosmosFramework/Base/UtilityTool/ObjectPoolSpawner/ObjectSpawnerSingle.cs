using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class ObjectSpawnerSingle : ObjectSpawner {

      
        [SerializeField]
        Transform spawnTransform;
        public Transform SpawnTransform { get { return spawnTransform; } }
        [SerializeField]
        ObjectPoolEventObject poolObject;
        public ObjectPoolEventObject PoolObject { get { return poolObject; } }
        [SerializeField] protected float collectDelay = 3;
        public override float CollectDelay { get { if (collectDelay <= 0.1) collectDelay = 0.1f; return collectDelay; } }
        public override void Spawn()
        {
            if (!poolObject.ObjectAddsResult)
                return;
            for (int i = 0; i < poolObject.SpawnCount; i++)
            {
                Facade.Instance.SetObjectSpawnItem(this, PoolObject.SpawnObject);
                var go = Facade.Instance.SpawnObject(this);
                if (SpawnTransform != null)
                    go.transform.position = SpawnTransform.position;
                go.transform.SetParent(ActiveObjectMount);
            }
        }
        protected override void RegisterSpawner()
        {
            if(poolObject!=null)
                Facade.Instance.RegisterObjcetSpawnPool(this, PoolObject.SpawnObject, SpawnHandler, DespawnHandler);
        }
    }
}