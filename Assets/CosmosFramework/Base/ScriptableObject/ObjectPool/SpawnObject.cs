using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    [CreateAssetMenu(fileName = "NewSpawnObject", menuName = "CosmosFramework/ObjectPoolObject/SpawnObject")]
    public class SpawnObject : ObjectPoolEventObject
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