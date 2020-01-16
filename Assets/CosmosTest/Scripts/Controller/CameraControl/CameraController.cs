using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Input;

namespace Cosmos
{
    public class CameraController : CharacterInputController
    {
        [SerializeField] Vector3 offset;
        [SerializeField]float distanceFromTarget=10;
        [SerializeField] bool lockCursor=false;
        [SerializeField] Vector2 pitchMinMax = new Vector2(-60, 85);
        CameraTarget cameraTarget;
        CameraTarget CameraTarget { get { if (cameraTarget == null)
                    cameraTarget = GameObject.FindGameObjectWithTag("Player").
                        GetComponentInChildren<CameraTarget>();return cameraTarget;} }
        float yaw;
        float pitch;
        short lateUpdateID;
        Vector3 currentRotation;
        protected override void OnInitialization()
        {
            Facade.Instance.AddMonoListener(LateUpdateCamera, Mono.UpdateType.Update, (id) => lateUpdateID = id);
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        protected override void OnTermination()
        {
            Facade.Instance.RemoveMonoListener(LateUpdateCamera, Mono.UpdateType.Update, lateUpdateID);
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            inputEventArg = arg as Input.InputEventArgs;
            yaw += inputEventArg.MouseAxis.x;
            pitch -= inputEventArg.MouseAxis.y;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        }
        void LateUpdateCamera()
        {
            Quaternion camPosRotation = Quaternion.Euler(pitch,yaw, 0);
            transform.position = camPosRotation * offset*distanceFromTarget + CameraTarget.transform.position;
            transform.LookAt(CameraTarget.transform.position);
        }
    }
}