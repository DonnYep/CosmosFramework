using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Cosmos;
using Cosmos.Entity;

public class EntityEnmeySpanwer : MonoBehaviour
{
    [SerializeField] string spanwEntityName = "EntityEnemy";
    [SerializeField] float deactiveEntityTime = 6;
    [SerializeField] float respawnTime = 3;
    [SerializeField] Transform[] spawnPoints;
    IEntityManager entityManager;
    private async void Start()
    {
        spawnPoints = transform.GetComponentsInChildren<Transform>().Where(sp => sp.name.Contains("SpawnPoint")).ToArray();
        entityManager = CosmosEntry.EntityManager;
        await entityManager.RegisterEntityGroupAsync(new EntityAssetInfo(spanwEntityName, spanwEntityName, true));
        var length = spawnPoints.Length;
        for (int i = 0; i < length; i++)
        {
            var entity = entityManager.ShowEntity(spanwEntityName);
            var ei = entity.EntityObject;
            var go = ei.gameObject;
            go.transform.SetAlignParent(spawnPoints[i]);
            var enmeyController = go.GetComponent<EntityEnmeyController>();
            enmeyController.EntityId = entity.EntityId;
            enmeyController.onDeath = OnEnmeyDeath;
        }
    }
    async void OnEnmeyDeath(EntityEnmeyController entityEnmey)
    {
        await new WaitForSeconds(deactiveEntityTime);
        entityManager.DeactiveEntity(entityEnmey.EntityId);
        await new WaitForSeconds(respawnTime);
        var entity = entityManager.ShowEntity(spanwEntityName);
        entityEnmey.EntityId = entity.EntityId;
        var parent = spawnPoints.Where(sp => sp.childCount == 0).FirstOrDefault();
        entityEnmey.transform.SetAlignParent(parent);
        entityEnmey.ResetController();

    }
}
