using UnityEngine;

public class PoopModeMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 5f;

    [Header("Steering")]
    [SerializeField] private float turnSpeed = 80f;
    [SerializeField] private float turnEaseInTime = 0.06f;
    [SerializeField] private float turnEaseOutTime = 0.25f;

    private float currentTurnSpeed;
    private float turnSmoothVelocity;

    public Vector3 CalculateMovement(Vector2 input, Transform player, float deltaTime)
    {
        CalculateSteering(input, player, deltaTime);

        Vector3 forward = player.forward;
        forward.y = 0f;
        forward.Normalize();

        return forward * forwardSpeed;
    }

    private void CalculateSteering(Vector2 input, Transform player, float deltaTime)
    {
        float targetTurnSpeed = input.x * turnSpeed;
        float smoothTime = Mathf.Abs(input.x) > 0.01f ? turnEaseInTime : turnEaseOutTime;

        currentTurnSpeed = Mathf.SmoothDamp(currentTurnSpeed, targetTurnSpeed, ref turnSmoothVelocity, smoothTime, Mathf.Infinity, deltaTime);

        player.Rotate(Vector3.up, currentTurnSpeed * deltaTime, Space.World);
    }

    public void ResetMovement()
    {
        currentTurnSpeed = 0f;
        turnSmoothVelocity = 0f;
    }
}
