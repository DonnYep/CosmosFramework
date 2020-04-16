using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class ObjectSpawnerMultiple : ObjectSpawner
    {
        [ContextMenuItem("Reset","ResetSpanwer")]
        [SerializeField] List<Transform> spawnTransforms = new List<Transform>();
        public List<Transform> SpawnTransforms { get { return spawnTransforms; } }
        [SerializeField]
        ObjectPoolDataSet poolDataSet;
        public ObjectPoolDataSet PoolDataSet { get { return poolDataSet; } }
        //[SerializeField] float collectDelay = 3;
        public override float CollectDelay { get { return poolDataSet.CollectDelay; } }
        HashSet<GameObject> uncollectableHashSet = new HashSet<GameObject>();
        public override HashSet<GameObject> UncollectableHashSet { get { return uncollectableHashSet; }protected set { uncollectableHashSet = value; } }
        public override void Spawn()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolDataSet.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolDataSet.SpawnCount; j++)
                {
                    Facade.Instance.SetObjectSpawnItem(this, PoolDataSet.SpawnObject);
                    var go = Facade.Instance.SpawnObject(this);
                    AlignObject(PoolDataSet.AlignType, go, SpawnTransforms[i]);
                }
            }
        }
        protected override void RegisterSpawner()
        {
            if(poolDataSet != null)
                Facade.Instance.RegisterObjcetSpawnPool(this, PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler);
        }
        //TODO不可回收的对象
        public override void SpawnUncollectable()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolDataSet.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolDataSet.SpawnCount; j++)
                {
                    var go = GameObject.Instantiate(PoolDataSet.SpawnObject);
                    AlignObject(PoolDataSet.AlignType, go, SpawnTransforms[i]);
                    uncollectableHashSet.Add(go);
                }
            }
        }
    }
}