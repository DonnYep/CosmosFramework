using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class ObjectSpawnerSingle : ObjectSpawner {

      
        [SerializeField]
        Transform spawnTransform;
        public Transform SpawnTransform { get { return spawnTransform; } }
        [SerializeField]
        ObjectPoolDataSet poolDataSet;
        public ObjectPoolDataSet PoolDataSet { get { return poolDataSet; } }
        [SerializeField] protected float collectDelay = 3;
        public override float CollectDelay { get { if (collectDelay <= 0.1) collectDelay = 0.1f; return collectDelay; } }
        public override void Spawn()
        {
            if (!poolDataSet.ObjectAddsResult)
                return;
            for (int i = 0; i < poolDataSet.SpawnCount; i++)
            {
                Facade.Instance.SetObjectSpawnItem(this, PoolDataSet.SpawnObject);
                var go = Facade.Instance.SpawnObject(this);
                if (SpawnTransform != null)
                    go.transform.position = SpawnTransform.position;
                go.transform.SetParent(ActiveObjectMount);
            }
        }
        protected override void RegisterSpawner()
        {
            if(poolDataSet != null)
                Facade.Instance.RegisterObjcetSpawnPool(this, PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler);
        }
    }
}