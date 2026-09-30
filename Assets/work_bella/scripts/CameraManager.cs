using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] InputManager inputManager;

    [Header("Camera Follow Variables")]
    [SerializeField] Transform targetTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform cameraPivot;

    [SerializeField] float cameraFollowSpeed = 0.2f;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    [Header("Camera Look Variables")]
    float lookYaw; //
    float lookPitch;

    [SerializeField] float pitchLookSpeed;
    [SerializeField] float yawLookSpeed;

    [SerializeField] Vector2 clampPitch;
    //[SerializeField] Vector2 clampYaw;


    public void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(cameraTransform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }


    public void RotateCamera()
    {
        lookYaw += (inputManager.ReadCameraMovement().x * yawLookSpeed);
       //lookYaw = Mathf.Clamp(lookYaw, clampYaw.x, clampYaw.y);

        lookPitch -= (inputManager.ReadCameraMovement().y * pitchLookSpeed);
        lookPitch = Mathf.Clamp(lookPitch, clampPitch.x, clampPitch.y);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookYaw;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        cameraTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = lookPitch;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
;
    }


    private void LateUpdate()
    {
        FollowTarget();
        RotateCamera();
    }
}
