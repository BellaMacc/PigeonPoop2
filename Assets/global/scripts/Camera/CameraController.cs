using Unity.Cinemachine;
using UnityEngine;

public abstract class CameraController : MonoBehaviour
{
    [SerializeField] protected Transform cameraTarget;
    [SerializeField] protected CinemachineCamera cinemachineCamera;

    public virtual void EnableCamera()
    {
        enabled = true;

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Priority = 10;
        }
    }

    public virtual void DisableCamera()
    {
        enabled = false;

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Priority = 0;
        }
    }
    public abstract void HandleCameraInput(Vector2 input);
}
