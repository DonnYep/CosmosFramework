using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Input;

namespace Cosmos
{
    public class CameraController : CFController
    {
        [Range(0.5f,10)]
        [SerializeField]float distanceFromTarget=10;
        [SerializeField] Vector2 pitchMinMax = new Vector2(-60, 85);
        [SerializeField] float yawSpeed=15;
        [SerializeField] float pitchSpeed=10;
        [SerializeField] float cameraViewDamp=10;
        Camera cam;
        public CameraTarget CameraTarget { get; private set; }
        float yaw;
        float pitch;
        short lateUpdateID;
        //当前与相机目标的距离
        float currentDistance;
        Vector3 cameraOffset = Vector3.zero;
        protected override void OnInitialization()
        {
            Facade.Instance.AddMonoListener(LateUpdateCamera, Mono.UpdateType.LateUpdate, (id) => lateUpdateID = id);
            Facade.Instance.AddEventListener(ControllerEventParam.CONTROLLEREVENT_INPUT, CameraHandler);
            Facade.Instance.RegisterController(this);
        }
        protected override void OnTermination()
        {
            Facade.Instance.RemoveMonoListener(LateUpdateCamera, Mono.UpdateType.LateUpdate, lateUpdateID);
            Facade.Instance.RemoveEventListener(ControllerEventParam.CONTROLLEREVENT_INPUT, CameraHandler);
            Facade.Instance.DeregisterController(this);
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            inputEventArgs = arg as InputEventArgs;
            yaw = -inputEventArgs.MouseAxis.x;
            pitch = inputEventArgs.MouseAxis.y;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            currentDistance -= inputEventArgs.MouseButtonWheel;
            currentDistance = Mathf.Clamp(currentDistance, 0.5f, 10);
            inputEventArgs.HideMouse();
        }
        void LateUpdateCamera()
        {
            cameraOffset.z = -currentDistance;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, 
                cameraOffset, Time.deltaTime * cameraViewDamp);
            float yawResult = yaw * Time.deltaTime*yawSpeed;
            float pitchResult = pitch * Time.deltaTime*pitchSpeed;
            Vector3 rotResult = new Vector3(pitchResult, yawResult, 0);
            transform.eulerAngles -= rotResult;
            transform.position = CameraTarget.transform.position;
        }
        /// <summary>
        /// 开始时候执行一次
        /// 属于Start函数
        /// </summary>
        void CameraHandler(object sender, GameEventArgs args)
        {
            controllerEventArgs = args as ControllerEventArgs;
            CameraTarget = controllerEventArgs.CameraTarget;
            cam = GetComponentInChildren<Camera>();
            cam.transform.ResetLocalTransform();
            transform.rotation = CameraTarget.transform.rotation;
            currentDistance = distanceFromTarget;
            cameraOffset.z = -currentDistance;
            cam.transform.localPosition = cameraOffset;
        }
    }
}