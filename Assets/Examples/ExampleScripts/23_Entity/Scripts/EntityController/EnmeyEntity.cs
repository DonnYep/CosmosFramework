using UnityEngine;
using Cosmos.Entity;
using System;

[RequireComponent(typeof(EntityAnimator))]
public class EnmeyEntity: EntityObject
{
    EntityAnimator entityAnimator;
    [SerializeField] int health = 200;
    bool death = false;
    bool onHitAnim = false;
    CapsuleCollider capsuleCollider;
    public Action<EnmeyEntity> onDeath;
    public int Heath
    {
        get { return health; }
        set { health = value; }
    }
    public bool Death
    {
        get { return death; }
    }
    public void Damage(int value)
    {
        if (death)
            return;
        health -= value;
        if (health <= 0)
        {
            health = 0;
            death = true;
            entityAnimator.Death();
            capsuleCollider.enabled = false;
            onDeath?.Invoke(this);
        }
        else
        {
            if (!onHitAnim)
                entityAnimator.Hit();
        }
    }
    public override void OnInit(string entityName, int entityInstanceId, string entityGroupName)
    {
        base.OnInit(entityName, entityInstanceId, entityGroupName);
        entityAnimator = GetComponent<EntityAnimator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        entityAnimator.OnHitOff += OnHitOff;
    }
    public override void OnHide()
    {
        gameObject.SetActive(false);
        ResetController();
    }
    void ResetController()
    {
        health = 200;
        onHitAnim = false;
        capsuleCollider.enabled = true;
        death = false;
    }
    void OnHitOff()
    {
        onHitAnim = false;
    }
}
