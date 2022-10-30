using System;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    int attackHash = Animator.StringToHash("Attack");
    int moveHash = Animator.StringToHash("Move");
    int deathHash = Animator.StringToHash("Death");
    int hitHash = Animator.StringToHash("Hit");
    Animator animator;
    Action onHitOff;
    Action onAttackOff;
    Action onAttackAnim;
    public event Action OnAttackOff
    {
        add { onAttackOff += value; }
        remove { onAttackOff -= value; }
    }
    public event Action OnHitOff
    {
        add { onHitOff += value; }
        remove { onHitOff -= value; }
    }
    public event Action OnAttackAnim
    {
        add { onAttackAnim += value; }
        remove { onAttackAnim -= value; }
    }
    public void Attack()
    {
        animator.SetBool(moveHash, false);
        animator.SetBool(deathHash, false);
        animator.SetBool(attackHash, true);
        animator.SetBool(hitHash, false);
    }
    public void Idle()
    {
        animator.SetBool(moveHash, false);
        animator.SetBool(deathHash, false);
        animator.SetBool(attackHash, false);
        animator.SetBool(hitHash, false);
    }
    public void Move()
    {
        animator.SetBool(moveHash, true);
        animator.SetBool(attackHash, false);
        animator.SetBool(hitHash, false);
        animator.SetBool(deathHash, false);
    }
    public void Death()
    {
        animator.SetBool(moveHash, false);
        animator.SetBool(attackHash, false);
        animator.SetBool(hitHash, false);
        animator.SetBool(deathHash, true);
    }
    public void Hit()
    {
        animator.SetBool(moveHash, false);
        animator.SetBool(attackHash, false);
        animator.SetBool(hitHash, true);
        animator.SetBool(deathHash, false);
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime > 0.7f)
        {
            onAttackOff?.Invoke();
            animator.SetBool(attackHash, false);
        }
        else if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.2f && stateInfo.normalizedTime < 0.3f)
        {
            onAttackAnim?.Invoke();
        }
        else if (stateInfo.IsName("Hit") && stateInfo.normalizedTime > 0.7f)
        {
            onHitOff?.Invoke();
            animator.SetBool(hitHash, false);
        }
    }
}
