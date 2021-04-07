using UnityEngine;
using System.Collections;
using Cosmos.Input;
using System;

namespace Cosmos
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class MultiplayerYBotController : ControllerBase<MultiplayerYBotController>
    {
        [SerializeField]
        [Range(0, 1)]
        float forwardDampTime = 0.1f;
        [SerializeField]
        [Range(0, 1)]
        float turnDampTime = 0.05f;
        Animator animator;
        int forwardHash = Animator.StringToHash("Forward");
        int turnHash = Animator.StringToHash("Turn");
        int inputHash = Animator.StringToHash("Input");
        float moveForword = 0;
        float moveTurn = 0;
        public Transform CameraTarget { get; private set; }
        IInputManager inputManager;
        //点积
        float dot = 0;
        protected override void RefreshHandler ()
        {
            if (inputManager.GetAxis(InputAxisType._Vertical) != 0 || inputManager.GetAxis(InputAxisType._Horizontal) != 0)
                animator.SetBool(inputHash, true);
            else
                animator.SetBool(inputHash, false);
            moveForword = inputManager.GetAxis(InputAxisType._Vertical);
            moveTurn = inputManager.GetAxis(InputAxisType._Horizontal);
            if (inputManager.GetButton(InputButtonType._LeftShift))
                moveForword *= 2;
            //合并旋转
            MatchRotation();
            //{
            //    if (dot >= 0)
            //    {
            //        moveTurn += (1-dot);
            //    }
            //    else
            //    {
            //        //?????
            //    }
            //}
            animator.SetFloat(forwardHash, moveForword, forwardDampTime, Time.deltaTime);
            animator.SetFloat(turnHash, moveTurn, turnDampTime, Time.deltaTime);
        }
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponentInChildren<Animator>();
            CameraTarget = transform.Find("CameraTarget").transform;
            CosmosEntry.InputManager.SetInputDevice(new StandardInputDevice());
            inputManager = CosmosEntry.InputManager;
        }
        void MatchRotation()
        {
            var cameraController = CosmosEntry.ControllerManager.GetController<MultiplayerYBotCamera>(typeof( MultiplayerYBotCamera).Name);
            if (cameraController == null)
            {
                Utility.Debug.LogInfo("cameraController empty", MessageColor.RED);
                return;
            }
            Vector3 cameraForward= cameraController.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            dot = Vector3.Dot( transform.forward,cameraForward);
            Debug.DrawLine(transform.position, transform.position + cameraForward, Color.red, 0.2f);
        }
    }
}