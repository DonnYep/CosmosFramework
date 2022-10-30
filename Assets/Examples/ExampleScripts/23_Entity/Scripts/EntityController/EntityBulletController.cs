using System;
using UnityEngine;
using Cosmos;
public class EntityBulletController : MonoBehaviour
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
    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * Speed;
        currentTime += Time.deltaTime;
        if (currentTime >= moveDuration)
        {
            onHit?.Invoke(null);
        }
    }
}
