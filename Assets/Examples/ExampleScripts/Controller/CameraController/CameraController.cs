using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Input;
using System;

namespace Cosmos
{
    public class CameraController : ControllerBase<CameraController>
    {
        [Range(0.5f,15)]
        [SerializeField]float distanceFromTarget=10;
        [SerializeField] Vector2 pitchMinMax = new Vector2(-60, 85);
        [SerializeField] float yawSpeed=30;
        [SerializeField] float pitchSpeed=30;
        [SerializeField] float cameraViewDamp=10;
        Camera cam;
        public CameraTarget CameraTarget { get; private set; }

        float yaw;
        float pitch;
        //当前与相机目标的距离
        float currentDistance;
        Vector3 cameraOffset = Vector3.zero;
        LogicEventArgs<CameraTarget> controllerEventArgs;
        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        public void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
        public void HideMouse()
        {
            if (Facade.GetButtonDown(InputButtonType._MouseLeft) ||
                Facade.GetButtonDown(InputButtonType._MouseRight) ||
                Facade.GetButtonDown(InputButtonType._MouseMiddle))
                LockMouse();
            if (Facade.GetButtonDown(InputButtonType._Escape))
                UnlockMouse();
        }
        protected override void Awake()
        {
            base.Awake();
            Facade.LateRefreshHandler += LateUpdateCamera;
            Facade.AddEventListener(ControllerEventDefine.CTRL_INPUT, CameraHandler);
            //Facade.RegisterController(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Facade.LateRefreshHandler -= LateUpdateCamera;
            Facade.RemoveEventListener(ControllerEventDefine.CTRL_INPUT, CameraHandler);
            //Facade.DeregisterController(this);
            Utility.DebugLog("CameraController destory");
        }
        protected override void OnValidate()
        {
            yawSpeed = Mathf.Clamp(yawSpeed, 0, 1000);
            pitchSpeed = Mathf.Clamp(pitchSpeed, 0, 1000);
            cameraViewDamp = Mathf.Clamp(cameraViewDamp, 0, 1000);
            pitchMinMax = Utility.Unity.Clamp(pitchMinMax, new Vector2(-90, 0), new Vector2(0, 90));
        }
        protected override void RefreshHandler()
        {
            yaw = -Facade.GetAxis(InputAxisType._MouseX);
            pitch = Facade.GetAxis(InputAxisType._MouseY);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            if (Facade.GetAxis(InputAxisType._MouseScrollWheel) != 0)
                Utility.DebugLog("MouseScrollWheel " ,MessageColor.INDIGO);
            distanceFromTarget -= Facade.GetAxis(InputAxisType._MouseScrollWheel);
            distanceFromTarget = Mathf.Clamp(distanceFromTarget, 0.5f, 10);
            HideMouse();
        }
        void LateUpdateCamera()
        {
            cameraOffset.z = -distanceFromTarget;
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
            controllerEventArgs = args as LogicEventArgs<CameraTarget>;
            CameraTarget = controllerEventArgs.Data;
            cam = GetComponentInChildren<Camera>();
            cam.transform.ResetLocalTransform();
            transform.rotation = CameraTarget.transform.rotation;
            currentDistance = distanceFromTarget;
            cameraOffset.z = -currentDistance;
            cam.transform.localPosition = cameraOffset;
        }
    }
}