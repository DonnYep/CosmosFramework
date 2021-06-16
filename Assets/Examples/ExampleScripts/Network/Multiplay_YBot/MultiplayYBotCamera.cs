using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Input;
using Cosmos.Event;
using System;

namespace Cosmos
{
    public class MultiplayYBotCamera : ControllerBase<MultiplayYBotCamera>
    {
        [Range(0.5f,15)]
        [SerializeField]float distanceFromTarget=4;
        [SerializeField] Vector2 pitchMinMax = new Vector2(-70, 85);
        [SerializeField] float yawSpeed=15;
        [SerializeField] float pitchSpeed=10;
        [SerializeField] float cameraViewDamp=10;
        [SerializeField] float sensitivity = 5f;
        Camera cam;
        Transform target;
        IInputManager inputManager;
        float yaw;
        float pitch;
        //当前与相机目标的距离
        float currentDistance;
        Vector3 cameraOffset = Vector3.zero;

        Vector3 originalPos;
        Vector3 originalRot;

        public Vector3 PitchYaw { get; private set; }
        public Vector3 CameraForwordDirection()
        {
            return cam.transform.forward;
        }
        public void HideMouse()
        {
            if (inputManager.GetButtonDown(InputButtonType._MouseRight) ||
                inputManager.GetButtonDown(InputButtonType._MouseMiddle))
                LockMouse();
            if (inputManager.GetButtonDown(InputButtonType._Escape))
                UnlockMouse();
        }
        public void SetCameraTarget(Transform cameraTarget)
        {
            this.target = cameraTarget;
            cam = GetComponentInChildren<Camera>();
            cam.transform.ResetLocalTransform();
            transform.rotation = this.target.transform.rotation;
            currentDistance = distanceFromTarget;
            cameraOffset.z = -currentDistance;
            cam.transform.localPosition = cameraOffset;
        }
        public void ReleaseTarget()
        {
            this.target = null;
            transform.position = originalPos;
            transform.eulerAngles=originalRot;
        }
        protected override void OnInitialization()
        {
            originalPos = transform.position;
            originalRot = transform.rotation.eulerAngles;
            inputManager = GameManager.GetModule<IInputManager>();
        }
        [TickRefresh]
        protected  void RefreshHandler()
        {
            yaw = -inputManager.GetAxis(InputAxisType._MouseX);
            pitch = inputManager.GetAxis(InputAxisType._MouseY);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            if (inputManager.GetAxis(InputAxisType._MouseScrollWheel) != 0)
                //Utility.Debug.LogInfo("MouseScrollWheel ", MessageColor.INDIGO);
            distanceFromTarget -= inputManager.GetAxis(InputAxisType._MouseScrollWheel);
            distanceFromTarget = Mathf.Clamp(distanceFromTarget, 0.5f, 10);
            HideMouse();
        }
        protected  void OnEnable()
        {
            CosmosEntry.LateRefreshHandler+= LateUpdateCamera;
        }
        protected void OnDisable()
        {
            CosmosEntry.LateRefreshHandler -= LateUpdateCamera;
        }
        protected void OnValidate()
        {
            yawSpeed = Mathf.Clamp(yawSpeed, 0, 1000);
            pitchSpeed = Mathf.Clamp(pitchSpeed, 0, 1000);
            cameraViewDamp = Mathf.Clamp(cameraViewDamp, 0, 1000);
            pitchMinMax = Utility.Unity.Clamp(pitchMinMax, new Vector2(-90, 0), new Vector2(0, 90));
        }
        void LateUpdateCamera()
        {
            if (Utility.Assert.IsNull(target))
                return;
            cameraOffset.z = -distanceFromTarget;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, 
                cameraOffset, Time.deltaTime * cameraViewDamp);
            float yawResult = yaw * Time.deltaTime*yawSpeed;
            float pitchResult = pitch * Time.deltaTime*pitchSpeed;
            Vector3 rotResult = new Vector3(pitchResult, yawResult, 0);
            PitchYaw= rotResult;
            transform.eulerAngles -= rotResult*sensitivity;
            transform.position =Vector3.Lerp(transform.position, target.transform.position,Time.deltaTime*cameraViewDamp);
        }
        void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}