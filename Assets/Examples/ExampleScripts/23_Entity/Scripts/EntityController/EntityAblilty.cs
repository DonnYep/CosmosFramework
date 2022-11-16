using UnityEngine;
using Cosmos;
using Cosmos.Entity;

public class EntityAblilty : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 15;
    [SerializeField] float bulletMoveDuration = 3;
    [SerializeField] int damage = 120;
    IEntityManager entityManager;
    private void Start()
    {
        entityManager = CosmosEntry.EntityManager;
    }
    public void Attack()
    {
        SpawnBullet();
    }
    private void SpawnBullet()
    {
        entityManager.ShowEntity(EntityContants.EntityBullet, out var entityObject);
        var bullet = entityObject as BulletEntity;
        bullet.onHit = (hit) => { entityManager.HideEntityObject(bullet); OnBulletHit(hit); };
        bullet.transform.position = transform.position + transform.forward * 0.5f + new Vector3(0, 1.5f, 0);
        bullet.transform.eulerAngles = transform.eulerAngles;
        bullet.Speed = bulletSpeed;
        bullet.MoveDuration = bulletMoveDuration;
        bullet.OnShoot();
    }
    void OnBulletHit(GameObject hitGo)
    {
        if (hitGo == null)
            return;
        var comp = hitGo.GetComponent<EnmeyEntity>();
        if (comp != null)
        {
            comp.Damage(damage);
        }
    }
}
