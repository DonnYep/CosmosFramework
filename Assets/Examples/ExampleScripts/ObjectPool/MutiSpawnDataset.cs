using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    [CreateAssetMenu(fileName = "NewMutiSpawnDataSet", menuName = "CosmosFramework/ObjectPoolDataSet/MutiSpawnObject")]
    public class MutiSpawnDataset : ObjectPoolDataset
    {
        [Header("连续出现的次数，0表示不做设置，最大50表示一个池的最大数")]
        [Range(0,50)]
        [SerializeField] int severalTime;
        [SerializeField]
        protected GameObject[] spawnObjects;
        public override GameObject SpawnObject { get { return spawnObjects[SpawnIndex()]; } }
        public override void Dispose()
        {
            objectName = "NewObject";
            spawnObjects = null;
            objectOdds = 25;
            groupSpawn = false;
            minSpawnCount = 0;
            maxSpawnCount = 1;
            randomRotationOnSpawn = false;
            severalTime = 0;
            maxCollectDelay = 3;
            minCollectDelay = 3;
        }
        int indexDisplayTime= 0;
        int previouseIndex = -1;
        int currentIndex = -1;
        int SpawnIndex()
        {
            if(severalTime==0)
                return Random.Range(0, spawnObjects.Length);
            currentIndex = Random.Range(0, spawnObjects.Length);
            if (previouseIndex != currentIndex)
            {
                previouseIndex =currentIndex;
                indexDisplayTime = 0;
            }
            else
            {
                indexDisplayTime++;
                if (indexDisplayTime >= severalTime)
                {
                    currentIndex = SpawnIndex();
                }
            }
            return currentIndex;
        }
    }
}