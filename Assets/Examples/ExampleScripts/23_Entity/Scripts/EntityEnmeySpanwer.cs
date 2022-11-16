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
            entityManager.ShowEntity(EntityContants.EntityEnmey, out var entity);
            entity.Handle.transform.SetAlignParent(spawnPoints[spawnPointIndex]);
            spawnPointIndex++;
            Utility.Debug.LogInfo($"ShowEntity==={entity.EntityName}={entity.EntityObjectId}");
            entity.As<EntityEnmeyController>().onDeath = OnEnemyDeath;
        }
    }
    async void OnEnemyDeath(EntityEnmeyController entityEnmey)
    {
        await new WaitForSeconds(deactiveEntityTime);
        entityManager.HideEntity(entityEnmey.EntityName, entityEnmey.EntityObjectId);
        await new WaitForSeconds(respawnTime);
        entityManager.ShowEntity(entityEnmey.EntityName, out _);
    }
}
