using UnityEngine;
using Unity.Cinemachine;
public class FlightCameraController : CameraController
{
    [Header("Flight References")]
    [SerializeField] private FlightMovement flightMovement;

    [Header("Flight Cameras")]
    //[SerializeField] private CinemachineCamera normalFlightCamera;
    [SerializeField] private CinemachineCamera speedFlightCamera;
    [SerializeField] private float speedCameraSwitchThreshold = 0.5f;

    [Header("Pitch")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Speed Pitch Limits")]
    [SerializeField] private float speedMinPitch = -30f;
    [SerializeField] private float speedMaxPitch = 30f;
    [SerializeField] private float pitchLimitSmoothness = 3f;

    private Vector2 cameraInput;
    private float pitch;

    private float currentMinPitch;
    private float currentMaxPitch;

    private bool isSpeedCameraActive;

    private void Awake()
    {
        if (cameraTarget == null || flightMovement == null ||
            cinemachineCamera == null || speedFlightCamera == null)
        {
            Debug.LogError("FlightCameraController: Missing references.", this);
            enabled = false;
            return;
        }

        pitch = 0f;
        currentMinPitch = minPitch;
        currentMaxPitch = maxPitch;

        cameraTarget.localRotation = Quaternion.identity;

        SetSpeedCamera(false);
    }

    public override void HandleCameraInput(Vector2 input)
    {
        cameraInput = input;
    }

    private void Update()
    {
        UpdateSpeedCamera();
        UpdatePitch();
    }

    private void UpdatePitch()
    {
        float deltaTime = Time.deltaTime;

        pitch -= cameraInput.y * sensitivity * deltaTime;
        pitch = Mathf.Clamp(pitch, currentMinPitch, currentMaxPitch);

        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void UpdateSpeedCamera()
    {
        float baseSpeed = flightMovement.GetBaseSpeed();
        if (baseSpeed <= 0f) return;

        float speedRatio = flightMovement.GetCurrentSpeed() / baseSpeed;
        float maxSpeedRatio = flightMovement.GetMaxSpeed() / baseSpeed;

        float speedFactor = Mathf.InverseLerp(1.1f, maxSpeedRatio, speedRatio);

        float targetMinPitch = Mathf.Lerp(minPitch, speedMinPitch, speedFactor);
        float targetMaxPitch = Mathf.Lerp(maxPitch, speedMaxPitch, speedFactor);

        currentMinPitch = Mathf.Lerp(currentMinPitch, targetMinPitch, pitchLimitSmoothness * Time.deltaTime);
        currentMaxPitch = Mathf.Lerp(currentMaxPitch, targetMaxPitch, pitchLimitSmoothness * Time.deltaTime);

        bool useSpeedCamera = speedFactor >= speedCameraSwitchThreshold;

        if (useSpeedCamera != isSpeedCameraActive)
        {
            SetSpeedCamera(useSpeedCamera);
        }
    }

    private void SetSpeedCamera(bool active)
    {
        isSpeedCameraActive = active;

        cinemachineCamera.Priority = active ? 10 : 20;
        speedFlightCamera.Priority = active ? 20 : 10;
    }

    public void ResetPitch()
    {
        pitch = 0f;
        cameraInput = Vector2.zero;
        cameraTarget.localRotation = Quaternion.identity;
    }
}
