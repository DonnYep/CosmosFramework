using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class MonoObjectSpawnerSingle : MonoObjectSpawner
    {
        [SerializeField]
        Transform spawnTransform;
        public Transform SpawnTransform { get { return spawnTransform; } }
        [SerializeField]
        ObjectPoolDataSet poolDataSet;
        public ObjectPoolDataSet PoolDataSet { get { return poolDataSet; } }
        public override float CollectDelay { get { return poolDataSet.CollectDelay; } }
        HashSet<MonoObjectBase> uncollectibleHashSet = new HashSet<MonoObjectBase>();
        public override HashSet<MonoObjectBase> UncollectibleHashSet { get { return uncollectibleHashSet; } protected set { uncollectibleHashSet = value; } }
        public override void Spawn()
        {
            if (!poolDataSet.ObjectAddsResult)
                return;
            for (int i = 0; i < poolDataSet.SpawnCount; i++)
            {
                var go = Facade.SpawnObject(objectKey);
                AlignObject(PoolDataSet.AlignType, go, SpawnTransform);
            }
        }
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void RegisterSpawner()
        {
            if (poolDataSet != null)
            {
                objectKey = new ObjectKey<MonoObjectItem>(this.GetType().FullName);
                poolVariable = new ObjectPoolVariable(PoolDataSet.SpawnObject,SpawnHandler, DespawnHandler, Create);
                Facade.RegisterObjcetSpawnPool(objectKey, poolVariable);
            }
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
        protected override MonoObjectBase Create()
        {
            var go = Instantiate(PoolDataSet.SpawnObject);
            return go;
        }
    }
}