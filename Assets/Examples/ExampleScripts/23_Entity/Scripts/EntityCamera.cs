using UnityEngine;

public class EntityCamera : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] float cameraMoveSpeed = 15;
    public void SetCameraTarget(Transform target)
    {
        cameraTarget = target;
        transform.position = target.position;
        transform.eulerAngles = target.eulerAngles;
    }
    public void ClearCameraTarger()
    {
        cameraTarget = null;
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
    }
    private void LateUpdate()
    {
        if (cameraTarget != null)
            transform.position = Vector3.Lerp(transform.position, cameraTarget.position, Time.deltaTime * cameraMoveSpeed);
    }
}
