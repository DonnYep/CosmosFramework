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
        public override float CollectDelay { get { return poolDataSet.CollectDelay; } }
        HashSet<GameObject> uncollectableHashSet = new HashSet<GameObject>();
        public override HashSet<GameObject> UncollectableHashSet { get { return uncollectableHashSet; } protected set { uncollectableHashSet = value; } }
        public override void Spawn()
        {
            if (!poolDataSet.ObjectAddsResult)
                return;
            for (int i = 0; i < poolDataSet.SpawnCount; i++)
            {
                Facade.Instance.SetObjectSpawnItem(this, PoolDataSet.SpawnObject);
                var go = Facade.Instance.SpawnObject(this);
                AlignObject(PoolDataSet.AlignType, go, SpawnTransform);
            }
        }
        protected override void RegisterSpawner()
        {
            if(poolDataSet != null)
                Facade.Instance.RegisterObjcetSpawnPool(this, PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler);
        }
        public override void SpawnUncollectable()
        {
            if (!poolDataSet.ObjectAddsResult)
                return;
            for (int i = 0; i < poolDataSet.SpawnCount; i++)
            {
                var go = GameObject.Instantiate(PoolDataSet.SpawnObject);
                AlignObject(PoolDataSet.AlignType, go, SpawnTransform);
                uncollectableHashSet.Add(go);
            }
        }
    }
}