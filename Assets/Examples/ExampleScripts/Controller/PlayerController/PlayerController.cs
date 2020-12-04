using UnityEngine;
using System.Collections;
using Cosmos.Input;
using System;

namespace Cosmos
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : ControllerBase<PlayerController>
    {
        [SerializeField]
        [Range(0, 1)]
        float forwardDampTime = 0.1f;
        [SerializeField]
        [Range(0, 1)]
        float turnDampTime = 0.1f;
        [Header("相机控制器所挂载的对象名")]
        [SerializeField]
        string cameraControllerName = "CameraController";
        Animator animator;
        int forwardHash = Animator.StringToHash("Forward");
        int turnHash = Animator.StringToHash("Turn");
        int inputHash = Animator.StringToHash("Input");
        float moveForword = 0;
        float moveTurn = 0;
        LogicEventArgs<CameraTarget> controllerEventArgs;
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
            controllerEventArgs = new LogicEventArgs<CameraTarget>().SetData(GetComponentInChildren<CameraTarget>());
            GameManager.GetModule<IInputManager>().SetInputDevice(new StandardInputDevice());
            inputManager = GameManager.GetModule<IInputManager>();
        }
        private void Start()
        {
            GameManager.GetModule<IEventManager>().DispatchEvent(ControllerEventDefine.CTRL_INPUT, this, controllerEventArgs);
        }
        void MatchRotation()
        {
            var cameraController = GameManager.GetModule<IControllerManager>().GetController<CameraController>(c=>c.ControllerName==cameraControllerName);
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