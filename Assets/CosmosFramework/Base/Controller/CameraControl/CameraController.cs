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
        [SerializeField] bool lockCursor=false;
        [SerializeField] Vector2 pitchMinMax = new Vector2(-60, 85);
        [SerializeField] float yawSpeed=5;
        [SerializeField] float pitchSpeed=5;
        Camera cam;
        CameraTarget cameraTarget;
        CameraTarget CameraTarget { get { if (cameraTarget == null)
                    cameraTarget = GameObject.FindGameObjectWithTag("Player").
                        GetComponentInChildren<CameraTarget>();return cameraTarget;} }
        float yaw;
        float pitch;
        short lateUpdateID;
        protected override void OnInitialization()
        {
            Facade.Instance.AddMonoListener(LateUpdateCamera, Mono.UpdateType.LateUpdate, (id) => lateUpdateID = id);
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            InitCamera();
        }
        protected override void OnTermination()
        {
            Facade.Instance.RemoveMonoListener(LateUpdateCamera, Mono.UpdateType.LateUpdate, lateUpdateID);
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            inputEventArg = arg as InputEventArgs;
            yaw = -inputEventArg.MouseAxis.x;
            pitch = inputEventArg.MouseAxis.y;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            if (inputEventArg.Escape)
                inputEventArg.SetMouseLock(!inputEventArg.IsMouseLocked);
        }
        /// <summary>
        /// 初始化摄像机
        /// </summary>
        void InitCamera()
        {
            cam = GetComponentInChildren<Camera>();
            cam.transform.ResetLocalTransform();
            transform.rotation = CameraTarget.transform.rotation;
        }
        void LateUpdateCamera()
        {
            cam.transform.localPosition = new Vector3( 0,0,-distanceFromTarget);
            float yawResult = yaw * Time.deltaTime*yawSpeed;
            float pitchResult = pitch * Time.deltaTime*pitchSpeed;
            Vector3 rotResult = new Vector3(pitchResult, yawResult, 0);
            transform.eulerAngles -= rotResult;
            transform.position = CameraTarget.transform.position;
        }

    }
}