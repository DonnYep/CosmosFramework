using System;
using UnityEngine;

public class EntityBulletTrigger : MonoBehaviour
{
    public Action<GameObject> onTriggerHit;
    private void OnTriggerEnter(Collider other)
    {
        //trigger不适用高速移动的对象
        if (other.name.Contains("EntityEnemy"))
        {
            onTriggerHit?.Invoke(other.gameObject);
        }
    }
}
