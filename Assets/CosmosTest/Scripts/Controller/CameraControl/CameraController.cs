using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Input;
using Cosmos.Event;

namespace Cosmos
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Vector3 offset;
        [SerializeField]float cameraZoom=10;
        [SerializeField] float pitch = 2;
        float currentYaw;
        short updateID;
        short lateUpdateID;
        CameraTarget cameraTarget;
        InputEventArgs inputHandler;
        private void OnEnable()
        {
            Register();
        }
        private void OnDestroy()
        {
            DeRegister();
        }
        void Register()
        {
            Facade.Instance.AddMonoListener(UpdateCamera, Mono.UpdateType.LateUpdate,(id)=> updateID = id);
            Facade.Instance.AddMonoListener(LateUpdateCamera, Mono.UpdateType.Update,(id)=> lateUpdateID = id);
            Facade.Instance.AddEventListener(ApplicationConst._InputEventKey, InputHandler);
        }
        void InputHandler(object sender, GameEventArgs arg)
        {
            inputHandler = arg as InputEventArgs;

        }
            void DeRegister()
        {
            Facade.Instance.RemoveMonoListener(UpdateCamera, Mono.UpdateType.LateUpdate, updateID);
            Facade.Instance.RemoveMonoListener(LateUpdateCamera, Mono.UpdateType.Update, lateUpdateID);
        }
        void UpdateCamera()
        {
        }
        void LateUpdateCamera()
        {
            transform.position = cameraTarget.transform.position - offset * cameraZoom;
            transform.LookAt(cameraTarget.transform.position + Vector3.up * pitch);
        }
    }
}