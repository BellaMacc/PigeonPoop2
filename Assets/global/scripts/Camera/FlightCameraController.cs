using UnityEngine;
using Unity.Cinemachine;
public class FlightCameraController : CameraController
{
    [Header("Flight References")]
    [SerializeField] private Transform player;

    [Header("Pitch")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Speed Camera Effects")]
    [SerializeField] private FlightMovement flightMovement;
    [SerializeField] private CinemachineCamera cineCamera;
    [SerializeField] private CinemachineThirdPersonFollow thirdPersonFollow;

    [SerializeField] private float speedThreshold = 1.1f;
    [SerializeField] private float maxFOV = 55f;
    [SerializeField] private float minCameraDistance = 8f;
    [SerializeField] private float zoomSmoothness = 3f;

    [Header("Speed Pitch Limits")]
    [SerializeField] private float speedMinPitch = -30f;
    [SerializeField] private float speedMaxPitch = 30f;
    [SerializeField] private float pitchLimitSmoothness = 3f;

    private float defaultFOV;
    private float defaultCameraDistance;




    private Vector2 cameraInput;
    private float pitch;
    private float currentMinPitch;
    private float currentMaxPitch;

    private void Awake()
    {
        if (cameraTarget == null || player == null)
        {
            Debug.LogError(
                "FlightCameraController: Missing camera target or player reference.",
                this
            );

            enabled = false;
            return;
        }

        pitch = 0f;

        currentMinPitch = minPitch;
        currentMaxPitch = maxPitch;

        cameraTarget.localRotation = Quaternion.identity;

        defaultFOV = cineCamera.Lens.FieldOfView;
        defaultCameraDistance = thirdPersonFollow.CameraDistance;

        
    }

    public override void HandleCameraInput(Vector2 input)
    {
        cameraInput = input;
    }

    private void Update()
    {
        if (cameraTarget == null || player == null)
            return;

        pitch -= cameraInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, currentMinPitch, currentMaxPitch);

        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        UpdateSpeedCamera();
    }
    /*
    private void LateUpdate()
    {
        if (cameraTarget == null || player == null)
            return;

        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }*/

    public void ResetPitch()
    {
        pitch = 0f;
        cameraInput = Vector2.zero;
    }

    //------------------------------------------------------------effects

    private void UpdateSpeedCamera()
    {
        float speedRatio = flightMovement.GetCurrentSpeed() / flightMovement.GetBaseSpeed();
        float speedFactor = Mathf.InverseLerp(speedThreshold, flightMovement.GetMaxSpeed() / flightMovement.GetBaseSpeed(), speedRatio);

        float targetFOV = Mathf.Lerp(defaultFOV, maxFOV, speedFactor);
        float targetDistance = Mathf.Lerp(defaultCameraDistance, minCameraDistance, speedFactor);

        float targetMinPitch = Mathf.Lerp(minPitch, speedMinPitch, speedFactor);
        float targetMaxPitch = Mathf.Lerp(maxPitch, speedMaxPitch, speedFactor);

        currentMinPitch = Mathf.Lerp(currentMinPitch, targetMinPitch, pitchLimitSmoothness * Time.deltaTime);
        currentMaxPitch = Mathf.Lerp(currentMaxPitch, targetMaxPitch, pitchLimitSmoothness * Time.deltaTime);

        pitch = Mathf.Clamp(pitch, currentMinPitch, currentMaxPitch);

        cineCamera.Lens.FieldOfView = Mathf.Lerp(cineCamera.Lens.FieldOfView, targetFOV, zoomSmoothness * Time.deltaTime);
        thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetDistance, zoomSmoothness * Time.deltaTime);
    }
}
