using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineCamera normalCamera;
    [SerializeField] private CinemachineCamera poopCamera;

    [Header("Priority Settings")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    private bool isPoopCameraActive;

    private void Start()
    {
        SetPoopCamera(false);
    }

    public void SetPoopCamera(bool active)
    {
        isPoopCameraActive = active;

        normalCamera.Priority = active
            ? inactivePriority
            : activePriority;

        poopCamera.Priority = active
            ? activePriority
            : inactivePriority;
    }

    public bool IsPoopCameraActive()
    {
        return isPoopCameraActive;
    }
    
}


