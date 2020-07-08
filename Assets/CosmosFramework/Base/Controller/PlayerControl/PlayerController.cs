using UnityEngine;
using System.Collections;
using Cosmos.Input;
namespace Cosmos
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : ControllerBase
    {
        [SerializeField]
        [Range(0, 1)]
        float forwardDampTime = 0.1f;
        [SerializeField]
        [Range(0, 1)]
        float turnDampTime = 0.1f;
        [Header("相机控制器所挂载的对象名")]
        [SerializeField]
        string cameraControllerName = "PlayerCamera";
        Animator animator;
        int forwardHash = Animator.StringToHash("Forward");
        int turnHash = Animator.StringToHash("Turn");
        int inputHash = Animator.StringToHash("Input");
        float moveForword = 0;
        float moveTurn = 0;
        LogicEventArgs<CameraTarget> controllerEventArgs;
        //点积
        float dot = 0;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponentInChildren<Animator>();
            controllerEventArgs = new LogicEventArgs<CameraTarget>().SetData(GetComponentInChildren<CameraTarget>());
            Facade.SetInputDevice(new StandardInputDevice());
        }
        private void Start()
        {
            Facade.DispatchEvent(ControllerEventCodeParams.CONTROLLER_INPUT, this, controllerEventArgs);
        }
        protected override void UpdateHandler()
        {
            //if (inputEventArgs.Data.HorizVertAxis.magnitude != 0)
            if (Facade.GetAxis(InputAxisType.Vertical) != 0|| Facade.GetAxis(InputAxisType.Horizontal)!=0)
                animator.SetBool(inputHash, true);
            else
                animator.SetBool(inputHash, false);
            moveForword =Facade.GetAxis(InputAxisType.Vertical);
            moveTurn = Facade.GetAxis(InputAxisType.Horizontal);
            if (Facade.GetButton(InputButtonType.LeftShift))
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
        void MatchRotation()
        {
            var cameraController = Facade.GetController<CameraController>(c=>c.ControllerName==cameraControllerName);
            Vector3 cameraForward= cameraController.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            dot = Vector3.Dot( transform.forward,cameraForward);
            Debug.DrawLine(transform.position, transform.position + cameraForward, Color.red, 0.2f);
        }
    }
}