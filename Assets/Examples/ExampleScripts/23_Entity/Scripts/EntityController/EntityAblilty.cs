using UnityEngine;
using Cosmos;
using Cosmos.ObjectPool;
public class EntityAblilty : MonoBehaviour
{
    [SerializeField] string bulletName = "EntityBullet";
    [SerializeField] float bulletSpeed = 15;
    [SerializeField] float bulletMoveDuration = 3;
    [SerializeField] int damage = 120;
    IObjectPoolManager objectPoolManager;
    IObjectPool bulletPool;

    private async void Start()
    {
        objectPoolManager = CosmosEntry.ObjectPoolManager;
        bulletPool = await objectPoolManager.RegisterObjectPoolAsync(new ObjectPoolAssetInfo(bulletName, bulletName));
        bulletPool.OnObjectSpawn += OnBulletSpawn;
        bulletPool.OnObjectDespawn += OnBulletDespawn;
    }
    public void Attack()
    {
        bulletPool.Spawn();
    }
    private void OnBulletSpawn(GameObject go)
    {
        go.SetActive(true);
        var eb = go.GetOrAddComponent<EntityBulletController>();
        eb.onHit = (hit) => { bulletPool.Despawn(eb.gameObject); OnBulletHit(hit); };
        eb.transform.position = transform.position + transform.forward * 0.5f + new Vector3(0, 1.5f, 0);
        eb.transform.eulerAngles = transform.eulerAngles;
        eb.Speed = bulletSpeed;
        eb.MoveDuration = bulletMoveDuration;
        eb.OnShoot();
    }
    private void OnBulletDespawn(GameObject go)
    {
        go.SetActive(false);
        go.transform.ResetWorldTransform();
    }
    void OnBulletHit(GameObject hitGo)
    {
        if (hitGo == null)
            return;
        var comp = hitGo.GetComponent<EntityEnmeyController>();
        if (comp != null)
        {
            comp.Damage(damage);
        }
    }
}
