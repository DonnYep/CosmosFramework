using System;
using UnityEngine;
using Cosmos;
using Cosmos.Entity;
public class BulletEntity: EntityObject, IEntityTickable
{
    float moveDuration;
    float speed;
    float currentTime = 0;
    public Action<GameObject> onHit;
    EntityBulletTrigger bulletTrigger;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public float MoveDuration
    {
        get { return moveDuration; }
        set { moveDuration = value; }
    }
    public void OnShoot()
    {
        currentTime = 0;
    }
    private void Start()
    {
        bulletTrigger = gameObject.GetOrAddComponentInChildren<EntityBulletTrigger>("BulletTrigger");
        bulletTrigger.onTriggerHit = onHit;
    }
    /// <summary>
    /// 由 EntityManager 集中派发（不再使用 MonoBehaviour.Update）：
    /// 子弹被隐藏、所在组被冻结时都会自动停止推进。
    /// </summary>
    public void OnEntityTick(float deltaTime)
    {
        transform.position += transform.forward * deltaTime * Speed;
        currentTime += deltaTime;
        if (currentTime >= moveDuration)
        {
            onHit?.Invoke(null);
        }
    }
    public override void OnHide()
    {
        base.OnHide();
        transform.ResetWorldTransform();
    }
    public override void OnRecycle()
    {
        base.OnRecycle();
    }
}
