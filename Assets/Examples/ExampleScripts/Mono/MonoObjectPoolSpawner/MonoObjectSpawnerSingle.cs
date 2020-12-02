using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.ObjectPool;
namespace Cosmos{
    public class MonoObjectSpawnerSingle : MonoObjectSpawner {

      
        [SerializeField]
        Transform spawnTransform;
        public Transform SpawnTransform { get { return spawnTransform; } }
        [SerializeField]
        ObjectPoolDataSet poolDataSet;
        public ObjectPoolDataSet PoolDataSet { get { return poolDataSet; } }
        public override float CollectDelay { get { return poolDataSet.CollectDelay; } }
        HashSet<GameObject> uncollectibleHashSet = new HashSet<GameObject>();
        public override HashSet<GameObject> UncollectibleHashSet { get { return uncollectibleHashSet; } protected set { uncollectibleHashSet = value; } }
        IObjectPoolManager objectPoolManager;
        protected override void Start()
        {
            base.Start();
            objectPoolManager = GameManager.GetModule<IObjectPoolManager>();
        }
        public override void Spawn()
        {
            if (!poolDataSet.ObjectAddsResult)
                return;
            for (int i = 0; i < poolDataSet.SpawnCount; i++)
            {
                objectPoolManager.SetSpawnItem(this, PoolDataSet.SpawnObject);
                var go = objectPoolManager.Spawn(this);
                AlignObject(PoolDataSet.AlignType, go, SpawnTransform);
            }
        }
        protected override void RegisterSpawner()
        {
            if(poolDataSet != null)
                objectPoolManager.RegisterSpawnPool(this, PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler);
        }
        public override void SpawnUncollectible()
        {
            if (!poolDataSet.ObjectAddsResult)
                return;
            for (int i = 0; i < poolDataSet.SpawnCount; i++)
            {
                var go = GameObject.Instantiate(PoolDataSet.SpawnObject);
                AlignObject(PoolDataSet.AlignType, go, SpawnTransform);
                uncollectibleHashSet.Add(go);
            }
        }
    }
}