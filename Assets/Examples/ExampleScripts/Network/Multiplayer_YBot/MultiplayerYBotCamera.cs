using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Input;
using Cosmos.Event;
using System;

namespace Cosmos
{
    public class MultiplayerYBotCamera : ControllerBase<MultiplayerYBotCamera>
    {
        [Range(0.5f,15)]
        [SerializeField]float distanceFromTarget=4;
        [SerializeField] Vector2 pitchMinMax = new Vector2(-70, 85);
        [SerializeField] float yawSpeed=15;
        [SerializeField] float pitchSpeed=10;
        [SerializeField] float cameraViewDamp=10;
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

        protected override void Awake()
        {
            base.Awake();
            originalPos = transform.position;
            originalRot= transform.rotation.eulerAngles;
            inputManager = GameManager.GetModule<IInputManager>();
        }
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
            if (inputManager.GetButtonDown(InputButtonType._MouseLeft) ||
                inputManager.GetButtonDown(InputButtonType._MouseRight) ||
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
        protected override void RefreshHandler()
        {
            yaw = -inputManager.GetAxis(InputAxisType._MouseX);
            pitch = inputManager.GetAxis(InputAxisType._MouseY);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            if (inputManager.GetAxis(InputAxisType._MouseScrollWheel) != 0)
                Utility.Debug.LogInfo("MouseScrollWheel ", MessageColor.INDIGO);
            distanceFromTarget -= inputManager.GetAxis(InputAxisType._MouseScrollWheel);
            distanceFromTarget = Mathf.Clamp(distanceFromTarget, 0.5f, 10);
            HideMouse();
        }
        protected override void OnEnable()
        {
            CosmosEntry.LateRefreshHandler+= LateUpdateCamera;
        }
        protected override void OnDisable()
        {
            CosmosEntry.LateRefreshHandler -= LateUpdateCamera;
        }
        protected override void OnValidate()
        {
            yawSpeed = Mathf.Clamp(yawSpeed, 0, 1000);
            pitchSpeed = Mathf.Clamp(pitchSpeed, 0, 1000);
            cameraViewDamp = Mathf.Clamp(cameraViewDamp, 0, 1000);
            pitchMinMax = Utility.Unity.Clamp(pitchMinMax, new Vector2(-90, 0), new Vector2(0, 90));
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
            transform.position = target.transform.position;
        }
    }
}