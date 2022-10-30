using UnityEngine;
using System;
[RequireComponent(typeof(EntityAnimator))]
public class EntityEnmeyController : MonoBehaviour
{
    public int EntityId { get; set; }
    EntityAnimator entityAnimator;
    [SerializeField] int health = 200;
    bool death = false;
    bool onHitAnim = false;
    CapsuleCollider capsuleCollider;
    public Action<EntityEnmeyController> onDeath;
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
    public void ResetController()
    {
        health = 200;
        onHitAnim = false;
        capsuleCollider.enabled = true;
        death = false;
    }
    private void Awake()
    {
        entityAnimator = GetComponent<EntityAnimator>();
        entityAnimator.OnHitOff += OnHitOff;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    private void OnHitOff()
    {
        onHitAnim = false;
    }
}
