using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos;
namespace Cosmos.Lockstep
{
    public class LockstepCamera : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5;
        [SerializeField] Transform targetTransform;

        [SerializeField] Vector3 targetViewRotation;
        [SerializeField] Vector3 targetViewOffset;

        Vector3 startPosition;
        Vector3 startRotation;
        IController cameraController;
        public void SetTransformTarget(Transform target)
        {
            targetTransform = target;
        }
        public void Clear()
        {
            targetTransform = null;
            transform.position = startPosition;
            transform.eulerAngles = startRotation;
        }
        void Start()
        {
            cameraController = GameEntry.ControllerManager.CreateController("LockstepCamera", this);
            startPosition = transform.position;
            startRotation = transform.eulerAngles;
        }
        void LateUpdate()
        {
            if (targetTransform == null)
            {
                transform.position = startPosition;
                transform.eulerAngles = startRotation;
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, targetTransform.position + targetViewOffset, Time.deltaTime * moveSpeed);
                transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetViewRotation, Time.deltaTime * 5);
            }
        }
        void OnDestroy()
        {
            GameEntry.ControllerManager.ReleaseController(cameraController);
        }
    }
}
