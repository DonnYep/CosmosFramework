using UnityEngine;
using System.Collections;
namespace Cosmos
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : CharacterInputController
    {
        short updateID;
        int moveSpeed = Animator.StringToHash("MoveSpeed");
        [SerializeField]
        [Range(0, 1)]
        float dampTime = 0.1f;
        Animator animator;
        float moveForword = 0;

        protected override void OnInitialization()
        {
            animator = GetComponentInChildren<Animator>();
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            inputEventArg = arg as Input.InputEventArgs;
             moveForword= inputEventArg.HorizVertAxis.y;
            if (inputEventArg.LeftShift)
                moveForword *=2;
            animator.SetFloat(moveSpeed, moveForword,dampTime,Time.deltaTime);
        }
    }
}