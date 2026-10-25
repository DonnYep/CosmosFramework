using System.Linq;
using UnityEngine;
using Cosmos;
using Cosmos.Entity;

public class EntityEnmeySpanwer : MonoBehaviour
{
    [SerializeField] float deactiveEntityTime = 6;
    [SerializeField] float respawnTime = 3;
    [SerializeField] Transform[] spawnPoints;
    IEntityManager entityManager;
    int spawnPointIndex = 0;
    private void Start()
    {
        spawnPoints = transform.GetComponentsInChildren<Transform>().Where(sp => sp.name.Contains("SpawnPoint")).ToArray();
        entityManager = CosmosEntry.EntityManager;
        SpawnEnmey();
    }
    void SpawnEnmey()
    {
        var length = spawnPoints.Length;
        for (int i = 0; i < length; i++)
        {
            var entityObject = entityManager.ShowEntity<EnmeyEntity>(EntityContants.EntityEnmey);
            entityObject.Handle.transform.SetAndAlignParent(spawnPoints[spawnPointIndex]);
            spawnPointIndex++;
            Utility.Debug.LogInfo($"ShowEntity==={entityObject.EntityName}={entityObject.EntityObjectId}");
            entityObject.onDeath = OnEnemyDeath;
        }
    }
    async void OnEnemyDeath(EnmeyEntity entityEnmey)
    {
        await new WaitForSeconds(deactiveEntityTime);
        entityManager.HideEntityObject(entityEnmey.EntityName, entityEnmey.EntityObjectId);
        await new WaitForSeconds(respawnTime);
        entityManager.ShowEntity<EnmeyEntity>(entityEnmey.EntityName);
    }
}
