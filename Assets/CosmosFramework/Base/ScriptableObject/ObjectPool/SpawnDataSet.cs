using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    [CreateAssetMenu(fileName = "NewSpawnDataSet", menuName = "CosmosFramework/ObjectPoolDataSet/SpawnObject")]
    public class SpawnDataSet : ObjectPoolDataSet
    {
        public override void Reset()
        {
            objectName = "NewObject";
            spawnObject = null;
            objectAdds = 25;
            groupSpawn = false;
            MinSpawnCount = 0;
            MaxSpawnCount = 1;
            randomRotationOnSpawn = false;
        }
    }
}