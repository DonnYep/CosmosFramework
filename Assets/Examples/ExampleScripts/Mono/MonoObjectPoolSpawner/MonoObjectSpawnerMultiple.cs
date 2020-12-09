using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.ObjectPool;
namespace Cosmos{
    public class MonoObjectSpawnerMultiple : MonoObjectSpawner
    {
        [ContextMenuItem("Reset","ResetSpanwer")]
        [SerializeField] List<Transform> spawnTransforms = new List<Transform>();
        public List<Transform> SpawnTransforms { get { return spawnTransforms; } }
        [SerializeField]
        ObjectPoolDataset poolDataSet;
        public ObjectPoolDataset PoolDataSet { get { return poolDataSet; } }
        public override float CollectDelay { get { return poolDataSet.CollectDelay; } }
        HashSet<GameObject> uncollectibleHashSet = new HashSet<GameObject>();
        public override HashSet<GameObject> UncollectibleHashSet { get { return uncollectibleHashSet; }protected set { uncollectibleHashSet = value; } }
        IObjectPoolManager objectPoolManager;
        
        public override void Spawn()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolDataSet.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolDataSet.SpawnCount; j++)
                {
                    //objectPoolManager.SetSpawnItem(this, PoolDataSet.SpawnObject);
                    //var go = objectPoolManager.Spawn(this);
                    //AlignObject(PoolDataSet.AlignType, go, SpawnTransforms[i]);
                }
            }
        }
        protected override void RegisterSpawner()
        {
            //if(poolDataSet != null)
            //    objectPoolManager.RegisterSpawnPool(this, PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler);
        }
        //TODO不可回收的对象
        public override void SpawnUncollectible()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolDataSet.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolDataSet.SpawnCount; j++)
                {
                    var go = GameObject.Instantiate(PoolDataSet.SpawnObject);
                    AlignObject(PoolDataSet.AlignType, go, SpawnTransforms[i]);
                    uncollectibleHashSet.Add(go);
                }
            }
        }
        protected override void Start()
        {
            base.Start();
            objectPoolManager = GameManager.GetModule<IObjectPoolManager>();
        }
    }
}