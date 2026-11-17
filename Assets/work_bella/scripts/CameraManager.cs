using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineCamera normalCamera;
    [SerializeField] private CinemachineCamera poopCamera;

    [Header("Camera Priority")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    private bool poopMode;

    private void Start()
    {
        SetCameraMode(false);
    }

    public void TogglePoopMode()
    {
        poopMode = !poopMode;
        SetCameraMode(poopMode);
    }

    private void SetCameraMode(bool usePoopCamera)
    {
        normalCamera.Priority = usePoopCamera
            ? inactivePriority
            : activePriority;

        poopCamera.Priority = usePoopCamera
            ? activePriority
            : inactivePriority;
    }

    public bool IsPoopMode()
    {
        return poopMode;
    }
}


