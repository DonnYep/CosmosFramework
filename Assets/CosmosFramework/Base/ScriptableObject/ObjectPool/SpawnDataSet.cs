using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    [CreateAssetMenu(fileName = "NewSpawnDataSet", menuName = "CosmosFramework/ObjectPoolDataSet/SpawnObject")]
    public class SpawnDataSet : ObjectPoolDataSet
    {
        [SerializeField]
        protected GameObject spawnObject;
        public override GameObject SpawnObject { get { return spawnObject; } }
        public override void Reset()
        {
            objectName = "NewObject";
            spawnObject = null;
            objectOdds = 25;
            groupSpawn = false;
            minSpawnCount = 0;
            maxSpawnCount = 1;
            randomRotationOnSpawn = false;
            maxCollectDelay = 3;
            minCollectDelay = 3;
            AlignType = ObjectSpawnAlignType.AlignTransform;
        }
    }
}