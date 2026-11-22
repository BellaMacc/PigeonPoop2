using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 240;
    [SerializeField] private float rotationSharpness = 8f;

    private Vector3 currentVelocity;
    

    public Vector3 CalculateMovement(
        Vector2 input,
        Transform movementReference,
        Transform playerTransform,
        float deltaTime)
    {
        // Get camera-relative directions
        Vector3 forward = movementReference.forward;
        Vector3 right = movementReference.right;

        // Ignore camera pitch
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Calculate desired movement direction
        Vector3 inputDirection =
            forward * input.y +
            right * input.x;

        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        Vector3 targetVelocity = inputDirection * moveSpeed;

        // Acceleration / deceleration
        float accelerationRate =
            inputDirection.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            accelerationRate * deltaTime
        );

        // Rotate towards movement direction
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(inputDirection);

            float rotationSharpness = 8f;

            float rotationFactor = 1f - Mathf.Exp(-rotationSharpness * deltaTime);

            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                rotationFactor
            );
        }

        return currentVelocity;
    }

    public void ResetMovement()
    {
        currentVelocity = Vector3.zero;
    }
}
