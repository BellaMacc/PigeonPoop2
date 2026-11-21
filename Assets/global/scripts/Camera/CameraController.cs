using Unity.Cinemachine;
using UnityEngine;

public abstract class CameraController : MonoBehaviour
{
    [SerializeField] protected Transform cameraTarget;
    [SerializeField] protected CinemachineCamera cinemachineCamera;

    public virtual void EnableCamera()
    {
        enabled = true;
    }

    public virtual void DisableCamera()
    {
        enabled = false;
    }

    public abstract void HandleCameraInput(Vector2 input);
}
