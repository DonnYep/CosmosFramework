using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    int idleHash = Animator.StringToHash("Idle");
    int attackHash = Animator.StringToHash("Attack");
    int moveHash = Animator.StringToHash("Move");
    int deathHash = Animator.StringToHash("Death");
    int hitHash = Animator.StringToHash("Hit");
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {

    }
}
