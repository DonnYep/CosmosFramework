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
        [SerializeField] protected float collectDelay = 3;
        public override float CollectDelay { get { if (collectDelay <= 0.1) collectDelay = 0.1f; return collectDelay; } }
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
                    if (SpawnTransforms[i] != null)
                        go.transform.position = SpawnTransforms[i].position;
                    go.transform.SetParent(ActiveObjectMount);
                }
            }
        }
        protected override void RegisterSpawner()
        {
            if(poolDataSet != null)
                Facade.Instance.RegisterObjcetSpawnPool(this, PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler);
        }
    }
}