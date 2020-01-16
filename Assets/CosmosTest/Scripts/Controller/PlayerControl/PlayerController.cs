using UnityEngine;
using System.Collections;
namespace Cosmos
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : CharacterInputController
    {
        short updateID;
        int forwardID = Animator.StringToHash("Forward");
        int turnID = Animator.StringToHash("Turn");
        [SerializeField]
        [Range(0, 1)]
        float forwardDampTime = 0.1f;
        [SerializeField]
        [Range(0, 1)]
        float turnDampTime = 0.1f;
        Animator animator;
        float moveForword = 0;
        float moveTurn = 0;
        protected override void OnInitialization()
        {
            animator = GetComponentInChildren<Animator>();
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            inputEventArg = arg as Input.InputEventArgs;
            moveForword= inputEventArg.HorizVertAxis.y;
            moveTurn= inputEventArg.HorizVertAxis.x;
            if (inputEventArg.LeftShift)
            {
                moveForword *= 2;
            }
            animator.SetFloat(forwardID, moveForword, forwardDampTime, Time.deltaTime);
            animator.SetFloat(turnID, moveTurn, turnDampTime, Time.deltaTime);
        }
    }
}