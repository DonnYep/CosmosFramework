using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class ObjectSpawnerMuti : ObjectSpawner
    {
        [ContextMenuItem("Reset","ResetSpanwer")]
        [SerializeField] List<Transform> spawnTransforms = new List<Transform>();
        public List<Transform> SpawnTransforms { get { return spawnTransforms; } }
        [SerializeField]
        ObjectPoolEventObject poolObject;
        public ObjectPoolEventObject PoolObject { get { return poolObject; } }
        [SerializeField] protected float collectDelay = 3;
        public override float CollectDelay { get { if (collectDelay <= 0.1) collectDelay = 0.1f; return collectDelay; } }
        public override void Spawn()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolObject.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolObject.SpawnCount; j++)
                {
                    Facade.Instance.SetObjectSpawnItem(this, PoolObject.SpawnObject);
                    var go = Facade.Instance.SpawnObject(this);
                    if (SpawnTransforms[i] != null)
                        go.transform.position = SpawnTransforms[i].position;
                    go.transform.SetParent(ActiveObjectMount);
                }
            }
        }
        protected override void RegisterSpawner()
        {
            Facade.Instance.RegisterObjcetSpawnPool(this, PoolObject.SpawnObject, SpawnHandler, DespawnHandler);
        }
    }
}