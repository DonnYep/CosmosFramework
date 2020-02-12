using UnityEngine;
using System.Collections;
namespace Cosmos
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : CFController
    {
        [SerializeField]
        [Range(0, 1)]
        float forwardDampTime = 0.1f;
        [SerializeField]
        [Range(0, 1)]
        float turnDampTime = 0.1f;
        Animator animator;
        int forwardHash = Animator.StringToHash("Forward");
        int turnHash = Animator.StringToHash("Turn");
        int inputHash = Animator.StringToHash("Input");
        float moveForword = 0;
        float moveTurn = 0;
        protected override void OnInitialization()
        {
            animator = GetComponentInChildren<Animator>();
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            inputEventArg = arg as InputEventArgs;
            if (inputEventArg.HorizVertAxis.magnitude != 0)
                animator.SetBool(inputHash, true);
            else
                animator.SetBool(inputHash, false);
            moveForword = inputEventArg.HorizVertAxis.y;
            moveTurn= inputEventArg.HorizVertAxis.x;
            if (inputEventArg.LeftShift)
            {
                moveForword *= 2;
            }
            animator.SetFloat(forwardHash, moveForword, forwardDampTime, Time.deltaTime);
            animator.SetFloat(turnHash, moveTurn, turnDampTime, Time.deltaTime);
        }
    }
}