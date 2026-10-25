using UnityEngine;
namespace Cosmos.Lockstep
{
    public class LockstepCamera : MonoBehaviour
    {
        /// <summary>
        /// 当前场景中的相机实例；由 Awake 注册、OnDestroy 注销，供多人同步层取用。
        /// </summary>
        public static LockstepCamera Instance { get; private set; }

        [SerializeField] float moveSpeed = 5;
        [SerializeField] Transform targetTransform;

        [SerializeField] Vector3 targetViewRotation;
        [SerializeField] Vector3 targetViewOffset;

        Vector3 startPosition;
        Vector3 startRotation;
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
        void Awake()
        {
            Instance = this;
        }
        void Start()
        {
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
            if (Instance == this)
                Instance = null;
        }
    }
}
