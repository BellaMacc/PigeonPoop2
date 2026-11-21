using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingCameraController : CameraController
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Camera Pivot")]
    [SerializeField]
    private Vector3 pivotOffset =new Vector3(0f, 1.5f, 0f);

    [Header("Camera Rotation")]
    [SerializeField] private float sensitivity = 120f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 65f;


    [Header("Mouse Only Settings")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    [Header("Data Reads")]
    [SerializeField] private Vector2 cameraInput;


    // Private Variables
    private float yaw;
    private float pitch;
    private bool usingMouse;


    private void Awake()
    {
        if (cameraTarget == null || player == null)
        {
            Debug.LogError("WalkingCameraController: Missing camera target or player reference.", this);
            enabled = false;
            return;
        }

        // Initialize rotation from the existing camera target
        Vector3 startingRotation = cameraTarget.eulerAngles;

        yaw = startingRotation.y;
        pitch = Mathf.DeltaAngle(0f, startingRotation.x);

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

   

    public override void HandleCameraInput(Vector2 input)
    {
        // Determine which device generated the input
        usingMouse = Mouse.current != null &&
                     Mouse.current.delta.ReadValue() != Vector2.zero;

        if (Gamepad.current != null &&
            Gamepad.current.rightStick.ReadValue() != Vector2.zero)
        {
            usingMouse = false;
        }

        // Store the latest stick input
        cameraInput = input;

        
    }

    private void Update()
    {
        if (!usingMouse)
        {
            //controller calculation
            // Continuously rotate while stick is held
            yaw += cameraInput.x * sensitivity * Time.deltaTime;

            pitch -= cameraInput.y * sensitivity * Time.deltaTime;
        }
        else
        {
            yaw += cameraInput.x * mouseSensitivity;
            pitch -= cameraInput.y * mouseSensitivity;
        }

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }


    private void LateUpdate()
    {
        if (player == null || cameraTarget == null)
            return;

        cameraTarget.position = player.position + pivotOffset;

        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Debug.Log($"Camera Target Rotation: {cameraTarget.eulerAngles}");
    }

    public Vector3 GetForwardDirection()
    {
        Vector3 forward = cameraTarget.forward;
        forward.y = 0f;

        return forward.normalized;
    }

    public Vector3 GetRightDirection()
    {
        Vector3 right = cameraTarget.right;
        right.y = 0f;

        return right.normalized;
    }
}

/*Notes about both this camera controller vs the cinemachine controller
 * WalkingCameraController controls the pivot:
- Follows the bird's position.
- Controls yaw and pitch.
- Determines the direction the camera faces.
CinemachineFollow controls the camera position:
- Determines how far behind the pivot the camera sits.
- Determines camera height relative to the pivot.
- Applies positional damping.
*/

