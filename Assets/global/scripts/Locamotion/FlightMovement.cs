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
    [SerializeField] private float flapForce = 12f;
    [SerializeField] private float maxUpwardVelocity = 20f;
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
        // Immediately add an upward impulse
        verticalVelocity += flapForce;

        // Prevent unlimited upward acceleration
        verticalVelocity = Mathf.Min(
            verticalVelocity,
            maxUpwardVelocity
        );

        // Reset the falling timer
        timeSinceLastFlap = 0f;
    }

    public Vector3 CalculateMovement(
        Vector2 input,
        Transform player,
        float deltaTime)
    {
        /*
        // Steer left and right
        player.Rotate(
            Vector3.up,
            input.x * turnSpeed * deltaTime,
            Space.World
        );*/

        // Steer left and right and fix jitter
        float turnAmount = input.x * turnSpeed * deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0f,player.eulerAngles.y + turnAmount,0f);
        player.rotation = targetRotation;

        // Adjust forward speed
        float targetSpeed = baseSpeed + input.y * speedAdjustment;

        targetSpeed = Mathf.Clamp(
            targetSpeed,
            minSpeed,
            maxSpeed
        );

        currentSpeed = targetSpeed;

        // Apply gravity after the flap grace period
        timeSinceLastFlap += deltaTime;

        if (timeSinceLastFlap > flapGracePeriod)
        {
            verticalVelocity += gravity * deltaTime;
        }

        // Move in the direction the bird is facing
        Vector3 forwardVelocity = flightDirection.forward * currentSpeed;

        // Add vertical flap / falling velocity
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

