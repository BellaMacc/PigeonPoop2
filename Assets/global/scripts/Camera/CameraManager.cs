using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Mode")]
    [SerializeField] private CameraMode currentMode;

    [Header("Camera Controller References")]
    [SerializeField] private CameraController poopCameraCtrl;
    [SerializeField] private CameraController flightCameraCtrl;
    [SerializeField] private CameraController walkingCameraCtrl;

    [Header("Input")]
    [SerializeField] private InputManager inputManager;

    //Private Variables
    private CameraController activeCamera;

    private void Start()
    {
        SwitchModes(currentMode);
    }

    public void SwitchModes(CameraMode mode)
    {
        if (currentMode == mode && activeCamera != null)
            return;

        switch (mode){
            case (CameraMode.Walking):
                SwitchCameras(walkingCameraCtrl);
                currentMode = CameraMode.Walking;
                break;

            case (CameraMode.Flight):
                SwitchCameras(flightCameraCtrl);
                currentMode = CameraMode.Flight;    
                break;

            case (CameraMode.Poop):
                SwitchCameras(poopCameraCtrl);
                currentMode = CameraMode.Poop;
                break;

            default:
                Debug.Log("Something went wrong with the camera manager switching modes");
                break;
        }
    }
    public void SwitchCameras(CameraController newCamera)
    {
        if (newCamera == null || activeCamera == newCamera)
            return;

        if (activeCamera != null)
        {
            activeCamera.DisableCamera();
        }

        activeCamera = newCamera;
        activeCamera.EnableCamera();
    }

    public void HandleCameraMovement()
    {
        Debug.Log("Camera Manager received input");

        if (activeCamera == null)
        {
            Debug.LogWarning("No active camera!");
            return;
        }

        Vector2 input = inputManager.ReadCameraMovement();

        Debug.Log("Sending input to: " + activeCamera.name);

        activeCamera.HandleCameraInput(input);
    }
}


public enum CameraMode
{
    Walking,
    Flight,
    Poop
}