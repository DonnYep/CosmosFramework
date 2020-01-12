using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    [CreateAssetMenu(fileName = "NewCosmosObject", menuName = "CosmosFramework/ObjectPoolObject/CosmosObject")]
    public class CosmosObject : ObjectPoolEventObject
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