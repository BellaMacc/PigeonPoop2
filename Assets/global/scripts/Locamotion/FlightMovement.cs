using UnityEngine;

public class FlightMovement : MonoBehaviour
{
    [Header("Flight References")]
    [SerializeField] private Transform flightDirection;

    [Header("Forward Movement")]
    [SerializeField] private float baseSpeed = 8f;
    [SerializeField] private float maxSpeed = 14f;
    [SerializeField] private float minSpeed = 4f;
    [SerializeField] private float speedAdjustment = 4f;

    [Header("Steering")]
    [SerializeField] private float turnSpeed = 100f;

    [Header("Flapping")]
    [SerializeField] private float flapForce = 7f;
    [SerializeField] private float flapGracePeriod = 1.5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -15f;

    private float currentSpeed;
    private float verticalVelocity;
    private float timeSinceLastFlap;

    private void Awake()
    {
        currentSpeed = baseSpeed;
    }

    // Called when the player presses the flap button
    public void HandleFlap()
    {
        verticalVelocity = flapForce;
        timeSinceLastFlap = 0f;
    }

    public Vector3 CalculateMovement(
        Vector2 input,
        Transform player,
        float deltaTime)
    {
        // Steer left and right
        player.Rotate(
            Vector3.up,
            input.x * turnSpeed * deltaTime,
            Space.World
        );

        // Adjust forward speed
        float targetSpeed = baseSpeed + input.y * speedAdjustment;
        targetSpeed = Mathf.Clamp(targetSpeed, minSpeed, maxSpeed);

        currentSpeed = targetSpeed;

        // Apply gravity after the flap grace period
        timeSinceLastFlap += deltaTime;

        if (timeSinceLastFlap > flapGracePeriod)
        {
            verticalVelocity += gravity * deltaTime;
        }

        // Move in the direction the bird is facing
        Vector3 forwardVelocity = flightDirection.forward * currentSpeed;

        // Add flap / falling velocity
        forwardVelocity.y += verticalVelocity;

        return forwardVelocity;
    }

    public void ResetFlight()
    {
        currentSpeed = baseSpeed;
        verticalVelocity = 0f;
        timeSinceLastFlap = 0f;
    }
}

