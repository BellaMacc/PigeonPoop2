using UnityEngine;

public class FlightCameraController : CameraController
{
    [Header("Flight References")]
    [SerializeField] private Transform player;

    [Header("Pitch")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;

    private Vector2 cameraInput;
    private float pitch;

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
        cameraTarget.localRotation = Quaternion.identity;
    }

    public override void HandleCameraInput(Vector2 input)
    {
        cameraInput = input;
    }

    private void Update()
    {
        pitch -= cameraInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void LateUpdate()
    {
        if (cameraTarget == null || player == null)
            return;

        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void ResetPitch()
    {
        pitch = 0f;
        cameraInput = Vector2.zero;
    }
}
